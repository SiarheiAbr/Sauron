using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Sauron.Identity.Models;
using Sauron.Services.Models;
using Sauron.ViewModels;

namespace Sauron.App_Start
{
	public static class AutoMapperRegistration
	{
		public static void RegisterMappings(IMapperConfigurationExpression configuration)
		{
			configuration.CreateMap<HomeWorkModel, HomeWorkViewModel>();
			configuration.CreateMap<StudentModel, StudentViewModel>();
			configuration.CreateMap<GitHubRepositoryModel, GitHubRepositoryViewModel>();
			configuration.CreateMap<TaskModel, TaskViewModel>();
			configuration.CreateMap<TaskModel, CreateEditTaskViewModel>();
			Sauron.Services.AutoMapperRegistration.RegisterMappings(configuration);
		}
	}
}