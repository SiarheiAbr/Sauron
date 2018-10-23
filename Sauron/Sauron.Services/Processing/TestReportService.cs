using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sauron.Services.DataServices;
using Sauron.Services.Helpers;
using Sauron.Services.Models;
using Sauron.Services.Settings;

namespace Sauron.Services.Processing
{
	public class TestReportService : ITestReportService
	{
		private readonly IServicesConfig config;
		private readonly IHomeWorksService homeWorksService;

		public TestReportService(IServicesConfig config, IHomeWorksService homeWorksService)
		{
			this.config = config;
			this.homeWorksService = homeWorksService;
		}

		public async Task<string> GenerateTestReportForHomeWork(Guid homeWorkId)
		{
			var homeWork = await this.homeWorksService.GetHomeWork(homeWorkId);

			var testReportHtml = this.GenerateTestReport(homeWorkId, homeWork.TestsResults);

			return testReportHtml;
		}

		private string GenerateTestReport(Guid homeWorkId, string xmlTestsResults)
		{
			var testReportFolderPath = string.Format(this.config.TempTestReportsPathTemplate, homeWorkId);

			if (!Directory.Exists(testReportFolderPath))
			{
				Directory.CreateDirectory(testReportFolderPath);
			}
			else
			{
				DirectoryHelper.CleanDirectory(testReportFolderPath);
			}

			var testsResultsXmlPath = Path.Combine(Path.Combine(testReportFolderPath), "testsResults.xml");
			var testsResultsHtmlPath = Path.Combine(Path.Combine(testReportFolderPath), "testsResults.html");

			File.WriteAllText(testsResultsXmlPath, xmlTestsResults);

			var reportGenerator = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"NUnitReportGenerator\\ReportUnit.exe"));

			using (Process process = new Process())
			{
				process.StartInfo = new ProcessStartInfo
				{
					FileName = reportGenerator,
					Arguments = $"{testsResultsXmlPath} {testsResultsHtmlPath}",
					UseShellExecute = false,
					CreateNoWindow = true
				};

				process.Start();
				process.WaitForExit();
			}

			var reportHtml = File.ReadAllText(testsResultsHtmlPath);

			DirectoryHelper.CleanDirectory(testReportFolderPath);
			DirectoryHelper.DeleteDirectory(testReportFolderPath);

			return reportHtml;
		}
	}
}
