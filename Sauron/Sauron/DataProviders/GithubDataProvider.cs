using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using Sauron.Models;

namespace Sauron.DataProviders
{
	public class GitHubDataProvider: IGitHubDataProvider
	{
		public async Task<IReadOnlyList<Repository>> GetUserRepositories(string accessToken)
		{
			var client = this.GetCLient(accessToken);

			return await client.Repository.GetAllForCurrent(new RepositoryRequest()
			{
				Type = RepositoryType.Owner
			});
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
			var credentials = new Credentials(accessToken);

			var client = new GitHubClient(new ProductHeaderValue("Sauron"))
			{
				Credentials = credentials
			};

			return client;
		}
		#endregion
	}
}