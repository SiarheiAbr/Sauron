using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sauron.Data.Repositories;
using Sauron.Services.Models;

namespace Sauron.Services.DataServices
{
	public class TasksService : ITasksService
	{
		private readonly ITasksRepository tasksRepository;

		public TasksService(ITasksRepository tasksRepository)
		{
			this.tasksRepository = tasksRepository;
		}

		public async Task<IList<TaskModel>> GetAvailableTasks()
		{
			var tasksEntities = await this.tasksRepository.GetAvailableTasks();

			var tasksModels = tasksEntities.Select(entity => new TaskModel()
			{
				Id = entity.Id,
				Name = entity.Name,
				GitHubUrl = entity.GitHubUrl
			}).ToList();

			return tasksModels;
		}
	}
}
