using System;
using System.IO;
using System.Web.Mvc;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.WeChat.Domain.ViewModelFront;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.CommonAPIs;
using Infrastructure.Utility.Secutiry;
using Infrastructure.Web.UI;
using Infrastructure.Web.Domain.Service;
using System.Text;
using System.Linq;
using Innocellence.Weixin.MP.Helpers;
using System.Collections.Generic;
using Innocellence.WeChat.Domain;
using System.Web.Configuration;
using Newtonsoft.Json;

namespace Innocellence.WeChatMP.Controllers
{
    public class AccessController : WeChatBaseController<SysAddressBookMember, AccessUserInfoView>
    {

        public AccessController(IAddressBookService addressBookServiceService)
            : base(addressBookServiceService)
        {

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



            string enTicket = EncryptionHelper.ConvertBase64(DesHelper.Encrypt(UserId + "|" + DateTime.UtcNow.ToFileTime().ToString(), CommonService.GetSysConfig("EncryptKey", "901212345678901234567890")));

            string backUrl = "";

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
                strRet = backUrl + (backUrl.IndexOf("?") >= 0 ? "&" : "?") + "ticket=" +Server.UrlEncode(enTicket);
            }

            return Redirect(strRet);
        }

        /// <summary>
        /// {"appid":"wx41a2bf0afed3b33d","noncestr":"USUtYzXJw4wELBKX","sign":"ed18cbd26d2f82b44dd2fb70743b484e68e68d3c","timestamp":"1438746212"}
        /// </summary>
        /// <returns></returns>
        public ActionResult sign(int appId, string url)
        {

            var config = WeChatCommonService.GetWeChatConfigByID(appId);
            var ret = JSSDKHelper.GetJsSdkUiPackage(config.WeixinCorpId, config.WeixinCorpSecret, url);

            return Json(new
            {
                appid = ret.AppId,
                noncestr = ret.NonceStr,
                sign = ret.Signature,
                timestamp = ret.Timestamp
            }, JsonRequestBehavior.AllowGet);
        }



        #region 判断是否为会员并返回会员手机号
        public ActionResult CheckMember()
        {
            string returnMsg = string.Empty;
            string mobile = string.Empty;//手机号

            // var agent = new WebServiceAgent(WebConfigurationManager.AppSettings["wentangService"]);

            var agent = new WebServiceAgent(CommonService.GetSysConfig("WentangshequService", ""));

            var methods = agent.Methods;
            dynamic resultd = agent.Invoke("SelectNew", "WechatID", ViewBag.WeChatUserID);
            var objRet = JsonConvert.DeserializeObject(resultd);
            int intStatus = objRet.Status;

            //var wechatBaseUrl = CommonService.GetSysConfig("WeChatUrl", "");

            switch (intStatus)
            {
                case 1:
                    returnMsg = "操作成功";
                    ViewBag.Mobile = objRet.Mobile;
                    //ViewBag.Url = WebConfigurationManager.AppSettings["doctorOnline"] + objRet.Mobile;
                    ViewBag.Url = CommonService.GetSysConfig("doctorOnline", "") + objRet.Mobile;
                    break;

                case 2:
                    returnMsg = "没找到会员";
                    ViewBag.Mobile = "";
                    //ViewBag.RegisterUrl = WebConfigurationManager.AppSettings["register"];
                    ViewBag.RegisterUrl = CommonService.GetSysConfig("register", "");
                    break;
                case 3:
                    returnMsg = "查询到多个会员";
                    ViewBag.Mobile = "3";
                    break;
                case -99:
                    returnMsg = "系统异常";
                    ViewBag.Mobile = "-99";
                    break;
                case -1:
                    returnMsg = "参数不正确";
                    ViewBag.Mobile = "-1";
                    break;
                default:
                    returnMsg = objRet.Remark;
                    ViewBag.Mobile = "";
                    break;
            }


            return View();

        }
        #endregion


    }
}