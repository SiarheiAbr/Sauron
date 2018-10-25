using System;
using System.Threading.Tasks;

namespace Sauron.Services.Processing.TestRunner
{
    public interface ITestRunnerService
    {
		 Task<string> RunUnitTests(long repositoryId, Guid taskId);
    }
}
