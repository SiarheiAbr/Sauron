using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Services.Settings
{
	public interface IServicesConfig
	{
		string DownloadRepositoryPathTemplate { get; }

		string OutputPathTemplate { get; }

		string TempTestReportsPathTemplate { get; }
	}
}
