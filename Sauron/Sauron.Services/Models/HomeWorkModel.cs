using System;

namespace Sauron.Services.Models
{
	public class HomeWorkModel
	{
		public Guid Id { get; set; }

		public Guid TaskId { get; set; }

		public string UserId { get; set; }

		public string TestsResults { get; set; }

		public bool IsBuildSuccessful { get; set; }

		public string TaskName { get; set; }

		public string TaskGitUrl { get; set; }

		public string RepoGitUrl { get; set; }

		public int AttemptsCount { get; set; }

		public int TestsMark { get; set; }
	}
}
