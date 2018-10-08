using System.Collections.Generic;
using System.Threading.Tasks;
using Sauron.Models;

namespace Sauron.Services
{
	public interface IGitHubService
	{
		Task<IList<RepositoryModel>> GetUserRepositories();

		Task DownloadRepository(long repositoryId);
	}
}