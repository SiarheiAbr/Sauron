using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using Sauron.Data.Entities;

namespace Sauron.Data.DataProviders
{
	public interface IGitHubDataProvider
	{
		Task<IReadOnlyList<RepositoryEntity>> GetUserRepositories(string accessToken);

		Task<byte[]> DownloadRepository(long repositoryId, string accessToken);
	}
}
