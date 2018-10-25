using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Sauron.Services;
using Sauron.Services.DataServices;
using Sauron.Services.Models;
using Sauron.Services.Processing;
using Sauron.Services.Processing.TestRunner;
using Sauron.ViewModels;

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
		public async Task<ActionResult> DownloadRepository(long repositoryId, Guid taskId)
		{
			try
			{
				await this.processingService.ProcessHomeWork(repositoryId, taskId, User.Identity.GetUserId());
				return RedirectToAction("Index", "HomeWorks");
			}
			catch (UnauthorizedAccessException e)
			{
				return RedirectToAction("Index", "Home");
			}
		}
	}
}