using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Sauron.Services.Models;

namespace Sauron.Services.Processing.TestRunner
{
	public class TestRunnerService : ITestRunnerService
	{
		private readonly IRepositoryService repositoryService;

		public TestRunnerService(IRepositoryService repositoryService)
		{
			this.repositoryService = repositoryService;
		}

		public async Task<string> RunUnitTests(long repositoryId, Guid taskId)
		{
			var localRepositoryInfo = this.repositoryService.GetLocalRepositoryInfo(repositoryId, taskId);
			
			TestResults testResultsParams = new TestResults()
			{
				ProjectDllPath = localRepositoryInfo.ProjectDllPath,
				SolutionFolderPath = localRepositoryInfo.SolutionFolderPath.Replace("\\\\", "\\")
			};

			using (var isolatedRunner = new IsolatedWorkExecutor<TestRunner>())
			{
				isolatedRunner.Value.RunUnitTestsForAssembly(testResultsParams);
				return await testResultsParams.Task;
			}
		}
	}
}
