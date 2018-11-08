using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sauron.Services.Models;

namespace Sauron.Services.DataServices
{
	public interface IHomeWorksService
	{
		Task<IList<HomeWorkModel>> GetHomeWorks(string userId);

		Task DeleteHomeWork(string userId, Guid taskId);

		Task<HomeWorkModel> GetHomeWork(string userId, Guid taskId);

		Task SaveHomeWork(HomeWorkModel homeWorkModel);

		Task<HomeWorkModel> GetHomeWork(Guid homeWorkId);

		Task<IList<StudentModel>> GetStudentsInfo();
	}
}
