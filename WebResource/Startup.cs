using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebResource.Startup))]
namespace WebResource
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
