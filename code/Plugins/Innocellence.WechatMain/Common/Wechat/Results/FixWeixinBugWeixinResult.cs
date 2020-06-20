using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.MessageHandlers;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.Entities;
using Innocellence.WeChat.Domain.Common;
using Infrastructure.Core.Logging;

namespace Innocellence.Weixin.MvcExtension
{
    public class FixWeixinBugWeixinResult : ContentResult
    {
        //private string _content;
        protected IMessageHandlerDocument _messageHandlerDocument;
        private ILogger log { get { return LogManager.GetLogger("WeChat"); } }
        /// <summary>
        /// 这个类型只用于特殊阶段：目前IOS版本微信有换行的bug，\r\n会识别为2行
        /// </summary>
        public FixWeixinBugWeixinResult(IMessageHandlerDocument messageHandlerDocument)
        {
            _messageHandlerDocument = messageHandlerDocument;
        }

        public FixWeixinBugWeixinResult(string content)
        {
            //_content = content;
            base.Content = content;
        }


        new public string Content
        {
            get
            {
                if (base.Content != null)
                {
                    return base.Content;
                }
                else if (_messageHandlerDocument.TextResponseMessage != null)
                {
                    return _messageHandlerDocument.TextResponseMessage;
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
                    //throw new Innocellence.Weixin.MP.WeixinException("FinalResponseDocument不能为Null！", null);
                }
                else
                {

                    

                    context.HttpContext.Response.ClearContent();
                    context.HttpContext.Response.ContentType = "text/xml";

                    // var xml = _messageHandlerDocument.FinalResponseDocument.ToString().Replace("\r\n", "\n"); //腾
                    foreach (var a in _messageHandlerDocument.ResponseMessage)
                    {
                       

                      

                       if ((a as ResponseMessageBaseWechat).isSafe)
                       {
                           var appid = (a as ResponseMessageBaseWechat).APPID;

                           log.Debug("Before Send Response to wechat:{0} type:{1}", appid, a.MsgType);
                           var config = WeChatCommonService.GetWeChatConfigByID(appid);

                           var strToken = WeChatCommonService.GetWeiXinToken(config.Id);

                           if (a is ResponseMessageImage)
                           {
                               var b = (ResponseMessageImage)a;
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
                           string xml = _messageHandlerDocument.FinalResponseDocument(a as ResponseMessageBaseWechat).ToString().Replace("\r\n", "\n");


                           log.Debug("Start Send Response Message: length:{0}", xml.Length);

                           if (!string.IsNullOrEmpty(xml))
                           {
                               var bytes = Encoding.UTF8.GetBytes(xml);//的
                               context.HttpContext.Response.OutputStream.Write(bytes, 0, bytes.Length);//很
                               context.HttpContext.Response.Flush();
                           }
                           log.Debug("End Response Message: length:{0}", xml.Length);
                       }


                    }
                 
                }
            }
        }
    }
}
