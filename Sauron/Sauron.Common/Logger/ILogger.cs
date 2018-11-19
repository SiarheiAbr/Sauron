using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Common.Logger
{
	public interface ILogger
	{
		void Error(string message);

		void Error(Exception exception);
	}
}
