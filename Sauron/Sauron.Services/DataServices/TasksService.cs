using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Sauron.Data.Entities;
using Sauron.Data.Repositories;
using Sauron.Services.Helpers;
using Sauron.Services.Models;
using Sauron.Services.Settings;
using ILogger = Sauron.Common.Logger.ILogger;

namespace Sauron.Services.DataServices
{
	public class TasksService : ITasksService
	{
		private readonly ITasksRepository tasksRepository;

		private readonly IServicesConfig config;

		private readonly IHomeWorksRepository homeWorksRepository;

		private readonly ILogger logger;

		public TasksService(ITasksRepository tasksRepository, IHomeWorksRepository homeWorksRepository, IServicesConfig config, ILogger logger)
		{
			this.tasksRepository = tasksRepository;
			this.config = config;
			this.homeWorksRepository = homeWorksRepository;
			this.logger = logger;
		}

		public async Task<IList<TaskModel>> GetAvailableTasks(string userId)
		{
			var tasksEntities = await this.tasksRepository.GetAvailableTasks();
			var homeWorks = await this.homeWorksRepository.GetHomeWorks(userId);

			var homeWorksDictionary = homeWorks.ToDictionary(x => x.TaskId, x => x.AttemptsCount);

			var tasksModels = tasksEntities.Select(entity =>
			{
				var attemptsCountExists = homeWorksDictionary.TryGetValue(entity.Id, out var attemptsCount);

				var taskModel = Mapper.Map<TaskModel>(entity);

				taskModel.AttemptsCount = attemptsCountExists ? attemptsCount : 0;

				return taskModel;
			}).ToList();

			return tasksModels;
		}

		public async Task<IList<TaskModel>> GetAvailableTasks()
		{
			var tasksEntities = await this.tasksRepository.GetAvailableTasks();

			var tasksModels = Mapper.Map<IList<TaskModel>>(tasksEntities);

			return tasksModels;
		}

		public async Task<TaskModel> GetTask(Guid taskId)
		{
			var entity = await this.tasksRepository.GetTask(taskId);

			if (entity != null)
			{
				return Mapper.Map<TaskModel>(entity);
			}

			return null;
		}

		public async Task CreateTask(TaskModel taskModel)
		{
			var entity = Mapper.Map<TaskEntity>(taskModel);

			await this.tasksRepository.CreateTask(entity);

			await this.SaveHiddenTestsFile(entity, taskModel.HiddenTestsFile);
		}

		public async Task EditTask(TaskModel taskModel)
		{
			var entity = Mapper.Map<TaskEntity>(taskModel);

			await this.tasksRepository.EditTask(entity);

			await this.SaveHiddenTestsFile(entity, taskModel.HiddenTestsFile);
		}

		public async Task DeleteTask(Guid taskId)
		{
			var hiddenTestsPath = string.Format(this.config.HiddenTestsPathTemplate, taskId);

			await this.tasksRepository.DeleteTask(taskId);

			DirectoryHelper.DeleteDirectory(hiddenTestsPath, this.logger);
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
					DirectoryHelper.CleanDirectory(directory, this.logger);
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
