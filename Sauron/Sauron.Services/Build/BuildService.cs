using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Utilities;
using Sauron.Services.Repository;
using Task = System.Threading.Tasks.Task;

namespace Sauron.Services.Build
{
	public class BuildService : IBuildService
	{
		private readonly IRepositoryService repositoryService;

		public BuildService(IRepositoryService repositoryService)
		{
			this.repositoryService = repositoryService;
		}

		public async Task BuildRepository(long repositoriId)
		{
			var pathToSolutionFile = this.repositoryService.GetSolutionFilePath(repositoriId);
			var pathToRepository = this.repositoryService.GetRepositoryFolderPath(repositoriId);
			string toolsPath = ToolLocationHelper.GetPathToBuildToolsFile("msbuild.exe", ToolLocationHelper.CurrentToolsVersion);

			////var logger = new FileLogger
			////{
			////	Verbosity = LoggerVerbosity.Detailed,
			////	ShowSummary = true,
			////	SkipProjectStartedText = true 
			////};

			////var loggers = new List<Microsoft.Build.Framework.ILogger>()
			////{
			////	logger
			////};

			var projectCollection = new ProjectCollection();

			projectCollection.AddToolset(new Toolset(ToolLocationHelper.CurrentToolsVersion, this.GetToolsPath(), projectCollection, string.Empty));

			////pc.Loggers.Add(logger);

			var globalProperties = new Dictionary<string, string>();

			globalProperties.Add("Configuration", "Debug");

			globalProperties.Add("Platform", "Any CPU");

			//// globalProperties.Add("OutputPath", string.Concat(pathToRepository, @"build\bin\Release"));

			globalProperties.Add("OutputPath", @"D:\Output");

			BuildParameters bp = new BuildParameters(projectCollection)
			{
				////Loggers = loggers,
				OnlyLogCriticalEvents = false,
				DetailedSummary = true
			};

			//// BuildManager.DefaultBuildManager.BeginBuild(bp);

			//// var buildRequest = new BuildRequestData(pathToSolutionFile, globalProperties, null, new string[] { "Build" }, null);

			//// var buildSubmission = BuildManager.DefaultBuildManager.PendBuildRequest(buildRequest);

			//// buildSubmission.Execute();

			//// BuildManager.DefaultBuildManager.EndBuild();

			var buildRequest = new BuildRequestData(pathToSolutionFile, globalProperties, null, new string[] { "Build" }, null);

			BuildResult buildResult = BuildManager.DefaultBuildManager.Build(bp, buildRequest);

			//// if (buildSubmission.BuildResult.OverallResult == BuildResultCode.Failure)
			//// {
			////	throw new Exception();
			//// }

			await Task.FromResult(0);
		}

		private string GetToolsPath()
		{
			string toolsPath = ToolLocationHelper.GetPathToBuildToolsFile("msbuild.exe", ToolLocationHelper.CurrentToolsVersion);

			if (string.IsNullOrEmpty(toolsPath))
			{
				toolsPath = this.PollForToolsPath().FirstOrDefault();
			}

			if (string.IsNullOrEmpty(toolsPath))
			{
				throw new Exception("Could not locate the tools (MSBuild) path.");
			}

			return Path.GetDirectoryName(toolsPath);
		}

		private string[] PollForToolsPath()
		{
			var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

			return new[]
			{
				Path.Combine(programFilesX86, @"Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"),
				Path.Combine(programFilesX86, @"Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"),
				Path.Combine(programFilesX86, @"Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"),
				Path.Combine(programFilesX86, @"MSBuild\14.0\Bin\MSBuild.exe"),
				Path.Combine(programFilesX86, @"MSBuild\12.0\Bin\MSBuild.exe")
			}.Where(File.Exists).ToArray();
		}
	}
}
