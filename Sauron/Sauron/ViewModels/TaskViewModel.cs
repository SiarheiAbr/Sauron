using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sauron.ViewModels
{
	public class TaskViewModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string GitHubUrl { get; set; }
	}
}