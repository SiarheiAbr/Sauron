using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Data.Entities
{
	[Table("Tasks")]
	public class TaskEntity
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		[Column("Id")]
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string GitHubUrl { get; set; }

		public string TestsFileName { get; set; }

		public bool HiddenTestsUploaded { get; set; }

		public ICollection<HomeWorkEntity> HoweWorks { get; set; }
	}
}
