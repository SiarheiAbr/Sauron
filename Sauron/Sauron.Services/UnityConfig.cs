using System;
using Sauron.Services.Build;
using Sauron.Services.GitHub;
using Sauron.Services.Repository;
using Sauron.Services.Settings;
using Sauron.Services.TestRunner;
using Unity;
using Unity.Lifetime;

namespace Sauron.Services
{
	public static class UnityConfig
	{
		public static void RegisterTypes(IUnityContainer container, Func<LifetimeManager> lifetimeManagerCreator)
		{
			container.RegisterType<IGitHubService, GitHubService>(lifetimeManagerCreator());
			container.RegisterType<IRepositoryService, RepositoryService>(lifetimeManagerCreator());
			container.RegisterType<IBuildService, BuildService>(lifetimeManagerCreator());
			container.RegisterType<IServicesConfig, ServicesConfig>(lifetimeManagerCreator());
			container.RegisterType<ITestRunnerService, TestRunnerService>(lifetimeManagerCreator());
			Sauron.Data.UnityConfig.RegisterTypes(container, lifetimeManagerCreator);
		}
	}
}
