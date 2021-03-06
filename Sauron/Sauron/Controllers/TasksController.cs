﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using Microsoft.AspNet.Identity;
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
			var tasks = await this.tasksService.GetAvailableTasks(User.Identity.GetUserId());

			IList<GitHubRepositoryModel> userRepositories = null;

			try
			{
				userRepositories = await this.gitHubService.GetUserRepositories();
			}
			catch (UnauthorizedAccessException e)
			{
				return RedirectToAction("Index", "Home");
			}

			var repoModels = Mapper.Map<IList<GitHubRepositoryViewModel>>(userRepositories);

			var tasksModels = Mapper.Map<List<TaskViewModel>>(tasks);

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

			var viewModel = Mapper.Map<CreateEditTaskViewModel>(task);

			return View(viewModel);
		}

		[Authorize(Roles = "admin")]
		[HttpPost]
		public async Task<ActionResult> Edit(CreateEditTaskViewModel model)
		{
			var task = new TaskModel()
			{
				Id = model.Id,
				Name = model.Name,
				GitHubUrl = model.GitHubUrl,
				TestsFileName = model.TestsFileName,
				HiddenTestsUploaded = !model.HiddenTestsUploaded ? model.HiddenTestFile != null : model.HiddenTestsUploaded,
				HiddenTestsFile = model.HiddenTestFile?.InputStream
			};

			await this.tasksService.EditTask(task);

			return RedirectToAction("ManageTasks");
		}

		[Authorize(Roles = "admin")]
		[HttpPost]
		public async Task<ActionResult> Create(CreateEditTaskViewModel model)
		{
			var task = new TaskModel()
			{
				Name = model.Name,
				GitHubUrl = model.GitHubUrl,
				TestsFileName = model.TestsFileName,
				HiddenTestsUploaded = model.HiddenTestFile != null,
				HiddenTestsFile = model.HiddenTestFile?.InputStream
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

			var tasksModels = Mapper.Map<IList<TaskViewModel>>(tasks);

			var viewModel = new TasksManageViewModel()
			{
				Tasks = tasksModels
			};

			return View(viewModel);
		}
	}
}