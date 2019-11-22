using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Wikipedia_Clone.Startup))]
namespace Wikipedia_Clone
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
