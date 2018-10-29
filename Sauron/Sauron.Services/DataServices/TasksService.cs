using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sauron.Data.Entities;
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

		public async Task<TaskModel> GetTask(Guid taskId)
		{
			var entity = await this.tasksRepository.GetTask(taskId);

			if (entity != null)
			{
				return new TaskModel()
				{
					Id = entity.Id,
					Name = entity.Name,
					GitHubUrl = entity.GitHubUrl
				};
			}

			return null;
		}

		public async Task CreateTask(TaskModel taskModel)
		{
			var entity = new TaskEntity()
			{
				Name = taskModel.Name,
				GitHubUrl = taskModel.GitHubUrl
			};

			await this.tasksRepository.CreateTask(entity);
		}

		public async Task EditTask(TaskModel taskModel)
		{
			var entity = new TaskEntity()
			{
				Id = taskModel.Id,
				Name = taskModel.Name,
				GitHubUrl = taskModel.GitHubUrl
			};

			await this.tasksRepository.EditTask(entity);
		}

		public async Task DeleteTask(Guid taskId)
		{
			await this.tasksRepository.DeleteTask(taskId);
		}
	}
}
