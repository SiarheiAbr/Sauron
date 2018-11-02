using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
		private readonly ITasksService tasksService;

		public ProcessingService(
			IGitHubService gitHubService,
			IRepositoryService repositoryService,
			IBuildService buildService,
			ITestRunnerService testRunnerService,
			IHomeWorksService homeWorksService,
			ITasksService tasksService)
		{
			this.gitHubService = gitHubService;
			this.repositoryService = repositoryService;
			this.buildService = buildService;
			this.testRunnerService = testRunnerService;
			this.homeWorksService = homeWorksService;
			this.tasksService = tasksService;
		}

		public async Task<bool> IsSubmittedRepoIsForkOfTask(long repositoryId, Guid taskId)
		{
			var task = await this.tasksService.GetTask(taskId);

			return await this.gitHubService.IsForkOfRepo(repositoryId, task.GitHubUrl);
		}

		public async Task ProcessHomeWork(long repositoryId, Guid taskId, string userId)
		{
			var repoInfo = await this.gitHubService.GetRepositoryInfo(repositoryId);

			var homeWork = new HomeWorkModel()
			{
				TaskId = taskId,
				UserId = userId,
				IsBuildSuccessful = false,
				TestsResults = null,
				RepoGitUrl = repoInfo.GitUrl
			};
			
			await this.gitHubService.DownloadRepository(repositoryId, taskId);

			await this.repositoryService.ExtractRepository(repositoryId, taskId);

			await this.repositoryService.PutHiddenTestsIntoRepository(repositoryId, taskId);

			await this.repositoryService.CopyNugetRunnerIntoRepository(repositoryId, taskId);

			var localRepositoryInfo = this.repositoryService.GetLocalRepositoryInfo(repositoryId, taskId);

			try
			{
				var buildResult = await this.buildService.BuildRepositorySolution(repositoryId, taskId);

				if (buildResult.OverallResult == BuildResultCode.Success)
				{
					homeWork.IsBuildSuccessful = true;
					var testsResult = await this.testRunnerService.RunUnitTests(repositoryId, taskId);
					homeWork.TestsResults = testsResult;
				}
			}
			catch (Exception e)
			{
				// TODO: log exception for user build
				// TODO: save build info into database???
			}
			finally
			{
				await this.homeWorksService.SaveHomeWork(homeWork);

				DirectoryHelper.CleanDirectory(localRepositoryInfo.RepositoryFolderPath);

				DirectoryHelper.DeleteDirectory(localRepositoryInfo.RepositoryFolderPath);
			}
		}
	}
}
