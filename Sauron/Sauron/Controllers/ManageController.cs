using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Sauron.Identity;
using Sauron.Identity.Managers;
using Sauron.Identity.Services;
using Sauron.ViewModels;

namespace Sauron.Controllers
{
	[Authorize]
	public class ManageController : Controller
	{
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private readonly IAuthenticationManager authenticationManager;
		private readonly IApplicationUserService userService;

		public ManageController(
			IApplicationUserService userService,
			IAuthenticationManager authenticationManager)
		{
			this.authenticationManager = authenticationManager;
		}

		public enum ManageMessageId
		{
			AddPhoneSuccess,
			ChangePasswordSuccess,
			SetTwoFactorSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
			RemovePhoneSuccess,
			Error
		}

		// GET: /Manage/Index
		public async Task<ActionResult> Index(ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
				: message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
				: message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
				: message == ManageMessageId.Error ? "An error has occurred."
				: message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
				: message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
				: string.Empty;

			var userId = User.Identity.GetUserId();

			var model = new IndexViewModel
			{
				HasPassword = await this.HasPassword(),
				PhoneNumber = await this.userService.GetPhoneNumberAsync(userId),
				TwoFactor = await this.userService.GetTwoFactorEnabledAsync(userId),
				Logins = await this.userService.GetLoginsAsync(userId),
				BrowserRemembered = await this.authenticationManager.TwoFactorBrowserRememberedAsync(userId)
			};

			return View(model);
		}

		// POST: /Manage/RemoveLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
		{
			ManageMessageId? message;
			var result = await this.userService.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));

			message = result.Succeeded ? ManageMessageId.RemoveLoginSuccess : ManageMessageId.Error;

			return RedirectToAction("ManageLogins", new { Message = message });
		}

		// GET: /Manage/AddPhoneNumber
		public ActionResult AddPhoneNumber()
		{
			return View();
		}

		// POST: /Manage/AddPhoneNumber
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			await this.userService.AddPhoneNumberAsync(User.Identity.GetUserId(), model.Number);

			return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
		}

		// POST: /Manage/EnableTwoFactorAuthentication
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EnableTwoFactorAuthentication()
		{
			await this.userService.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);

			return RedirectToAction("Index", "Manage");
		}

		// POST: /Manage/DisableTwoFactorAuthentication
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DisableTwoFactorAuthentication()
		{
			await this.userService.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);

			return RedirectToAction("Index", "Manage");
		}

		// GET: /Manage/VerifyPhoneNumber
		public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
		{
			var code = await this.userService.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
			
			// Send an SMS through the SMS provider to verify the phone number
			return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
		}

		// POST: /Manage/VerifyPhoneNumber
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await this.userService.VerifyPhoneNumber(User.Identity.GetUserId(), model.PhoneNumber, model.Code);

			if (result.Succeeded)
			{
				return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
			}

			// If we got this far, something failed, redisplay form
			ModelState.AddModelError(string.Empty, "Failed to verify phone");
			return View(model);
		}

		// POST: /Manage/RemovePhoneNumber
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RemovePhoneNumber()
		{
			var result = await this.userService.RemovePhoneNumberAsync(User.Identity.GetUserId());

			if (!result.Succeeded)
			{
				return RedirectToAction("Index", new { Message = ManageMessageId.Error });
			}

			return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
		}

		// GET: /Manage/ChangePassword
		public ActionResult ChangePassword()
		{
			return View();
		}

		// POST: /Manage/ChangePassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await this.userService.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

			if (result.Succeeded)
			{
				return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
			}

			AddErrors(result);
			return View(model);
		}

		// GET: /Manage/SetPassword
		public ActionResult SetPassword()
		{
			return View();
		}

		// POST: /Manage/SetPassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await this.userService.SetPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

				if (result.Succeeded)
				{
					return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
				}

				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		// GET: /Manage/ManageLogins
		public async Task<ActionResult> ManageLogins(ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
				: message == ManageMessageId.Error ? "An error has occurred."
				: string.Empty;
			var user = await this.userService.FindByIdAsync(User.Identity.GetUserId());

			if (user == null)
			{
				return View("Error");
			}

			var userLogins = await this.userService.GetLoginsAsync(User.Identity.GetUserId());
			var otherLogins = this.authenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
			ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;

			return View(new ManageLoginsViewModel
			{
				CurrentLogins = userLogins,
				OtherLogins = otherLogins
			});
		}

		// POST: /Manage/LinkLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LinkLogin(string provider)
		{
			// Request a redirect to the external login provider to link a login for the current user
			return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
		}

		// GET: /Manage/LinkLoginCallback
		public async Task<ActionResult> LinkLoginCallback()
		{
			var loginInfo = await this.authenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());

			if (loginInfo == null)
			{
				return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
			}

			var result = await this.userService.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
			return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
		}

		#region Helpers

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error);
			}
		}

		private async Task<bool> HasPassword()
		{
			var user = await this.userService.FindByIdAsync(User.Identity.GetUserId());

			if (user != null)
			{
				return user.PasswordHash != null;
			}

			return false;
		}

		private async Task<bool> HasPhoneNumber()
		{
			var user = await this.userService.FindByIdAsync(User.Identity.GetUserId());

			return user?.PhoneNumber != null;
		}

		#endregion
	}
}