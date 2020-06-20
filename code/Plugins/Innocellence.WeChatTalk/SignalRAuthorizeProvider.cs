using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using Infrastructure.Web.Mvc;
using Infrastructure.Web.Mvc.Routing;
using System;
using Infrastructure.Core;
using Infrastructure.Web.MVC;
using Microsoft.Owin;
using Owin;
using System;
using System.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using System.Web.Configuration;

namespace Innocellence.Authentication
{
    public partial class SignalRAuthorizeProvider : IOWinAuthorizeProvider
    {
      public  void ConfigureAuth(IAppBuilder app)
        {

            app.MapSignalR();

            //XXX SAML2
            //app.UseSAML2Authentication(new SAML2AuthenticationOptions()
            //{

            //    CallbackPath = new Microsoft.Owin.PathString("/sso/ssoresult"),
            //    LoginBackUrl = "/OWinLogin/SsoResult",
            //    //  CertFilePath =HttpContext.Current.Server.MapPath("~/cert.crt"),
            //    SigningKeys = new List<X509Certificate>() { new X509Certificate2(HttpContext.Current.Server.MapPath("~/cert.crt")) },
            //    AllowUnsolicitedAuthnResponse = true,
            //    //EntityId = new EntityId("lly-qa:saml2:idp"),
            //    EntityId = new EntityId("CN_WeChat_Ent"),
            //    Configuration = new WsFederationConfiguration()
            //    {
            //        Issuer =WebConfigurationManager.AppSettings["SSOUrl"] ,
            //        // SigningKeys= "",
            //        TokenEndpoint = WebConfigurationManager.AppSettings["SSOUrl"]

            //    }
            //});





            ////丁香园OAuth2
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
