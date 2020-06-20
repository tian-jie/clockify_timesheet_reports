/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：QyCustomMessageHandler.cs
    文件功能描述：自定义QyMessageHandler
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

//using Innocellence.CA.Service.Common;
//using Innocellence.CA.Services;
//using Innocellence.Weixin.CommonService.QyMessageHandler;
using Infrastructure.Core.Logging;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Services;
using Innocellence.Weixin;
using Innocellence.Weixin.CommonService.QyMessageHandler;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.Entities.QY;
using Innocellence.Weixin.Exceptions;
using Innocellence.Weixin.QY;
using Innocellence.Weixin.QY.Entities;
using Innocellence.Weixin.QY.MessageHandlers;
using System;
using System.Collections.Generic;






namespace Innocellence.Activity.QyMessageHandlers
{
    public class QyCustomMessageHandler : QyMessageHandler<QyCustomMessageContext>
    {
        private WeChatAppUserService _weChatAppUserService = new WeChatAppUserService();

        // private BarrageSummaryService _BarrageSummaryService = new BarrageSummaryService();
        //private BarrageService _BarrageService = new BarrageService();
        private static ILogger _log = LogManager.GetLogger("WeChat");


        public static void StaticQyMessageEventHandler(IQyMessageHandler messageHandler)
        {
            _log.Debug("Entering Innocellence.Activity.QyMessageHandlers.StaticQyMessageEventHandler.....");
            var newHandler = new QyCustomMessageHandler((RequestMessageBase)messageHandler.RequestMessage);
            _log.Debug("Executing Innocellence.Activity.QyMessageHandlers..... from user:{0}", newHandler.RequestMessage.FromUserName);
            newHandler.Execute();
            _log.Debug("Executed Innocellence.Activity.QyMessageHandlers.....");

            messageHandler.ResponseMessage = newHandler.ResponseMessage;
            _log.Debug("Leaving Innocellence.Activity.QyMessageHandlers.....");



        }

        private ILogger log { get { return LogManager.GetLogger("WeChat"); } }

        public QyCustomMessageHandler(RequestMessageBase requestMessageBase, int maxRecordCount = 0, object postData = null, bool isDebug = false)
            : base(requestMessageBase, maxRecordCount, postData, isDebug)
        {

            _isDebug = isDebug;



        }

        static WebRequestPost _wr = new WebRequestPost();

        //public override List<IResponseMessageBase> OnTextRequest(RequestMessageText requestMessage)
        //{
        //    //记录用户行为
        //    //_wr.CallUserBehavior("TEXT_EVENT", requestMessage.FromUserName, requestMessage.AgentID.ToString(CultureInfo.InvariantCulture), requestMessage.Content, "", 8);
        //    LogManager.GetLogger(this.GetType()).Debug("ON Activity TEXT behavior1 ");

        //    var agentid = requestMessage.AgentID.ToString();
        //    string keyword = string.Empty;
        //    var barrageSummary = new BarrageSummary();
        //    int signal = 0;
        //    string content = requestMessage.Content;
        //    if (string.IsNullOrEmpty(content))
        //    {
        //        return null;
        //    }
        //    content = content.Trim();
        //    LogManager.GetLogger(this.GetType()).Debug("BarrageSummary - content={0}", content);
        //    int startIndex = content.IndexOf("+");
        //    if (content.Length >= 3 && startIndex > 0 && content.Length > startIndex)
        //    {
        //        keyword = content.Substring(0, startIndex);
        //        content = content.Substring(startIndex + 1);

        //        LogManager.GetLogger(this.GetType()).Debug("BarrageSummary - keyword={0}", keyword);
        //        barrageSummary = _BarrageSummaryService.Repository.Entities.AsNoTracking().Where(a => a.AppId == agentid && a.Keyword.Equals(keyword, StringComparison.InvariantCultureIgnoreCase) && a.IsDeleted == false).FirstOrDefault();
        //    }
        //    else if (content.Length > 0 && startIndex < 0)
        //    {
        //        barrageSummary = _BarrageSummaryService.Repository.Entities.AsNoTracking().Where(a => a.AppId == agentid && a.Keyword.Equals(null, StringComparison.InvariantCultureIgnoreCase) && a.IsDeleted == false).FirstOrDefault();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //    if (barrageSummary != null)
        //    {
        //        LogManager.GetLogger(this.GetType()).Debug("BarrageSummary.Title={0}", barrageSummary.Title);
        //        string wxId = requestMessage.FromUserName;
        //        var name = string.Empty;
        //        var weixinPic = string.Empty;
        //        if (!string.IsNullOrEmpty(wxId))
        //        {
        //          var config=  WeChatCommonService.lstSysWeChatConfig.Find(a=>a.WeixinCorpId==requestMessage.ToUserName);

        //          List<GetMemberResult> userlist = WeChatCommonService.lstUser(config.AccountManageId.Value);
        //            var emp = userlist.SingleOrDefault(a => a.userid.Equals(wxId, StringComparison.InvariantCultureIgnoreCase));
        //            if (emp != null)
        //            {
        //                name = emp.name;
        //                weixinPic = emp.avatar == null ? null : emp.avatar;
        //            }
        //        }
        //        var barrage = new Barrage
        //        {
        //            AppId = agentid,
        //            Keyword = keyword,
        //            FeedBackContent = ConvertEmotion(content),
        //            CreatedDate = DateTime.Now,
        //            SummaryId = barrageSummary.Id,
        //            WeixinId = requestMessage.FromUserName,
        //            Status = 0,
        //            WeixinName = name,
        //            IsDisplay = false,
        //            WeixinPic = weixinPic

        //        };
        //        signal = _BarrageService.Repository.Insert(barrage);
        //        LogManager.GetLogger(this.GetType()).Debug("插入成功");
        //        if (signal > 0)
        //        {
        //            LogManager.GetLogger(this.GetType()).Debug("BarrageSummary.ReturnText={0}", barrageSummary.ReturnText);
        //            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //            responseMessage.Content = barrageSummary.ReturnText;
        //            return new List<IResponseMessageBase>() { responseMessage };
        //        }

        //    }
        //    LogManager.GetLogger(this.GetType()).Debug("not found, BarrageSummary is null");
        //    return null;

        //}

        public static string ConvertEmotion(string content)
        {
            var emotions = WeChatCommonService.lstWechatEmotion;
            foreach (var e in emotions)
            {
                content = content.Replace(e.Code, string.Format("<img src=\"/plugins/Innocellence.Activity/Content/face/{0}\"/>", e.Target));
            }
            return content;
        }

        /// <summary>
        /// Event事件类型请求之CLICK
        /// </summary>
        public override List<IResponseMessageBase> OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 首次关注应用事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 用户进入应用事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_EnterAgentRequest(RequestMessageEvent_Enter_Agent requestMessage)
        {
            return null;
        }

        public override List<IResponseMessageBase> OnImageRequest(RequestMessageImage requestMessage)
        {
            return null;
        }

        public override List<IResponseMessageBase> OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            return null;
        }

        public override List<IResponseMessageBase> OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            return null;
        }

        public override List<IResponseMessageBase> OnEvent_BatchJobResultRequest(RequestMessageEvent_Batch_Job_Result requestMessage)
        {
            return null;
        }

        public override List<IResponseMessageBase> DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            throw new NotImplementedException();
        }

        public override void Execute()
        {
            LogManager.GetLogger(this.GetType()).Debug("ON Activity Execute behavior1 ");
            switch (RequestMessage.MsgType)
            {
                case RequestMsgType.DEFAULT://第三方回调
                    {
                        if (RequestMessage is IThirdPartyInfoBase)
                        {
                            var thirdPartyInfo = RequestMessage as IThirdPartyInfoBase;
                            switch (thirdPartyInfo.InfoType)
                            {
                                case ThirdPartyInfo.SUITE_TICKET:
                                    break;
                                case ThirdPartyInfo.CHANGE_AUTH:
                                    break;
                                case ThirdPartyInfo.CANCEL_AUTH:
                                    break;
                                default:
                                    throw new UnknownRequestMsgTypeException("未知的InfoType请求类型", null);
                            }
                            TextResponseMessage = "success";//设置文字类型返回
                        }
                        else
                        {
                            throw new WeixinException("没有找到合适的消息类型。");
                        }
                    }
                    break;

                case RequestMsgType.Chat:

                    ResponseMessage = OnChatRequest(RequestMessage as RequestMessageChat);
                    ResponseMessage = null;  //不参与上下文记录
                    break;

                //以下是普通信息
                case RequestMsgType.Text:
                    {
                        LogManager.GetLogger(this.GetType()).Debug("ON Activity Execute behavior - RequestMsgType.Text ");
                        var requestMessage = RequestMessage as RequestMessageText;
                        ResponseMessage = OnTextOrEventRequest(requestMessage) ?? OnTextRequest(requestMessage);
                    }
                    break;
                case RequestMsgType.Location:
                    ResponseMessage = OnLocationRequest(RequestMessage as RequestMessageLocation);
                    break;
                case RequestMsgType.Image:
                    ResponseMessage = OnImageRequest(RequestMessage as RequestMessageImage);
                    break;
                case RequestMsgType.Voice:
                    ResponseMessage = OnVoiceRequest(RequestMessage as RequestMessageVoice);
                    break;
                case RequestMsgType.Video:
                    ResponseMessage = OnVideoRequest(RequestMessage as RequestMessageVideo);
                    break;
                case RequestMsgType.ShortVideo:
                    ResponseMessage = OnShortVideoRequest(RequestMessage as RequestMessageShortVideo);
                    break;
                case RequestMsgType.Event:
                    {
                        LogManager.GetLogger(this.GetType()).Debug("ON Activity Execute behavior - RequestMsgType.Event ");
                        var requestMessageText = (RequestMessage as IRequestMessageEventBase).ConvertToRequestMessageText();
                        ResponseMessage = OnTextOrEventRequest(requestMessageText) ?? OnEventRequest(RequestMessage as IRequestMessageEventBase);
                    }
                    break;
                default:
                    throw new UnknownRequestMsgTypeException("未知的MsgType请求类型", null);
            }
        }

    }
}