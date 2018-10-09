using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Owin.Security;

namespace Sauron.Services.Identity
{
	public class UserIdentityService : IUserIdentityService
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