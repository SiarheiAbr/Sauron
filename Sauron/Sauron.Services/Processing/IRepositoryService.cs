using System.Threading.Tasks;
using Sauron.Services.Models;

namespace Sauron.Services.Processing
{
	public interface IRepositoryService
	{
		Task ExtractRepository(long repositoryId);

		Task SaveRepository(long repositoryId, byte[] archiveData);

		LocalRepositoryModel GetLocalRepositoryInfo(long repositoryId);
	}
}
