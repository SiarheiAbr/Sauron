using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Common.Logger
{
	public class Logger : ILogger
	{
		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		public void Error(string message)
		{
			logger.Error(message);
		}

		public void Error(Exception exception)
		{
			logger.Error(exception.ToString());
		}
	}
}
