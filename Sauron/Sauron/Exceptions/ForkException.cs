using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sauron.Exceptions
{
	public class ForkException : Exception
	{
		public ForkException() : base("Submitted repo is not a fork of task's repository. Please select another repository.")
		{
		}
	}
}