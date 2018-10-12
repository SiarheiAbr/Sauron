using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Services.Repository
{
	public interface IRepositoryService
	{
		Task ExtractRepository(long repositoryId);

		Task SaveRepository(long repositoryId, byte[] archiveData);

		string GetSolutionFilePath(long repositoryId);

		string GetProjectFilePath(long repositoryId);

		string GetRepositoryFolderPath(long repositoryId);

		string GetZipRepositoryPath(long repositoryId);
	}
}
