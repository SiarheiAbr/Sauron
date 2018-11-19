using System;

namespace Sauron.Services.Exceptions
{
	public class BuildException : Exception
	{
		public BuildException(string userId, long repositoryId, Guid taskId) :
			base($"Build Error. UserId: {userId}; RepoId: {repositoryId}, TaskId: {taskId}")
		{
		}
	}
}