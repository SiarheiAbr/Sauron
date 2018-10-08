using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sauron.Startup))]
namespace Sauron
{
    public partial class Startup
    {
		public void Configuration(IAppBuilder app)
        {
			ConfigureAuth(app);
        }
    }
}
