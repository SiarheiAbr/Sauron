using System.Collections.Generic;
using System.Security.Claims;

namespace Sauron.Identity.Services
{
	public interface IGitHubIdentityService
	{
		Claim GetAccessTokenClaim(IEnumerable<Claim> claims = null);

		string GetAccesToken(IEnumerable<Claim> claims = null);
	}
}
