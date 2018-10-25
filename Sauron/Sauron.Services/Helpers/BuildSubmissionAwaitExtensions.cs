using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Execution;

namespace Sauron.Services.Helpers
{
	public static class BuildSubmissionAwaitExtensions
	{
		public static Task<BuildResult> ExecuteAsync(this BuildSubmission submission)
		{
			var tcs = new TaskCompletionSource<BuildResult>();
			submission.ExecuteAsync(SetBuildComplete, tcs);
			return tcs.Task;
		}

		private static void SetBuildComplete(BuildSubmission submission)
		{
			var tcs = (TaskCompletionSource<BuildResult>)submission.AsyncContext;
			tcs.SetResult(submission.BuildResult);
		}
	}
}
