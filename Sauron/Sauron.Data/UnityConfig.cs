using System;
using System.Data.Entity;
using Sauron.Data.DataProviders;
using Sauron.Data.Db;
using Unity;
using Unity.Lifetime;

namespace Sauron.Data
{
	public static class UnityConfig
	{
		public static void RegisterTypes(IUnityContainer container, Func<LifetimeManager> lifetimeManagerCreator)
		{
			container.RegisterType<DbContext, ApplicationDbContext>(lifetimeManagerCreator());
			container.RegisterType<IGitHubDataProvider, GitHubDataProvider>(lifetimeManagerCreator());
		}
	}
}
