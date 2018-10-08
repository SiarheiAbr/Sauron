using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using Sauron.Models;

namespace Sauron.DataProviders
{
	public interface IGitHubDataProvider
	{
		Task<IReadOnlyList<Repository>> GetUserRepositories(string accessToken);

		Task<byte[]> DownloadRepository(long repositoryId, string accessToken);
	}
}
