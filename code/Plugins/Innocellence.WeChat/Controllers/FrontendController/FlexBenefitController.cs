using System.Web.Mvc;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts;
using Infrastructure.Web.Domain.Service.Common;
using System.Collections.Generic;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChat.Controllers
{
    public partial class FlexBenefitController : WeChatBaseController<FlexBenefit, FlexBenefitView>
    {
        private IFlexBenefitService _objService;
        public FlexBenefitController(IFlexBenefitService objService)
            : base(objService)
        {
            _objService = objService;
        }

        public ActionResult WxDetail(string accessYear)
        {

#if !DEBUG
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }
#endif

            var flexBenefit = _objService.GetFlexBenefitByConditions(ViewBag.WeChatUserID, accessYear);

            if (flexBenefit == null)
            {
                return View("../FlexBenefit/BillNotFind");
            }

            var flexBenefitView = new FlexBenefitView().ConvertAPIModel(flexBenefit);
            return View(flexBenefitView);
        }
    }
}