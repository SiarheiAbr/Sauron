using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sauron.Services.TestRunner
{
	public class TestResults : MarshalByRefObject
	{
		public string Results { get; set; }

		public string ProjectDllPath { get; set; }
	}
}
