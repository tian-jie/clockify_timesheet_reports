using Infrastructure.Core.Logging;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.Weixin.QY.AdvancedAPIs;
//using Innocellence.Weixin.QY.AdvancedAPIs.Concern;
using Innocellence.Weixin.QY.CommonAPIs;
using System.Web.Mvc;

namespace Innocellence.WeChatMain.Controllers
{
    public class DisclamerController : WeChatBaseController<ArticleInfo, ArticleInfoView>
    {
        private IArticleInfoService _objService;

        public DisclamerController(IArticleInfoService objService)
            : base(objService)
        {
            _objService = objService;
        }
        //
        // GET: /Disclamer/
        [AllowAnonymous]
        public ActionResult Agree()
        {
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }
            var config = WeChatCommonService.GetWeChatConfigByID(AppId);
            var Token = AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);

            LogManager.GetLogger(this.GetType()).Debug("Starting ConcernApi.TwoVerification... Token=" + Token + " - WeChatUserID=" + ViewBag.WeChatUserID);
            var result = ConcernApi.TwoVerification(Token, ViewBag.WeChatUserID);

            return Redirect("~/subscribed.html");
        }
    }
}
