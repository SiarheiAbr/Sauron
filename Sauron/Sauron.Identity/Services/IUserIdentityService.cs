using System.Collections.Generic;
using System.Security.Claims;

namespace Sauron.Identity.Services
{
	public interface IUserIdentityService
	{
		IEnumerable<Claim> GetClaims();

		string GetUserId();
	}
}
