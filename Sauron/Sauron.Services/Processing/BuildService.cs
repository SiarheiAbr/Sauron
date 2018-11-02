using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Utilities;
using Sauron.Services.Helpers;
using Task = System.Threading.Tasks.Task;

namespace Sauron.Services.Processing
{
	public class BuildService : IBuildService
	{
		private readonly IRepositoryService repositoryService;

		public BuildService(IRepositoryService repositoryService)
		{
			this.repositoryService = repositoryService;
		}

		public async Task BuildRepositorySingleProject(long repositoriId, Guid taskId)
		{
			var localRepositoryInfo = this.repositoryService.GetLocalRepositoryInfo(repositoriId, taskId);

			string toolsPath = this.GetToolsPath();
			var globalProperties = this.GetGlobalProperties(localRepositoryInfo.ProjectFilePath, toolsPath, localRepositoryInfo.OutputPath);

			StringBuilder logBuilder = new StringBuilder();
			ConsoleLogger logger = new Microsoft.Build.Logging.ConsoleLogger(LoggerVerbosity.Normal, x => logBuilder.Append(x), null, null);

			await this.RestoreNugetPackages(localRepositoryInfo.SolutionFilePath, localRepositoryInfo.NugetPath);

			using (var projectCollection = new ProjectCollection(globalProperties))
			{
				projectCollection.AddToolset(new Toolset(ToolLocationHelper.CurrentToolsVersion, toolsPath, projectCollection, toolsPath));
				this.SetEnvironmentVariables(globalProperties);

				var project = projectCollection.LoadProject(localRepositoryInfo.ProjectFilePath);

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

		public async Task<BuildResult> BuildRepositorySolution(long repositoryId, Guid taskId)
		{
			var localRepositoryInfo = this.repositoryService.GetLocalRepositoryInfo(repositoryId, taskId);
			BuildResult buildResult = null;

			string toolsPath = this.GetToolsPath();
			var globalProperties = this.GetGlobalProperties(localRepositoryInfo.SolutionFilePath, toolsPath, localRepositoryInfo.OutputPath);

			StringBuilder logBuilder = new StringBuilder();
			ConsoleLogger logger = new Microsoft.Build.Logging.ConsoleLogger(LoggerVerbosity.Normal, x => logBuilder.Append(x), null, null);

			var restoreExitCode = await this.RestoreNugetPackages(localRepositoryInfo.SolutionFilePath, localRepositoryInfo.NugetPath);

			using (var projectCollection = new ProjectCollection(globalProperties))
			{
				projectCollection.AddToolset(new Toolset(ToolLocationHelper.CurrentToolsVersion, toolsPath, projectCollection, toolsPath));
				this.SetEnvironmentVariables(globalProperties);

				projectCollection.RegisterLogger(logger);

				try
				{
					using (var buildManager = new BuildManager(localRepositoryInfo.CombinedGuid))
					{
						BuildParameters bp = new BuildParameters(projectCollection)
						{
							Loggers = new List<ILogger>()
							{
								logger
							}
						};

						var buildRequest = new BuildRequestData(localRepositoryInfo.SolutionFilePath, globalProperties, null, new string[] { "Build" }, null, BuildRequestDataFlags.ReplaceExistingProjectInstance);

						BuildSubmission submission = null;

						buildManager.BeginBuild(bp);
						submission = buildManager.PendBuildRequest(buildRequest);

						buildResult = await submission.ExecuteAsync();
						buildManager.EndBuild();
					}
				}
				finally
				{
					projectCollection.UnloadAllProjects();
					projectCollection.UnregisterAllLoggers();
				}
			}

			return buildResult;
		}

		private async Task<int> RestoreNugetPackages(string solutionPath, string nugetPath)
		{
			var arguments = $"restore \"{solutionPath}\"";

			return await AsyncProcessRunnerHelper.RunProcessAsync(nugetPath, arguments);
		}

		private void SetEnvironmentVariables(Dictionary<string, string> globalProperties)
		{
			Environment.SetEnvironmentVariable("MSBuildExtensionsPath", globalProperties["MSBuildExtensionsPath"]);
			Environment.SetEnvironmentVariable("MSBuildFrameworkToolsPath", globalProperties["MSBuildFrameworkToolsPath"]);
			Environment.SetEnvironmentVariable("MSBuildToolsPath32", globalProperties["MSBuildToolsPath32"]);
			Environment.SetEnvironmentVariable("MSBuildSDKsPath", globalProperties["MSBuildSDKsPath"]);
			Environment.SetEnvironmentVariable("RoslynTargetsPath", globalProperties["RoslynTargetsPath"]);
		}

		private Dictionary<string, string> GetGlobalProperties(string projectPath, string toolsPath, string outputPath)
		{
			string extensionsPath = Path.GetFullPath(Path.Combine(toolsPath, @"..\..\"));
			string sdksPath = Path.Combine(extensionsPath, "Sdks");
			string roslynTargetsPath = Path.Combine(toolsPath, "Roslyn");

			return new Dictionary<string, string>
			{
				{ "MSBuildExtensionsPath", extensionsPath },
				{ "MSBuildSDKsPath", sdksPath },
				{ "MSBuildFrameworkToolsPath", toolsPath },
				{ "MSBuildToolsPath32", toolsPath },
				{ "RoslynTargetsPath", roslynTargetsPath }
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
				Path.Combine(programFilesX86, @"Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe"),
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
