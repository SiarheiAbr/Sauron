using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Build.Execution;
using Sauron.Identity.Services;
using Sauron.Services.DataServices;
using Sauron.Services.Helpers;
using Sauron.Services.Models;
using Sauron.Services.Processing.TestRunner;

namespace Sauron.Services.Processing
{
	public class ProcessingService : IProcessingService
	{
		public const int MaxBuildAttemptsCount = 3;

		private readonly IGitHubService gitHubService;
		private readonly IBuildService buildService;
		private readonly IRepositoryService repositoryService;
		private readonly ITestRunnerService testRunnerService;
		private readonly IHomeWorksService homeWorksService;
		private readonly ITasksService tasksService;
		private readonly IUserIdentityService userIdentityService;

		public ProcessingService(
			IGitHubService gitHubService,
			IRepositoryService repositoryService,
			IBuildService buildService,
			ITestRunnerService testRunnerService,
			IHomeWorksService homeWorksService,
			ITasksService tasksService,
			IUserIdentityService userIdentityService)
		{
			this.gitHubService = gitHubService;
			this.repositoryService = repositoryService;
			this.buildService = buildService;
			this.testRunnerService = testRunnerService;
			this.homeWorksService = homeWorksService;
			this.tasksService = tasksService;
			this.userIdentityService = userIdentityService;
		}

		public async Task<bool> IsSubmittedRepoIsForkOfTask(long repositoryId, Guid taskId)
		{
			var task = await this.tasksService.GetTask(taskId);

			return await this.gitHubService.IsForkOfRepo(repositoryId, task.GitHubUrl);
		}

		public async Task ProcessHomeWork(long repositoryId, Guid taskId, string userId)
		{
			HomeWorkModel homeWork = null;

			homeWork = await this.homeWorksService.GetHomeWork(userId, taskId);
			var repoInfo = await this.gitHubService.GetRepositoryInfo(repositoryId);

			if (homeWork == null)
			{
				homeWork = new HomeWorkModel()
				{
					TaskId = taskId,
					UserId = userId,
					IsBuildSuccessful = false,
					TestsResults = null,
					RepoGitUrl = repoInfo.GitUrl,
					AttemptsCount = 1
				};
			}
			else
			{
				if (homeWork.AttemptsCount >= MaxBuildAttemptsCount && !this.userIdentityService.IsAdmin())
				{
					throw new ApplicationException($"You can't submit your home work more than {MaxBuildAttemptsCount} times.");
				}

				homeWork.AttemptsCount++;
			}

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
			catch (Exception)
			{
				// TODO: log exception for user build
				// TODO: save build info into database???
			}
			finally
			{
				await this.homeWorksService.AddOrUpdateHomeWork(homeWork);

				DirectoryHelper.CleanDirectory(localRepositoryInfo.RepositoryFolderPath);

				DirectoryHelper.DeleteDirectory(localRepositoryInfo.RepositoryFolderPath);
			}
		}
	}
}
