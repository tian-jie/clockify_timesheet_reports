/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：CustomMessageHandler.cs
    文件功能描述：自定义MessageHandler
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Innocellence.Weixin.MP.Agent;
using Innocellence.Weixin.Context;
using Innocellence.Weixin.MP.Entities;
using Innocellence.Weixin.MP.Entities.Request;
using Innocellence.Weixin.MP.MessageHandlers;
using Innocellence.Weixin.MP.Helpers;
using Innocellence.Weixin.CommonService.Utilities;
using Innocellence.Weixin.MP;
using Innocellence.Weixin.CommonService.CustomMessageHandler;
using Infrastructure.Core.Logging;
//using Innocellence.LED.Common;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.Entities;
using System.Collections.Generic;
using Innocellence.WeChat.Domain.Common;

namespace Innocellence.Weixin.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        /*
         * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
         * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
         * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
         */


        private ILogger log { get { return LogManager.GetLogger("WeChat"); } }

#if DEBUG
        string agentUrl = "http://localhost:12222/App/Weixin/4";
        string agentToken = "27C455F496044A87";
        string wiweihiKey = "CNadjJuWzyX5bz5Gn+/XoyqiqMa5DjXQ";
#else
        //下面的Url和Token可以用其他平台的消息，或者到www.weiweihi.com注册微信用户，将自动在“微信营销工具”下得到
        private string agentUrl = WebConfigurationManager.AppSettings["WeixinAgentUrl"];//这里使用了www.weiweihi.com微信自动托管平台
        private string agentToken = WebConfigurationManager.AppSettings["WeixinAgentToken"];//Token
        private string wiweihiKey = WebConfigurationManager.AppSettings["WeixinAgentWeiweihiKey"];//WeiweihiKey专门用于对接www.Weiweihi.com平台，获取方式见：http://www.weiweihi.com/ApiDocuments/Item/25#51
#endif

        //private string appId = WebConfigurationManager.AppSettings["WeixinAppId"];
        //private string appSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];

        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, bool isDebug = false)
            : base(inputStream, postModel, maxRecordCount, null, isDebug)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;
        }

        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }


        public override void AfterGetData(string AgentID, PostModel objPostModel)
        {
            log.Debug("Entering AfterGetData0 EncryptPostData AgentID:{0} PostModel AgentID:{1}", AgentID, objPostModel.AppId);

            log.Debug("Entering AfterGetData1 EncryptPostData Msg_Signature:{0} PostModel Timestamp:{1}  Nonce:{2}",
                objPostModel.Msg_Signature, objPostModel.Timestamp, objPostModel.Nonce);
            // log.Debug("objPostData is null?{0} ", objPostData==null);

            var objConfig = Innocellence.WeChat.Domain.Common.WeChatCommonService.GetWeChatConfigByID(int.Parse(objPostModel.AppId));
            if (objConfig == null)
            {
                log.Error("AfterGetData GetWeChatConfig get Error.  EncryptPostData AgentID:{0} PostModel AgentID:{1}", AgentID, objPostModel.AppId);
            }
            else
            {
                objPostModel.CorpId = objConfig.WeixinCorpId;
                objPostModel.EncodingAESKey = objConfig.WeixinEncodingAESKey;
                objPostModel.Token = objConfig.WeixinToken;
                objPostModel.AppSecret = objConfig.WeixinCorpSecret;
            }

        }

        public override void AfterDecryptData(string strXML, PostModel objPostModel)
        {
            log.Debug("Entering AfterDecryptData isDebug:{0}", _isDebug);

            if (_isDebug)
            {
                log.Debug("AfterDecryptData - " + strXML);
            }

        }


        //private List<IResponseMessageBase> doRequest(string strReq,RequestMessageBase req)
        //{


        //    log.Debug("RequestMessages Count:" + WeixinContext.GetMessageContext(req).RequestMessages.Count);

        //    log.Debug("RequestMessages FindIndex:" + WeixinContext.GetMessageContext(req).RequestMessages.FindIndex(
        //            a => (a.MsgType == RequestMsgType.Text && ((RequestMessageText)a).Content.IndexOf("使用语音回复") >= 0)));

        //    bool bolTextMsg = true;
        //    if (strReq.IndexOf("结束语音回复") >= 0 || strReq.IndexOf("请使用文字回复") >= 0)
        //    {
        //        WeixinContext.GetMessageContext(req).RequestMessages.Clear();
        //    }
        //    else

        //        if (WeixinContext.GetMessageContext(req).RequestMessages.FindIndex(
        //            a => (a.MsgType == RequestMsgType.Text && ((RequestMessageText)a).Content.IndexOf("使用语音回复") >= 0) ||
        //                (a.MsgType == RequestMsgType.Voice && ((RequestMessageVoice)a).Recognition.IndexOf("使用语音回复") >= 0)
        //            ) >= 0)
        //        {
        //            log.Debug("使用语音回复！");
        //            bolTextMsg = false;
        //        }



        //    var retMsg = TulingRobot.GetMsg(strReq, req.FromUserName, this);

        //    if (retMsg.MsgType == ResponseMsgType.Text && bolTextMsg == false) //语音接口
        //    {
        //        string strPara = string.Format("text={0}&ctp=1&per=0", HttpContext.Current.Server.UrlEncode(((ResponseMessageText)retMsg).Content));
        //        dynamic obj = BaiduAPI.GetMsg("/baidutts/tts", strPara);

        //        // Convert.FromBase64String(obj.retData);

        //        if (obj.errNum.ToString() != "0")
        //        {
        //            ResponseMessageText a = (ResponseMessageText)retMsg;
        //            a.Content = "[语音错误]" + a.Content;
        //            return a;
        //        }
        //        else
        //        {
        //            log.Debug("获取语音数据！");


        //            //   File.WriteAllBytes("d:\\mp3\\"+DateTime.Now.ToString("yyyyMMddHHmmss")+".mp3",Convert.FromBase64String(obj.retData.ToString()));

        //            using (Stream s = new MemoryStream(Convert.FromBase64String(obj.retData.ToString())))
        //            {
        //                var objJ = MediaApi.UploadTemporaryMedia(_postModel.AppId, _postModel.AppSecret, UploadMediaFileType.voice, s, "test.mp3");

        //                log.Debug("上传资料库！" + objJ.media_id);

        //                var ret = base.CreateResponseMessage<ResponseMessageVoice>();
        //                ret.Voice.MediaId = objJ.media_id;
        //                return ret;
        //            }
        //        }



        //    }
        //    else
        //    {
        //        return retMsg;
        //    }

        //    return new List<IResponseMessageBase>() { base.CreateResponseMessage<ResponseMessageText>() };
        //}

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnTextRequest(RequestMessageText requestMessage)
        {
            return CreateResponseMessage(requestMessage.Content, (int)AutoReplyMPKeywordEnum.TEXT);
            #region ignore code
            //return doRequest(requestMessage.Content, requestMessage);

            //var responseMessage = base.CreateResponseMessage<ResponseMessageText>();

            //if (requestMessage.Content == null)
            //{

            //}
            //else if (requestMessage.Content == "约束")
            //{
            //    responseMessage.Content =
            //        "<a href=\"http://weixin.Innocellence.com/FilterTest/\">点击这里</a>进行客户端约束测试（地址：http://weixin.Innocellence.com/FilterTest/）。";
            //}
            //else if (requestMessage.Content == "托管" || requestMessage.Content == "代理")
            //{
            //    //开始用代理托管，把请求转到其他服务器上去，然后拿回结果
            //    //甚至也可以将所有请求在DefaultResponseMessage()中托管到外部。

            //    DateTime dt1 = DateTime.Now; //计时开始

            //    var responseXml = MessageAgent.RequestXml(this, agentUrl, agentToken, RequestDocument.ToString());
            //    //获取返回的XML
            //    //上面的方法也可以使用扩展方法：this.RequestResponseMessage(this,agentUrl, agentToken, RequestDocument.ToString());

            //    /* 如果有WeiweihiKey，可以直接使用下面的这个MessageAgent.RequestWeiweihiXml()方法。
            //     * WeiweihiKey专门用于对接www.weiweihi.com平台，获取方式见：http://www.weiweihi.com/ApiDocuments/Item/25#51
            //     */
            //    //var responseXml = MessageAgent.RequestWeiweihiXml(weiweihiKey, RequestDocument.ToString());//获取Weiweihi返回的XML

            //    DateTime dt2 = DateTime.Now; //计时结束

            //    //转成实体。
            //    /* 如果要写成一行，可以直接用：
            //     * responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            //     * 或
            //     * 
            //     */
            //    responseMessage = responseXml.CreateResponseMessage() as ResponseMessageText;

            //    responseMessage.Content += string.Format("\r\n\r\n代理过程总耗时：{0}毫秒", (dt2 - dt1).Milliseconds);
            //}
            //else if (requestMessage.Content == "测试" || requestMessage.Content == "退出")
            //{
            //    /* 
            //    * 这是一个特殊的过程，此请求通常来自于微微嗨（http://www.weiweihi.com）的“盛派网络小助手”应用请求（http://www.weiweihi.com/User/App/Detail/1），
            //    * 用于演示微微嗨应用商店的处理过程，由于微微嗨的应用内部可以单独设置对话过期时间，所以这里通常不需要考虑对话状态，只要做最简单的响应。
            //    */
            //    if (requestMessage.Content == "测试")
            //    {
            //        //进入APP测试
            //        responseMessage.Content = "您已经进入【盛派网络小助手】的测试程序，请发送任意信息进行测试。发送文字【退出】退出测试对话。10分钟内无任何交互将自动退出应用对话状态。";
            //    }
            //    else
            //    {
            //        //退出APP测试
            //        responseMessage.Content = "您已经退出【盛派网络小助手】的测试程序。";
            //    }
            //}
            //else if (requestMessage.Content == "AsyncTest")
            //{
            //    //异步并发测试（提供给单元测试使用）
            //    DateTime begin = DateTime.Now;
            //    int t1, t2, t3;
            //    System.Threading.ThreadPool.GetAvailableThreads(out t1, out t3);
            //    System.Threading.ThreadPool.GetMaxThreads(out t2, out t3);
            //    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(4));
            //    DateTime end = DateTime.Now;
            //    var thread = System.Threading.Thread.CurrentThread;
            //    responseMessage.Content = string.Format("TId:{0}\tApp:{1}\tBegin:{2:mm:ss,ffff}\tEnd:{3:mm:ss,ffff}\tTPool：{4}",
            //            thread.ManagedThreadId,
            //            HttpContext.Current != null ? HttpContext.Current.ApplicationInstance.GetHashCode() : -1,
            //            begin,
            //            end,
            //            t2 - t1
            //            );
            //}
            //else
            //{
            //    var result = new StringBuilder();
            //    result.AppendFormat("您刚才发送了文字信息：{0}\r\n\r\n", requestMessage.Content);

            //    if (CurrentMessageContext.RequestMessages.Count > 1)
            //    {
            //        result.AppendFormat("您刚才还发送了如下消息（{0}/{1}）：\r\n", CurrentMessageContext.RequestMessages.Count,
            //            CurrentMessageContext.StorageData);
            //        for (int i = CurrentMessageContext.RequestMessages.Count - 2; i >= 0; i--)
            //        {
            //            var historyMessage = CurrentMessageContext.RequestMessages[i];
            //            result.AppendFormat("{0} 【{1}】{2}\r\n",
            //                historyMessage.CreateTime.ToShortTimeString(),
            //                historyMessage.MsgType.ToString(),
            //                (historyMessage is RequestMessageText)
            //                    ? (historyMessage as RequestMessageText).Content
            //                    : "[非文字类型]"
            //                );
            //        }
            //        result.AppendLine("\r\n");
            //    }

            //    result.AppendFormat("如果您在{0}分钟内连续发送消息，记录将被自动保留（当前设置：最多记录{1}条）。过期后记录将会自动清除。\r\n",
            //        WeixinContext.ExpireMinutes, WeixinContext.MaxRecordCount);
            //    result.AppendLine("\r\n");
            //    result.AppendLine(
            //        "您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：http://weixin.Innocellence.com");

            //    responseMessage.Content = result.ToString();
            //}
            //return new List<IResponseMessageBase>(){responseMessage};
            #endregion
        }

        /// <summary>
        /// 处理位置请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnLocationRequest(RequestMessageLocation requestMessage)
        {
            //var locationService = new LocationService();
            //var responseMessage = locationService.GetResponseMessage(requestMessage as RequestMessageLocation);
            //return new List<IResponseMessageBase>(){responseMessage};
            return this.CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.LOCATION);
        }

        public override List<IResponseMessageBase> OnShortVideoRequest(RequestMessageShortVideo requestMessage)
        {
            //var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "您刚才发送的是小视频";
            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.VIDEO);
        }

        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnImageRequest(RequestMessageImage requestMessage)
        {
            //var responseMessage = CreateResponseMessage<ResponseMessageNews>();
            //responseMessage.Articles.Add(new Article()
            //{
            //    Title = "您刚才发送了图片信息",
            //    Description = "您发送的图片将会显示在边上",
            //    PicUrl = requestMessage.PicUrl,
            //    Url = "http://weixin.Innocellence.com"
            //});
            //responseMessage.Articles.Add(new Article()
            //{
            //    Title = "第二条",
            //    Description = "第二条带连接的内容",
            //    PicUrl = requestMessage.PicUrl,
            //    Url = "http://weixin.Innocellence.com"
            //});

            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.IMAGE);
        }

        /// <summary>
        /// 处理语音请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnVoiceRequest(RequestMessageVoice requestMessage)
        {

            return CreateResponseMessage(requestMessage.MediaId, (int)AutoReplyMPKeywordEnum.AUDIO);
            //return doRequest(requestMessage.Recognition, requestMessage);

            // return TulingRobot.GetMsg(requestMessage.Recognition, requestMessage.FromUserName, this);


            // var responseMessage = CreateResponseMessage<ResponseMessageMusic>();
            // //上传缩略图
            //// var accessToken = Innocellence.Weixin.MP.CommonAPIs.AccessTokenContainer.GetToken(appId, appSecret);
            // var uploadResult = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(appId, appSecret, UploadMediaFileType.image,
            //                                              Server.GetMapPath("~/Images/Logo.jpg"));

            // //设置音乐信息
            // responseMessage.Music.Title = "天籁之音";
            // responseMessage.Music.Description = "播放您上传的语音";
            // responseMessage.Music.MusicUrl = "http://weixin.Innocellence.com/Media/GetVoice?mediaId=" + requestMessage.MediaId;
            // responseMessage.Music.HQMusicUrl = "http://weixin.Innocellence.com/Media/GetVoice?mediaId=" + requestMessage.MediaId;
            // responseMessage.Music.ThumbMediaId = uploadResult.media_id;
            // return new List<IResponseMessageBase>(){responseMessage};
        }
        /// <summary>
        /// 处理视频请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnVideoRequest(RequestMessageVideo requestMessage)
        {
            //var responseMessage = CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "您发送了一条视频信息，ID：" + requestMessage.MediaId;
            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.VIDEO);
        }

        /// <summary>
        /// 处理链接消息请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnLinkRequest(RequestMessageLink requestMessage)
        {
            //            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            //            responseMessage.Content = string.Format(@"您发送了一条连接信息：
            //Title：{0}
            //Description:{1}
            //Url:{2}", requestMessage.Title, requestMessage.Description, requestMessage.Url);
            //            return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.LINK);
        }

        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var eventResponseMessage = base.OnEventRequest(requestMessage);//对于Event下属分类的重写方法，见：CustomerMessageHandler_Events.cs
            //TODO: 对Event信息进行统一操作
            return eventResponseMessage;
        }

        public override List<IResponseMessageBase> DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
             * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
             * 只需要在这里统一发出委托请求，如：
             * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
             * return new List<IResponseMessageBase>(){responseMessage};
             */

            log.Debug("Default RequestMessage！MsgType:{0}", requestMessage.MsgType);

            if (requestMessage.MsgType== RequestMsgType.Event)
            {
                var rm = requestMessage as IRequestMessageEventBase;
                if (rm != null)
                {
                    log.Debug("Default RequestMessage！Event:{0}", rm.Event);
                }
            }

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "";
            return new List<IResponseMessageBase>() { responseMessage };
        }
    }
}
