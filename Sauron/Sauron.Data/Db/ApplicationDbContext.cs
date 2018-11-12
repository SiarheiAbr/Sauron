using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Sauron.Data.Entities;

namespace Sauron.Data.Db
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
			Database.SetInitializer(new SauronDbInitializer());
		}

		public DbSet<HomeWorkEntity> HomeWorks { get; set; }

		public DbSet<TaskEntity> Tasks { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<HomeWorkEntity>()
				.HasRequired(s => s.Task)
				.WithMany(g => g.HoweWorks)
				.HasForeignKey<Guid>(s => s.TaskId)
				.WillCascadeOnDelete(true);

			modelBuilder.Entity<HomeWorkEntity>()
				.HasRequired(hm => hm.User)
				.WithMany(user => user.HoweWorks)
				.HasForeignKey(hm => hm.UserId)
				.WillCascadeOnDelete(true);
		}
	}
}
