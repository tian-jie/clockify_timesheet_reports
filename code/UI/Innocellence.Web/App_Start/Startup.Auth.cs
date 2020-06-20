using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;

using Owin;
using System.Web.Configuration;

namespace Innocellence.Web
{
    public partial class Startup
    {
        // 有关配置身份验证的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            System.Web.Configuration.AuthenticationSection section =
    (System.Web.Configuration.AuthenticationSection)System.Web.Configuration.WebConfigurationManager.GetSection("system.web/authentication");
            if (section.Mode != System.Web.Configuration.AuthenticationMode.Windows)
            {
                // Do something because it is running under Windows mode


                // System.Web.Configuration.AuthenticationSection
                // 使应用程序可以使用 Cookie 来存储已登录用户的信息

                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new PathString("/Account/Login"),
                    CookiePath = "/",
                    //#if  DEBUG
                    //#else
                    
                    CookieDomain = WebConfigurationManager.AppSettings["CookieDomain"] ?? ""
                    //#endif
                   // ExpireTimeSpan=new System.TimeSpan()

                });

                //System.IO.File.AppendAllText("d:\\a.txt", System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"] + "\r\n");

            }

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // 取消注释以下行可允许使用第三方登录提供程序登录
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseOAuthClientAuthentication(
            //  new OAuthClientAuthenticationOptions
            //  {
            //      AppId = "228714892725",
            //      AppSecret = "f5947310789ab6c91f3b8fa8642deb00",
            //      Endpoints = new OAuthClientAuthenticationOptions.OAuthClientAuthenticationEndpoints()
            //      {
            //          AuthorizationEndpoint = "https://auth.dxy.cn/conn/oauth2/authorize",
            //          TokenEndpoint = "https://auth.dxy.cn/conn/oauth2/accessToken",
            //          UserInfoEndpoint = "https://auth.dxy.cn/conn/oauth2/profile"
            //      }
            //  });

            
        }
    }
}