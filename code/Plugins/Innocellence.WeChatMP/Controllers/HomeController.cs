

using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;

namespace Innocellence.WeChatMP.Controllers
{
    //TODO: only for example
    public class HomeController : WeChatBaseController<AccessDashboard, AccessDashboardView>
    {
        public IAccessDashboardService _objService;
        public HomeController(IAccessDashboardService objService)
            : base(objService)
        {
            _objService = objService;
        }
    }
}