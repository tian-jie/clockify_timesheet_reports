using System;
using System.IO;
using System.Web.Mvc;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.WeChat.Domain.ViewModelFront;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.CommonAPIs;
using Infrastructure.Utility.Secutiry;
using Infrastructure.Web.Domain.Service;
using Newtonsoft.Json;
using System.ComponentModel;
using Innocellence.Weixin.QY.Helpers;
using Infrastructure.Core.Logging;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.ModelsView;

namespace Innocellence.WeChat.Controllers
{
    public class AccessController : WeChatBaseController<SysAddressBookMember, AddressBookMemberView>
    {
      //  readonly IAddressBookService _addressBookServiceService;
        ILogger log;
      //  private const string SESSION_KEY_OAUTH_USERID = "Session_oAuthUserId";

        public AccessController(IAddressBookService addressBookServiceService)
            : base(addressBookServiceService)
        {
            //_addressBookServiceService = addressBookServiceService;
            log = LogManager.GetLogger("WeChatAPI");
        }

        /// <summary>
        /// 跳转通过oauth获取ticket, 主要逻辑以及跳转在WeChatBaseController中完成
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public ActionResult GetTicket(int appId, string uri)
        {
            string UserId = ViewBag.WeChatUserID;

            if (string.IsNullOrEmpty(UserId))
            {
                return Content("用户ID为空，请确定您已经关注企业号并且您的状态是在职！");
            }

            log.Debug("API GetTicket Start  UserID:{0} Uri:{1} URL:{2}", UserId, uri,Request.Url);

            string enTicket = EncryptionHelper.ConvertBase64(DesHelper.Encrypt(UserId + "|" + DateTime.UtcNow.ToFileTime().ToString(), CommonService.GetSysConfig("EncryptKey", "")));

            string backUrl ;//= EncryptionHelper.DecodeFrom64(uri) + enTicket;

            if (uri.IndexOf("_") >= 0)
            {
                var u = uri.Split('_');
                var u1 = EncryptionHelper.DecodeFrom64(u[0]);

                backUrl = u1;
                // backUrl = (u1.IndexOf("?") >= 0 ? "&" : "?") + EncryptionHelper.DecodeFrom64(u[1]) + enTicket;
            }
            else
            {
                backUrl = EncryptionHelper.DecodeFrom64(uri);

            }

            string strRet = "";

            if (backUrl.IndexOf("ticket=") > 0)
            {
                strRet = backUrl + Server.UrlEncode(enTicket);
            }
            else
            {
                strRet = backUrl + (backUrl.IndexOf("?") >= 0 ? "&" : "?") + "ticket=" + Server.UrlEncode(enTicket);
            }

            log.Debug("API GetTicket End  backUrl:{0}", strRet);

            return Redirect(strRet);
        }

        /// <summary>
        /// 跳转通过oauth获取ticket, 主要逻辑以及跳转在WeChatBaseController中完成
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public ActionResult GetTicketExt(int appId, string uri)
        {
            string UserId = ViewBag.WeChatUserID;

            if (string.IsNullOrEmpty(UserId))
            {
                return Content("用户ID为空，请确定您已经关注企业号并且您的状态是在职！");
            }

            log.Debug("API GetTicket Start  UserID:{0} Uri:{1} URL:{2}", UserId, uri, Request.Url);

            string enTicket = EncryptionHelper.ConvertBase64(DesHelper.Encrypt(UserId + "|" + DateTime.UtcNow.ToFileTime().ToString(), CommonService.GetSysConfig("EncryptKey", "")));

            string backUrl;//= EncryptionHelper.DecodeFrom64(uri) + enTicket;

            if (uri.IndexOf("_") >= 0)
            {
                var u = uri.Split('_');
                var u1 = EncryptionHelper.DecodeFrom64(u[0]);

                backUrl = u1;
                // backUrl = (u1.IndexOf("?") >= 0 ? "&" : "?") + EncryptionHelper.DecodeFrom64(u[1]) + enTicket;
            }
            else
            {
                backUrl = EncryptionHelper.DecodeFrom64(uri);

            }

            string strRet = "";

            if (backUrl.IndexOf("ticket=") > 0)
            {
                strRet = backUrl + enTicket;
            }
            else
            {
                strRet = backUrl + (backUrl.IndexOf("?") >= 0 ? "&" : "?") + "ticket=" + enTicket;
            }

            var userInfo = ((IAddressBookService)_BaseService).GetMemberByUserId(UserId);

            // var userInfo = UserApi.Info(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, openid);

            strRet = strRet + "&extend1=";

            if (userInfo != null)
            {
                strRet = strRet + userInfo.Extend1;
            }
            else
            {
                log.Error("用户不存在: userid:{0}",  UserId);
            }

            log.Debug("API GetTicket End  backUrl:{0}", strRet);

            return Redirect(strRet);
        }


        /// <summary>
        /// {"appid":"wx41a2bf0afed3b33d","noncestr":"USUtYzXJw4wELBKX","sign":"ed18cbd26d2f82b44dd2fb70743b484e68e68d3c","timestamp":"1438746212"}
        /// </summary>
        /// <returns></returns>
        public ActionResult sign(int appId, string url)
        {

            log.Debug("web API sign Start  appId:{0} Uri:{1} Request URL:{2}", appId, url, Request.Url);

            var config = WeChatCommonService.GetWeChatConfigByID(appId);

            var ret = JSSDKHelper.GetJsSdkUiPackage(config.WeixinCorpId, config.WeixinCorpSecret, url);


            log.Debug("web API sign End  UserID:{0} noncestr:{1} Signature:{2} Timestamp:{3}", config.WeixinAppId, ret.NonceStr, ret.Signature, ret.Timestamp);

            return Json(new
            {
                appid = ret.AppId,
                noncestr = ret.NonceStr,
                sign = ret.Signature,
                timestamp = ret.Timestamp
            }, JsonRequestBehavior.AllowGet);


          
        }

       


    }
}