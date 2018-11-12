using System;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using Sauron.Common;
using Sauron.Common.Static;

namespace Sauron.Data.Migrations
{
	using System.Data.Entity.Migrations;

	public sealed class MigrationConfiguration : DbMigrationsConfiguration<Sauron.Data.Db.ApplicationDbContext>
	{
		public MigrationConfiguration()
		{
			AutomaticMigrationsEnabled = true;
		}

		protected override void Seed(Sauron.Data.Db.ApplicationDbContext context)
		{
			var existingAdminRole = context.Roles.FirstOrDefault(x => x.Name == UserRoles.Admin);

			if (existingAdminRole == null)
			{
				//// Add admin role
				var adminRole = new IdentityRole()
				{
					Id = Guid.NewGuid().ToString(),
					Name = UserRoles.Admin
				};

				context.Roles.Add(adminRole);
			}

			base.Seed(context);
		}
	}
}
