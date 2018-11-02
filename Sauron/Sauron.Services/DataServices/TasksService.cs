using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sauron.Data.Entities;
using Sauron.Data.Repositories;
using Sauron.Services.Helpers;
using Sauron.Services.Models;
using Sauron.Services.Settings;

namespace Sauron.Services.DataServices
{
	public class TasksService : ITasksService
	{
		private readonly ITasksRepository tasksRepository;

		private readonly IServicesConfig config;

		public TasksService(ITasksRepository tasksRepository, IServicesConfig config)
		{
			this.tasksRepository = tasksRepository;
			this.config = config;
		}

		public async Task<IList<TaskModel>> GetAvailableTasks()
		{
			var tasksEntities = await this.tasksRepository.GetAvailableTasks();

			var tasksModels = tasksEntities.Select(entity => new TaskModel()
			{
				Id = entity.Id,
				Name = entity.Name,
				GitHubUrl = entity.GitHubUrl,
				TestsFileName = entity.TestsFileName,
				HiddenTestsUploaded = entity.HiddenTestsUploaded
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
					GitHubUrl = entity.GitHubUrl,
					TestsFileName = entity.TestsFileName,
					HiddenTestsUploaded = entity.HiddenTestsUploaded
				};
			}

			return null;
		}

		public async Task CreateTask(TaskModel taskModel)
		{
			var entity = new TaskEntity()
			{
				Name = taskModel.Name,
				GitHubUrl = taskModel.GitHubUrl,
				TestsFileName = taskModel.TestsFileName,
				HiddenTestsUploaded = taskModel.HiddenTestsUploaded
			};

			await this.tasksRepository.CreateTask(entity);

			await this.SaveHiddenTestsFile(entity, taskModel.HiddenTestsFile);
		}

		public async Task EditTask(TaskModel taskModel)
		{
			var entity = new TaskEntity()
			{
				Id = taskModel.Id,
				Name = taskModel.Name,
				GitHubUrl = taskModel.GitHubUrl,
				TestsFileName = taskModel.TestsFileName,
				HiddenTestsUploaded = taskModel.HiddenTestsUploaded
			};

			await this.tasksRepository.EditTask(entity);

			await this.SaveHiddenTestsFile(entity, taskModel.HiddenTestsFile);
		}

		public async Task DeleteTask(Guid taskId)
		{
			var hiddenTestsPath = string.Format(this.config.HiddenTestsPathTemplate, taskId);
			
			await this.tasksRepository.DeleteTask(taskId);

			DirectoryHelper.DeleteDirectory(hiddenTestsPath);
		}

		private async Task SaveHiddenTestsFile(TaskEntity entity, Stream testsFileStream)
		{
			if (testsFileStream != null && testsFileStream.Length > 0)
			{
				var hiddenTestsPath = string.Format(this.config.HiddenTestsPathTemplate, entity.Id);

				var filePath = Path.Combine(hiddenTestsPath, entity.TestsFileName);
				string directory = Path.GetDirectoryName(filePath);

				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}
				else
				{
					DirectoryHelper.CleanDirectory(directory);
				}

				byte[] testsFileData = new byte[testsFileStream.Length];

				await testsFileStream.ReadAsync(testsFileData, 0, testsFileData.Length);

				using (FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
				{
					await stream.WriteAsync(testsFileData, 0, testsFileData.Length);
				}
			}
		}
	}
}
