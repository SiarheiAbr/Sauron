using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sauron.Services.DataServices;
using Sauron.Services.Models;
using Sauron.ViewModels;

namespace Sauron.Controllers
{
	[Authorize]
	public class TasksController : Controller
	{
		private readonly ITasksService tasksService;
		private readonly IGitHubService gitHubService;

		public TasksController(ITasksService tasksService, IGitHubService gitHubService)
		{
			this.tasksService = tasksService;
			this.gitHubService = gitHubService;
		}

		// GET
		public async Task<ActionResult> Index()
		{
			var tasks = await this.tasksService.GetAvailableTasks();

			IList<GitHubRepositoryModel> userRepositories = null;

			try
			{
				userRepositories = await this.gitHubService.GetUserRepositories();
			}
			catch (UnauthorizedAccessException e)
			{
				return RedirectToAction("Index", "Home");
			}

			var repoModels = userRepositories.Select(repo => new GitHubRepositoryViewModel()
			{
				Id = repo.Id,
				Name = repo.Name,
				GitUrl = repo.GitUrl,
				Url = repo.Url
			}).ToList();

			var tasksModels = tasks.Select(task => new TaskViewModel()
			{
				Id = task.Id,
				GitHubUrl = task.GitHubUrl,
				Name = task.Name
			}).ToList();

			var viewModel = new TasksIndexViewModel()
			{
				Tasks = tasksModels,
				Repositories = repoModels
			};

			return View(viewModel);
		}
	}
}