using AutoMapper;
using Sauron.Data.Entities;
using Sauron.Services.Models;

namespace Sauron.Services
{
	public static class AutoMapperRegistration
	{
		public static void RegisterMappings(IMapperConfigurationExpression configuration)
		{
			configuration.CreateMap<GitHubRepositoryEntity, GitHubRepositoryModel>();
			configuration.CreateMap<StudentEntity, StudentModel>();

			configuration
				.CreateMap<HomeWorkEntity, HomeWorkModel>()
				.ForMember(dest => dest.TaskName, opt => opt.MapFrom(src => src.Task.Name))
				.ForMember(dest => dest.TaskGitUrl, opt => opt.MapFrom(src => src.Task.GitHubUrl));

			configuration.CreateMap<HomeWorkModel, HomeWorkEntity>();

			configuration.CreateMap<TaskEntity, TaskModel>();
			configuration.CreateMap<TaskModel, TaskEntity>();
		}
	}
}
