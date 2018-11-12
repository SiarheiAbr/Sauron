using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Sauron.Data.Entities;
using Sauron.Identity.Managers;

namespace Sauron.Identity.Helpers
{
	public static class RegenerateIdentityHelper
	{
		public static Func<CookieValidateIdentityContext, Task> OnValidateIdentity()
		{
			return SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
				validateInterval: TimeSpan.FromMinutes(30),
				regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager));
		}
	}
}
