using System;
using System.Threading.Tasks;
using Sauron.Services.Models;

namespace Sauron.Services.Processing
{
	public interface IRepositoryService
	{
		Task ExtractRepository(long repositoryId, Guid taskId);

		Task SaveRepository(long repositoryId, Guid taskId, byte[] archiveData);

		LocalRepositoryModel GetLocalRepositoryInfo(long repositoryId, Guid taskId);

		Task PutHiddenTestsIntoRepository(long repositoryId, Guid taskId);

		Task CopyNugetRunnerIntoRepository(long repositoryId, Guid taskId);
	}
}
