using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sauron.ViewModels
{
	public class StudentResultsViewModel
	{
		public IList<HomeWorkViewModel> HomeWorks { get; set; }

		public StudentViewModel StudentInfo { get; set; }
	}
}