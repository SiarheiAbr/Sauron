using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Sauron.Data.Entities;
using Sauron.Identity.Managers;
using Sauron.Identity.Services;
using Unity;
using Unity.Lifetime;

namespace Sauron.Identity
{
	public static class UnityConfig
	{
		public static void RegisterTypes(IUnityContainer container, Func<LifetimeManager> lifetimeManagerCreator)
		{
			container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(lifetimeManagerCreator());
			container.RegisterType<IdentityUser, ApplicationUser>(lifetimeManagerCreator());
			container.RegisterType<IUserIdentityService, UserIdentityService>(lifetimeManagerCreator());
			container.RegisterType<ApplicationSignInManager>(lifetimeManagerCreator());
			container.RegisterType<ApplicationUserManager>(lifetimeManagerCreator());
			container.RegisterType<IGitHubIdentityService, GitHubIdentityService>(lifetimeManagerCreator());
			container.RegisterType<IApplicationUserService, ApplicationUserService>(lifetimeManagerCreator());
		}
	}
}
