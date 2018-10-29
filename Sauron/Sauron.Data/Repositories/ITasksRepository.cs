using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sauron.Data.Entities;

namespace Sauron.Data.Repositories
{
	public interface ITasksRepository
	{
		Task<IList<TaskEntity>> GetAvailableTasks();

		Task<TaskEntity> GetTask(Guid taskId);

		Task CreateTask(TaskEntity entity);

		Task EditTask(TaskEntity entity);

		Task DeleteTask(Guid taskId);
	}
}
