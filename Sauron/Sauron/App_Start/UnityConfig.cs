using System;
using System.Web;
using Microsoft.Owin.Security;
using Unity;
using Unity.AspNet.Mvc;
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
			// singleton for logger
			container.RegisterType<Sauron.Common.Logger.ILogger, Sauron.Common.Logger.Logger>(new ContainerControlledLifetimeManager());

			Sauron.Services.UnityConfig.RegisterTypes(container, () => new PerRequestLifetimeManager());
			Sauron.Identity.UnityConfig.RegisterTypes(container, () => new PerRequestLifetimeManager());

			container.RegisterType<IAuthenticationManager>(new PerRequestLifetimeManager(), new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));
		}
	}
}