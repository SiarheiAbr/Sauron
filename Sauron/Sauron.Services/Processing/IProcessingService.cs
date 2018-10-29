using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Services.Processing
{
	public interface IProcessingService
	{
		Task ProcessHomeWork(long repositoryId, Guid taskId, string userId);

		Task<bool> IsSubmittedRepoIsForkOfTask(long repositoryId, Guid taskId);
	}
}
