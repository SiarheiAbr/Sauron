using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Engine;

namespace Sauron.Services.TestRunner
{
	public class TestRunner : MarshalByRefObject, ITestEventListener
	{
		public TestResults RunUnitTestsForAssembly(TestResults results)
		{
			var assemblyPath = results.ProjectDllPath;
			XmlNode result = null;
			TestPackage package = new TestPackage(assemblyPath);
			package.AddSetting("WorkDirectory", Path.GetDirectoryName(assemblyPath));

			// prepare the engine
			ITestEngine engine = TestEngineActivator.CreateInstance();
			var filterService = engine.Services.GetService<ITestFilterService>();
			ITestFilterBuilder builder = filterService.GetTestFilterBuilder();
			TestFilter emptyFilter = builder.GetFilter();

			using (ITestRunner runner = engine.GetRunner(package))
			{
				result = runner.Run(this, emptyFilter);
				results.Results = result.OuterXml;
			}

			return results;
		}

		public void OnTestEvent(string report)
		{
			var loggedReport = report;
			////throw new NotImplementedException();
		}
	}
}
