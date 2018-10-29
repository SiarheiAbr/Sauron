using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sauron.Services.Models;

namespace Sauron.Services.DataServices
{
	public interface ITasksService
	{
		Task<IList<TaskModel>> GetAvailableTasks();

		Task<TaskModel> GetTask(Guid taskId);

		Task CreateTask(TaskModel taskModel);

		Task EditTask(TaskModel taskModel);

		Task DeleteTask(Guid taskId);
	}
}
