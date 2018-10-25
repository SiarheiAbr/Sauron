using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Sauron.Identity.Services;
using Sauron.Services.Helpers;
using Sauron.Services.Models;
using Sauron.Services.Settings;

namespace Sauron.Services.Processing
{
	public class RepositoryService : IRepositoryService
	{
		private readonly IServicesConfig config;
		private readonly IUserIdentityService userIdentityService;

		public RepositoryService(
			IUserIdentityService userIdentityService,
			IServicesConfig config)
		{
			this.config = config;
			this.userIdentityService = userIdentityService;
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

		public async Task SaveRepository(long repositoryId, Guid taskId, byte[] archiveData)
		{
			var repositoryFolderPath = this.GetRepositoryFolderPath(repositoryId, taskId);

			if (!Directory.Exists(repositoryFolderPath))
			{
				Directory.CreateDirectory(repositoryFolderPath);
			}
			else
			{
				DirectoryHelper.CleanDirectory(repositoryFolderPath);
			}

			using (FileStream stream = File.Open(this.GetZipRepositoryPath(repositoryId, taskId), FileMode.Create, FileAccess.ReadWrite))
			{
				await stream.WriteAsync(archiveData, 0, archiveData.Length);
			}
		}

		public LocalRepositoryModel GetLocalRepositoryInfo(long repositoryId, Guid taskId)
		{
			var solutionFolderPath = this.GetSolutionFolderPath(repositoryId, taskId);
			var outputPath = string.Format(this.config.OutputPathTemplate, solutionFolderPath);

			var localRepositoryInfo = new LocalRepositoryModel()
			{
				SolutionFilePath = this.GetSolutionFilePath(repositoryId, taskId),
				ProjectFilePath = this.GetProjectFilePath(repositoryId, taskId),
				RepositoryFolderPath = this.GetRepositoryFolderPath(repositoryId, taskId),
				SolutionFolderPath = solutionFolderPath,
				OutputPath = outputPath,
				ProjectDllPath = Path.Combine(outputPath, "SimpleCalc.dll")
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
			var solutionFiles = System.IO.Directory.GetFiles(repositoryFolderPath, "*.csproj", SearchOption.AllDirectories);

			if (solutionFiles.Length <= 0)
			{
				throw new FileNotFoundException("There is no project file in repository");
			}

			if (solutionFiles.Length > 1)
			{
				throw new Exception("More than one project files in the repository");
			}

			return solutionFiles[0];
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
