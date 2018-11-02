namespace Sauron.Data.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class HiddenTests : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Tasks", "TestsFileName", c => c.String());
			AddColumn("dbo.Tasks", "HiddenTestsUploaded", c => c.Boolean(nullable: false));
		}

		public override void Down()
		{
			DropColumn("dbo.Tasks", "HiddenTestsUploaded");
			DropColumn("dbo.Tasks", "TestsFileName");
		}
	}
}
