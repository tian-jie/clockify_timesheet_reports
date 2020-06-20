using System.Web.Mvc;
using Infrastructure.Web.Mvc.Routing;

namespace Innocellence.Activity
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(AreaRegistrationContext routes)
        {
            routes.MapRoute("Activity", "Activity/{controller}/{action}", 
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "Innocellence.Activity.Controllers", "Innocellence.Activity.Admin.Controllers", "Innocellence.Activity.Controllers.BackendController" }
           );
            routes.MapRoute(
               name: "ActivityPra",
               url: "Activity/{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "Innocellence.Activity.Controllers", "Innocellence.Activity.Admin.Controllers" }
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
            get { return "Activity"; }
        }
    }
}
