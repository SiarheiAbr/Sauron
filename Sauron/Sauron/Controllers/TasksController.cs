using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
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

		[Authorize(Roles = "admin")]
		[HttpGet]
		public async Task<ActionResult> Edit(Guid taskId)
		{
			var task = await this.tasksService.GetTask(taskId);

			var viewModel = new CreateEditTaskViewModel()
			{
				Id = task.Id,
				Name = task.Name,
				GitHubUrl = task.GitHubUrl
			};

			return View(viewModel);
		}

		[Authorize(Roles = "admin")]
		[HttpPost]
		public async Task<ActionResult> Edit(CreateEditTaskViewModel taskModel)
		{
			var task = new TaskModel()
			{
				Id = taskModel.Id,
				Name = taskModel.Name,
				GitHubUrl = taskModel.GitHubUrl
			};

			await this.tasksService.EditTask(task);

			return RedirectToAction("ManageTasks");
		}

		[Authorize(Roles = "admin")]
		[HttpPost]
		public async Task<ActionResult> Create(CreateEditTaskViewModel taskModel)
		{
			var task = new TaskModel()
			{
				Name = taskModel.Name,
				GitHubUrl = taskModel.GitHubUrl
			};

			await this.tasksService.CreateTask(task);

			return RedirectToAction("ManageTasks");
		}

		[Authorize(Roles = "admin")]
		[HttpGet]
		public async Task<ActionResult> Create()
		{
			return View(new CreateEditTaskViewModel());
		}

		[Authorize(Roles = "admin")]
		public async Task<ActionResult> Delete(Guid taskId)
		{
			await this.tasksService.DeleteTask(taskId);

			return RedirectToAction("ManageTasks");
		}

		[Authorize(Roles = "admin")]
		public async Task<ActionResult> ManageTasks()
		{
			var tasks = await this.tasksService.GetAvailableTasks();

			var tasksModels = tasks.Select(task => new TaskViewModel()
			{
				Id = task.Id,
				GitHubUrl = task.GitHubUrl,
				Name = task.Name
			}).ToList();

			var viewModel = new TasksManageViewModel()
			{
				Tasks = tasksModels
			};

			return View(viewModel);
		}
	}
}