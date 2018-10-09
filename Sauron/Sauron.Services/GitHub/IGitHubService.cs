using System.Collections.Generic;
using System.Threading.Tasks;
using Sauron.Services.Models;

namespace Sauron.Services.GitHub
{
	public interface IGitHubService
	{
		Task<IList<RepositoryModel>> GetUserRepositories();

		Task DownloadRepository(long repositoryId);
	}
}