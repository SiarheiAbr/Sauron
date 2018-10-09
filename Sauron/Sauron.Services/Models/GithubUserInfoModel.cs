using System.Collections.Generic;

namespace Sauron.Services.Models
{
	public class GitHubUserInfoModel
	{
		public string UserName { get; set; }

		public string GitHubAddress { get; set; }

		public IList<RepositoryModel> Repositories { get; set; }
	}
}