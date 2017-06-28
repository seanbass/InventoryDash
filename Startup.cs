using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InventoryDash.Startup))]
namespace InventoryDash
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
