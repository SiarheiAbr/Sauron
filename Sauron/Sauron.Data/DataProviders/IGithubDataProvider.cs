using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using Sauron.Data.Entities;

namespace Sauron.Data.DataProviders
{
	public interface IGitHubDataProvider
	{
		Task<IReadOnlyList<GitHubRepositoryEntity>> GetUserRepositories(string accessToken);

		Task<byte[]> DownloadRepository(long repositoryId, string accessToken);

		Task<bool> IsForkOfRepo(long repositoryId, string parentRepoUrl, string accessToken);

		Task<GitHubRepositoryEntity> GetRepository(long repositoryId, string accessToken);
	}
}
