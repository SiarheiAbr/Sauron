﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Services.Helpers
{
	public static class DirectoryHelper
	{
		public static void CleanDirectory(string directoryPath)
		{
			var directoryInfo = new DirectoryInfo(directoryPath);

			foreach (FileInfo file in directoryInfo.EnumerateFiles())
			{
				file.Delete();
			}

			foreach (DirectoryInfo dir in directoryInfo.EnumerateDirectories())
			{
				dir.Delete(true);
			}
		}

		public static void DeleteDirectory(string directoryPath)
		{
			var directoryInfo = new DirectoryInfo(directoryPath);
			directoryInfo.Delete(true);
		}
	}
}