using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Sauron.Data.Db;
using Sauron.Data.Entities;

namespace Sauron.Data.Repositories
{
	public class HomeWorksRepository : IHomeWorksRepository
	{
		private readonly ApplicationDbContext context;

		public HomeWorksRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public async Task<IList<HomeWorkEntity>> GetHomeWorks(string userId)
		{
			var homeWorks = await this.context.HomeWorks
				.Include(hm => hm.Task)
				.Where(hm => hm.UserId == userId).ToListAsync();

			return homeWorks;
		}

		public async Task<IList<StudentEntity>> GetStudentsInfo()
		{
			var studentsInfo = await this.context.Users.Select(g => new StudentEntity()
			{
				Name = g.UserName,
				UserId = g.Id,
				SubmittedHomeWorks = g.HoweWorks.Count
			}).ToListAsync();

			return studentsInfo;
		}

		public async Task DeleteHomeWork(string userId, Guid taskId)
		{
			var homeWork = await this.context.HomeWorks.FirstOrDefaultAsync(hm => hm.UserId == userId && hm.TaskId == taskId);

			if (homeWork != null)
			{
				this.context.HomeWorks.Remove(homeWork);
				await this.context.SaveChangesAsync();
			}
		}

		public async Task<HomeWorkEntity> GetHomeWork(string userId, Guid taskId)
		{
			var homeWork = await this.context.HomeWorks
				.Include(hm => hm.Task)
				.FirstOrDefaultAsync(hm => hm.UserId == userId && hm.TaskId == taskId);

			return homeWork;
		}

		public async Task<HomeWorkEntity> GetHomeWork(Guid homeWorkId)
		{
			var homeWork = await this.context.HomeWorks
				.Include(hw => hw.Task)
				.FirstOrDefaultAsync(hm => hm.Id == homeWorkId);
			return homeWork;
		}

		public async Task AddOrUpdateHomeWork(HomeWorkEntity homeWork)
		{
			this.context.HomeWorks.AddOrUpdate(homeWork);
			await this.context.SaveChangesAsync();
		}
	}
}
