using System;

namespace Sauron.Services.Processing.TestRunner
{
	public class TestResults : MarshalByRefObject
	{
		public string Results { get; set; }

		public string ProjectDllPath { get; set; }
	}
}
