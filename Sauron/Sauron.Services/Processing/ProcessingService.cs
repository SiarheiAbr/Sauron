using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Execution;
using Sauron.Services.DataServices;
using Sauron.Services.Helpers;
using Sauron.Services.Models;
using Sauron.Services.Processing.TestRunner;

namespace Sauron.Services.Processing
{
	public class ProcessingService : IProcessingService
	{
		private readonly IGitHubService gitHubService;
		private readonly IBuildService buildService;
		private readonly IRepositoryService repositoryService;
		private readonly ITestRunnerService testRunnerService;
		private readonly IHomeWorksService homeWorksService;

		public ProcessingService(
			IGitHubService gitHubService,
			IRepositoryService repositoryService,
			IBuildService buildService,
			ITestRunnerService testRunnerService,
			IHomeWorksService homeWorksService)
		{
			this.gitHubService = gitHubService;
			this.repositoryService = repositoryService;
			this.buildService = buildService;
			this.testRunnerService = testRunnerService;
			this.homeWorksService = homeWorksService;
		}

		public async Task ProcessHomeWork(long repositoryId, Guid taskId, string userId)
		{
			var homeWork = new HomeWorkModel()
			{
				TaskId = taskId,
				UserId = userId,
				IsBuildSuccessful = false,
				TestsResults = null
			};

			await this.gitHubService.DownloadRepository(repositoryId, taskId);
			await this.repositoryService.ExtractRepository(repositoryId, taskId);
			var localRepositoryInfo = this.repositoryService.GetLocalRepositoryInfo(repositoryId, taskId);
			var buildResult = await this.buildService.BuildRepositorySolution(repositoryId, taskId);

			if (buildResult.OverallResult == BuildResultCode.Success)
			{
				homeWork.IsBuildSuccessful = true;
				var testsResult = await this.testRunnerService.RunUnitTests(repositoryId, taskId);
				homeWork.TestsResults = testsResult;
			}

			await this.homeWorksService.SaveHomeWork(homeWork);

			DirectoryHelper.CleanDirectory(localRepositoryInfo.RepositoryFolderPath);

			DirectoryHelper.DeleteDirectory(localRepositoryInfo.RepositoryFolderPath);
		}
	}
}
