using System;
using System.Net;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Innocellence.WeChat.Domain.ModelsView;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.QY.Entities;
using Infrastructure.Web.ImageTools;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts;
using System.IO;
using Infrastructure.Web.Domain.Service.Common;
using Innocellence.Weixin.QY.CommonAPIs;
using Infrastructure.Core.Logging;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.WeChat.Domain.Common;
//using Innocellence.Weixin.QY.AdvancedAPIs.Concern;

namespace Innocellence.WeChat.Controllers
{
    public partial class SubscribController : Controller
    {

        public SubscribController()
        {
           
        }

        
        /// <summary>
        /// 微信认证成功
        /// </summary>
        /// <returns></returns>
        public ActionResult Subscribed(string WeChatUserID)
        {
            var objConfig = WeChatCommonService.GetWeChatConfigByID(1);
            var token = AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);

            LogManager.GetLogger(this.GetType()).Debug("Starting ConcernApi.TwoVerification... Token=" + token + " - WeChatUserID=" + WeChatUserID);
            var result = ConcernApi.TwoVerification(token, WeChatUserID);

            return Redirect("/subscribed.html");
        }
    }
}
