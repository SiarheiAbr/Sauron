using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sauron.Services.Models;

namespace Sauron.Services.Repository
{
	public interface IRepositoryService
	{
		Task ExtractRepository(long repositoryId);

		Task SaveRepository(long repositoryId, byte[] archiveData);

		LocalRepositoryModel GetLocalRepositoryInfo(long repositoryId);
	}
}
