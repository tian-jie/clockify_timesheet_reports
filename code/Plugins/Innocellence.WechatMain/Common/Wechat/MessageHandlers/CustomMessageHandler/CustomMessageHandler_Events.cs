/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：CustomMessageHandler_Events.cs
    文件功能描述：自定义MessageHandler
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Innocellence.Weixin.MP;
using Innocellence.Weixin.MP.Agent;
using Innocellence.Weixin.Context;
using Innocellence.Weixin.MP.Entities;
using Innocellence.Weixin.MP.Helpers;
using Innocellence.Weixin.MP.MessageHandlers;
using Innocellence.Weixin.CommonService.Utilities;
using Innocellence.Weixin.Entities;
using System.Collections.Generic;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.Weixin.MP.AdvancedAPIs.User;
using System.Text;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.ModelsView;
using Infrastructure.Core.Logging;
using Newtonsoft.Json;
using Innocellence.Weixin.CommonService.MessageHandlers;
using Infrastructure.Core.Infrastructure;

namespace Innocellence.Weixin.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
        private ISysWechatConfigService sysWechatConfigService = EngineContext.Current.Resolve<ISysWechatConfigService>();
        private IFocusHistoryService focusHistoryService = EngineContext.Current.Resolve<IFocusHistoryService>();
        private IAutoReplyContentService autoReplyContentService = EngineContext.Current.Resolve<IAutoReplyContentService>();
        private ISystemUserTagMappingService systemUserTagMappingService = EngineContext.Current.Resolve<ISystemUserTagMappingService>();
        private IWechatMPUserService wechatMPUserService = EngineContext.Current.Resolve<IWechatMPUserService>();


        private static object objLockUser = new object();

        public override List<IResponseMessageBase> OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            // 预处理文字或事件类型请求。
            // 这个请求是一个比较特殊的请求，通常用于统一处理来自文字或菜单按钮的同一个执行逻辑，
            // 会在执行OnTextRequest或OnEventRequest之前触发，具有以下一些特征：
            // 1、如果返回null，则继续执行OnTextRequest或OnEventRequest
            // 2、如果返回不为null，则终止执行OnTextRequest或OnEventRequest，返回最终ResponseMessage
            // 3、如果是事件，则会将RequestMessageEvent自动转为RequestMessageText类型，其中RequestMessageText.Content就是RequestMessageEvent.EventKey


            return null;//返回null，则继续执行OnTextRequest或OnEventRequest
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {

            return CreateResponseMessage(requestMessage.EventKey, (int)AutoReplyMPKeywordEnum.MENU);
        }

        /// <summary>
        /// 进入微信号
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        {
            //var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            //responseMessage.Content = "您刚才发送了ENTER事件请求。";
            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.ALL);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            //这里是微信客户端（通过微信服务器）自动发送过来的位置信息
            // var responseMessage = CreateResponseMessage<ResponseMessageText>();
            //  responseMessage.Content = "这里写什么都无所谓，比如：上帝爱你！";
            //  return new List<IResponseMessageBase>(){responseMessage};//这里也可以返回null（需要注意写日志时候null的问题）
            //log.Debug("Latitude: {0}, Longitude :{1}, Precision:{2}", requestMessage.Latitude, requestMessage.Longitude, requestMessage.Precision);
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.LOCATION);
        }

        /// <summary>
        /// 菜单扫描二维码
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            //通过扫描关注
            //var responseMessage = CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "通过扫描关注。";
            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(requestMessage.EventKey, (int)AutoReplyMPKeywordEnum.SCAN);
        }

        /// <summary>
        /// 菜单点击（URL跳转）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        {
            //说明：这条消息只作为接收，下面的responseMessage到达不了客户端，类似OnEvent_UnsubscribeRequest
            //var responseMessage = CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "您点击了view按钮，将打开网页：" + requestMessage.EventKey;
            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.ALL);
        }

        /// <summary>
        /// 接收到了群发完成的信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_MassSendJobFinishRequest(RequestMessageEvent_MassSendJobFinish requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "接收到了群发完成的信息。";
            return new List<IResponseMessageBase>() { responseMessage };
            // return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.ALL);
        }

        /// <summary>
        /// 订阅（关注）事件。关注时需要把用户信息导入到数据库中
        /// </summary>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            //var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            //responseMessage.Content = GetWelcomeInfo();
            //Add user to local database
            if (!string.IsNullOrEmpty(WeixinOpenId))
            {
                // SycUserFromWeixin(WeixinOpenId);

                //同步用户
                var weChatConfig = GetWechatConfig();

                lock (objLockUser) //不lock的话，会插入多次
                {
                    WechatMPUser objUser = wechatMPUserService.Repository.Entities.FirstOrDefault(u => u.OpenId.Equals(WeixinOpenId));
                    if (null != objUser)
                    {
                        log.Debug("user :{0}, {1}", objUser.Id, objUser.IsCanceled);
                    }
                    WeChatCommonService.SycUserFromWeixinMP(WeixinOpenId, ref objUser, weChatConfig);
                }

                log.Debug("EventKey:{0}  Event:{1}", requestMessage.EventKey, requestMessage.Event);

                #region 通过扫码关注
                int orCodeSceneId;
                string prefix = "qrscene_";
                if (!string.IsNullOrEmpty(requestMessage.EventKey)
                   && requestMessage.EventKey.StartsWith(prefix)
                   && int.TryParse(requestMessage.EventKey.Substring(prefix.Length), out orCodeSceneId))
                {
                    var entity = new FocusHistory()
                    {
                        CreatedTime = DateTime.Now,
                        QrCodeSceneId = orCodeSceneId,
                        UserId = requestMessage.FromUserName,
                        PeopleCount=1,
                         PurePeopleCount=1,
                        Status = 1,
                    };
                    focusHistoryService.Repository.Insert(entity);
                    //添加关联标签,分组,
                    log.Debug("SubscribeWithScan :{0}", orCodeSceneId.ToString());
                    return CreateResponseMessage(orCodeSceneId.ToString(), (int)AutoReplyMPKeywordEnum.SubscribeWithScan);
                }
                #endregion

                return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.SubscribeEvent);
            }
            log.Error("WeixinOpenId is null");
            return null;
        }


        //private void SycUserFromWeixin(string WeixinOpenId)
        //{
        //    log.Debug("SycUserFromWeixin");
        //    var chat = GetWechatConfig();

        //    MP.AdvancedAPIs.User.BatchGetUserInfoData user = new MP.AdvancedAPIs.User.BatchGetUserInfoData
        //    {
        //        openid = WeixinOpenId,
        //        lang = Language.zh_CN.ToString()
        //    };
        //    var result = Innocellence.Weixin.MP.AdvancedAPIs.UserApi.BatchGetUserInfo(chat.WeixinAppId,
        //        chat.WeixinCorpSecret,
        //        new List<MP.AdvancedAPIs.User.BatchGetUserInfoData> {
        //                user
        //        });
        //    log.Debug("SycUserFromWeixin result");
        //    if (result != null && result.user_info_list != null)
        //    {
        //        var userInfo = result.user_info_list.Select(a => WechatMPUserView.ConvertWeChatUserToMpUser(a, chat.AccountManageId.Value, chat.Id)).ToList().FirstOrDefault();
        //        if (userInfo != null)
        //        {
        //            log.Debug("SycUserFromWeixin userInfo: " + userInfo.OpenId + userInfo.NickName);
        //            userInfo.AccountManageId = chat.AccountManageId;
        //            wechatMPUserService.RegistToWeiXin(userInfo);
        //            log.Debug("SycUserFromWeixin userInfo updated.");
        //        }
        //    }
        //}

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_UnsubscribeRequest(RequestMessageEvent_UnSubscribe requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "有空再来";
            //unregist to database
            if (!string.IsNullOrEmpty(WeixinOpenId))
            {
                wechatMPUserService.CancelRegist(WeixinOpenId);
            }
            var last = focusHistoryService.Repository.Entities.Where(a => a.Status == 1 && a.UserId == requestMessage.FromUserName).OrderByDescending(a => a.CreatedTime).FirstOrDefault();
            if (last != null)
            {
                var entity = new FocusHistory()
                {
                    CreatedTime = DateTime.Now,
                    QrCodeSceneId = last.QrCodeSceneId,
                    UserId = requestMessage.FromUserName,
                    PeopleCount = 1,
                    PurePeopleCount = 1,
                    Status = 4,
                };
                focusHistoryService.Repository.Insert(entity);
            }

            //StringBuilder builder = new StringBuilder();
            //builder.Append("MsgId:").Append(requestMessage.MsgId).Append("\n");
            //builder.Append("MsgType:").Append(requestMessage.MsgType).Append("\n");
            //builder.Append("ToUserName:").Append(requestMessage.ToUserName).Append("\n");
            //builder.Append("FromUserName:").Append(requestMessage.FromUserName).Append("\n");
            //builder.Append("Event:").Append(requestMessage.Event.ToString()).Append("\n");
            //var mlog = builder.ToString();
            //log.Debug(mlog);
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.ALL);
        }

        /// <summary>
        /// 事件之扫码推事件(scancode_push)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_ScancodePushRequest(RequestMessageEvent_Scancode_Push requestMessage)
        {
            //var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "事件之扫码推事件";
            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(AutoReplyMenuEnum.SCAN_PUSH_EVENT.ToString(), (int)AutoReplyMPKeywordEnum.MENU);
        }

        /// <summary>
        /// 事件之扫码推事件且弹出“消息接收中”提示框(scancode_waitmsg)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            //var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "事件之扫码推事件且弹出“消息接收中”提示框";
            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(AutoReplyMenuEnum.SCAN_WITH_PROMPT.ToString(), (int)AutoReplyMPKeywordEnum.MENU);
        }

        /// <summary>
        /// 事件之弹出拍照或者相册发图（pic_photo_or_album）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            //var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "事件之弹出拍照或者相册发图";
            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.ALL);
        }

        /// <summary>
        /// 事件之弹出系统拍照发图(pic_sysphoto)
        /// 实际测试时发现微信并没有推送RequestMessageEvent_Pic_Sysphoto消息，只能接收到用户在微信中发送的图片消息。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_PicSysphotoRequest(RequestMessageEvent_Pic_Sysphoto requestMessage)
        {
            //var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "事件之弹出系统拍照发图";
            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.ALL);
        }

        /// <summary>
        /// 事件之弹出微信相册发图器(pic_weixin)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_PicWeixinRequest(RequestMessageEvent_Pic_Weixin requestMessage)
        {
            //var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "事件之弹出微信相册发图器";
            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.ALL);
        }

        /// <summary>
        /// 事件之弹出地理位置选择器（location_select）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_LocationSelectRequest(RequestMessageEvent_Location_Select requestMessage)
        {
            //var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "事件之弹出地理位置选择器";
            //return new List<IResponseMessageBase>(){responseMessage};
            return CreateResponseMessage(string.Empty, (int)AutoReplyMPKeywordEnum.ALL);
        }

        private List<IResponseMessageBase> CreateResponseMessage(string inputStr, int type)
        {
            int appId = int.Parse(_postModel.AppId);
            log.Debug("keyword :{0}, type :{1}.", inputStr, type);
            List<IResponseMessageBase> responseMsgList = new List<IResponseMessageBase>();
            var contents = autoReplyContentService.GetList<AutoReplyContentView>(appId, inputStr, type);
            if (null != contents && contents.Count > 0)
            {
                foreach (var content in contents)
                {
                    log.Debug("Response : appId:{0}, type :{1}.", appId, content.PrimaryType);

                    IResponseMessageBase responseMsg = null;
                    try
                    {
                        switch (content.PrimaryType)
                        {
                            case (int)AutoReplyContentEnum.LINK: //被动回复不支持该类型,当成文本回复
                                responseMsg = CreateLinkeResponseMessage(content);
                                break;
                            case (int)AutoReplyContentEnum.TEXT:
                                responseMsg = CreateTextResponseMessage(content);
                                log.Debug("Response TEXT: appId:{0}, type :{1}", appId, responseMsg.MsgType.ToString());
                                break;
                            case (int)AutoReplyContentEnum.VOICE:
                                responseMsg = CreateVoiceResponseMessage(content);
                                log.Debug("Response VOICE: appId:{0}, type :{1}", appId, responseMsg.MsgType.ToString());
                                break;
                            case (int)AutoReplyContentEnum.VIDEO:
                                responseMsg = CreateVideoResponseMessage(content);
                                log.Debug("Response VIDEO: appId:{0}, type :{1}", appId, responseMsg.MsgType.ToString());
                                break;
                            case (int)AutoReplyContentEnum.NEWS:
                                responseMsg = CreateNewsResponseMessage(content, appId);
                                break;
                            case (int)AutoReplyContentEnum.IMAGE:
                                responseMsg = CreateImageResponseMessage(content);
                                break;
                            case (int)AutoReplyContentEnum.FILE: //被动回复不支持该类型
                            default:
                                log.Warn("Some content type({0}) do not be supported in current logic.", content.PrimaryType);
                                break;
                        }
                        if (responseMsg != null)
                        {
                            log.Debug("Response responseMsg: appId:{0}, type :{1}", appId, responseMsg.MsgType.ToString());
                            (responseMsg as ResponseMessageBaseWechat).APPID = appId;
                        }
                        OtherOperationImpl(content);
                    }
                    catch (Exception e)
                    {
                        log.Error("Lost some auto reply content.");
                        log.Error(e);
                    }
                    if (responseMsg != null)
                    {
                        log.Debug("Response from :[{0}]; to :[{1}]  type:{2}", responseMsg.FromUserName, responseMsg.ToUserName, responseMsg.MsgType.ToString());
                        responseMsgList.Add(responseMsg);
                    }
                }
            }
            if (responseMsgList.Count > 0)
            {
                return responseMsgList;
            }
            else
            {
                //  var ret = this.CreateResponseMessage<ResponseMessageText>();
                // ret.Content = "";
                //  responseMsgList.Add(ret);
                return responseMsgList;
            }

        }

        private void OtherOperationImpl(AutoReplyContentView content)
        {
            var chat = GetWechatConfig();
            if (content.UserGroups != null && content.UserGroups.Count > 0)
            {
                var user = wechatMPUserService.GetUserByOpenId(WeixinOpenId);
                if (!string.IsNullOrEmpty(user.TagIdList))
                {
                    var untagResult = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.BatchUntagging(chat.WeixinAppId, chat.WeixinCorpSecret, int.Parse(user.TagIdList), new List<string> { WeixinOpenId });
                    if (untagResult.errcode != ReturnCode.请求成功)
                    {
                        throw new Exception(untagResult.errmsg);
                    }
                }
                var tagResult = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.BatchTagging(chat.WeixinAppId, chat.WeixinCorpSecret, content.UserGroups[0], new List<string> { WeixinOpenId });
                if (tagResult != null && tagResult.errcode == ReturnCode.请求成功)
                {
                    bool result = wechatMPUserService.ChangeGroup(content.UserGroups[0], WeixinOpenId);
                }
            }
            if (content.UserTags != null && content.UserTags.Count > 0)
            {
                //systemUserTagMappingService.DeleteByOpenId(WeixinOpenId);
                var oldTags = systemUserTagMappingService.GetUserTagByOpenId(WeixinOpenId);
                var userTagMappings = content.UserTags.Select(a => new SystemUserTagMapping() { TagId = a, UserOpenid = WeixinOpenId }).ToList();
                var newIds = userTagMappings.Select(a => a.TagId).ToList();
                foreach (var old in oldTags)
                {
                    if (newIds.Contains<int>(old.TagId))
                    {
                        var item = userTagMappings.Where(a => a.TagId == old.TagId).First();
                        userTagMappings.Remove(item);
                    }
                }
                systemUserTagMappingService.Create(userTagMappings);
            }
            //if (content.MessageTags != null && content.MessageTags.Count > 0)
            //{

            //}
            if (string.IsNullOrEmpty(content.InterfaceLink))
            {
                //to do
            }
        }

        private IResponseMessageBase CreateVideoResponseMessage(AutoReplyContentView content)
        {
            var videoMsg = this.CreateResponseMessage<ResponseMessageVideo>();



            //var video = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content)[0];
            //videoMsg.Video = new Video()
            //{
            //    Title = video.NewsTitle,
            //    Description = video.NewsComment,
            //    MediaId = video.MediaId,
            //};

            videoMsg.Video = CustomMessageCommon.CreateVideoResponseMessage(content, _postModel.CorpId);
            log.Debug("ResponseMessageVideo Type:{0}", videoMsg.MsgType.ToString());
            return videoMsg;
        }

        private IResponseMessageBase CreateVoiceResponseMessage(AutoReplyContentView content)
        {
            var voiceMsg = this.CreateResponseMessage<ResponseMessageVoice>();

            //var voice = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content)[0];
            //voiceMsg.Voice = new Voice() { MediaId = voice.MediaId };

            voiceMsg.Voice = CustomMessageCommon.CreateVoiceResponseMessage(content, _postModel.CorpId);

            return voiceMsg;
        }

        private IResponseMessageBase CreateImageResponseMessage(AutoReplyContentView content)
        {
            var imageMsg = this.CreateResponseMessage<ResponseMessageImage>();

            //var image = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content)[0];
            //imageMsg.Image = new Image() { MediaId = image.MediaId };

            imageMsg.Image = CustomMessageCommon.CreateImageResponseMessage(content, _postModel.CorpId);

            return imageMsg;
        }

        private IResponseMessageBase CreateNewsResponseMessage(AutoReplyContentView content, int appId)
        {
            var photoTextMsg = this.CreateResponseMessage<ResponseMessageNews>();
            var list = CustomMessageCommon.CreateNewsResponseMessage(content, appId, false, false, true);
            photoTextMsg.Articles = list.Select(a => (Article)a).ToList();

            //photoTextMsg.Articles = new List<Article>();

            //var info = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content)[0];
            //var configs = Infrastructure.Web.Domain.Service.CommonService.lstSysConfig;
            //var config = configs.Where(a => a.ConfigName.Equals("Content Server", StringComparison.CurrentCultureIgnoreCase)).First();
            //var contentConfig = configs.Where(a => a.ConfigName.Equals("Content Server", StringComparison.CurrentCultureIgnoreCase)).First();
            //string host = config.ConfigValue;
            //if (host.EndsWith("/"))
            //{
            //    host = host.Substring(0, host.Length - 1);
            //}
            //string contentHost = contentConfig.ConfigValue;
            //if (contentHost.EndsWith("/"))
            //{
            //    contentHost = contentHost.Substring(0, contentHost.Length - 1);
            //}
            //var picUrl = contentHost + info.ImageSrc;
            //var url = host + "/MPNews/ArticleInfo/GetNews?id=" + content.AutoReplyId +"&type="+(int)NewsTypeEnum.AutoReply + "&wechatid=" + appId;// "&subId=" + item.Id;
            //var newArticle = new Article()
            //{
            //    Title = info.NewsTitle,
            //    Url = url,
            //    PicUrl = picUrl,
            //    Description = info.NewsComment
            //};
            //photoTextMsg.Articles.Add(newArticle);

            return photoTextMsg;
        }

        private IResponseMessageBase CreateLinkeResponseMessage(AutoReplyContentView content)
        {
            var testMsg = this.CreateResponseMessage<ResponseMessageText>();
            testMsg.Content = CustomMessageCommon.CreateLinkResponseMessage(content);
            log.Debug("Create link response :{0}", content);
            return testMsg;
        }

        private IResponseMessageBase CreateTextResponseMessage(AutoReplyContentView content)
        {
            var testMsg = this.CreateResponseMessage<ResponseMessageText>();
            //var info = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content);
            //testMsg.Content = info[0].NewsContent;

            testMsg.Content = CustomMessageCommon.CreateTextResponseMessage(content);

            log.Debug("Create text response :{0}", content);
            return testMsg;
        }



        private SysWechatConfig GetWechatConfig()
        {
            log.Debug("GetWechatConfig in customer handler ");
            int appId = int.Parse(_postModel.AppId);
            log.Debug("GetWechatConfig in customer handler ，appId ：" + appId);
            var sysWechatConfig = sysWechatConfigService.Repository.Entities.Where(a => a.Id == appId).FirstOrDefault();

            if (sysWechatConfig != null)
            {
                log.Debug("GetWechatConfig in customer handler ，sysWechatConfig ：" + sysWechatConfig.AppName);

                return sysWechatConfig;
            }
            return new SysWechatConfig { };
        }
        //private WechatMPUserView ConvertToWechatMPUserView(UserInfoJson model)
        //{
        //    return new WechatMPUserView
        //    {
        //        City = model.city,
        //        Country = model.country,
        //        Province = model.province,
        //        GroupId = model.groupid,
        //        HeadImgUrl = model.headimgurl,
        //        Language = model.language,
        //        NickName = model.nickname,
        //        OpenId = model.openid,
        //        Remark = model.remark,
        //        Sex = model.sex,
        //        SubScribe = model.subscribe,
        //        SubScribeTime = model.subscribe_time,
        //        TagIdList = string.Join(",", model.tagid_list),
        //        UnionId = model.unionid,
        //    };
        //}
    }
}