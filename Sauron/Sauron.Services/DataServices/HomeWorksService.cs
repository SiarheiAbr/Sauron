using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sauron.Data.Entities;
using Sauron.Data.Repositories;
using Sauron.Services.Models;

namespace Sauron.Services.DataServices
{
	public class HomeWorksService : IHomeWorksService
	{
		private readonly IHomeWorksRepository homeWorksRepository;

		public HomeWorksService(IHomeWorksRepository homeWorksRepository)
		{
			this.homeWorksRepository = homeWorksRepository;
		}

		public async Task<IList<StudentModel>> GetStudentsInfo()
		{
			var studentsInfoEntities = await this.homeWorksRepository.GetStudentsInfo();

			var studentsInfoModels = studentsInfoEntities.Select(x => new StudentModel()
			{
				Name = x.Name,
				UserId = x.UserId,
				SubmittedHomeWorks = x.SubmittedHomeWorks,
				TotalScore = x.TotalScore
			}).ToList();

			return studentsInfoModels;
		}

		public async Task<IList<HomeWorkModel>> GetHomeWorks(string userId)
		{
			var entities = await this.homeWorksRepository.GetHomeWorks(userId);

			var models = entities.Select(hm => new HomeWorkModel()
			{
				Id = hm.Id,
				TaskId = hm.TaskId,
				UserId = hm.UserId,
				IsBuildSuccessful = hm.IsBuildSuccessful,
				TestsResults = hm.TestsResults,
				TaskName = hm.Task.Name,
				TaskGitUrl = hm.Task.GitHubUrl,
				RepoGitUrl = hm.RepoGitUrl,
				AttemptsCount = hm.AttemptsCount,
				TestsMark = hm.TestsMark
			}).ToList();

			return models;
		}

		public async Task<HomeWorkModel> GetHomeWork(Guid homeWorkId)
		{
			var entity = await this.homeWorksRepository.GetHomeWork(homeWorkId);

			var model = new HomeWorkModel()
			{
				Id = entity.Id,
				TaskId = entity.TaskId,
				UserId = entity.UserId,
				IsBuildSuccessful = entity.IsBuildSuccessful,
				TestsResults = entity.TestsResults,
				TaskName = entity.Task.Name,
				TaskGitUrl = entity.Task.GitHubUrl,
				RepoGitUrl = entity.RepoGitUrl,
				AttemptsCount = entity.AttemptsCount,
				TestsMark = entity.TestsMark
			};

			return model;
		}

		public async Task DeleteHomeWork(string userId, Guid taskId)
		{
			await this.homeWorksRepository.DeleteHomeWork(userId, taskId);
		}

		public async Task AddOrUpdateHomeWork(HomeWorkModel homeWork)
		{
			var homeWorkEntity = new HomeWorkEntity()
			{
				Id = homeWork.Id,

				TaskId = homeWork.TaskId,

				IsBuildSuccessful = homeWork.IsBuildSuccessful,

				UserId = homeWork.UserId,

				TestsResults = homeWork.TestsResults,

				RepoGitUrl = homeWork.RepoGitUrl,

				AttemptsCount = homeWork.AttemptsCount,

				TestsMark = homeWork.TestsMark
			};

			await this.homeWorksRepository.AddOrUpdateHomeWork(homeWorkEntity);
		}

		public async Task<HomeWorkModel> GetHomeWork(string userId, Guid taskId)
		{
			var entity = await this.homeWorksRepository.GetHomeWork(userId, taskId);

			if (entity == null)
			{
				return null;
			}

			var model = new HomeWorkModel()
			{
				Id = entity.Id,
				TaskId = entity.TaskId,
				UserId = entity.UserId,
				IsBuildSuccessful = entity.IsBuildSuccessful,
				TestsResults = entity.TestsResults,
				TaskName = entity.Task.Name,
				TaskGitUrl = entity.Task.GitHubUrl,
				RepoGitUrl = entity.RepoGitUrl,
				AttemptsCount = entity.AttemptsCount,
				TestsMark = entity.TestsMark
			};

			return model;
		}
	}
}
