using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sauron.ViewModels
{
	public class HomeWorkViewModel
	{
		public Guid Id { get; set; }

		public Guid TaskId { get; set; }

		public string UserId { get; set; }

		public string TestsResults { get; set; }

		public bool IsBuildSuccessful { get; set; }

		public string TaskName { get; set; }

		public string TaskGitUrl { get; set; }

		public string RepoGitUrl { get; set; }

		public int AttempsCount { get; set; }
	}
}