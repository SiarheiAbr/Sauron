using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using Sauron.Data.Entities;

namespace Sauron.Data.DataProviders
{
	public class GitHubDataProvider : IGitHubDataProvider
	{
		public async Task<IReadOnlyList<GitHubRepositoryEntity>> GetUserRepositories(string accessToken)
		{
			var client = this.GetCLient(accessToken);

			var repositories = await client.Repository.GetAllForCurrent(new RepositoryRequest()
			{
				Type = RepositoryType.Owner
			});

			return repositories.Select(x => new GitHubRepositoryEntity()
			{
				Name = x.Name,
				Url = x.Url,
				GitUrl = x.HtmlUrl,
				Id = x.Id
			}).ToList();
		}

		public async Task<byte[]> DownloadRepository(long repositoryId, string accessToken)
		{
			var client = this.GetCLient(accessToken);

			return await client.Repository.Content.GetArchive(repositoryId, ArchiveFormat.Zipball);
		}

		public async Task<bool> IsForkOfRepo(long repositoryId, string parentRepoUrl, string accessToken)
		{
			var result = false;

			var client = this.GetCLient(accessToken);

			var repoInfo = await client.Repository.Get(repositoryId);

			if (repoInfo.Parent != null)
			{
				if (repoInfo.Parent.HtmlUrl == parentRepoUrl)
				{
					result = true;
				}
			}

			return result;
		}

		public async Task<GitHubRepositoryEntity> GetRepository(long repositoryId, string accessToken)
		{
			var client = this.GetCLient(accessToken);

			var repository = await client.Repository.Get(repositoryId);

			return new GitHubRepositoryEntity()
			{
				Name = repository.Name,
				Url = repository.Url,
				GitUrl = repository.HtmlUrl,
				Id = repository.Id
			};
		}

		public void GetUserInfo(string userName)
		{
			throw new NotImplementedException();
		}

		#region private methods
		private GitHubClient GetCLient(string accessToken)
		{
			var credentials = new Octokit.Credentials(accessToken);

			var client = new Octokit.GitHubClient(new ProductHeaderValue("Sauron"))
			{
				Credentials = credentials
			};

			return client;
		}
		#endregion
	}
}