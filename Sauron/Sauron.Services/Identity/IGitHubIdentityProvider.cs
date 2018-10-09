using System.Collections.Generic;
using System.Security.Claims;

namespace Sauron.Services.Identity
{
	public interface IGitHubIdentityProvider
	{
		Claim GetAccessTokenClaim(IEnumerable<Claim> claims = null);

		string GetAccesToken(IEnumerable<Claim> claims = null);
	}
}
