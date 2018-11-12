using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Sauron.Common;
using Sauron.Common.Static;
using Sauron.Identity;
using Sauron.Services.DataServices;
using Sauron.Services.Processing;
using Sauron.ViewModels;

namespace Sauron.Controllers
{
	[Authorize]
	public class HomeWorksController : Controller
	{
		private readonly IHomeWorksService homeWorksService;
		private readonly ITestReportService testReportService;

		public HomeWorksController(IHomeWorksService homeWorksService, ITestReportService testReportService)
		{
			this.homeWorksService = homeWorksService;
			this.testReportService = testReportService;
		}

		// GET: HomeWorks
		public async Task<ActionResult> Index()
		{
			var homeWorks = await this.homeWorksService.GetHomeWorks(User.Identity.GetUserId());

			var models = homeWorks.Select(hm => new HomeWorkViewModel()
			{
				Id = hm.Id,
				TaskId = hm.TaskId,
				UserId = hm.UserId,
				TaskName = hm.TaskName,
				TaskGitUrl = hm.TaskGitUrl,
				IsBuildSuccessful = hm.IsBuildSuccessful,
				TestsResults = hm.TestsResults,
				RepoGitUrl = hm.RepoGitUrl,
				AttempsCount = hm.AttemptsCount
			}).ToList();

			var indexModel = new HomeWorkIndexViewModel()
			{
				HomeWorks = models
			};

			return View(indexModel);
		}

		[HttpGet]
		public async Task<ActionResult> ViewTestReportResults(string userId, Guid taskId)
		{
			if (!User.IsInRole(UserRoles.Admin))
			{
				if (userId != User.Identity.GetUserId())
				{
					return RedirectToAction("Index", "Home");
				}
			}

			var reportHtml = await this.testReportService.GenerateTestReportForHomeWork(userId, taskId);

			return Content(reportHtml);
		}
	}
}