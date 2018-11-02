using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sauron.ViewModels
{
	public class CreateEditTaskViewModel
	{
		public Guid Id { get; set; }

		[Required]
		[DisplayName("Name")]
		public string Name { get; set; }

		[Required]
		[DisplayName("GitHub Url")]
		public string GitHubUrl { get; set; }

		[DisplayName("Hidden Tests File")]
		public HttpPostedFileBase HiddenTestFile { get; set; }

		public bool HiddenTestsUploaded { get; set; }

		[DisplayName("Tests File Name(in the task's repo)")]
		public string TestsFileName { get; set; }
	}
}