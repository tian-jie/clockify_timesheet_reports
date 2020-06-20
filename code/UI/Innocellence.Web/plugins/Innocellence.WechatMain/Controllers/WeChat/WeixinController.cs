/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：WeixinController.cs
    文件功能描述：用于处理微信回调的信息
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml.Linq;
//using Innocellence.Weixin.QY.Entities.Request;
using Innocellence.WeChat.Domain.Contracts;

using Innocellence.Weixin.QY.MessageHandlers;
using Innocellence.Weixin.QY.Entities;
using Innocellence.Weixin.QY.Helpers;
using Innocellence.Weixin.QY;
using System.Configuration;
using Infrastructure.Core.Logging;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.CommonService.QyMessageHandlers;
using Innocellence.Weixin.MvcExtension;
using Innocellence.Weixin.Entities;
using System.Threading;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.WeChat.Domain.Service;
//using Innocellence.Weixin.MvcExtension;

namespace Innocellence.WeChatMain.Controllers
{




    public partial class WeixinController : WinxinBaseController
    {
        private WeChatUserRequestMessageLogHandler _requestMsgLog = new WeChatUserRequestMessageLogHandler();
        ILogger log;
        public WeixinController() : base(null)
        {
            log = LogManager.GetLogger("WeChat");
        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信企业后台应用的“修改配置”的Url填写如：http://weixin.Innocellence.com/qy
        /// </summary>
        //[AllowAnonymous]
        [HttpGet]
        [ActionName("Index"), AllowAnonymous]
        public ActionResult Get(string msg_signature = "", string timestamp = "", string nonce = "", string echostr = "")
        {
            try
            {
                var iAppID = int.Parse(Request["AppID"] ?? "0");
                var objConfig = WeChatCommonService.GetWeChatConfigByID(iAppID);
                //return Content(echostr); //返回随机字符串则表示验证通过
                var verifyUrl = Signature.VerifyURL(objConfig.WeixinToken, objConfig.WeixinEncodingAESKey, objConfig.WeixinCorpId, msg_signature, timestamp, nonce,
                    echostr);
                var logstr = string.Format("-objConfig.WeixinToken = {0}\n-objConfig.WeixinEncodingAESKey={1}\n-msg_signature={2}\n-timestamp={3}\n-nonce={4}\n-echostr={5}\n-objConfig.WeixinCorpId={6}", objConfig.WeixinToken, objConfig.WeixinEncodingAESKey, msg_signature, timestamp, nonce, echostr, objConfig.WeixinCorpId);
                log.Debug(logstr);
                if (verifyUrl != null)
                {
                    log.Debug(verifyUrl);
                    return Content(verifyUrl); //返回解密后的随机字符串则表示验证通过
                }
                else
                {
                    log.Debug("verifyUrl is null");
                    return Content("如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, Request.Url.ToString());
            }

            return Content("");
        }

        /// <summary>
        /// 微信后台验证地址（使用Post），微信企业后台应用的“修改配置”的Url填写如：http://weixin.Innocellence.com/qy
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel, bool isDebug = false)
        {
            var maxRecordCount = 10;

            log.Debug("Entering WeChat Post ");

            QyCustomMessageHandler messageHandler = null;

            //var iAppID = int.Parse(Request["AppID"] ?? "1");
            //try
            //{
            //    var objConfig = WeChatCommonService.GetWeChatConfig(int.Parse(WebConfigurationManager.AppSettings["WeixinAppId"]));

            //    postModel.Token = objConfig.WeixinToken;// Token;
            //    postModel.EncodingAESKey = objConfig.WeixinEncodingAESKey;// EncodingAESKey;
            //    postModel.CorpId = objConfig.WeixinCorpId;// CorpId;
            //}
            //catch (Exception ex)
            //{
            //    log.Error(ex, "CommonService.GetWeChatConfig - Exception: " + ex.Message);
            //}

            try
            {
                log.Debug("weixin maxRecordCount2: " + maxRecordCount);
                //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
                messageHandler = new QyCustomMessageHandler(Request.InputStream, postModel, maxRecordCount, isDebug);

                log.Debug("messageHandler.RequestMessage : " + messageHandler.RequestMessage);

                if (messageHandler.RequestMessage == null)
                {
                    log.Debug("messageHandler.RequestMessage is null");
                    //验证不通过或接受信息有错误
                }
                //messageHandler.RequestDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_Request_" + messageHandler.RequestMessage.FromUserName + ".txt"));
                //执行微信处理过程
                messageHandler.Execute();
                //测试时可开启，帮助跟踪数据
                //messageHandler.ResponseDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_Response_" + messageHandler.ResponseMessage.ToUserName + ".txt"));
                //messageHandler.FinalResponseDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_FinalResponse_" + messageHandler.ResponseMessage.ToUserName + ".txt"));
                _requestMsgLog.WriteRequestLog(messageHandler.RequestMessage, postModel.AppId);

                if (isDebug)
                {
                    // messageHandler.FinalResponseDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_FinalResponse_" + messageHandler.ResponseMessage.ToUserName + ".txt"));
                    foreach (var a in messageHandler.ResponseMessage)
                    {
                        var strXML = messageHandler.ResponseDocument(a as ResponseMessageBaseWechat).ToString(SaveOptions.OmitDuplicateNamespaces);
                        log.Debug("messageHandler.ResponseMessage : " + strXML);
                    }
                }
                _requestMsgLog.WriteResponseLog(messageHandler.ResponseMessage, postModel.AppId);
                //自动返回加密后结果
                return new FixWeixinBugWeixinResult(messageHandler);

            }
            catch (Exception ex)
            {
                log.Error(ex, "执行微信处理过程 - Exception: " + ex.Message);
                if (messageHandler != null)
                {
                    log.Error("执行微信处理过程 - Request: " + messageHandler.RequestDocument.ToString());
                    foreach (var a in messageHandler.ResponseMessage)
                    {
                        var strXML = messageHandler.ResponseDocument(a as ResponseMessageBaseWechat).ToString(SaveOptions.OmitDuplicateNamespaces);
                        log.Debug("执行微信处理过程 - Response : " + strXML);
                    }
                }

            }
            return Content("");
        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信企业后台应用的“修改配置”的Url填写如：http://weixin.Innocellence.com/qy
        /// </summary>
        // [AllowAnonymous]
        [HttpGet]
        [ActionName("DebugIndex"), AllowAnonymous]
        public ActionResult MiniGet(string msg_signature = "", string timestamp = "", string nonce = "", string echostr = "")
        {
            return Get(msg_signature, timestamp, nonce, echostr);
        }


        /// <summary>
        /// 
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [ActionName("DebugIndex")]
        public ActionResult MiniPost(PostModel postModel)
        {
            log.Debug("DebugIndex Post");
            return Post(postModel, true);
        }



        /// <summary>
        /// 为测试并发性能而建
        /// </summary>
        /// <returns></returns>
        public ActionResult ForTest()
        {
            //异步并发测试（提供给单元测试使用）
            DateTime begin = DateTime.Now;
            int t1, t2, t3;
            System.Threading.ThreadPool.GetAvailableThreads(out t1, out t3);
            System.Threading.ThreadPool.GetMaxThreads(out t2, out t3);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5));
            DateTime end = DateTime.Now;
            var thread = System.Threading.Thread.CurrentThread;
            var result = string.Format("TId:{0}\tApp:{1}\tBegin:{2:mm:ss,ffff}\tEnd:{3:mm:ss,ffff}\tTPool：{4}",
                    thread.ManagedThreadId,
                    HttpContext.ApplicationInstance.GetHashCode(),
                    begin,
                    end,
                    t2 - t1
                    );
            return Content(result);
        }
    }
}
