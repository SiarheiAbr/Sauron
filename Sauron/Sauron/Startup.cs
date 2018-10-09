using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sauron.Startup))]

namespace Sauron
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			this.ConfigureAuth(app);
		}
	}
}
