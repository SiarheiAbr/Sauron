using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using Owin.Security.Providers.GitHub;
using Sauron.Identity;
using Sauron.Identity.Helpers;
using Sauron.Identity.Managers;
using Sauron.Identity.Services;
using Unity;

namespace Sauron
{
	public partial class Startup
	{
		// For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			UnityConfig.Container.RegisterInstance(app.GetDataProtectionProvider());

			// TODO: check DBCONTEXT registration
			app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<ApplicationUserManager>());
			app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<ApplicationSignInManager>());

			// Enable the application to use a cookie to store information for the signed in user
			// and to use a cookie to temporarily store information about a user logging in with a third party login provider
			// Configure the sign in cookie
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login"),
				Provider = new CookieAuthenticationProvider
				{
					// Enables the application to validate the security stamp when the user logs in.

					// This is a security feature which is used when you change a password or add an external login to your account.
					OnValidateIdentity = RegenerateIdentityHelper.OnValidateIdentity()
				}
			});
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

			GitHubAuthenticationOptions gitHubAuthenticationOptions = new GitHubAuthenticationOptions()
			{
				////ClientId = "ebc2a77584c4ab13705b",
				////ClientSecret = "15045e995aa7698d18d0c92055dbead2fd027354",

				ClientId = "d434a053d003d2ca0159",
				ClientSecret = "a1c72b44256bb7deadeca5d0371960885e74c83e",

				Provider = new GitHubAuthenticationProvider()
				{
					OnAuthenticated = (context) =>
					{
						var accessTokenClaim = new Claim(GitHubIdentityService.GitHubAccessTokenClaimType, context.AccessToken, ClaimValueTypes.String, "GitHub");
						context.Identity.AddClaim(accessTokenClaim);
						return Task.FromResult<object>(null);
					}
				}
			};

			app.UseGitHubAuthentication(gitHubAuthenticationOptions);
		}
	}
}