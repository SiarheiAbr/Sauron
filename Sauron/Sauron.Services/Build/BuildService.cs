using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

		// TODO: Check build with changed downloaded repo, delete multiple solutions if exist
		public BuildService(IRepositoryService repositoryService)
		{
			this.repositoryService = repositoryService;
		}

		public async Task BuildRepositorySingleProject(long repositoriId)
		{
			var pathToSolutionFile = this.repositoryService.GetSolutionFilePath(repositoriId);
			var pathToProjectFile = this.repositoryService.GetProjectFilePath(repositoriId);

			string toolsPath = this.GetToolsPath();
			var globalProperties = this.GetGlobalProperties(pathToProjectFile, toolsPath);

			StringBuilder logBuilder = new StringBuilder();
			ConsoleLogger logger = new Microsoft.Build.Logging.ConsoleLogger(LoggerVerbosity.Normal, x => logBuilder.Append(x), null, null);

			this.RestoreNugetPackages(pathToSolutionFile);

			using (var projectCollection = new ProjectCollection(globalProperties))
			{
				projectCollection.AddToolset(new Toolset(ToolLocationHelper.CurrentToolsVersion, toolsPath, projectCollection, toolsPath));
				this.SetEnvironmentVariables(globalProperties);

				var project = projectCollection.LoadProject(pathToProjectFile);

				projectCollection.RegisterLogger(logger);

				try
				{
					var projectInstance = project.CreateProjectInstance();

					projectInstance.Build("Build", new ILogger[] { logger });
				}
				finally
				{
					projectCollection.UnloadAllProjects();
					projectCollection.UnregisterAllLoggers();
				}
			}

			await Task.FromResult(0);
		}

		public async Task BuildRepositorySolution(long repositoriId)
		{
			var pathToSolutionFile = this.repositoryService.GetSolutionFilePath(repositoriId);
			string toolsPath = this.GetToolsPath();
			var globalProperties = this.GetGlobalProperties(pathToSolutionFile, toolsPath);

			StringBuilder logBuilder = new StringBuilder();
			ConsoleLogger logger = new Microsoft.Build.Logging.ConsoleLogger(LoggerVerbosity.Normal, x => logBuilder.Append(x), null, null);

			this.RestoreNugetPackages(pathToSolutionFile);

			using (var projectCollection = new ProjectCollection(globalProperties))
			{
				projectCollection.AddToolset(new Toolset(ToolLocationHelper.CurrentToolsVersion, toolsPath, projectCollection, toolsPath));
				this.SetEnvironmentVariables(globalProperties);

				projectCollection.RegisterLogger(logger);

				try
				{
					BuildParameters bp = new BuildParameters(projectCollection)
					{
						Loggers = new List<ILogger>()
						{
							logger
						}
					};

					var buildRequest = new BuildRequestData(pathToSolutionFile, globalProperties, null, new string[] { "Build" }, null);

					BuildResult buildResult = BuildManager.DefaultBuildManager.Build(bp, buildRequest);
				}
				finally
				{
					projectCollection.UnloadAllProjects();
					projectCollection.UnregisterAllLoggers();
				}
			}

			await Task.FromResult(0);
		}

		private void RestoreNugetPackages(string solutionPath)
		{
			var nugetPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\\nuget\\nuget.exe"));

			using (Process process = new Process())
			{
				process.StartInfo = new ProcessStartInfo
				{
					FileName = nugetPath,
					Arguments = $"restore {solutionPath}",
					UseShellExecute = false,
					CreateNoWindow = true
				};

				process.Start();
				process.WaitForExit();
			}
		}

		private void SetEnvironmentVariables(Dictionary<string, string> globalProperties)
		{
			Environment.SetEnvironmentVariable("MSBuildExtensionsPath", globalProperties["MSBuildExtensionsPath"]);
			Environment.SetEnvironmentVariable("MSBuildFrameworkToolsPath", globalProperties["MSBuildFrameworkToolsPath"]);
			Environment.SetEnvironmentVariable("MSBuildToolsPath32", globalProperties["MSBuildToolsPath32"]);
			Environment.SetEnvironmentVariable("MSBuildSDKsPath", globalProperties["MSBuildSDKsPath"]);
			Environment.SetEnvironmentVariable("RoslynTargetsPath", globalProperties["RoslynTargetsPath"]);
		}

		private Dictionary<string, string> GetGlobalProperties(string projectPath, string toolsPath)
		{
			string solutionDir = Path.GetDirectoryName(projectPath);
			string extensionsPath = Path.GetFullPath(Path.Combine(toolsPath, @"..\..\"));
			string sdksPath = Path.Combine(extensionsPath, "Sdks");
			string roslynTargetsPath = Path.Combine(toolsPath, "Roslyn");

			return new Dictionary<string, string>
			{
				{ "SolutionDir", solutionDir },
				{ "MSBuildExtensionsPath", extensionsPath },
				{ "MSBuildSDKsPath", sdksPath },
				{ "MSBuildFrameworkToolsPath", toolsPath },
				{ "MSBuildToolsPath32", toolsPath },
				{ "RoslynTargetsPath", roslynTargetsPath },
				{ "Configuration", "Release" },
				{ "Platform", "Any CPU" },
				{ "OutputPath", string.Concat(solutionDir, @"\build\bin\Release") }
			};
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
	}
}
