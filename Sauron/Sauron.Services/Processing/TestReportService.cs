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
using ILogger = Sauron.Common.Logger.ILogger;

namespace Sauron.Services.Processing
{
	public class TestReportService : ITestReportService
	{
		private readonly IServicesConfig config;
		private readonly IHomeWorksService homeWorksService;
		private readonly ILogger logger;

		public TestReportService(IServicesConfig config, IHomeWorksService homeWorksService, ILogger logger)
		{
			this.config = config;
			this.homeWorksService = homeWorksService;
			this.logger = logger;
		}

		public async Task<string> GenerateTestReportForHomeWork(string userId, Guid taskId)
		{
			var homeWork = await this.homeWorksService.GetHomeWork(userId, taskId);

			var testReportHtml = await this.GenerateTestReport(userId, homeWork.TaskId, homeWork.TestsResults);

			return testReportHtml;
		}

		private async Task<string> GenerateTestReport(string userId, Guid taskId, string xmlTestsResults)
		{
			var testReportFolderPath = string.Format(this.config.TempTestReportsPathTemplate, userId, taskId);

			if (!Directory.Exists(testReportFolderPath))
			{
				Directory.CreateDirectory(testReportFolderPath);
			}
			else
			{
				DirectoryHelper.CleanDirectory(testReportFolderPath, this.logger);
			}

			var testsResultsXmlPath = Path.Combine(Path.Combine(testReportFolderPath), "testsResults.xml");
			var testsResultsHtmlPath = Path.Combine(Path.Combine(testReportFolderPath), "testsResults.html");

			File.WriteAllText(testsResultsXmlPath, xmlTestsResults);

			var reportGenerator = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"NUnitReportGenerator\\ReportUnit.exe"));
			var arguments = $"{testsResultsXmlPath} {testsResultsHtmlPath}";

			await AsyncProcessRunnerHelper.RunProcessAsync(reportGenerator, arguments);

			var reportHtml = File.ReadAllText(testsResultsHtmlPath);
			reportHtml = reportHtml.Replace(testsResultsXmlPath, string.Empty);

			DirectoryHelper.CleanDirectory(testReportFolderPath, this.logger);
			DirectoryHelper.DeleteDirectory(testReportFolderPath, this.logger);

			return reportHtml;
		}
	}
}
