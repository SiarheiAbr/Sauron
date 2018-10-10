using System.Collections.Generic;
using System.Security.Claims;

namespace Sauron.Services.Identity
{
	public interface IUserIdentityService
	{
		IEnumerable<Claim> GetClaims();

		string GetUserId();
	}
}
