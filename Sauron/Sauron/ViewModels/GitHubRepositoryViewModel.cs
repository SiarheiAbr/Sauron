using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sauron.ViewModels
{
	public class GitHubRepositoryViewModel
	{
		public string Name { get; set; }

		public string Url { get; set; }

		public string GitUrl { get; set; }

		public long Id { get; set; }
	}
}