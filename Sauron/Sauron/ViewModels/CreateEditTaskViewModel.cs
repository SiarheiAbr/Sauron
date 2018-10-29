using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sauron.ViewModels
{
	public class CreateEditTaskViewModel
	{
		public Guid Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string GitHubUrl { get; set; }
	}
}