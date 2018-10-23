using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sauron.Services.Models;

namespace Sauron.ViewModels
{
	public class TasksIndexViewModel
	{
		public List<TaskViewModel> Tasks { get; set; }

		public IList<GitHubRepositoryViewModel> Repositories { get; set; }
	}
}