using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Services.Models
{
	public class TaskModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string GitHubUrl { get; set; }

		public string TestsFileName { get; set; }

		public bool HiddenTestsUploaded { get; set; }

		public Stream HiddenTestsFile { get; set; }
	}
}
