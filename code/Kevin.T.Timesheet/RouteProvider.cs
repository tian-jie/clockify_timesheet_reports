using Infrastructure.Web.Mvc.Routing;
using System.Web.Mvc;

namespace Innocellence.Lccp.Backend
{

    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(AreaRegistrationContext routes)
        {

            routes.MapRoute(
            name: "Timesheet",
            url: "Timesheet/{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "index", id = UrlParameter.Optional },
            namespaces: new[] { "Kevin.T.Timesheet.Controllers" }
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
            get { return "Kevin.T.Timesheet"; }
        }
    }
}