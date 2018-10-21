using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sauron.Data.Entities;

namespace Sauron.Data.Db
{
	public class SauronDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
	{
		protected override void Seed(ApplicationDbContext context)
		{
			IList<TaskEntity> defaultTasks = new List<TaskEntity>();

			defaultTasks.Add(new TaskEntity() { Name = "Simple Calculator Task", GitHubUrl = "https://github.com/MikalaiKasmachou/SimpleCalcHomeWork" });
			
			context.Tasks.AddRange(defaultTasks);

			base.Seed(context);
		}
	}
}
