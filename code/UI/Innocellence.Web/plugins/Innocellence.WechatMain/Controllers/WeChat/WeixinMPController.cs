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

using Innocellence.Weixin.MP.MessageHandlers;
using Innocellence.Weixin.MP.Entities;
using Innocellence.Weixin.MP.Entities.Request;
using Innocellence.Weixin.MP.Helpers;
using Innocellence.Weixin.MP;
using System.Configuration;
using Infrastructure.Core.Logging;
using Innocellence.WeChat.Domain.Common;
//using Innocellence.Weixin.CommonService.QyMessageHandlers;
using Innocellence.Weixin.MvcExtension;
using Innocellence.Weixin.CommonService.CustomMessageHandler;

using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.Entities;
//using Innocellence.Weixin.MvcExtension;

namespace Innocellence.WeChatMain.Controllers
{
   
  


    public partial class WeixinMPController : WinxinBaseController
    {
        private WeChatUserRequestMessageLogHandler _requestMsgLog = new WeChatUserRequestMessageLogHandler();
        ILogger log;
        public WeixinMPController()
            : base(null)
        {
            log = LogManager.GetLogger("WeChat");
        }

        ///// <summary>
        ///// 微信后台验证地址（使用Get），微信企业后台应用的“修改配置”的Url填写如：http://weixin.Innocellence.com/qy
        ///// </summary>
        //[HttpGet]
        //[ActionName("Index"), AllowAnonymous]
        //public ActionResult Get(string msg_signature = "", string timestamp = "", string nonce = "", string echostr = "")
        //{
        //    try
        //    {
        //        var iAppID = int.Parse(Request["AppID"] ?? "0");
        //        var objConfig = WeChatCommonService.GetWeChatConfig(iAppID);
        //        //return Content(echostr); //返回随机字符串则表示验证通过
        //        var verifyUrl = Signature.VerifyURL(objConfig.WeixinToken, objConfig.WeixinEncodingAESKey, objConfig.WeixinCorpId, msg_signature, timestamp, nonce,
        //            echostr);
        //        var logstr = string.Format("-objConfig.WeixinToken = {0}\n-objConfig.WeixinEncodingAESKey={1}\n-msg_signature={2}\n-timestamp={3}\n-nonce={4}\n-echostr={5}\n-objConfig.WeixinCorpId={6}", objConfig.WeixinToken, objConfig.WeixinEncodingAESKey, msg_signature, timestamp, nonce, echostr, objConfig.WeixinCorpId);
        //        log.Debug(logstr);
        //        if (verifyUrl != null)
        //        {
        //            log.Debug(verifyUrl);
        //            return Content(verifyUrl); //返回解密后的随机字符串则表示验证通过
        //        }
        //        else
        //        {
        //            log.Debug("verifyUrl is null");
        //            return Content("如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex, Request.Url.ToString());
        //    }

        //    return Content("");
        //}

        ///// <summary>
        ///// 微信后台验证地址（使用Post），微信企业后台应用的“修改配置”的Url填写如：http://weixin.Innocellence.com/qy
        ///// </summary>
        //[HttpPost]
        //[ActionName("Index")]
        //public ActionResult Post(PostModel postModel, bool isDebug = false)
        //{
        //    var maxRecordCount = 10;

        //    log.Debug("Entering WeChat Post ");

        //    QyCustomMessageHandler messageHandler = null;

        //    //var iAppID = int.Parse(Request["AppID"] ?? "1");
        //    try
        //    {
        //        var objConfig = WeChatCommonService.GetWeChatConfig(int.Parse(WebConfigurationManager.AppSettings["WeixinAppId"]));

        //        postModel.Token = objConfig.WeixinToken;// Token;
        //        postModel.EncodingAESKey = objConfig.WeixinEncodingAESKey;// EncodingAESKey;
        //        postModel.CorpId = objConfig.WeixinCorpId;// CorpId;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex, "CommonService.GetWeChatConfig - Exception: " + ex.Message);
        //    }

        //    try
        //    {
        //        log.Debug("weixin maxRecordCount2: " + maxRecordCount);
        //        //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
        //        messageHandler = new QyCustomMessageHandler(Request.InputStream, postModel, maxRecordCount, isDebug);

        //        log.Debug("messageHandler.RequestMessage : " + messageHandler.RequestMessage);

        //        if (messageHandler.RequestMessage == null)
        //        {
        //            log.Debug("messageHandler.RequestMessage is null");
        //            //验证不通过或接受信息有错误
        //        }

        //        //messageHandler.RequestDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_Request_" + messageHandler.RequestMessage.FromUserName + ".txt"));
        //        //执行微信处理过程
        //        messageHandler.Execute();
        //        //测试时可开启，帮助跟踪数据
        //        //messageHandler.ResponseDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_Response_" + messageHandler.ResponseMessage.ToUserName + ".txt"));
        //        //messageHandler.FinalResponseDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_FinalResponse_" + messageHandler.ResponseMessage.ToUserName + ".txt"));

        //        if (isDebug)
        //        {
        //            // messageHandler.FinalResponseDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_FinalResponse_" + messageHandler.ResponseMessage.ToUserName + ".txt"));

        //            log.Debug("messageHandler.ResponseMessage : " + messageHandler.FinalResponseDocument.Document.ToString());
        //        }

        //        //自动返回加密后结果
        //        return new FixWeixinBugWeixinResult(messageHandler);

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex, "执行微信处理过程 - Exception: " + ex.Message);
        //        if (messageHandler != null)
        //        {
        //            log.Error("执行微信处理过程 - Request: " + messageHandler.RequestDocument.ToString());
        //            log.Error("执行微信处理过程 - Response: " + messageHandler.FinalResponseDocument.ToString());
        //        }

        //    }
        //    return Content("");
        //}


        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://weixin.senparc.com/weixin
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, "Innocellence"))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + Innocellence.Weixin.MP.CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, "Innocellence") + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel, bool isDebug = false)
        {
            //if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            //{
            //    return Content("参数错误！");
            //}

            log.Debug("执行微信处理过程MP - Post: ");

            //postModel.Token = Token;//根据自己后台的设置保持一致
            //postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            //postModel.AppId = AppId;//根据自己后台的设置保持一致

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = 50;

            //var logPath = Server.MapPath(string.Format("~/App_Data/MP/{0}/", DateTime.Now.ToString("yyyy-MM-dd")));
            //if (!Directory.Exists(logPath))
            //{
            //    Directory.CreateDirectory(logPath);
            //}

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, maxRecordCount, isDebug);


            try
            {
                ////测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                //messageHandler.RequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                //if (messageHandler.UsingEcryptMessage)
                //{
                //    messageHandler.EcryptRequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_Ecrypt_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                //}

                /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                 * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
                messageHandler.OmitRepeatedMessage = true;
               
                log.Debug("******************Begin Execute");
                //执行微信处理过程
                messageHandler.Execute();

                //日志中需要记录人员信息
                _requestMsgLog.WriteRequestLogMP(messageHandler.RequestMessage, postModel.AppId);

                //测试时可开启，帮助跟踪数据

                //if (messageHandler.ResponseDocument == null)
                //{
                //    throw new Exception(messageHandler.RequestDocument.ToString());
                //}

                //if (messageHandler.ResponseDocument != null)
                //{
                //    messageHandler.ResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                //}

                //if (messageHandler.UsingEcryptMessage)
                //{
                //    //记录加密后的响应信息
                //    messageHandler.FinalResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_Final_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                //}



                if (isDebug)
                {
                    // messageHandler.FinalResponseDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_FinalResponse_" + messageHandler.ResponseMessage.ToUserName + ".txt"));
                    foreach (var a in messageHandler.ResponseMessage)
                    {
                        var strXML = messageHandler.ResponseDocument(a as ResponseMessageBaseWechat).ToString(SaveOptions.OmitDuplicateNamespaces);
                        log.Debug("messageHandler.ResponseMessage : " + strXML);
                    }
                }
                log.Debug("******************Begin response log");
                _requestMsgLog.WriteResponseLogMP(messageHandler.ResponseMessage, postModel.AppId);
                //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
                return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
                //return new WeixinResult(messageHandler);//v0.8+
            }
            catch (Exception ex)
            {
                log.Error(ex, "执行微信处理过程MP - Exception: " + ex.Message);
              
                if (messageHandler != null)
                {
                    log.Error("执行微信处理过程MP - Request: " + messageHandler.RequestDocument.ToString());
                    foreach (var a in messageHandler.ResponseMessage)
                    {
                        var strXML = messageHandler.FinalResponseDocument(a as ResponseMessageBaseWechat).ToString(SaveOptions.OmitDuplicateNamespaces);
                        log.Debug("messageHandler.ResponseMessage : " + strXML);
                    }
                }
                return Content("");
            }
        }


        /// <summary>
        /// 微信后台验证地址（使用Get），微信企业后台应用的“修改配置”的Url填写如：http://weixin.Innocellence.com/qy
        /// </summary>
     //   [AllowAnonymous]
        [HttpGet]
        [ActionName("DebugIndex"), AllowAnonymous]
        public ActionResult MiniGet(PostModel postModel, string echostr)
        {
            return Get(postModel, echostr);
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

        // [AllowAnonymous]
        //public ActionResult TestTuling(string strMsg)
        //{
        //   //var result= TulingRobot.GetTulingMsg(strMsg, "cwwhy1");


        //    string strPara = string.Format("text={0}&ctp=1&per=0", Server.UrlEncode(strMsg));
        //    dynamic obj = BaiduAPI.GetMsg("/baidutts/tts", strPara);

        //    return Content(obj.retData.ToString());
        //}



         //[AllowAnonymous]
         //public ActionResult TestSucai(string strMsg)
         //{
         //    //var result= TulingRobot.GetTulingMsg(strMsg, "cwwhy1");

         //    var by = System.IO.File.ReadAllBytes("d:\\mp3\\20160511095844.mp3");

         //    using (Stream s = new MemoryStream(by))
         //    {
         //        var objJ = MediaApi.UploadTemporaryMedia("wx2a3f5167603c5caf", "07794839c40bfecc0e1f89322921558e", UploadMediaFileType.voice, "d:\\mp3\\20160511095844.mp3");
         //        return Content(objJ.media_id);
         //    }

            
         //}
    }
}
