using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Data.Entities
{
	public class GitHubRepositoryEntity
	{
		public string Name { get; set; }

		public string Url { get; set; }

		public string GitUrl { get; set; }

		public long Id { get; set; }
	}
}
