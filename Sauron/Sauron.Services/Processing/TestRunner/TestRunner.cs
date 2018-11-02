using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using NUnit.Engine;
using Sauron.Services.Helpers;

namespace Sauron.Services.Processing.TestRunner
{
	public class TestRunner : MarshalByRefObject, ITestEventListener
	{
		public async void RunUnitTestsForAssembly(TestResults results)
		{
			var assemblyPath = results.ProjectDllPath;
			XmlNode result = null;
			TestPackage package = new TestPackage(assemblyPath);
			package.AddSetting("WorkDirectory", Path.GetDirectoryName(assemblyPath));

			// prepare the engine
			// TODO: parse hidden tests names and add to filter to prevent running other tests
			ITestEngine engine = TestEngineActivator.CreateInstance();
			var filterService = engine.Services.GetService<ITestFilterService>();
			ITestFilterBuilder builder = filterService.GetTestFilterBuilder();
			TestFilter emptyFilter = builder.GetFilter();

			using (ITestRunner runner = engine.GetRunner(package))
			{
				var testRunAsync = (AsyncTestEngineResult)runner.RunAsync(this, emptyFilter);

				await AsyncProcessRunnerHelper.FromWaitHandle(testRunAsync.WaitHandle, new TimeSpan(0, 0, 3));

				result = testRunAsync.EngineResult.Xml;

				var doc = new XmlDocument();

				try
				{
					doc.LoadXml(result.OuterXml);

					var nodesToRemove = new List<XmlNode>();

					nodesToRemove.AddRange(new List<XmlNode>()
					{
						this.FindNode(doc.ChildNodes, "settings"),
						this.FindNode(doc.ChildNodes, "environment")
					});

					foreach (var node in nodesToRemove)
					{
						node?.RemoveAll();
					}
				}
				catch (Exception)
				{
					throw new InvalidDataException("Invalid format of tests results xml");
				}

				results.Results = doc.InnerXml;
				results.SetResult();
			}
		}

		public void OnTestEvent(string report)
		{
			var loggedReport = report;
			////throw new NotImplementedException();
		}

		private XmlNode FindNode(XmlNodeList list, string nodeName)
		{
			if (list.Count > 0)
			{
				foreach (XmlNode node in list)
				{
					if (node.Name.Equals(nodeName))
					{
						return node;
					}

					if (node.HasChildNodes)
					{
						XmlNode nodeFound = FindNode(node.ChildNodes, nodeName);
						if (nodeFound != null)
						{
							return nodeFound;
						}
					}
				}
			}

			return null;
		}
	}
}
