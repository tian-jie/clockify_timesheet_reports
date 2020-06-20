using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(qyAPITest.Startup))]
namespace qyAPITest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
