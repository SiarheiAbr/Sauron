using System;
using System.Data.Entity;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Sauron.Db;
using Sauron.Identity;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

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
			container.RegisterType(typeof(IUserStore<ApplicationUser>), typeof(UserStore<ApplicationUser>));
			container.RegisterType<IdentityUser, ApplicationUser>(new ContainerControlledLifetimeManager());

			container.RegisterType<ApplicationSignInManager>();
			container.RegisterType<ApplicationUserManager>();
			container.RegisterType<IAuthenticationManager>(new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));

			RegisterServicesDependencies(container);
		}

		private static void RegisterServicesDependencies(IUnityContainer container)
		{
			Sauron.Services.UnityConfig.RegisterTypes(container);
		}
	}
}