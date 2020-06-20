/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：WeixinController.cs
    文件功能描述：用于处理微信回调的信息
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml.Linq;
//using Innocellence.Weixin.QY.Entities.Request;
using Innocellence.WeChat.Domain.Contracts;

namespace Innocellence.WeChatMain.Controllers
{
    using Innocellence.Weixin.QY.MessageHandlers;
    using Innocellence.Weixin.QY.Entities;
    using Innocellence.Weixin.QY.Helpers;
    using Innocellence.Weixin.QY;
    using Innocellence.WeChat.Domain.Service;
    using Innocellence.Weixin.QY.CommonAPIs;
    using Infrastructure.Core.Logging;
    using Innocellence.WeChat.Domain.Common;


    public partial class ATokenController : Controller
    {

        //public ATokenController(ICategoryService newsService)
        //    : base(newsService)
        //{

        //}

        /// <summary>
        /// 微信后台验证地址（使用Get），微信企业后台应用的“修改配置”的Url填写如：http://weixin.Innocellence.com/qy
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(string AppID)
        {
            try
            {
                if (string.IsNullOrEmpty(Request["AppID"]))
                {
                    return Json(new { Token = "", ErrorMsg = "Please Provide AppID!" }, JsonRequestBehavior.AllowGet);
                }
                var iAppID = int.Parse(Request["AppID"]);
                var strToken = GetTokenByID(iAppID);


                return Json(new { Token = strToken, ErrorMsg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Token = "", ErrorMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
       
        }

        public string GetTokenByID(int iWeChatID)
        {
            LogManager.GetLogger(this.GetType()).Debug("Request.Url.AbsoluteUri:" + Request.Url.AbsoluteUri);
            var Config = WeChatCommonService.GetWeChatConfigByID(iWeChatID);
            LogManager.GetLogger(this.GetType()).Debug("host:" + Request.Url.Host);
            return AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);

            // return resultToken.access_token;
        }

        [HttpGet]
        [ActionName("GetToken")]
        public ActionResult GetToken(string corpId, string corpSecret, bool forceGet = false)
        {
            LogManager.GetLogger(this.GetType()).Debug("Request.Url.AbsoluteUri:" + Request.Url.AbsoluteUri);
            try
            {
                if (string.IsNullOrEmpty(corpSecret) || string.IsNullOrEmpty(corpSecret))
                {
                    return Json(new { Token = "", ErrorMsg = "Please Provide corpId or corpSecret!" }, JsonRequestBehavior.AllowGet);
                }

                //var conf = WeChatCommonService.lstAccountManage.Find();

                var strToken = Innocellence.Weixin.CommonAPIs.AccessTokenCache<Innocellence.Weixin.Entities.AccessTokenBag, Innocellence.Weixin.Entities.AccessTokenResult>.GetAccessTokenResult(corpId, corpSecret, Innocellence.Weixin.MP.CommonAPIs.CommonApi.GetToken, forceGet);


                return Json(new Innocellence.Weixin.CommonAPIs.TokenEntity(){  AccessTokenExpireTime = strToken.ExpireTime, expires_in = strToken.TokenResult.expires_in, Token = strToken.TokenResult.access_token, ErrorMsg = strToken.TokenResult.errmsg  }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(this.GetType()).Error(ex);

                return Json(new { Token = "", ErrorMsg = ex.Message + ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult GetTicket(string corpId, string corpSecret, bool forceGet = false)
        {
            LogManager.GetLogger(this.GetType()).Debug("Request.Url.AbsoluteUri:" + Request.Url.AbsoluteUri);
            try
            {
                if (string.IsNullOrEmpty(corpSecret) || string.IsNullOrEmpty(corpSecret))
                {
                    return Json(new { Token = "", ErrorMsg = "Please Provide corpId or corpSecret!" }, JsonRequestBehavior.AllowGet);
                }

                var ticket = Innocellence.Weixin.MP.CommonAPIs.JsApiTicketContainer.GetJsApiTicketResult(corpId, corpSecret, forceGet);


                return Json(new { Token = ticket.ticket, expires_in = ticket.expires_in, ErrorMsg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                 LogManager.GetLogger(this.GetType()).Error(ex);
                return Json(new { Token = "", ErrorMsg = ex.Message + ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
