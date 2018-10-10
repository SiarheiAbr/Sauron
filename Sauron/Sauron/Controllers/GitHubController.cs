using System.Threading.Tasks;
using System.Web.Mvc;
using Sauron.Services;
using Sauron.Services.Build;
using Sauron.Services.GitHub;
using Sauron.Services.Models;
using Sauron.Services.Repository;
using Sauron.ViewModels;

namespace Sauron.Controllers
{
	public class GitHubController : Controller
	{
		private readonly IGitHubService gitHubService;
		private readonly IBuildService buildService;
		private readonly IRepositoryService repositoryService;

		public GitHubController(
			IGitHubService gitHubService,
			IRepositoryService repositoryService,
			IBuildService buildService)
		{
			this.gitHubService = gitHubService;
			this.repositoryService = repositoryService;
			this.buildService = buildService;
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
			await this.repositoryService.ExtractRepository(repositoryId);
			await this.buildService.BuildRepository(repositoryId);
		}
	}
}