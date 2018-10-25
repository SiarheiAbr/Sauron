using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Sauron.Identity.Services
{
	public class GitHubIdentityService : IGitHubIdentityService
	{
		private readonly IUserIdentityService userIdentityService;
		private readonly IAuthenticationManager authenticationManager;

		public GitHubIdentityService(IUserIdentityService userIdentityService, IAuthenticationManager authenticationManager)
		{
			this.userIdentityService = userIdentityService;
			this.authenticationManager = authenticationManager;
		}

		public static string GitHubAccessTokenClaimType { get; } = "urn:gitHub:access_token";

		public Claim GetAccessTokenClaim(IEnumerable<Claim> claims = null)
		{
			claims = claims ?? this.userIdentityService.GetClaims();

			var accessTokenClaim = claims.FirstOrDefault(c => c.Type.Equals(GitHubAccessTokenClaimType));
			
			if (accessTokenClaim == null)
			{
				this.authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
				return null;
			}

			return accessTokenClaim;
		}

		public string GetAccessToken(IEnumerable<Claim> claims = null)
		{
			string accessToken = null;

			claims = claims?.ToList();

			var accessTokenClaim = this.GetAccessTokenClaim(claims);

			if (accessTokenClaim != null)
			{
				accessToken = accessTokenClaim.Value;
			}

			return accessToken;
		}
	}
}