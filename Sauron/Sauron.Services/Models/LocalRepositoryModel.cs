using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Services.Models
{
	public class LocalRepositoryModel
	{
		public string SolutionFolderPath { get; set; }

		public string SolutionFilePath { get; set; }

		public string OutputPath { get; set; }

		public string ProjectFilePath { get; set; }

		public string RepositoryFolderPath { get; set; }

		public string ProjectDllPath { get; set; }
	}
}
