using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sauron.Data.Entities;

namespace Sauron.Data.Repositories
{
	public interface IHomeWorksRepository
	{
		Task<IList<HomeWorkEntity>> GetHomeWorks(string userId);

		Task DeleteHomeWork(string userId, Guid taskId);

		Task<HomeWorkEntity> GetHomeWork(string userId, Guid taskId);

		Task<HomeWorkEntity> GetHomeWork(Guid homeWorkId);

		Task SaveHomeWork(HomeWorkEntity homeWork);

		Task<IList<StudentEntity>> GetStudentsInfo();
	}
}
