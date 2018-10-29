using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sauron.ViewModels
{
	public class TasksManageViewModel
	{
		public IList<TaskViewModel> Tasks { get; set; }
	}
}