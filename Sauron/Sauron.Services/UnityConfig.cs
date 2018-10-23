using System;
using Sauron.Services.DataServices;
using Sauron.Services.Processing;
using Sauron.Services.Processing.TestRunner;
using Sauron.Services.Settings;
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
			container.RegisterType<ITasksService, TasksService>(lifetimeManagerCreator());
			container.RegisterType<IHomeWorksService, HomeWorksService>(lifetimeManagerCreator());
			container.RegisterType<IProcessingService, ProcessingService>(lifetimeManagerCreator());
			container.RegisterType<ITestReportService, TestReportService>(lifetimeManagerCreator());
			Sauron.Data.UnityConfig.RegisterTypes(container, lifetimeManagerCreator);
		}
	}
}
