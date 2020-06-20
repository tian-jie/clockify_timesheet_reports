using Infrastructure.Core.Logging;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.QY;
using Innocellence.Weixin.QY.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.ModelsView;
using Infrastructure.Utility.Data;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.CommonAPIs;
using Innocellence.Weixin;
using System.Web.Hosting;
using Infrastructure.Web.ImageTools;
using MP = Innocellence.Weixin.MP;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Common
{
    public class WeChatUserRequestMessageLogHandler : IWeChatUserRequestMessageLogHandler
    {
        //private 
        //private AttachmentsItemService _fileManageService = new AttachmentsItemService();
        //private AutoReplyContentService _autoReplyContentService = new AutoReplyContentService();
        //private IAddressBookService _addressBookService = EngineContext.Current.Resolve<IAddressBookService>();
        //private WechatMPUserService _wechatMPUserService = new WechatMPUserService();
        private static ILogger log = LogManager.GetLogger(typeof(WeChatUserRequestMessageLogHandler));

        public void WriteRequestLog(IRequestMessageBase requestMessage, string appId)
        {
            Task.Run(() =>
            {
                WechatUserRequestMessageLogService _wechatUserRequestMessageLogService = new WechatUserRequestMessageLogService();
                SmartPhoneMenuService _menuService = new SmartPhoneMenuService();
                AddressBookService _addressBookService = new AddressBookService();
                try
                {
                    if (requestMessage != null && !string.IsNullOrEmpty(appId))
                    {
                        int appID = int.Parse(appId);
                        log.Debug("Request type :{0}. AppID :{1}.", requestMessage.MsgType, appId);
                        WechatUserRequestMessageLogView view = new WechatUserRequestMessageLogView()
                        {
                            UserID = requestMessage.FromUserName,
                            CreatedTime = DateTime.Now,
                            AppID = appID,
                        };
                        switch (requestMessage.MsgType)
                        {
                            #region 组装View
                            case Weixin.RequestMsgType.Text:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Text;
                                string textContent = ((RequestMessageText)requestMessage).Content;
                                view.Content = textContent;
                                break;
                            case Weixin.RequestMsgType.Location:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Location;
                                object json = new
                                {
                                    Label = (requestMessage as RequestMessageLocation).Label,
                                    X = (requestMessage as RequestMessageLocation).Location_X,
                                    Y = (requestMessage as RequestMessageLocation).Location_Y,
                                    Scale = (requestMessage as RequestMessageLocation).Scale,
                                };
                                view.Content = JsonHelper.ToJson(json);
                                break;
                            case Weixin.RequestMsgType.Image:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Image;
                                var imageRequest = requestMessage as RequestMessageImage;
                                view.Content = SaveMediaFile(appID, imageRequest.MediaId, RequestMsgType.Image);
                                break;
                            case Weixin.RequestMsgType.Voice:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Voice;
                                var voiceRequest = requestMessage as RequestMessageVoice;
                                view.Content = SaveMediaFile(appID, voiceRequest.MediaId, RequestMsgType.Voice);
                                view.Duration = GetAMRDuraion(view.Content);
                                break;
                            case Weixin.RequestMsgType.ShortVideo:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Video;
                                var shortVideoRequest = requestMessage as RequestMessageShortVideo;
                                view.Content = SaveMediaFile(appID, shortVideoRequest.MediaId, RequestMsgType.Video);
                                log.Debug("ThumbMediaId :{0}", shortVideoRequest.ThumbMediaId);
                                break;
                            case Weixin.RequestMsgType.Video:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Video;
                                var videoRequest = requestMessage as RequestMessageVideo;
                                view.Content = SaveMediaFile(appID, videoRequest.MediaId, RequestMsgType.Video);
                                log.Debug("ThumbMediaId :{0}", videoRequest.ThumbMediaId);
                                break;
                            case Weixin.RequestMsgType.Link:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Link;
                                var linkRequest = requestMessage as RequestMessageLink;
                                object linkJson = new
                                {
                                    Title = linkRequest.Title,
                                    Description = linkRequest.Description,
                                    Url = linkRequest.Url
                                };
                                view.Content = "Link";
                                break;
                            case Weixin.RequestMsgType.Event:
                                try
                                {
                                    switch (((RequestMessageEventBase)requestMessage).Event)
                                    {
                                        #region Event
                                        case Event.ENTER:
                                        case Event.ENTER_AGENT:
                                            view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_Enter;
                                            view.Content = WechatUserMessageLogContentType.Request_Event_Enter.GetDescriptionByName();
                                            break;
                                        case Event.CLICK:
                                            view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_Click;
                                            view.Content = ((RequestMessageEvent_Click)requestMessage).EventKey;
                                            break;
                                        case Event.VIEW:
                                            view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_View;
                                            string viewUrl = ((RequestMessageEvent_View)requestMessage).EventKey;
                                            string viewContent = string.Empty;
                                            var menu = _menuService.Repository.Entities.FirstOrDefault(m => m.AppId == appID && viewUrl.Equals(m.CategoryDesc));
                                            if (null != menu)
                                            {
                                                viewContent = string.Format("[{0}] {1}", menu.CategoryName, viewUrl);
                                            }
                                            else
                                            {
                                                viewContent = string.Format("{0}", viewUrl);
                                            }
                                            view.Content = viewContent;
                                            break;
                                        case Event.SUBSCRIBE:
                                            view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_Subscribe;
                                            view.Content = WechatUserMessageLogContentType.Request_Event_Subscribe.GetDescriptionByName();
                                            break;
                                        case Event.UNSUBSCRIBE:
                                            view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_UnSubscribe;
                                            view.Content = WechatUserMessageLogContentType.Request_Event_UnSubscribe.GetDescriptionByName();
                                            break;
                                        case Event.SCAN:
                                        case Event.LOCATION:
                                        case Event.MASSSENDJOBFINISH:
                                        case Event.TEMPLATESENDJOBFINISH:
                                        case Event.SCANCODE_PUSH:
                                        case Event.SCANCODE_WAITMSG:
                                        case Event.PIC_SYSPHOTO:
                                        case Event.PIC_PHOTO_OR_ALBUM:
                                        case Event.PIC_WEIXIN:
                                        case Event.LOCATION_SELECT:
                                        case Event.BATCH_JOB_RESULT:
                                        case Event.create_chat:
                                        case Event.update_chat:
                                        case Event.quit_chat:
                                        default:
                                            view.ContentType = (int)WechatUserMessageLogContentType.Request_Event;
                                            //检查属性是否存在
                                            var members = requestMessage.GetType().GetMember("EventKey", MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance);
                                            if (members != null && members.Length > 0)
                                            {
                                                dynamic m = requestMessage;
                                                view.Content = m.EventKey;
                                            }
                                            else
                                            {
                                                view.Content = "Event";
                                            }
                                            break;
                                            #endregion
                                    }
                                }
                                catch (Exception e)
                                {
                                    log.Error(e);
                                    if (string.IsNullOrEmpty(view.Content))
                                    {
                                        view.ContentType = (int)WechatUserMessageLogContentType.Request_Event;
                                        view.Content = "Event";
                                    }
                                }
                                break;
                            case Weixin.RequestMsgType.Chat:
                            case Weixin.RequestMsgType.DEFAULT:
                            default:
                                view.Content = requestMessage.MsgType.ToString();
                                break;
                                #endregion
                        }
                        var user = _addressBookService.GetMemberByUserId(view.UserID);
                        view.UserName = user == null ? string.Empty : user.UserName;
                        _wechatUserRequestMessageLogService.Add(view);
                        log.Debug("Added audit log :{0} input {1} at {2}", view.UserID, view.Content, view.CreatedTime);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            });
        }

        public void WriteRequestLogMP(IRequestMessageBase requestMessage, string appId)
        {
            Task.Run(() =>
            {
                try
                {
                    WechatUserRequestMessageLogService _wechatUserRequestMessageLogService = new WechatUserRequestMessageLogService();
                    AutoReplyContentService _autoReplyContentService = new AutoReplyContentService();
                    SmartPhoneMenuService _menuService = new SmartPhoneMenuService();
                    WechatMPUserService _wechatMPUserService = new WechatMPUserService();

                    if (requestMessage != null && !string.IsNullOrEmpty(appId))
                    {
                        int appID = int.Parse(appId);
                        log.Debug("RequestLog Request type :{0}. AppID :{1}.", requestMessage.MsgType, appId);
                        WechatUserRequestMessageLogView view = new WechatUserRequestMessageLogView()
                        {
                            UserID = requestMessage.FromUserName,
                            CreatedTime = DateTime.Now,
                            AppID = appID,
                            HasReaded = false,
                        };
                        int ktype = (int)AutoReplyMPKeywordEnum.ALL;
                        string inputStr = string.Empty;
                        switch (requestMessage.MsgType)
                        {
                            #region 组装View
                            case Weixin.RequestMsgType.Text:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Text;
                                string textContent = ((RequestMessageText)requestMessage).Content;
                                view.Content = textContent;
                                ktype = (int)AutoReplyMPKeywordEnum.TEXT;
                                inputStr = textContent;
                                break;
                            case Weixin.RequestMsgType.Location:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Location;
                                object json = new
                                {
                                    Label = (requestMessage as RequestMessageLocation).Label,
                                    X = (requestMessage as RequestMessageLocation).Location_X,
                                    Y = (requestMessage as RequestMessageLocation).Location_Y,
                                    Scale = (requestMessage as RequestMessageLocation).Scale,
                                };
                                view.Content = JsonHelper.ToJson(json);
                                ktype = (int)AutoReplyMPKeywordEnum.LOCATION;
                                break;
                            case Weixin.RequestMsgType.Image:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Image;
                                var imageRequest = requestMessage as RequestMessageImage;
                                view.Content = SaveMediaFile(appID, imageRequest.MediaId, RequestMsgType.Image, true);
                                ktype = (int)AutoReplyMPKeywordEnum.IMAGE;
                                break;
                            case Weixin.RequestMsgType.Voice:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Voice;
                                var voiceRequest = requestMessage as RequestMessageVoice;
                                view.Content = SaveMediaFile(appID, voiceRequest.MediaId, RequestMsgType.Voice, true);
                                view.Duration = GetAMRDuraion(view.Content);
                                ktype = (int)AutoReplyMPKeywordEnum.AUDIO;
                                break;
                            case Weixin.RequestMsgType.ShortVideo:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Video;
                                var shortVideoRequest = requestMessage as RequestMessageShortVideo;
                                view.Content = SaveMediaFile(appID, shortVideoRequest.MediaId, RequestMsgType.Video, true);
                                log.Debug("ThumbMediaId :{0}", shortVideoRequest.ThumbMediaId);
                                ktype = (int)AutoReplyMPKeywordEnum.VIDEO;
                                break;
                            case Weixin.RequestMsgType.Video:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Video;
                                var videoRequest = requestMessage as RequestMessageVideo;
                                view.Content = SaveMediaFile(appID, videoRequest.MediaId, RequestMsgType.Video, true);
                                log.Debug("ThumbMediaId :{0}", videoRequest.ThumbMediaId);
                                ktype = (int)AutoReplyMPKeywordEnum.VIDEO;
                                break;
                            case Weixin.RequestMsgType.Link:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Link;
                                view.Content = "Link";
                                var linkRequest = requestMessage as RequestMessageLink;
                                object linkJson = new
                                {
                                    Title = linkRequest.Title,
                                    Description = linkRequest.Description,
                                    Url = linkRequest.Url
                                };
                                view.Content = JsonConvert.SerializeObject(linkJson);
                                ktype = (int)AutoReplyMPKeywordEnum.LINK;
                                break;
                            case Weixin.RequestMsgType.Event:
                                view.ContentType = (int)WechatUserMessageLogContentType.Request_Event;
                                view.HasReaded = true;
                                try
                                {
                                    if (((RequestMessageEventBase)requestMessage).Event == Event.VIEW)
                                    {
                                        view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_View;
                                        string viewUrl = ((RequestMessageEvent_View)requestMessage).EventKey;
                                        string viewContent = string.Empty;
                                        var menu = _menuService.Repository.Entities.FirstOrDefault(m => m.AppId == appID && viewUrl.Equals(m.CategoryDesc));
                                        if (null != menu)
                                        {
                                            viewContent = string.Format("[{0}] {1}", menu.CategoryName, viewUrl);
                                        }
                                        else
                                        {
                                            viewContent = string.Format("{0}", viewUrl);
                                        }
                                        view.Content = viewContent;
                                        ktype = (int)AutoReplyMPKeywordEnum.MENU;
                                    }
                                    else if (((RequestMessageEventBase)requestMessage).Event ==Event.CLICK)
                                    {
                                        view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_Click;
                                        view.Content = ((RequestMessageEvent_Click)requestMessage).EventKey;
                                        ktype = (int)AutoReplyMPKeywordEnum.MENU;
                                        inputStr = view.Content;
                                    }
                                    else if (((RequestMessageEventBase)requestMessage).Event == Event.SCAN)
                                    {
                                        view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_Scan;
                                        view.Content = ((RequestMessageEvent_Scan)requestMessage).EventKey;
                                        ktype = (int)AutoReplyMPKeywordEnum.SCAN;
                                        inputStr = view.Content;
                                    }
                                    else if (((RequestMessageEventBase)requestMessage).Event == Event.SUBSCRIBE)
                                    {
                                        var subscribeRequestMsg = ((RequestMessageEvent_Subscribe)requestMessage);
                                        view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_Subscribe;
                                        view.Content = subscribeRequestMsg.EventKey;
                                        ktype = (int)AutoReplyMPKeywordEnum.SubscribeEvent;
                                        int orCodeSceneId;
                                        string prefix = "qrscene_";
                                        if (!string.IsNullOrEmpty(subscribeRequestMsg.EventKey)
                                            && subscribeRequestMsg.EventKey.StartsWith(prefix)
                                            && int.TryParse(subscribeRequestMsg.EventKey.Substring(prefix.Length), out orCodeSceneId))
                                        {
                                            view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_Subscribe_With_Scan;
                                            ktype = (int)AutoReplyMPKeywordEnum.SubscribeWithScan;
                                            view.Content = orCodeSceneId.ToString();
                                            inputStr = orCodeSceneId.ToString();
                                        }
                                    }
                                    else if (((RequestMessageEventBase)requestMessage).Event == Event.UNSUBSCRIBE)
                                    {
                                        view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_UnSubscribe;
                                        view.Content = WechatUserMessageLogContentType.Request_Event_UnSubscribe.GetDescriptionByName();
                                    }
                                    else if (((RequestMessageEventBase)requestMessage).Event == Event.SCANCODE_PUSH)
                                    {
                                        view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_ScanCodePush;
                                        view.Content = ((RequestMessageEvent_Scancode_Push)requestMessage).EventKey;
                                        ktype = (int)AutoReplyMPKeywordEnum.MENU;
                                        inputStr = AutoReplyMenuEnum.SCAN_PUSH_EVENT.ToString();
                                    }
                                    else if (((RequestMessageEventBase)requestMessage).Event == Event.SCANCODE_WAITMSG)
                                    {
                                        view.ContentType = (int)WechatUserMessageLogContentType.Request_Event_ScanCodePush;
                                        view.Content = ((RequestMessageEvent_Scancode_Waitmsg)requestMessage).EventKey;
                                        ktype = (int)AutoReplyMPKeywordEnum.MENU;
                                        inputStr = AutoReplyMenuEnum.SCAN_WITH_PROMPT.ToString();
                                    }
                                    else
                                    {
                                        //检查属性是否存在
                                        var members = requestMessage.GetType().GetMember("EventKey", MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance);
                                        if (members != null && members.Length > 0)
                                        {
                                            dynamic m = requestMessage;
                                            view.Content = m.EventKey;
                                        }
                                        else
                                        {
                                            view.Content = "Event";
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    log.Error(e);
                                    if (string.IsNullOrEmpty(view.Content))
                                    {
                                        view.Content = "Event";
                                    }
                                }
                                break;
                            case Weixin.RequestMsgType.Chat:
                            case Weixin.RequestMsgType.DEFAULT:
                            default:
                                view.Content = requestMessage.MsgType.ToString();
                                break;
                                #endregion
                        }
                        if (view.ContentType != (int)WechatUserMessageLogContentType.Request_Event_View)
                        {
                            var contents = _autoReplyContentService.GetList<AutoReplyContentView>(appID, inputStr, ktype);
                            foreach (var content in contents)
                            {
                                if (content.MessageTags != null && content.MessageTags.Count > 0)
                                {
                                    view.TagId = JsonConvert.SerializeObject(content.MessageTags[0]);
                                }
                            }
                            if (view.ContentType == (int)WechatUserMessageLogContentType.Request_Event_Subscribe_With_Scan)
                            {
                                string qrCode = view.Content;
                                List<int> userTags = new List<int>();
                                if (contents != null && contents.Count > 0)
                                {
                                    userTags = contents[0].UserTags;
                                    log.Debug("user tags:{0}", userTags == null ? "null" : userTags.Count.ToString());
                                }
                                var qrObject = new { QrCode = qrCode, UserTags = userTags };
                                view.Content = JsonConvert.SerializeObject(qrObject);
                            }
                            var allPicks = _autoReplyContentService.GetList<AutoReplyContentView>(appID, string.Empty, (int)AutoReplyMPKeywordEnum.ALL).Select(c => c.AutoReplyId).ToList();
                            view.IsAutoReply = contents != null && contents.Count > 0 && contents.Any(c => !allPicks.Contains(c.AutoReplyId));
                            if (view.IsAutoReply.HasValue && view.IsAutoReply.Value)
                            {
                                view.HasReaded = true;
                            }
                        }
                        var user = _wechatMPUserService.GetUserByOpenId(view.UserID);  //此处建议使用缓存
                        view.UserName = user == null ? string.Empty : user.NickName;
                        //fix bug #804
                        List<int> hasReadedDisplayMsgTypes = new List<int>()
                        {
                            (int)WechatUserMessageLogContentType.Request_Text,
                            (int)WechatUserMessageLogContentType.Request_Voice,
                            (int)WechatUserMessageLogContentType.Request_Video,
                            (int)WechatUserMessageLogContentType.Request_Image,
                            (int)WechatUserMessageLogContentType.Request_Link,
                            (int)WechatUserMessageLogContentType.Response_Empty
                        };
                        if (!hasReadedDisplayMsgTypes.Contains(view.ContentType))
                        {
                            view.HasReaded = true;
                        }
                        _wechatUserRequestMessageLogService.Add(view);
                        log.Debug("RequestLog Added audit log :{0} UserName:{1} input: {2} at {3}", view.UserID, view.UserName, view.Content, view.CreatedTime);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            });
        }

        private long GetAMRDuraion(string filePath)
        {
            long duration = 0;
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    string fileName = MapPath(filePath);
                    if (File.Exists(fileName))
                    {
                        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            byte[] packed_size = new byte[16] { 12, 13, 15, 17, 19, 20, 26, 31, 5, 0, 0, 0, 0, 0, 0, 0 };
                            int pos = 0;
                            pos += 6;
                            long lenth = fs.Length;
                            byte[] toc = new byte[1];
                            int framecount = 0;
                            byte ft;
                            while (pos < lenth)
                            {
                                fs.Seek(pos, SeekOrigin.Begin);
                                if (1 != fs.Read(toc, 0, 1))
                                {
                                    duration = lenth > 0 ? ((lenth - 6) / 650) : 0;
                                    fs.Close();
                                    break;
                                }
                                ft = (byte)((toc[0] / 8) & 0x0F);
                                pos += packed_size[ft] + 1;
                                framecount++;
                            }
                            duration = framecount * 20 / 1000;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return duration;
        }

        #region Method For Write Request Log
        private string SaveMediaFile(int iWeChatID, string mediaId, RequestMsgType mediaType, bool isMP = false)
        {
            log.Debug("Begin Save Media File from Wechat Client :{0}", iWeChatID);
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    var config = isMP ? WeChatCommonService.GetWeChatConfigByID(iWeChatID) : null;
                    log.Debug("MediaApi.Get :{0}", mediaId);
                    var fileInfo = isMP ? MP.AdvancedAPIs.MediaApi.Get(config.WeixinCorpId, config.WeixinCorpSecret, mediaId, stream)
                                        : MediaApi.Get(GetToken(iWeChatID), mediaId, stream);
                    if (fileInfo != null)
                    {
                        string contentDispostion = fileInfo.Get("Content-disposition");
                        log.Debug("Content-disposition:{0}", contentDispostion);
                        string extention = string.Empty;
                        //filename="MEDIA_ID.jpg"
                        if (!string.IsNullOrEmpty(contentDispostion))
                        {
                            var temp = contentDispostion.Split(';').ToList();
                            foreach (var item in temp)
                            {
                                if (item.Trim().StartsWith("filename"))
                                {
                                    string fileName = item.Trim().Split('=')[1].Trim('\"');
                                    extention = fileName.Substring(fileName.LastIndexOf('.'));
                                }
                            }
                        }
                        log.Debug("extention:{0}", extention);
                        stream.Seek(0, SeekOrigin.Begin);

                        string filePath = string.Empty;
                        string thumbFilePath = string.Empty;
                        filePath = GetFilePath(extention, out thumbFilePath);
                        log.Debug("filePath:{0}", filePath);
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            WriteToFile(filePath, stream, mediaType);
                            log.Debug("thumbFilePath:{0}", thumbFilePath);
                            log.Debug("End Save Media File from Wechat Client");
                            return thumbFilePath;
                        }
                    }
                    else
                    {
                        log.Error("WebHeaderCollection is null.");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            log.Error("Save Media File Failed.");
            return string.Empty;
        }

        /// <summary>
        /// 文件夹使用DateTime.Now.Year, Month, Day
        /// 文件名使用DateTime.Now.Ticks
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetFilePath(RequestMsgType type, out string thumbFilePath)
        {
            DateTime now = DateTime.Now;
            string folderPath = InitDirectory(now, out thumbFilePath);
            if (!string.IsNullOrEmpty(folderPath))
            {
                string extention = string.Empty;
                switch (type)
                {
                    case RequestMsgType.Image:
                        extention = ".jpg";
                        break;
                    case RequestMsgType.Voice:
                        extention = ".mp3";
                        break;
                    case RequestMsgType.Video:
                        extention = ".mp4";
                        break;
                    default:
                        return string.Empty;
                }
                string fileName = now.Ticks.ToString() + extention;
                string filePath = Path.Combine(folderPath, fileName);
                thumbFilePath = string.Format("{0}/{1}", thumbFilePath, fileName);
                log.Debug("filePath :{0}", filePath);
                log.Debug("thumbFilePath :{0}.", thumbFilePath);
                return filePath;
            }
            return string.Empty;
        }


        public static string GetFilePath(string extention, out string thumbFilePath)
        {
            DateTime now = DateTime.Now;
            string folderPath = InitDirectory(now, out thumbFilePath);
            if (!string.IsNullOrEmpty(extention) && !string.IsNullOrEmpty(folderPath))
            {
                string fileName = now.Ticks.ToString() + extention;
                string filePath = Path.Combine(folderPath, fileName);
                thumbFilePath = string.Format("{0}/{1}", thumbFilePath, fileName);
                log.Debug("filePath :{0}", filePath);
                log.Debug("thumbFilePath :{0}.", thumbFilePath);
                return filePath;
            }
            return string.Empty;
        }
        /// <summary>
        /// \Content\WeChatUserUploaded\Year\Month\Day\Ticks.extention
        /// </summary>
        /// <param name="now"></param>
        /// <param name="isSecond"></param>
        /// <returns></returns>
        private static string InitDirectory(DateTime now, out string thumbFilePath, bool isSecond = false)
        {
            string dicPath = string.Empty;
            bool flag = true;
            List<string> fix = new List<string>()
            {
                "/Content",
                "WechatUserUploaded",
                now.Year.ToString(),
                now.Month.ToString(),
                now.Day.ToString()
            };
            for (int i = 0; i < fix.Count; i++)
            {
                if (!flag) break;
                if (i == 0)
                {
                    dicPath = MapPath(fix[i]);
                }
                else
                {
                    dicPath = Path.Combine(dicPath, fix[i]);
                }
                flag = CreateFolder(dicPath);
            }
            if (!flag && !isSecond)
            {
                log.Warn("try again");
                InitDirectory(now, out thumbFilePath, true);
            }
            thumbFilePath = flag ? string.Join("/", fix.ToArray()) : string.Empty;
            return flag ? dicPath : string.Empty;
        }

        private static bool CreateFolder(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                log.Debug("create {0} success.", path);
                return true;
            }
            catch (Exception e)
            {
                log.Error(e);
                return false;
            }
        }

        private void WriteToFile(string filePath, MemoryStream ms, RequestMsgType type)
        {
            if (type == RequestMsgType.Image)
            {
                log.Debug("ImageUtility.MakeThumbnail");
                ImageUtility.MakeThumbnail(null, ms, filePath, 900, 0, "W", 1, true);
            }
            else
            {
                using (Stream localFile = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    localFile.Write(ms.ToArray(), 0, (int)ms.Length);
                }
            }
        }

        private static string GetToken(int iWeChatID)
        {
            var config = WeChatCommonService.GetWeChatConfigByID(iWeChatID);
            return AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
        }

        private static string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }

            //not hosted. For example, run in unit tests
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }
        #endregion

        public void WriteResponseLog(List<IResponseMessageBase> responseList, string appId)
        {
            Task.Run(() =>
            {
                try
                {
                    WechatUserRequestMessageLogService _wechatUserRequestMessageLogService = new WechatUserRequestMessageLogService();
                    //  AddressBookService _addressBookService = new AddressBookService();

                    if (responseList != null && responseList.Count > 0 && !string.IsNullOrEmpty(appId))
                    {
                        log.Debug("begin to wirte response log");
                        int appID = int.Parse(appId);
                        //目前只支持单类型回复.
                        foreach (var response in responseList)
                        {
                            log.Debug("response type :{0}. AppID :{1}.", ((int)response.MsgType).ToString(), appId);
                            WechatUserRequestMessageLogView view = new WechatUserRequestMessageLogView()
                            {
                                UserID = response.ToUserName,
                                CreatedTime = DateTime.Now,
                                AppID = appID,
                                IsAutoReply = true,
                            };
                            switch (response.MsgType)
                            {
                                #region 组装View
                                case Weixin.ResponseMsgType.Text:
                                    var textResponse = response as ResponseMessageText;
                                    view.ContentType = (int)WechatUserMessageLogContentType.Response_Text;
                                    view.Content = textResponse.Content;
                                    if (string.IsNullOrWhiteSpace(view.Content))
                                    {
                                        return;
                                    }
                                    break;
                                case Weixin.ResponseMsgType.News:
                                    var newsResponse = response as ResponseMessageNews;
                                    view.ContentType = (int)WechatUserMessageLogContentType.Response_News;
                                    view.Content = JsonHelper.ToJson(newsResponse.Articles);
                                    break;
                                case Weixin.ResponseMsgType.Image:
                                    var imgResponse = response as ResponseMessageImage;
                                    view.ContentType = (int)WechatUserMessageLogContentType.Response_Image;
                                    view.Content = GetFileUrlByMeidaId(imgResponse.Image.MediaId, ResponseMsgType.Image);
                                    break;
                                case Weixin.ResponseMsgType.Voice:
                                    var voiceResponse = response as ResponseMessageVoice;
                                    view.ContentType = (int)WechatUserMessageLogContentType.Response_Voice;
                                    view.Content = GetFileUrlByMeidaId(voiceResponse.Voice.MediaId, ResponseMsgType.Voice);
                                    break;
                                case Weixin.ResponseMsgType.Video:
                                    var videoResponse = response as ResponseMessageVideo;
                                    view.ContentType = (int)WechatUserMessageLogContentType.Response_Video;
                                    view.Content = GetFileUrlByMeidaId(videoResponse.Video.MediaId, ResponseMsgType.Video);
                                    break;
                                case Weixin.ResponseMsgType.NoResponse:
                                    return;
                                case Weixin.ResponseMsgType.Music:
                                case Weixin.ResponseMsgType.MpNews:
                                case Weixin.ResponseMsgType.Transfer_Customer_Service:
                                case Weixin.ResponseMsgType.MultipleNews:
                                case Weixin.ResponseMsgType.LocationMessage:
                                default:
                                    view.Content = response.MsgType.ToString();
                                    break;
                                    #endregion
                            }
                            _wechatUserRequestMessageLogService.Add(view);
                            log.Debug("Added audit log :{0} input {1} at {2}", view.UserID, view.Content, view.CreatedTime);
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            });
        }

        /// <summary>
        /// 口令有两种形式,1.从素材表中选择素材, 2.直接上传新文件
        /// 对于1, 需要根据MediaId 去素材表中查询Url
        /// 对于2, 需要根据MediaId 去AutoReplyContent表中查询Url
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        private string GetFileUrlByMeidaId(string mediaId, ResponseMsgType type)
        {
            AutoReplyContentService _autoReplyContentService = new AutoReplyContentService();
            AttachmentsItemService _fileManageService = new AttachmentsItemService();


            log.Debug("media id : {0}", mediaId);
            var fileInfo = _fileManageService.GetByMediaId<AttachmentsItemView>(mediaId);
            if (null != fileInfo)
            {
                string filePath = string.Format("/{0}", fileInfo.AttachmentUrl.Trim('/'));
                log.Debug("filePath :{0}", filePath);
                return filePath;
            }
            else
            {
                Expression<Func<AutoReplyContent, bool>> predicate = c => mediaId.Equals(c.MediaId);
                var q = _autoReplyContentService.GetList<AutoReplyContentView>(predicate).FirstOrDefault();
                log.Debug("content is {0} null", q == null ? "" : "not");
                if (null != q)
                {
                    var lst = JsonConvert.DeserializeObject<List<NewsInfoView>>(q.Content);
                    if (null != lst && lst.Count > 0)
                    {
                        string filePath = string.Empty;
                        switch (type)
                        {
                            case ResponseMsgType.Image:
                                filePath = lst[0].ImageContent;
                                break;
                            case ResponseMsgType.Voice:
                                filePath = lst[0].SoundSrc;
                                break;
                            case ResponseMsgType.Video:
                                filePath = lst[0].VideoContent;
                                break;
                            default:
                                log.Debug("type :{0}", type.ToString());
                                break;
                        }
                        filePath = string.Format("/{0}", filePath.Trim('/'));
                        log.Debug("filePath :{0}", filePath);
                        return filePath;
                    }
                }
            }
            log.Debug("something is wrong.");
            return string.Empty;
        }

        public Task<WechatUserRequestMessageLogView> WriteResponseLogMP(List<IResponseMessageBase> responseList, string appId, bool isAutoReply = true)
        {
            var task = Task<WechatUserRequestMessageLogView>.Run(() =>
            {
                WechatUserRequestMessageLogView view = null;
                try
                {
                    WechatUserRequestMessageLogService _wechatUserRequestMessageLogService = new WechatUserRequestMessageLogService();
                    if (responseList != null && responseList.Count > 0 && !string.IsNullOrEmpty(appId))
                    {
                        log.Debug("begin to wirte response log");
                        int appID = int.Parse(appId);
                        var config = WeChatCommonService.GetWeChatConfigByID(appID);
                        //目前只支持单类型回复.
                        foreach (var response in responseList)
                        {
                            log.Debug("response type :{0}. AppID :{1}.", ((int)response.MsgType).ToString(), appId);
                            view = new WechatUserRequestMessageLogView()
                            {
                                UserID = response.ToUserName,
                                CreatedTime = DateTime.Now,
                                AppID = appID,
                                IsAutoReply = isAutoReply
                            };
                            switch (response.MsgType)
                            {
                                #region 组装View
                                case Weixin.ResponseMsgType.Text:
                                    var textResponse = response as  ResponseMessageText;
                                    view.ContentType = (int)WechatUserMessageLogContentType.Response_Text;
                                    view.Content = textResponse.Content;
                                    if (string.IsNullOrWhiteSpace(view.Content))
                                    {
                                        return null;
                                    }
                                    break;
                                case Weixin.ResponseMsgType.News:
                                    var newsResponse = response as  ResponseMessageNews;
                                    view.ContentType = (int)WechatUserMessageLogContentType.Response_News;
                                    view.Content = JsonHelper.ToJson(newsResponse.Articles);
                                    break;
                                case Weixin.ResponseMsgType.Image:
                                    var imgResponse = response as  ResponseMessageImage;
                                    view.ContentType = (int)WechatUserMessageLogContentType.Response_Image;
                                    view.Content = GetFileUrlByMeidaId(imgResponse.Image.MediaId, ResponseMsgType.Image);
                                    break;
                                case Weixin.ResponseMsgType.Voice:
                                    var voiceResponse = response as  ResponseMessageVoice;
                                    view.ContentType = (int)WechatUserMessageLogContentType.Response_Voice;
                                    view.Content = GetFileUrlByMeidaId(voiceResponse.Voice.MediaId, ResponseMsgType.Voice);
                                    break;
                                case Weixin.ResponseMsgType.Video:
                                    var videoResponse = response as  ResponseMessageVideo;
                                    view.ContentType = (int)WechatUserMessageLogContentType.Response_Video;
                                    view.Content = GetFileUrlByMeidaId(videoResponse.Video.MediaId, ResponseMsgType.Video);
                                    break;
                                case Weixin.ResponseMsgType.NoResponse:
                                    return null;
                                case Weixin.ResponseMsgType.Music:
                                case Weixin.ResponseMsgType.MpNews:
                                case Weixin.ResponseMsgType.Transfer_Customer_Service:
                                case Weixin.ResponseMsgType.MultipleNews:
                                case Weixin.ResponseMsgType.LocationMessage:
                                default:
                                    view.Content = response.MsgType.ToString();
                                    break;
                                    #endregion
                            }
                            _wechatUserRequestMessageLogService.Add(view);
                            view.AppLogo = config.CoverUrl;
                            log.Debug("response Added audit log :{0} input {1} at {2}", view.UserID, view.Content, view.CreatedTime);

                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
                return view;
            });
            return task;
        }

    }
}
