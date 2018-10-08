﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Google;
using Owin.Security.Providers;
using Owin;
using Owin.Security.Providers.GitHub;
using Sauron.Models;
using Sauron.Services;
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
					 //Enables the application to validate the security stamp when the user logs in.

					 //This is a security feature which is used when you change a password or add an external login to your account.
					OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
						validateInterval: TimeSpan.FromMinutes(30),
						regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
				}
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

	        GitHubAuthenticationOptions gitHubAuthenticationOptions = new GitHubAuthenticationOptions()
	        {
				ClientId = "58a25e28044a178787a0",
				ClientSecret = "9673d03cb89c938095134702c09b0a5c074d30d6",
				
				Provider = new GitHubAuthenticationProvider()
				{
					OnAuthenticated = (context) =>
					{
						var accessTokenClaim = new Claim(GitHubIdentityProvider.GitHubAccessTokenClaimType, context.AccessToken, ClaimValueTypes.String, "GitHub");
						context.Identity.AddClaim(accessTokenClaim);
						return Task.FromResult<object>(null);
					}
				}
	        };

			app.UseGitHubAuthentication(gitHubAuthenticationOptions);
		}
    }
}