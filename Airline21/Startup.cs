using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Airline21.Startup))]
namespace Airline21
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
