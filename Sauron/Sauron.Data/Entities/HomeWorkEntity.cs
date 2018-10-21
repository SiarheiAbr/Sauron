using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sauron.Identity.Entities;

namespace Sauron.Data.Entities
{
	[Table("HomeWorks")]
	public class HomeWorkEntity
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		[Column("Id")]
		public Guid Id { get; set; }

		public TaskEntity Task { get; set; }

		public Guid TaskId { get; set; }

		public ApplicationUser User { get; set; }

		public string UserId { get; set; }

		public string TestsResults { get; set; }

		public bool IsBuildSuccessful { get; set; }
	}
}
