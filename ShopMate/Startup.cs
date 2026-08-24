using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShopMate.Startup))]
namespace ShopMate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

