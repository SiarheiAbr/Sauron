using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Sauron.Identity.Models;

namespace Sauron.Identity.Services
{
	public interface IApplicationUserService
	{
		Task<IdentityResult> AddExternalLoginAsync(string email, ExternalLoginInfo loginInfo);

		Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo loginInfo);

		Task AddPhoneNumberAsync(string userId, string phoneNumber);

		Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword);

		Task<IdentityResult> ConfirmEmailAsync(string userId, string code);

		Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent);

		Task<UserModel> FindByIdAsync(string userId);

		Task<UserModel> FindByNameAsync(string email);

		Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string phoneNumber);

		Task<IList<UserLoginInfo>> GetLoginsAsync(string userId);

		Task<string> GetPhoneNumberAsync(string userId);

		Task<bool> GetTwoFactorEnabledAsync(string userId);

		Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId);

		Task<string> GetVerifiedUserIdAsync();

		Task<bool> HasBeenVerifiedAsync();

		Task<bool> IsEmailConfirmedAsync(string userId);

		Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout);

		Task<IdentityResult> Register(RegisterModel model);

		Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo loginInfo);

		Task<IdentityResult> RemovePhoneNumberAsync(string userId);

		Task<IdentityResult> ResetPasswordAsync(string userId, string code, string password);

		Task<bool> SendTwoFactorCodeAsync(string provider);

		Task<IdentityResult> SetPasswordAsync(string userId, string password);

		Task SetTwoFactorEnabledAsync(string userId, bool enabled);

		Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);

		Task<IdentityResult> VerifyPhoneNumber(string userId, string phoneNumber, string code);
	}
}