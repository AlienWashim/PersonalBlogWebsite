using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlogWebsite.Startup))]
namespace BlogWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
