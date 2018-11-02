using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Sauron.Data.Entities;
using Sauron.Data.Migrations;
using Sauron.Identity;

namespace Sauron.Data.Db
{
	public class SauronDbInitializer : MigrateDatabaseToLatestVersion<ApplicationDbContext, MigrationConfiguration>
	{
	}
}
