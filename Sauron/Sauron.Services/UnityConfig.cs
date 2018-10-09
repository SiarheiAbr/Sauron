using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sauron.Data.DataProviders;
using Sauron.Services.GitHub;
using Sauron.Services.Identity;
using Sauron.Services.Settings;
using Unity;

namespace Sauron.Services
{
	public static class UnityConfig
	{
		public static void RegisterTypes(IUnityContainer container)
		{
			container.RegisterType<IGitHubDataProvider, GitHubDataProvider>();
			container.RegisterType<IGitHubService, GitHubService>();
			container.RegisterType<IGitHubIdentityProvider, GitHubIdentityProvider>();
			container.RegisterType<IUserIdentityService, UserIdentityService>();
			container.RegisterType<IServicesConfig, ServicesConfig>();
		}
	}
}
