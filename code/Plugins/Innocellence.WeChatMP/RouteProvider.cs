using System.Web.Mvc;
using Infrastructure.Web.Mvc.Routing;

namespace Innocellence.Finance
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(AreaRegistrationContext routes)
        {
            // routes.MapRoute("Weixin", "Weixin/index", new { controller = "Weixin", action = "Index", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            // );

            // routes.MapRoute("Message", "Message/{action}/{id}", new { controller = "Message", action = "Index", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            // );
            // routes.MapRoute("News", "News/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            // );
            // routes.MapRoute("Video", "Video/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            // );
            // routes.MapRoute("Hongtu", "Hongtu/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            // );
            // routes.MapRoute("Activity", "Activity/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            // );
            // routes.MapRoute("HREService", "HREService/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            // );
            // routes.MapRoute("SPP", "SPP/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            // );
            // routes.MapRoute("SalesTraining", "SalesTraining/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            // );
            // routes.MapRoute("NSC", "NSC/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            //);
            // routes.MapRoute("Common", "Common/File/{id}", new { controller = "Common", action = "File", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            //);
            // routes.MapRoute("CommonThumb", "Common/ThumbFile/{id}", new { controller = "Common", action = "ThumbFile", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            //);
            // routes.MapRoute("CAAccount", "CAAccount/{controller}/{action}", new { controller = "Common", action = "ThumbFile", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            //);
            // routes.MapRoute("Subscribed", "Subscribed/Subscrib/Subscribed", new { controller = "Subscrib", action = "Subscribed", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" }
            //);
            routes.MapRoute("Finance", "Finance/{controller}/{action}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "Innocellence.WeChatMP.Controllers" });

            routes.MapRoute(
                name: "MPNews",
                url: "MPNews/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Innocellence.WeChatMP.Controllers" }
            );

            //判断是否为会员并返回会员手机号
        //    routes.MapRoute(
        //    name: "japi_check_member",
        //    url: "japi/open/weixin/CheckMember",
        //    defaults: new { controller = "Access", action = "CheckMember", id = UrlParameter.Optional },
        //     namespaces: new[] { "Innocellence.WeChatMP.Controllers" }
        //);

            routes.MapRoute(
            "Innocellence.WeChatMP.Access.CheckMember",
            "japi/js/CheckMember",
            new { controller = "Access", action = "CheckMember", id = UrlParameter.Optional },
            new[] { "Innocellence.WeChatMP.Controllers" }
            );





            // 以下是oAuth相关的路径    -- START
            // 跳转通过oauth获取ticket
            //routes.MapRoute(
            //     "Innocellence.WeChatMP.Access.GetTicket",
            //     "japi/qiye/oauth/ticket/{appId}/{uri}",
            //     new { controller = "Access", action = "GetTicket", id = UrlParameter.Optional },
            //     new[] { "Innocellence.WeChatMP.Controllers" }
            //     );

            // 跳转通过oauth获取ticket-服务号
            routes.MapRoute(
                 "Innocellence.WeChatMP.Access.GetMPTicket",
                 "japi/weixin/oauth/ticket/{appId}/{*uri}",
                 new { controller = "Access", action = "GetTicket", uri = UrlParameter.Optional },
                 new[] { "Innocellence.WeChatMP.Controllers" }
                 );

            // 根据ticket获得员工信息
            routes.MapRoute(
                 "Innocellence.WeChatMP.API.GetByTicket",
                 "japi/open/member/info/ticket",
                 new { controller = "API", action = "GetByTicket", id = UrlParameter.Optional },
                 new[] { "Innocellence.WeChatMP.Controllers" }
                 );

            routes.MapRoute(
                "Innocellence.WeChatMP.API.DownloadMedia",
                "japi/open/weixin/media",
                new { controller = "API", action = "DownloadMedia", id = UrlParameter.Optional },
                new[] { "Innocellence.WeChatMP.Controllers" }
                );
            routes.MapRoute(
                "Innocellence.WeChatMP.API.BindDevice",
                "japi/open/weixin/hardware/bind",
                new { controller = "API", action = "BindDevice", id = UrlParameter.Optional },
                new[] { "Innocellence.WeChatMP.Controllers" }
                );
            routes.MapRoute(
                "Innocellence.WeChatMP.API.devicelist",
                "japi/open/weixin/hardware/device/list",
                new { controller = "API", action = "GetByTicket", id = UrlParameter.Optional },
                new[] { "Innocellence.WeChatMP.Controllers" }
                );
            routes.MapRoute(
                "Innocellence.WeChatMP.API.RegisterDevice",
                "japi/open/weixin/hardware/register",
                new { controller = "API", action = "RegisterDevice", id = UrlParameter.Optional },
                new[] { "Innocellence.WeChatMP.Controllers" }
                );
            routes.MapRoute(
                "Innocellence.WeChatMP.API.Verify",
                "japi/open/weixin/hardware/verify",
                new { controller = "API", action = "Verify", id = UrlParameter.Optional },
                new[] { "Innocellence.WeChatMP.Controllers" }
                );

            routes.MapRoute(
               "Innocellence.WeChatMP.API.CheckDeviceStatus",
               "japi/open/weixin/hardware/device/status",
               new { controller = "API", action = "CheckDeviceStatus", id = UrlParameter.Optional },
               new[] { "Innocellence.WeChatMP.Controllers" }
               );

            routes.MapRoute(
             "Innocellence.WeChatMP.Access.sign",
             "japi/js/sign/{appId}",
             new { controller = "API", action = "GetJsSDK", id = UrlParameter.Optional },
             new[] { "Innocellence.WeChatMP.Controllers" }
             );

            routes.MapRoute(
            "Innocellence.WeChatMP.Access.signAPI",
            "japi/open/sign/{appId}",
            new { controller = "API", action = "GetJsSDK", id = UrlParameter.Optional },
            new[] { "Innocellence.WeChatMP.Controllers" }
            );

            

            // 根据userId获取用户信息
            //routes.MapRoute(
            //     "Innocellence.WeChatMP.Access.GetByUserId",
            //     "japi/open/qiye/contact/getbyuserid",
            //     new { controller = "Access", action = "GetByUserId", id = UrlParameter.Optional },
            //     new[] { "Innocellence.WeChatMP.Controllers" }
            //     );


            //模板消息
            routes.MapRoute(
             name: "japi_template_send",
             url: "japi/open/weixin/template/send",
             defaults: new { controller = "API", action = "SendTemplateMessage", id = UrlParameter.Optional },
              namespaces: new[] { "Innocellence.WeChatMP.Controllers" }
         );
            // 根据Appid获取APP信息
            //routes.MapRoute(
            //     "Innocellence.WeChat.Access.GetByUserId",
            //     "japi/js/qiye/sign/{agentid}",
            //     new { controller = "Access", action = "GetAppInfoByAgentiId", id = UrlParameter.Optional },
            //     new[] { "Innocellence.WeChatMP.Controllers" }
            //     );
            // ------------------------- END



            ////稳糖活动专用api
            routes.MapRoute(
                name: "wentangapi",
                url: "wentang/api/{action}/{id}",
                defaults: new { controller = "WentangAPI", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Innocellence.WeChatMP.Controllers" }
            );

        }
        public int Priority
        {
            get
            {
                return 101;
            }
        }
        public string ModuleName
        {
            get { return "Finance"; }
        }
    }
}
