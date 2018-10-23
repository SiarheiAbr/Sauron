using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Services.Models
{
	public class HomeWorkModel
	{
		public Guid Id { get; set; }

		public Guid TaskId { get; set; }

		public string UserId { get; set; }

		public string TestsResults { get; set; }

		public bool IsBuildSuccessful { get; set; }

		public string TaskName { get; set; }
	}
}
