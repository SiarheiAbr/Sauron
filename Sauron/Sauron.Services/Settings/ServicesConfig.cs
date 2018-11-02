using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Services.Settings
{
	public class ServicesConfig : IServicesConfig
	{
		public string DownloadRepositoryPathTemplate => ConfigurationManager.AppSettings["DownloadRepositoryPathTemplate"];

		public string OutputPathTemplate => ConfigurationManager.AppSettings["OutputPathTemplate"];

		public string TempTestReportsPathTemplate => ConfigurationManager.AppSettings["TempTestReportsPathTemplate"];

		public string HiddenTestsPathTemplate => ConfigurationManager.AppSettings["HiddenTestsPathTemplate"];
	}
}
