using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Project_MVC_CF_Special.Startup))]
namespace Project_MVC_CF_Special
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
