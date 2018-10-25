using System;
using System.Threading.Tasks;

namespace Sauron.Services.Processing.TestRunner
{
	public class TestResults : MarshalByRefObject
	{
		private readonly TaskCompletionSource<string> marshalCompletionSource = new TaskCompletionSource<string>();

		public string Results { get; set; }

		public string ProjectDllPath { get; set; }

		public Task<string> Task => marshalCompletionSource.Task;

		public void SetResult()
		{
			marshalCompletionSource.SetResult(this.Results);
		}
	}
}
