using System.Threading.Tasks;
using System.Xml;
using Sauron.Services.Repository;

namespace Sauron.Services.TestRunner
{
	public class TestRunnerService : ITestRunnerService
	{
		private readonly IRepositoryService repositoryService;

		public TestRunnerService(IRepositoryService repositoryService)
		{
			this.repositoryService = repositoryService;
		}

		public async Task<string> RunUnitTests(long repositoryId)
		{
			var localRepositoryInfo = this.repositoryService.GetLocalRepositoryInfo(repositoryId);

			TestResults testResultsParams = new TestResults()
			{
				ProjectDllPath = localRepositoryInfo.ProjectDllPath
			};

			using (var isolatedRunner = new IsolatedWorkExecutor<TestRunner>())
			{
				isolatedRunner.Value.RunUnitTestsForAssembly(testResultsParams);
			}

			return await Task.FromResult(testResultsParams.Results);
		}
	}
}
