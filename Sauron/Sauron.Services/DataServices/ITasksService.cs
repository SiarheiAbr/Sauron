using System.Collections.Generic;
using System.Threading.Tasks;
using Sauron.Services.Models;

namespace Sauron.Services.DataServices
{
	public interface ITasksService
	{
		Task<IList<TaskModel>> GetAvailableTasks();
	}
}
