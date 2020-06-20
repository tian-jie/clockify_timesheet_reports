using System.Web.Mvc;
using System.Web.Routing;
using Infrastructure.Web.Mvc;
using Infrastructure.Web.Mvc.Routing;
using System;

namespace Innocellence.WeChat
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(AreaRegistrationContext routes)
        {

            //routes.MapRoute(
            //    name: "FaqInfo",
            //    url: "FaqInfo/{action}/{id}",
            //    defaults: new { controller = "FaqInfo", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "Innocellence.WeChat.Controllers" }
            //);
            // routes.MapRoute(
            //    name: "QuestionManage",
            //    url: "QuestionManage/{action}/{id}",
            //    defaults: new { controller = "QuestionManage", action = "RaiseQuestion", id = UrlParameter.Optional },
            //    namespaces: new[] { "Innocellence.WeChat.Controllers" }
            //);
            routes.MapRoute(
                name: "Message",
                url: "Message/{action}/{id}",
                defaults: new { controller = "Message", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Innocellence.WeChat.Controllers" }
            );
            routes.MapRoute(
                name: "News",
                url: "News/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Innocellence.WeChat.Controllers" }
            );
            

            // 以下是oAuth相关的路径    -- START
            // 跳转通过oauth获取ticket
            routes.MapRoute(
                 "Innocellence.WeChat.Access.GetTicket",
                 "japi/qiye/oauth/ticket/{appId}/{*uri}",
                 new { controller = "Access", action = "GetTicket", id = UrlParameter.Optional },
                 new[] { "Innocellence.WeChat.Controllers" }
                 );

            routes.MapRoute(
               "Innocellence.WeChat.Access.GetTicketExt",
               "japi/qiye/oauth/ticketext/{appId}/{*uri}",
               new { controller = "Access", action = "GetTicketExt", id = UrlParameter.Optional },
               new[] { "Innocellence.WeChat.Controllers" }
               );

            //// 跳转通过oauth获取ticket-服务号
            //routes.MapRoute(
            //     "Innocellence.WeChat.Access.GetMPTicket",
            //     "japi/weixin/oauth/ticket/{appId}/{uri}",
            //     new { controller = "Access", action = "GetTicket", id = UrlParameter.Optional },
            //     new[] { "Innocellence.WeChat.Controllers" }
            //     );

            //  // 根据ticket获得员工信息
            routes.MapRoute(
                 "Innocellence.WeChat.Access.GetByTicket",
                 "japi/open/qiye/contact/getbyticket",
                 new { controller = "API", action = "GetByTicket", id = UrlParameter.Optional },
                 new[] { "Innocellence.WeChat.Controllers" }
                 );

            //  // 根据userId获取用户信息
            routes.MapRoute(
                 "Innocellence.WeChat.Access.GetByUserId",
                 "japi/open/qiye/contact/getbyuserid",
                 new { controller = "API", action = "GetByUserId", id = UrlParameter.Optional },
                 new[] { "Innocellence.WeChat.Controllers" }
                 );

            //// 根据Appid获取APP信息
            routes.MapRoute(
                 "Innocellence.WeChat.Access.sign",
                 "japi/js/qiye/sign/{appid}",
                 new { controller = "API", action = "GetJsSDK", id = UrlParameter.Optional },
                 new[] { "Innocellence.WeChat.Controllers" }
                 );

            routes.MapRoute(
               "Innocellence.WeChat.Access.signAPI",
               "japi/open/qiye/sign/{appid}",
               new { controller = "API", action = "GetJsSDK", id = UrlParameter.Optional },
               new[] { "Innocellence.WeChat.Controllers" }
               );

            routes.MapRoute(
                "Innocellence.WeChat.API.SendMsg",
                "japi/open/qiye/message/send",
                new { controller = "API", action = "SendMsg", id = UrlParameter.Optional },
                new[] { "Innocellence.WeChat.Controllers" }
                );

            routes.MapRoute(
               "Innocellence.WeChat.API.SendTextMsg",
               "japi/open/qiye/weixin/TextMessage",
               new { controller = "API", action = "SendTextMsg", id = UrlParameter.Optional },
               new[] { "Innocellence.WeChat.Controllers" }
               );

            routes.MapRoute(
              "Innocellence.WeChat.API.DownloadMedia",
              "japi/open/qiye/weixin/media",
              new { controller = "API", action = "DownloadMedia", id = UrlParameter.Optional },
              new[] { "Innocellence.WeChat.Controllers" }
              );

            routes.MapRoute(
            "Innocellence.WeChat.API.listbydepartment",
            "japi/open/qiye/contact/listbydepartment",
            new { controller = "API", action = "ListByDepartment", id = UrlParameter.Optional },
            new[] { "Innocellence.WeChat.Controllers" }
            );
            //  routes.MapRoute(
            //"Innocellence.WeChat.Access.GetByTicket",
            //"japi/js/qy/sign/{appId}",
            //new { controller = "Access", action = "sign", id = UrlParameter.Optional },
            //new[] { "Innocellence.WeChat.Controllers" }
            // );
            //// ------------------------- END
        }
        public int Priority
        {
            get
            {
                return 99;
            }
        }
        public string ModuleName
        {
            get { return "CA"; }
        }
    }
}
