using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sauron.Identity.Services;
using Sauron.Services.DataServices;
using Sauron.ViewModels;

namespace Sauron.Controllers
{
	[Authorize(Roles = "admin")]
	public class StudentsResultsController : Controller
	{
		private readonly IHomeWorksService homeWorksService;

		private readonly IApplicationUserService userService;

		public StudentsResultsController(IHomeWorksService homeWorksService, IApplicationUserService userService)
		{
			this.homeWorksService = homeWorksService;
			this.userService = userService;
		}

		// GET
		public async Task<ActionResult> Index()
		{
			var studentsInfoModels = await this.homeWorksService.GetStudentsInfo();

			var studentsInfoViewModels = studentsInfoModels
				.Select(x => new StudentViewModel()
				{
					Name = x.Name,
					UserId = x.UserId,
					SubmittedHomeWorks = x.SubmittedHomeWorks
				}).ToList();

			var indexViewModel = new StudentsResultsIndexViewModel()
			{
				Students = studentsInfoViewModels
			};

			return View(indexViewModel);
		}

		public async Task<ActionResult> StudentResults(string userId)
		{
			var homeWorks = await this.homeWorksService.GetHomeWorks(userId);
			var user = await this.userService.FindByIdAsync(userId);

			var student = new StudentViewModel()
			{
				UserId = user.Id,
				Name = user.UserName,
				SubmittedHomeWorks = homeWorks.Count
			};

			var homeWorksModels = homeWorks.Select(hm => new HomeWorkViewModel()
			{
				Id = hm.Id,
				TaskId = hm.TaskId,
				UserId = hm.UserId,
				TaskName = hm.TaskName,
				TaskGitUrl = hm.TaskGitUrl,
				IsBuildSuccessful = hm.IsBuildSuccessful,
				TestsResults = hm.TestsResults,
				RepoGitUrl = hm.RepoGitUrl,
				AttempsCount = hm.AttemptsCount,
				TestsMark = hm.TestsMark
			}).ToList();

			var resultModel = new StudentResultsViewModel()
			{
				HomeWorks = homeWorksModels,
				StudentInfo = student
			};

			return View(resultModel);
		}
	}
}