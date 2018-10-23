using System.Threading.Tasks;
using Microsoft.Build.Execution;

namespace Sauron.Services.Processing
{
	public interface IBuildService
	{
		Task<BuildResult> BuildRepositorySolution(long repositoriId);

		Task BuildRepositorySingleProject(long repositoriId);
	}
}
