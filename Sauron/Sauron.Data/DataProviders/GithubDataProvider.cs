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
				GitUrl = x.GitUrl,
				Id = x.Id
			}).ToList();
		}

		public async Task<byte[]> DownloadRepository(long repositoryId, string accessToken)
		{
			var client = this.GetCLient(accessToken);

			return await client.Repository.Content.GetArchive(repositoryId, ArchiveFormat.Zipball);
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