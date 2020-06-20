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
            //    namespaces: new[] { "Innocellence.WeChatMeeting.Controllers" }
            //);
            // routes.MapRoute(
            //    name: "QuestionManage",
            //    url: "QuestionManage/{action}/{id}",
            //    defaults: new { controller = "QuestionManage", action = "RaiseQuestion", id = UrlParameter.Optional },
            //    namespaces: new[] { "Innocellence.WeChatMeeting.Controllers" }
            //);
            routes.MapRoute(
                name: "WeChatMeeting",
                url: "WeChatMeeting/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Innocellence.WeChatMeeting.Controllers" }
            );

          
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
            get { return "WeChatMeeting"; }
        }
    }
}
