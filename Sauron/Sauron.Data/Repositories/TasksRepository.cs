using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Sauron.Data.Db;
using Sauron.Data.Entities;

namespace Sauron.Data.Repositories
{
	public class TasksRepository : ITasksRepository
	{
		private readonly ApplicationDbContext context;

		public TasksRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public async Task<IList<TaskEntity>> GetAvailableTasks()
		{
			var tasks = await this.context.Tasks.ToListAsync();

			return tasks;
		}
	}
}
