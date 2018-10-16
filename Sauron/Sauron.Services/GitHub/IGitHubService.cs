using System.Collections.Generic;
using System.Threading.Tasks;
using Sauron.Services.Models;

namespace Sauron.Services.GitHub
{
	public interface IGitHubService
	{
		Task<IList<GitHubRepositoryModel>> GetUserRepositories();

		Task DownloadRepository(long repositoryId);
	}
}