using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Data.Entities
{
	public class StudentEntity
	{
		public string Name { get; set; }

		public int SubmittedHomeWorks { get; set; }

		public string UserId { get; set; }

		public int TotalScore { get; set; }
	}
}
