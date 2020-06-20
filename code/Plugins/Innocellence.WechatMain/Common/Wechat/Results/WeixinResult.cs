using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.MessageHandlers;
using System.Xml.Linq;
using Innocellence.Weixin.QY.Entities;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.WeChat.Domain.Common;
using Infrastructure.Core.Logging;

namespace Innocellence.Weixin.MP.MvcExtension
{

    //public static class WeixinResultExtension
    //{
    //    public WeixinResult WeixinResult()
    //    { 

    //    }
    //}

    /// <summary>
    /// 返回MessageHandler结果
    /// </summary>
    public class WeixinResult : ContentResult
    {
        //private string _content;
        protected IMessageHandlerDocument _messageHandlerDocument;

        private ILogger log { get { return LogManager.GetLogger("WeChat"); } }

        public WeixinResult(string content)
        {
            //_content = content;
            base.Content = content;
        }

        public WeixinResult(IMessageHandlerDocument messageHandlerDocument)
        {
            _messageHandlerDocument = messageHandlerDocument;
        }

        /// <summary>
        /// 获取ContentResult中的Content或IMessageHandler中的ResponseDocument文本结果。
        /// 一般在测试的时候使用。
        /// </summary>
        new public string Content
        {
            get
            {
                if (base.Content != null)
                {
                    return base.Content;
                }
                else if (_messageHandlerDocument != null && _messageHandlerDocument.ResponseMessage != null)
                {
                    string strXML = "";
                    foreach (var a in _messageHandlerDocument.ResponseMessage)
                    {
                        strXML += _messageHandlerDocument.FinalResponseDocument(a as ResponseMessageBaseWechat).ToString(SaveOptions.OmitDuplicateNamespaces);
                    }
                    return strXML.Replace("\r\n", "\n");
                }
                else
                {
                    return null;
                }
            }
            set { base.Content = value; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (base.Content == null)
            {
                //使用IMessageHandler输出
                if (_messageHandlerDocument == null)
                {
                    throw new Innocellence.Weixin.Exceptions.WeixinException("执行WeixinResult时提供的MessageHandler不能为Null！", null);
                }

                if (_messageHandlerDocument.ResponseMessage == null)
                {
                    //throw new Innocellence.Weixin.MP.WeixinException("ResponseMessage不能为Null！", null);
                }
                else
                {
                    context.HttpContext.Response.ClearContent();
                    context.HttpContext.Response.ContentType = "text/xml";
                    foreach (var a in _messageHandlerDocument.ResponseMessage)
                    {
                       var config= WeChatCommonService.lstSysWeChatConfig.Find(aa => aa.WeixinCorpId == a.FromUserName);
                       var strToken = WeChatCommonService.GetWeiXinToken(config.Id);

                        if ((a as ResponseMessageBaseWechat).isSafe)
                        {
                            if (a is ResponseMessageImage)
                            {
                                 var b=(ResponseMessageImage)a;
                                 MassApi.SendImage(strToken, b.ToUserName, "", "", config.WeixinAppId.ToString(), b.Image.MediaId, 1);
                            }
                            else if (a is ResponseMessageVideo)
                            {
                                var b = (ResponseMessageVideo)a;
                                MassApi.SendVideo(strToken, b.ToUserName, "", "", config.WeixinAppId.ToString(), b.Video.MediaId, b.Video.Title, b.Video.Description, 1);
                            }
                            else if (a is ResponseMessageVoice)
                            {
                                var b = (ResponseMessageVoice)a;
                                MassApi.SendVoice(strToken, b.ToUserName, "", "", config.WeixinAppId.ToString(), b.Voice.MediaId, 1);
                            }
                            else if (a is ResponseMessageText)
                            {
                                var b = (ResponseMessageText)a;
                                MassApi.SendText(strToken, b.ToUserName, "", "", config.WeixinAppId.ToString(), b.Content, 1);
                            }
                            else if (a is ResponseMessageMpNews)
                            {
                                var b = (ResponseMessageMpNews)a;
                                MassApi.SendMpNews(strToken, b.ToUserName, "", "", config.WeixinAppId.ToString(), b.MpNewsArticles, 1);
                            }
                            else if (a is ResponseMessageNews)
                            {
                                log.Error("ExecuteResult ResponseMessageNews but Type is safe message!");
                                _messageHandlerDocument.FinalResponseDocument(a as ResponseMessageBaseWechat).Save(context.HttpContext.Response.OutputStream);
                            }
                        }
                        else
                        {
                            _messageHandlerDocument.FinalResponseDocument(a as ResponseMessageBaseWechat).Save(context.HttpContext.Response.OutputStream);
                        }
                        
                    }

                }
            }

            base.ExecuteResult(context);
        }
    }
}
