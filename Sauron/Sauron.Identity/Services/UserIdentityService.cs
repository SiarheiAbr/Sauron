using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Owin.Security;

namespace Sauron.Identity.Services
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

		public string GetUserId()
		{
			var userId = this.authenticationManager.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

			if (userId == null)
			{
				throw new UnauthorizedAccessException("You are not authorized.");
			}

			return userId.Value;
		}
	}
}