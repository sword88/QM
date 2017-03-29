using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QM.Web.Startup))]
namespace QM.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
