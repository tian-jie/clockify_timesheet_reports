using System.Web.Mvc;
using System.Web.Routing;
using Infrastructure.Web.Mvc;
using Infrastructure.Web.Mvc.Routing;
using System;

namespace Innocellence.WeChatMain
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(AreaRegistrationContext routes)
        {
            routes.MapRoute("Innocellence.WeChatMain.Token",
                 "AToken/GetToken",
                 new { controller = "AToken", action = "GetToken", id = UrlParameter.Optional },
                 new[] { "Innocellence.WeChatMain.Controllers" }
            );
            routes.MapRoute("Innocellence.WeChatMain",
                 "WeChatMain/{controller}/{action}/{id}",
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                 new[] { "Innocellence.WeChatMain.Controllers" }
            );
            routes.MapRoute("Innocellence.WeChatMain.UserBehavior",
                 "UserBehavior/{action}",
                 new { controller = "UserBehavior", action = "Create", id = UrlParameter.Optional },
                 new[] { "Innocellence.WeChatMain.Controllers" }
            );
            routes.MapRoute("Innocellence.WeChatMain.AppManage",
               "AppManage/{action}",
               new { controller = "AppManage", action = "SetAppInfo", id = UrlParameter.Optional },
               new[] { "Innocellence.WeChatMain.Controllers" }
          );
            routes.MapRoute(
               name: "Weixin",
               url: "Weixin/{action}",
               defaults: new { controller = "Weixin", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "Innocellence.WeChatMain.Controllers" }
            );
            routes.MapRoute(
              name: "WeixinMP",
              url: "WeixinMP/{action}",
              defaults: new { controller = "WeixinMP", action = "Index", id = UrlParameter.Optional },
              namespaces: new[] { "Innocellence.WeChatMain.Controllers" }
           );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }


        public string ModuleName {
            get { return "WeChatMain"; } 
        }
    }
}
