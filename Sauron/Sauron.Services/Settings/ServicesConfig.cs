using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Services.Settings
{
	public class ServicesConfig : IServicesConfig
	{
		public string DownloadRepositotyPathTemplate => ServicesSettings.Default.DownloadRepositotyPathTemplate;

		public string OutputPathTemplate => ServicesSettings.Default.OutputPathTemplate;
	}
}
