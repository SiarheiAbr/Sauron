using System;

using Unity;
using Unity.Injection;
using System.Web.Mvc;
using System.Web;
using Owin;
using Microsoft.AspNet.Identity;
using Sauron.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Unity.Lifetime;
using System.Data.Entity;
using Sauron.DataProviders;
using Sauron.Services;

namespace Sauron
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
	        container.RegisterType<DbContext, ApplicationDbContext>(new HierarchicalLifetimeManager());
			// container.RegisterType<ApplicationDbContext>();
	        container.RegisterType(typeof(IUserStore<ApplicationUser>), typeof(UserStore<ApplicationUser>));
			container.RegisterType<IdentityUser, ApplicationUser>(new ContainerControlledLifetimeManager());

			container.RegisterType<ApplicationSignInManager>();
	        container.RegisterType<ApplicationUserManager>();
	        container.RegisterType<IGitHubDataProvider, GitHubDataProvider>();
	        container.RegisterType<IGitHubService, GitHubService>();
	        container.RegisterType<IGitHubIdentityProvider, GitHubIdentityProvider>();
	        container.RegisterType<IUserIdentityService, UserIdentityService>();

	        container.RegisterType<IAuthenticationManager>(
		        new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));

			// TODO: Register your type's mappings here.
			// container.RegisterType<IProductRepository, ProductRepository>();
			// container.RegisterType<Owin.IOwinContextProvider, Owin.OwinContextProvider>();
			// container.RegisterType<Owin.IOwinContextProvider>(new InjectionFactory(x => HttpContext.Current.GetOwinContext()));
		}
    }
}