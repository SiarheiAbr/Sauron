using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sauron.Services
{
	public interface IUserIdentityService
	{
		IEnumerable<Claim> GetClaims();
	}
}
