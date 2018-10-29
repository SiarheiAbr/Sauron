using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Sauron.Identity.Entities;
using Sauron.Identity.Managers;
using Sauron.Identity.Models;

namespace Sauron.Identity.Services
{
	public class ApplicationUserService : IApplicationUserService
	{
		private ApplicationUserManager userManager;
		private ApplicationSignInManager signInManager;

		public ApplicationUserService(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
		{
			return await this.signInManager.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
		}

		public async Task<bool> HasBeenVerifiedAsync()
		{
			return await this.signInManager.HasBeenVerifiedAsync();
		}

		public async Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
		{
			return await this.signInManager.TwoFactorSignInAsync(provider, code, isPersistent: isPersistent, rememberBrowser: rememberBrowser);
		}

		public async Task<IdentityResult> Register(RegisterModel model)
		{
			var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

			var result = await this.userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				await this.signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
			}

			return result;
		}

		public async Task<IdentityResult> AddExternalLoginAsync(string email, ExternalLoginInfo loginInfo)
		{
			var user = new ApplicationUser { UserName = email, Email = email };

			var result = await this.userManager.CreateAsync(user);

			if (result.Succeeded)
			{
				result = await this.userManager.AddLoginAsync(user.Id, loginInfo.Login);

				// TODO:remove after testing
				////await this.userManager.AddToRoleAsync(user.Id, "admin");

				if (result.Succeeded)
				{
					await this.signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
				}
			}

			return result;
		}

		public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo loginInfo)
		{
			return await this.userManager.AddLoginAsync(userId, loginInfo);
		}

		public async Task<IdentityResult> ConfirmEmailAsync(string userId, string code)
		{
			return await this.userManager.ConfirmEmailAsync(userId, code);
		}

		public async Task<UserModel> FindByNameAsync(string email)
		{
			var user = await this.userManager.FindByNameAsync(email);

			if (user == null)
			{
				return null;
			}

			return new UserModel() { Id = user.Id, Email = user.Email, UserName = user.UserName, PasswordHash = user.PasswordHash, PhoneNumber = user.PhoneNumber };
		}

		public async Task<UserModel> FindByIdAsync(string userId)
		{
			var user = await this.userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return null;
			}

			return new UserModel() { Id = user.Id, Email = user.Email, UserName = user.UserName, PasswordHash = user.PasswordHash, PhoneNumber = user.PhoneNumber };
		}

		public async Task<bool> IsEmailConfirmedAsync(string userId)
		{
			return await this.userManager.IsEmailConfirmedAsync(userId);
		}

		public async Task<IdentityResult> ResetPasswordAsync(string userId, string code, string password)
		{
			return await this.userManager.ResetPasswordAsync(userId, code, password);
		}

		public async Task<string> GetVerifiedUserIdAsync()
		{
			return await this.signInManager.GetVerifiedUserIdAsync();
		}

		public async Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId)
		{
			return await this.userManager.GetValidTwoFactorProvidersAsync(userId);
		}

		public async Task<bool> SendTwoFactorCodeAsync(string provider)
		{
			return await this.signInManager.SendTwoFactorCodeAsync(provider);
		}

		public async Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent)
		{
			return await this.signInManager.ExternalSignInAsync(loginInfo, isPersistent: isPersistent);
		}

		public async Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
		{
			return await this.userManager.GetLoginsAsync(userId);
		}

		public async Task<string> GetPhoneNumberAsync(string userId)
		{
			return await this.userManager.GetPhoneNumberAsync(userId);
		}

		public async Task<bool> GetTwoFactorEnabledAsync(string userId)
		{
			return await this.userManager.GetTwoFactorEnabledAsync(userId);
		}

		public async Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo loginInfo)
		{
			var result = await this.userManager.RemoveLoginAsync(userId, loginInfo);

			if (result.Succeeded)
			{
				var user = await this.userManager.FindByIdAsync(userId);

				if (user != null)
				{
					await this.signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
				}
			}

			return result;
		}

		public async Task AddPhoneNumberAsync(string userId, string phoneNumber)
		{
			// Generate the token and send it
			var code = await this.userManager.GenerateChangePhoneNumberTokenAsync(userId, phoneNumber);

			if (this.userManager.SmsService != null)
			{
				var message = new IdentityMessage
				{
					Destination = phoneNumber,
					Body = "Your security code is: " + code
				};

				await this.userManager.SmsService.SendAsync(message);
			}
		}

		public async Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string phoneNumber)
		{
			return await this.userManager.GenerateChangePhoneNumberTokenAsync(userId, phoneNumber);
		}

		public async Task SetTwoFactorEnabledAsync(string userId, bool enabled)
		{
			await this.userManager.SetTwoFactorEnabledAsync(userId, enabled);
			var user = await this.userManager.FindByIdAsync(userId);

			if (user != null)
			{
				await this.signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
			}
		}

		public async Task<IdentityResult> VerifyPhoneNumber(string userId, string phoneNumber, string code)
		{
			var result = await this.userManager.ChangePhoneNumberAsync(userId, phoneNumber, code);

			if (result.Succeeded)
			{
				var user = await this.userManager.FindByIdAsync(userId);

				if (user != null)
				{
					await this.signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
				}
			}

			return result;
		}

		public async Task<IdentityResult> RemovePhoneNumberAsync(string userId)
		{
			var result = await this.userManager.SetPhoneNumberAsync(userId, null);

			if (result.Succeeded)
			{
				var user = await this.userManager.FindByIdAsync(userId);

				if (user != null)
				{
					await this.signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
				}
			}

			return result;
		}

		public async Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
		{
			var result = await this.userManager.ChangePasswordAsync(userId, oldPassword, newPassword);

			if (result.Succeeded)
			{
				var user = await this.userManager.FindByIdAsync(userId);

				if (user != null)
				{
					await this.signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
				}
			}

			return result;
		}

		public async Task<IdentityResult> SetPasswordAsync(string userId, string password)
		{
			var result = await this.userManager.AddPasswordAsync(userId, password);

			if (result.Succeeded)
			{
				var user = await this.userManager.FindByIdAsync(userId);

				if (user != null)
				{
					await this.signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
				}
			}

			return result;
		}
	}
}
