using System;
using System.IO;
using ILogger = Sauron.Common.Logger.ILogger;

namespace Sauron.Services.Helpers
{
	public static class DirectoryHelper
	{
		public static void CleanDirectory(string directoryPath, ILogger logger)
		{
			var directoryInfo = new DirectoryInfo(directoryPath);

			try
			{
				foreach (FileInfo file in directoryInfo.EnumerateFiles())
				{
					file.Delete();
				}

				foreach (DirectoryInfo dir in directoryInfo.EnumerateDirectories())
				{
					dir.Delete(true);
				}
			}
			catch (Exception e)
			{
				logger.Error(e);
			}
		}

		public static void DeleteDirectory(string directoryPath, ILogger logger)
		{
			try
			{
				var directoryInfo = new DirectoryInfo(directoryPath);
				directoryInfo.Delete(true);
			}
			catch (Exception e)
			{
				logger.Error(e);
			}
		}
	}
}
