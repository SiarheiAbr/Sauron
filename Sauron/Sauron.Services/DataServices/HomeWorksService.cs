using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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

			var studentsInfoModels = Mapper.Map<IList<StudentModel>>(studentsInfoEntities);

			return studentsInfoModels;
		}

		public async Task<IList<HomeWorkModel>> GetHomeWorks(string userId)
		{
			var entities = await this.homeWorksRepository.GetHomeWorks(userId);

			var models = Mapper.Map<IList<HomeWorkModel>>(entities);

			return models;
		}

		public async Task<HomeWorkModel> GetHomeWork(Guid homeWorkId)
		{
			var entity = await this.homeWorksRepository.GetHomeWork(homeWorkId);

			var model = Mapper.Map<HomeWorkModel>(entity);

			return model;
		}

		public async Task DeleteHomeWork(string userId, Guid taskId)
		{
			await this.homeWorksRepository.DeleteHomeWork(userId, taskId);
		}

		public async Task AddOrUpdateHomeWork(HomeWorkModel homeWork)
		{
			var homeWorkEntity = Mapper.Map<HomeWorkEntity>(homeWork);

			await this.homeWorksRepository.AddOrUpdateHomeWork(homeWorkEntity);
		}

		public async Task<HomeWorkModel> GetHomeWork(string userId, Guid taskId)
		{
			var entity = await this.homeWorksRepository.GetHomeWork(userId, taskId);

			if (entity == null)
			{
				return null;
			}

			var model = Mapper.Map<HomeWorkModel>(entity);

			return model;
		}
	}
}
