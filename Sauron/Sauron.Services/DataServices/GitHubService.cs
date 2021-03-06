﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NUnit.Framework.Constraints;
using Sauron.Data.DataProviders;
using Sauron.Identity.Services;
using Sauron.Services.Models;
using Sauron.Services.Processing;

namespace Sauron.Services.DataServices
{
	public class GitHubService : IGitHubService
	{
		private readonly IGitHubIdentityService gitHubIdentityservice;
		private readonly IGitHubDataProvider gitHubDataProvider;
		private readonly IRepositoryService repositoryService;

		public GitHubService(
			IGitHubIdentityService gitHubIdentityservice,
			IGitHubDataProvider gitHubDataProvider,
			IRepositoryService repositoryService)
		{
			this.gitHubIdentityservice = gitHubIdentityservice;
			this.gitHubDataProvider = gitHubDataProvider;
			this.repositoryService = repositoryService;
		}

		public async Task<IList<GitHubRepositoryModel>> GetUserRepositories()
		{
			var accessToken = this.gitHubIdentityservice.GetAccessToken();
			List<GitHubRepositoryModel> resultList = null;

			try
			{
				var repositories = await this.gitHubDataProvider.GetUserRepositories(accessToken);

				resultList = Mapper.Map<List<GitHubRepositoryModel>>(repositories);
			}
			catch (Exception e)
			{
				throw new UnauthorizedAccessException("Cannot retrieve repositories using current access token");
			}

			return resultList;
		}

		public async Task DownloadRepository(long repositoryId, Guid taskId)
		{
			var accessToken = this.gitHubIdentityservice.GetAccessToken();

			byte[] repoArchive = null;

			try
			{
				repoArchive = await this.gitHubDataProvider.DownloadRepository(repositoryId, accessToken);
			}
			catch (Exception e)
			{
				throw new UnauthorizedAccessException("Cannot retrieve repository archive using current access token");
			}

			await this.repositoryService.SaveRepository(repositoryId, taskId, repoArchive);
		}

		public async Task<bool> IsForkOfRepo(long repositoryId, string parentRepoUrl)
		{
			var accessToken = this.gitHubIdentityservice.GetAccessToken();

			bool result = false;

			try
			{
				result = await this.gitHubDataProvider.IsForkOfRepo(repositoryId, parentRepoUrl, accessToken);
			}
			catch (Exception e)
			{
				throw new UnauthorizedAccessException("Cannot retrieve repository archive using current access token");
			}

			return result;
		}

		public async Task<GitHubRepositoryModel> GetRepositoryInfo(long repositoryId)
		{
			GitHubRepositoryModel result;

			var accessToken = this.gitHubIdentityservice.GetAccessToken();

			try
			{
				var entity = await this.gitHubDataProvider.GetRepository(repositoryId, accessToken);
				
				result = Mapper.Map<GitHubRepositoryModel>(entity);
			}
			catch (Exception e)
			{
				throw new UnauthorizedAccessException("Cannot retrieve repository archive using current access token");
			}

			return result;
		}
	}
}