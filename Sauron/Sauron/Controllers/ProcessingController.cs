﻿using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Sauron.Common.Static;
using Sauron.Exceptions;
using Sauron.Services.Processing;

namespace Sauron.Controllers
{
	[Authorize]
	public class ProcessingController : Controller
	{
		private readonly IProcessingService processingService;

		public ProcessingController(IProcessingService processingService)
		{
			this.processingService = processingService;
		}

		[HttpPost]
		public async Task<ActionResult> ProcessHomeWork(long repositoryId, Guid taskId)
		{
			try
			{
				if (!User.IsInRole(UserRoles.Admin))
				{
					var forkOfRepo = await this.processingService.IsSubmittedRepoIsForkOfTask(repositoryId, taskId);

					if (!forkOfRepo)
					{
						return View("Error", new HandleErrorInfo(new ForkException(), "Processing", "ProcessHomeWork"));
					}
				}

				await this.processingService.ProcessHomeWork(repositoryId, taskId, User.Identity.GetUserId());

				return RedirectToAction("Index", "HomeWorks");
			}
			catch (UnauthorizedAccessException)
			{
				return RedirectToAction("Index", "Home");
			}
		}
	}
}