using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Sauron.Identity.Services;
using Sauron.Services.DataServices;
using Sauron.Services.Helpers;
using Sauron.Services.Models;
using Sauron.Services.Settings;
using ILogger = Sauron.Common.Logger.ILogger;

namespace Sauron.Services.Processing
{
	public class RepositoryService : IRepositoryService
	{
		private readonly IServicesConfig config;
		private readonly IUserIdentityService userIdentityService;
		private readonly ITasksService tasksService;
		private readonly ILogger logger;

		public RepositoryService(
			IUserIdentityService userIdentityService,
			IServicesConfig config,
			ITasksService tasksService,
			ILogger logger)
		{
			this.config = config;
			this.userIdentityService = userIdentityService;
			this.tasksService = tasksService;
			this.logger = logger;
		}

		public async Task ExtractRepository(long repositoryId, Guid taskId)
		{
			var zipPath = this.GetZipRepositoryPath(repositoryId, taskId);
			var extractPath = this.GetRepositoryFolderPath(repositoryId, taskId);

			using (var archive = ZipFile.OpenRead(zipPath))
			{
				foreach (ZipArchiveEntry file in archive.Entries)
				{
					string completeFileName = Path.Combine(extractPath, file.FullName);
					string directory = Path.GetDirectoryName(completeFileName);

					if (!Directory.Exists(directory))
					{
						Directory.CreateDirectory(directory);
					}

					if (!string.IsNullOrEmpty(file.Name))
					{
						file.ExtractToFile(completeFileName, true);
					}
				}
			}

			await Task.FromResult(0);
		}

		public async Task CopyNugetRunnerIntoRepository(long repositoryId, Guid taskId)
		{
			var nugetPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"nuget\\nuget.exe"));
			var info = this.GetLocalRepositoryInfo(repositoryId, taskId);

			using (var inputFile = new FileStream(
				nugetPath,
				FileMode.Open,
				FileAccess.Read,
				FileShare.ReadWrite))
			{
				using (var outputFile = new FileStream(info.NugetPath, FileMode.Create))
				{
					var buffer = new byte[inputFile.Length];

					await inputFile.ReadAsync(buffer, 0, buffer.Length);
					await outputFile.WriteAsync(buffer, 0, buffer.Length);
				}
			}
		}

		public async Task PutHiddenTestsIntoRepository(long repositoryId, Guid taskId)
		{
			var task = await this.tasksService.GetTask(taskId);

			if (!string.IsNullOrEmpty(task.TestsFileName) && task.HiddenTestsUploaded)
			{
				var hiddenTestsPath = Path.Combine(string.Format(this.config.HiddenTestsPathTemplate, taskId), task.TestsFileName);

				if (!File.Exists(hiddenTestsPath))
				{
					throw new FileNotFoundException("Configuration of hidden tests files is wrong");
				}

				var repositoryFolderPath = this.GetRepositoryFolderPath(repositoryId, taskId);
				var testsFiles = System.IO.Directory.GetFiles(repositoryFolderPath, task.TestsFileName, SearchOption.AllDirectories);

				if (testsFiles.Length <= 0)
				{
					throw new FileNotFoundException("There is no tests files in repository");
				}

				if (testsFiles.Length > 1)
				{
					throw new Exception("More than one tests file in the repository");
				}

				var testsFile = testsFiles[0];

				using (var inputFile = new FileStream(
					hiddenTestsPath,
					FileMode.Open,
					FileAccess.Read,
					FileShare.ReadWrite))
				{ 
					using (var outputFile = new FileStream(testsFile, FileMode.Create))
					{
						var buffer = new byte[inputFile.Length];

						await inputFile.ReadAsync(buffer, 0, buffer.Length);
						await outputFile.WriteAsync(buffer, 0, buffer.Length);
					}
				}
			}
		}

		public async Task SaveRepository(long repositoryId, Guid taskId, byte[] archiveData)
		{
			var repositoryFolderPath = this.GetRepositoryFolderPath(repositoryId, taskId);

			if (!Directory.Exists(repositoryFolderPath))
			{
				Directory.CreateDirectory(repositoryFolderPath);
			}
			else
			{
				DirectoryHelper.CleanDirectory(repositoryFolderPath, this.logger);
			}

			using (FileStream stream = File.Open(this.GetZipRepositoryPath(repositoryId, taskId), FileMode.Create, FileAccess.ReadWrite))
			{
				await stream.WriteAsync(archiveData, 0, archiveData.Length);
			}
		}

		public LocalRepositoryModel GetLocalRepositoryInfo(long repositoryId, Guid taskId)
		{
			var solutionFolderPath = this.GetSolutionFolderPath(repositoryId, taskId);

			var projectFilePath = this.GetProjectFilePath(repositoryId, taskId);
			var projectFolderPath = Path.GetDirectoryName(projectFilePath);
			var outputPath = Path.Combine(projectFolderPath, @"bin\\Debug");

			var match = Regex.Match(projectFilePath, "(\\w+).csproj$");
			var projectDllName = string.Concat(match.Groups[1].Value, ".dll");
			var projectDllPath = Path.Combine(outputPath, projectDllName);

			var localRepositoryInfo = new LocalRepositoryModel()
			{
				SolutionFilePath = this.GetSolutionFilePath(repositoryId, taskId),
				ProjectFilePath = projectFilePath,
				RepositoryFolderPath = this.GetRepositoryFolderPath(repositoryId, taskId),
				SolutionFolderPath = solutionFolderPath,
				OutputPath = outputPath,
				ProjectDllPath = projectDllPath,
				CombinedGuid = string.Concat(this.userIdentityService.GetUserId(), repositoryId, taskId),
				NugetPath = Path.Combine(solutionFolderPath, "nuget.exe")
			};

			return localRepositoryInfo;
		}

		private string GetSolutionFolderPath(long repositoryId, Guid taskId)
		{
			return Path.GetDirectoryName(this.GetSolutionFilePath(repositoryId, taskId));
		}

		private string GetSolutionFilePath(long repositoryId, Guid taskId)
		{
			var repositoryFolderPath = this.GetRepositoryFolderPath(repositoryId, taskId);
			var solutionFiles = System.IO.Directory.GetFiles(repositoryFolderPath, "*.sln", SearchOption.AllDirectories);

			if (solutionFiles.Length <= 0)
			{
				throw new FileNotFoundException("There is no solution file in repository");
			}

			if (solutionFiles.Length > 1)
			{
				throw new Exception("More than one solution files in the repository");
			}

			return solutionFiles[0];
		}

		private string GetProjectFilePath(long repositoryId, Guid taskId)
		{
			var repositoryFolderPath = this.GetRepositoryFolderPath(repositoryId, taskId);
			var projectFiles = System.IO.Directory.GetFiles(repositoryFolderPath, "*.csproj", SearchOption.AllDirectories);

			if (projectFiles.Length <= 0)
			{
				throw new FileNotFoundException("There is no project file in repository");
			}

			if (projectFiles.Length > 1)
			{
				throw new Exception("More than one project files in the repository");
			}

			return projectFiles[0];
		}

		private string GetRepositoryFolderPath(long repositoryId, Guid taskId)
		{
			return string.Format(this.config.DownloadRepositoryPathTemplate, this.userIdentityService.GetUserId(), repositoryId, taskId);
		}

		private string GetZipRepositoryPath(long repositoryId, Guid taskId)
		{
			var zipName = $"{repositoryId}.zip";
			return Path.Combine(this.GetRepositoryFolderPath(repositoryId, taskId), zipName);
		}
	}
}
