using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.Owin.Security;

namespace Sauron.Services
{
	public class UserIdentityService: IUserIdentityService
	{
		private IAuthenticationManager authenticationManager;

		public UserIdentityService(IAuthenticationManager authenticationManager)
		{
			this.authenticationManager = authenticationManager;
		}

		public IEnumerable<Claim> GetClaims()
		{
			return this.authenticationManager.User.Claims;
		}
	}
}