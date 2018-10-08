using System.Threading.Tasks;
using System.Web.Mvc;
using Sauron.Models;
using Sauron.Services;

namespace Sauron.Controllers
{
	public class GitHubController : Controller
	{
		private IGitHubService gitHubService;

		public GitHubController(IGitHubService gitHubService)
		{
			this.gitHubService = gitHubService;
		}

		// GET
		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var indexViewModel = new GitHubIndexViewModel()
			{
				UserInfo = new GitHubUserInfoModel()
			};

			var userRepositories = await this.gitHubService.GetUserRepositories();

			indexViewModel.UserInfo.Repositories = userRepositories;

			return View(indexViewModel);
		}

		[HttpPost]
		public async Task DownloadRepository(long repositoryId)
		{
			await this.gitHubService.DownloadRepository(repositoryId);
		}
	}
}