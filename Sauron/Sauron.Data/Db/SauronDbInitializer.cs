using System.Data.Entity;
using Sauron.Data.Migrations;

namespace Sauron.Data.Db
{
	public class SauronDbInitializer : MigrateDatabaseToLatestVersion<ApplicationDbContext, MigrationConfiguration>
	{
	}
}
