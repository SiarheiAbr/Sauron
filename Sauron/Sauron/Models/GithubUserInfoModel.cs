using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sauron.Models
{
	public class GitHubUserInfoModel
	{
		public string UserName { get; set; }

		public string GitHubAddress { get; set; }

		public IList<RepositoryModel> Repositories { get; set; }
	}
}