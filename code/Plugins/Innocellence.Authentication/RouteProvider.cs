using System.Web.Mvc;
using System.Web.Routing;
using Infrastructure.Web.Mvc;
using Infrastructure.Web.Mvc.Routing;
using System;

namespace Innocellence.Authentication
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(AreaRegistrationContext routes)
        {
          routes.MapRoute("Innocellence.Authentication.SSO",
                 "OWinLogin/ssoresult",
                 new { controller = "OWinLogin", action = "ssoresult", id = UrlParameter.Optional },
                 new[] { "Innocellence.Authentication.Controllers" }
            );
          routes.MapRoute(
            name: "Innocellence.Authentication.SsoResult",
            url: "Sso/SsoResult",
            defaults: new { controller = "sso", action = "ssoresult", id = UrlParameter.Optional },
            namespaces: new[] { "Innocellence.Authentication.Controllers" }
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
            get { return "Authentication"; } 
        }
    }
}
