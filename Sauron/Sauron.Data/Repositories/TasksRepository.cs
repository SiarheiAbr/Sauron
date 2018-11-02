using System;
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

		public async Task<TaskEntity> GetTask(Guid taskId)
		{
			var task = await this.context.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);
			return task;
		}

		public async Task CreateTask(TaskEntity entity)
		{
			this.context.Tasks.Add(entity);
			await this.context.SaveChangesAsync();
		}

		public async Task EditTask(TaskEntity entity)
		{
			var task = await this.GetTask(entity.Id);

			task.GitHubUrl = entity.GitHubUrl;
			task.Name = entity.Name;
			task.HiddenTestsUploaded = entity.HiddenTestsUploaded;
			task.TestsFileName = entity.TestsFileName;

			await this.context.SaveChangesAsync();
		}

		public async Task DeleteTask(Guid taskId)
		{
			var task = new TaskEntity() { Id = taskId };

			this.context.Tasks.Attach(task);
			this.context.Tasks.Remove(task);

			await this.context.SaveChangesAsync();
		}
	}
}
