using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QuanLyKhoHang.UI.Startup))]
namespace QuanLyKhoHang.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
