using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sauron.Data.DataProviders;
using Sauron.Identity.Services;
using Sauron.Services.Models;
using Sauron.Services.Processing;

namespace Sauron.Services.DataServices
{
	public class GitHubService : IGitHubService
	{
		private readonly IGitHubIdentityService gitHubIdentityservice;
		private readonly IGitHubDataProvider gitHubDataProvider;
		private readonly IRepositoryService repositoryService;

		public GitHubService(
			IGitHubIdentityService gitHubIdentityservice,
			IGitHubDataProvider gitHubDataProvider,
			IRepositoryService repositoryService)
		{
			this.gitHubIdentityservice = gitHubIdentityservice;
			this.gitHubDataProvider = gitHubDataProvider;
			this.repositoryService = repositoryService;
		}

		public async Task<IList<GitHubRepositoryModel>> GetUserRepositories()
		{
			var accessToken = this.gitHubIdentityservice.GetAccesToken();

			var repositories = await this.gitHubDataProvider.GetUserRepositories(accessToken);

			var resultList = repositories.Select(x => new GitHubRepositoryModel()
			{
				Name = x.Name,
				Url = x.Url,
				GitUrl = x.GitUrl,
				Id = x.Id
			}).ToList();

			return resultList;
		}

		public async Task DownloadRepository(long repositoryId)
		{
			var accessToken = this.gitHubIdentityservice.GetAccesToken();

			var repoArchive = await this.gitHubDataProvider.DownloadRepository(repositoryId, accessToken);

			await this.repositoryService.SaveRepository(repositoryId, repoArchive);
		}
	}
}