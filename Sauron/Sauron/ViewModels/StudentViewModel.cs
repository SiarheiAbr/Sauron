using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sauron.ViewModels
{
	public class StudentViewModel
	{
		public string Name { get; set; }

		public int SubmittedHomeWorks { get; set; }

		public string UserId { get; set; }

		public int TotalScore { get; set; }
	}
}