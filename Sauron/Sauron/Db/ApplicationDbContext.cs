using Microsoft.AspNet.Identity.EntityFramework;
using Sauron.Identity;

namespace Sauron.Db
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
		}
	}
}