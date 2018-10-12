using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Services.Build
{
	public interface IBuildService
	{
		Task BuildRepositorySolution(long repositoriId);

		Task BuildRepositorySingleProject(long repositoriId);
	}
}
