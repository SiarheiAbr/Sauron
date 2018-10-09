using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Sauron.Services.Identity
{
	public class GitHubIdentityProvider : IGitHubIdentityProvider
	{
		private readonly IUserIdentityService userIdentityService;

		public GitHubIdentityProvider(IUserIdentityService userIdentityService)
		{
			this.userIdentityService = userIdentityService;
		}

		public static string GitHubAccessTokenClaimType { get; } = "urn:gitHub:access_token";

		public Claim GetAccessTokenClaim(IEnumerable<Claim> claims = null)
		{
			claims = claims ?? this.userIdentityService.GetClaims();

			var accessTokenClaim = claims.FirstOrDefault(c => c.Type.Equals(GitHubAccessTokenClaimType));

			if (accessTokenClaim == null)
			{
				throw new KeyNotFoundException("Access token doesn't exist");
			}

			return accessTokenClaim;
		}

		public string GetAccesToken(IEnumerable<Claim> claims = null)
		{
			var accesToken = this.GetAccessTokenClaim(claims).Value;

			return accesToken;
		}
	}
}