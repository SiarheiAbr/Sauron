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

		public async Task ExtractRepository(long repositoryId)
		{
			var zipPath = this.GetZipRepositoryPath(repositoryId);
			var extractPath = this.GetRepositoryFolderPath(repositoryId);

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

		public async Task SaveRepository(long repositoryId, byte[] archiveData)
		{
			var repositoryFolderPath = this.GetRepositoryFolderPath(repositoryId);

			if (!Directory.Exists(repositoryFolderPath))
			{
				Directory.CreateDirectory(repositoryFolderPath);
			}
			else
			{
				DirectoryHelper.CleanDirectory(repositoryFolderPath);
			}

			using (FileStream stream = File.Open(this.GetZipRepositoryPath(repositoryId), FileMode.Create, FileAccess.ReadWrite))
			{
				await stream.WriteAsync(archiveData, 0, archiveData.Length);
			}
		}

		public LocalRepositoryModel GetLocalRepositoryInfo(long repositoryId)
		{
			var solutionFolderPath = this.GetSolutionFolderPath(repositoryId);
			var outputPath = string.Format(this.config.OutputPathTemplate, solutionFolderPath);

			var localRepositoryInfo = new LocalRepositoryModel()
			{
				SolutionFilePath = this.GetSolutionFilePath(repositoryId),
				ProjectFilePath = this.GetProjectFilePath(repositoryId),
				RepositoryFolderPath = this.GetRepositoryFolderPath(repositoryId),
				SolutionFolderPath = solutionFolderPath,
				OutputPath = outputPath,
				ProjectDllPath = Path.Combine(outputPath, "SimpleCalc.dll")
			};

			return localRepositoryInfo;
		}

		private string GetSolutionFolderPath(long repositoryId)
		{
			return Path.GetDirectoryName(this.GetSolutionFilePath(repositoryId));
		}

		private string GetSolutionFilePath(long repositoryId)
		{
			var repositoryFolderPath = this.GetRepositoryFolderPath(repositoryId);
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

		private string GetProjectFilePath(long repositoryId)
		{
			var repositoryFolderPath = this.GetRepositoryFolderPath(repositoryId);
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

		private string GetRepositoryFolderPath(long repositoryId)
		{
			return string.Format(this.config.DownloadRepositoryPathTemplate, this.userIdentityService.GetUserId(), repositoryId);
		}

		private string GetZipRepositoryPath(long repositoryId)
		{
			var zipName = $"{repositoryId}.zip";
			return Path.Combine(this.GetRepositoryFolderPath(repositoryId), zipName);
		}
	}
}
