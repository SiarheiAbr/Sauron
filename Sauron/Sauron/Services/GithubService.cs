using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sauron.DataProviders;
using Sauron.Models;

namespace Sauron.Services
{
	public class GitHubService: IGitHubService
	{
		private readonly IGitHubIdentityProvider gitHubIdentityProvider;
		private readonly IGitHubDataProvider gitHubDataProvider;

		public GitHubService(IGitHubIdentityProvider gitHubIdentityProvider,
			IGitHubDataProvider gitHubDataProvider)
		{
			this.gitHubIdentityProvider = gitHubIdentityProvider;
			this.gitHubDataProvider = gitHubDataProvider;
		}

		public async Task<IList<RepositoryModel>> GetUserRepositories()
		{
			var accessToken = this.gitHubIdentityProvider.GetAccesToken();

			var repositories = await this.gitHubDataProvider.GetUserRepositories(accessToken);

			var resultList = repositories.Select(x => new RepositoryModel()
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
			var accessToken = this.gitHubIdentityProvider.GetAccesToken();

			var repoArchive = await this.gitHubDataProvider.DownloadRepository(repositoryId, accessToken);

			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"downloads\repo.zip");

			using (FileStream stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite))
			{
				await stream.WriteAsync(repoArchive, 0, repoArchive.Length);
			}
		}
	}
}