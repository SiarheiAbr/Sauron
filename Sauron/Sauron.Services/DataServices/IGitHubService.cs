﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sauron.Services.Models;

namespace Sauron.Services.DataServices
{
	public interface IGitHubService
	{
		Task<IList<GitHubRepositoryModel>> GetUserRepositories();

		Task DownloadRepository(long repositoryId, Guid taskId);

		Task<bool> IsForkOfRepo(long repositoryId, string parentRepoUrl);

		Task<GitHubRepositoryModel> GetRepositoryInfo(long repositoryId);
	}
}