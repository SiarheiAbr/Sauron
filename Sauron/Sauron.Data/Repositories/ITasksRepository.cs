using System.Collections.Generic;
using System.Threading.Tasks;
using Sauron.Data.Entities;

namespace Sauron.Data.Repositories
{
	public interface ITasksRepository
	{
		Task<IList<TaskEntity>> GetAvailableTasks();
	}
}
