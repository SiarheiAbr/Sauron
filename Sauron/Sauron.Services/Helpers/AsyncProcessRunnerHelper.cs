using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sauron.Services.Helpers
{
	public static class AsyncProcessRunnerHelper
	{
		public static async Task<int> RunProcessAsync(string fileName, string arguments)
		{
			var tcs = new TaskCompletionSource<int>();

			var process = new Process
			{
				StartInfo =
				{
					FileName = fileName,
					Arguments = arguments,
					UseShellExecute = false,
					CreateNoWindow = true,
				},

				EnableRaisingEvents = true
			};

			EventHandler exitedHandler = null;

			exitedHandler = (sender, args) =>
			{
				var exitCode = process.ExitCode;

				if (exitedHandler != null)
				{
					process.Exited -= exitedHandler;
				}

				process.Dispose();

				tcs.SetResult(exitCode);
			};

			process.Exited += exitedHandler;

			process.Start();

			return await tcs.Task;
		}

		public static async Task<bool> FromWaitHandle(WaitHandle handle, TimeSpan timeout)
		{
			// Handle synchronous cases.
			var alreadySignalled = handle.WaitOne(0);

			if (alreadySignalled)
			{
				return await Task.FromResult(true);
			}

			if (timeout == TimeSpan.Zero)
			{
				return await Task.FromResult(false);
			}

			// Register all asynchronous cases.
			var tcs = new TaskCompletionSource<bool>();

			var threadPoolRegistration = ThreadPool.RegisterWaitForSingleObject(
				handle,
				(state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
				tcs,
				timeout,
				true);

			await tcs.Task.ContinueWith(_ => { threadPoolRegistration.Unregister(handle); }, TaskScheduler.Default);

			return await tcs.Task;
		}
	}
}
