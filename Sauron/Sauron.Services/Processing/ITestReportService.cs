using System;
using System.Threading.Tasks;

namespace Sauron.Services.Processing
{
	public interface ITestReportService
	{
		Task<string> GenerateTestReportForHomeWork(string userId, Guid taskId);
	}
}
