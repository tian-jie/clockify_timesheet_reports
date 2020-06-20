using Infrastructure.Core.Data;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Logging;
using Infrastructure.Web.ImageTools;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChatTalk.Domain.Entity;
using Innocellence.WeChatTalk.Domain.Service;
using Innocellence.Weixin;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Innocellence.WeChatTalk.SignalR
{
    [HubName("chat")]
    public class ChatHub : Hub
    {
        protected IMultiTalkService _mutiTalkService = EngineContext.Current.Resolve<IMultiTalkService>();
        protected IWechatMPUserService _wechatMPUserService = EngineContext.Current.Resolve<IWechatMPUserService>();
        protected IAddressBookService _wechatUserService = EngineContext.Current.Resolve<IAddressBookService>();
        private ILogger log = LogManager.GetLogger(typeof(ChatHub));
        public void Group(string groupId)
        {
            //var lit = _mutiTalkService.GetRecentTalkRecords(Convert.ToInt32(groupId));
            Groups.Add(Context.ConnectionId, groupId);
        }

        /// <summary>
        /// 供客户端调用的服务器端代码
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void Send(string clientId, string message, string groupId)
        {
#if DEBUG
            var headerimg = "/Plugins/Innocellence.WechatTalk/Content/img/no_image.jpg";
            var name = "test";
#else
            var headerimg = getUserHeadImg(clientId, System.Convert.ToInt32(groupId));
            var name = getUserName(clientId, System.Convert.ToInt32(groupId));
#endif

            // 调用所有客户端的sendMessage方法
            Clients.Group(groupId).sendMessage(clientId, name, message, headerimg);
            MultiTalk talkRecord = new MultiTalk();
            talkRecord.OpenId = clientId;
            talkRecord.AppId = System.Convert.ToInt32(groupId);
            talkRecord.CreatedDate = DateTime.Now.Date;
            talkRecord.TextContent = message;
            talkRecord.MsgType = "text";
            _mutiTalkService.Repository.Insert(talkRecord);

        }

        //发送图片
        public void SendImage(string clientId, string imagesId, string groupId)
        {
            int appid = System.Convert.ToInt32(groupId);
            var name = getUserName(clientId, appid);
            var headerimg = getUserHeadImg(clientId, appid);
            var imagesUrl = SaveMediaFile(System.Convert.ToInt32(groupId), imagesId, RequestMsgType.Image, isMPWechat(appid));
            Clients.Group(groupId).receiveImage(clientId, name, imagesUrl, headerimg); // 调用客户端receiveImage方法将图片进行显示
            MultiTalk talkRecord = new MultiTalk();
            talkRecord.OpenId = clientId;
            talkRecord.AppId = System.Convert.ToInt32(groupId);
            talkRecord.CreatedDate = DateTime.Now.Date;
            talkRecord.TextContent = imagesUrl;
            talkRecord.MsgType = "image";
            _mutiTalkService.Repository.Insert(talkRecord);

        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="wechatid"></param>
        /// <returns></returns>
        private string getUserHeadImg(string userid, int wechatid)
        {
            //服务号
            if (isMPWechat(wechatid))
            {
                var currentUser = _wechatMPUserService.GetUserByOpenId(userid);

                if (string.IsNullOrEmpty(currentUser.HeadImgUrl))
                    return "/Plugins/Innocellence.WechatTalk/Content/img/no_image.jpg";
                else
                    return currentUser.HeadImgUrl;
            }
            else //企业号
            {
                var currentUser = _wechatUserService.GetMemberByUserId(userid);

                if (string.IsNullOrEmpty(currentUser.Avatar))
                    return "/Plugins/Innocellence.WechatTalk/Content/img/no_image.jpg";
                else
                    return currentUser.Avatar;
            }

        }

        /// <summary>
        /// 获取用户昵称
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="wechatid"></param>
        /// <returns></returns>
        private string getUserName(string userid, int wechatid)
        {
            //服务号
            if (isMPWechat(wechatid))
            {
                var currentUser = _wechatMPUserService.GetUserByOpenId(userid);
                return currentUser.NickName;
            }
            else //企业号
            {
                var currentUser = _wechatUserService.GetMemberByUserId(userid);
                return currentUser.UserName;
            }

        }

        private bool isMPWechat(int wechatid)
        {
            var weChatConfig = WeChatCommonService.GetWeChatConfigByID(wechatid);
            if (weChatConfig.IsCorp.HasValue && !weChatConfig.IsCorp.Value)
            {
                return true;
            }
            else //企业号
            {
                return false;
            }


        }
        /// <summary>
        /// 客户端连接的时候调用
        /// </summary>
        /// <returns></returns>
        /// 

        private string SaveMediaFile(int iWeChatID, string mediaId, RequestMsgType mediaType, bool isMP = false)
        {
            log.Debug("Begin Save Media File from Wechat Client :{0}", iWeChatID);
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    var config = isMP ? WeChatCommonService.GetWeChatConfigByID(iWeChatID) : null;
                    log.Debug("MediaApi.Get :{0}", mediaId);
                    var fileInfo = isMP ? Weixin.MP.AdvancedAPIs.MediaApi.Get(config.WeixinCorpId, config.WeixinCorpSecret, mediaId, stream)
                                        : MediaApi.Get(WeChatCommonService.GetWeiXinToken(iWeChatID), mediaId, stream);
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


        private string GetFilePath(string extention, out string thumbFilePath)
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
        ///Content\WeChatUserUploaded\Year\Month\Day\Ticks.extention
        /// </summary>
        /// <param name="now"></param>
        /// <param name="isSecond"></param>
        /// <returns></returns>
        private string InitDirectory(DateTime now, out string thumbFilePath, bool isSecond = false)
        {
            string dicPath = string.Empty;
            bool flag = true;
            List<string> fix = new List<string>()
            {
                "/Content",
                "WechatTalkImageHistory",
                now.Year.ToString(),
                now.Month.ToString()
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

        private bool CreateFolder(string path)
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

        public virtual string MapPath(string path)
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

        public override Task OnConnected()
        {
            Trace.WriteLine("客户端连接成功");
            return base.OnConnected();
        }
    }

}