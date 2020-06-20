/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：QyCustomMessageHandler.cs
    文件功能描述：自定义QyMessageHandler
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Utility;
using Infrastructure.Utility.Data;
using Innocellence.Weixin.CommonService.QyMessageHandler;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.Entities;
using Innocellence.Weixin.QY.MessageHandlers;
using Infrastructure.Core.Logging;

using Infrastructure.Utility.Extensions;
using Infrastructure.Web.Domain.Service;
using System.Web.Configuration;
using System.Net;
using Innocellence.Weixin.QY.AdvancedAPIs.Media;
using Innocellence.Weixin.QY.CommonAPIs;

using Infrastructure.Web.ImageTools;
using System.Web.Hosting;
using Infrastructure.Web.Domain.Entity;

using Innocellence.Weixin.QY.AdvancedAPIs;
using IResponseMessageBase = Innocellence.Weixin.QY.Entities.IResponseMessageBase;
using ResponseMessageBase = Innocellence.Weixin.QY.Entities.ResponseMessageBase;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.Weixin.CommonService.QyMessageHandlers
{
    public class QyCustomMessageHandler : QyMessageHandler<QyCustomMessageContext>
    {
        private ArticleInfoService _articleInfoService = new ArticleInfoService();
        private WeChatAppUserService _weChatAppUserService = new WeChatAppUserService();
        private BatchJobLogService _batchJobLogService = new BatchJobLogService();

        private string _newsHost = WebConfigurationManager.AppSettings["NewsHost"];
        private string _videoHost = WebConfigurationManager.AppSettings["VideoHost"];
        private string _activityHost = WebConfigurationManager.AppSettings["ActivityHost"];
        private string _hongtuHost = WebConfigurationManager.AppSettings["HongtuHost"];
        private string _stHost = WebConfigurationManager.AppSettings["SalesTrainingHost"];
        private string _nscHost = WebConfigurationManager.AppSettings["NSCHost"];
        private string _sppHost = WebConfigurationManager.AppSettings["SPPHost"];
        private string _hresHost = WebConfigurationManager.AppSettings["HRESHost"];

        //TODO
        private ArticleImagesService _articleImagesService = new ArticleImagesService();
        private const string USER_IMAGE_SAVE_FOLDER_PATH = @"/wxUserImage/";

        // private bool _isDebug = false;

        private ILogger log { get { return LogManager.GetLogger("WeChat"); } }

        public QyCustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, bool isDebug = false)
            : base(inputStream, postModel, maxRecordCount, isDebug)
        {
            LogManager.GetLogger(this.GetType()).Debug("maxRecordCount is" + maxRecordCount);
            // this.actAfterGetData = AfterGetData;

            // this.actAfterDecryptData = AfterDecryptData;

            //log = LogManager.GetLogger("WeChat");
            //_isDebug = isDebug;

            log.Debug("Entering QyCustomMessageHandler debug: " + isDebug.ToString());
        }

        public override void AfterGetData(EncryptPostData objPostData, PostModel objPostModel)
        {
            log.Debug("Entering AfterGetData0 EncryptPostData AgentID:{0} PostModel AgentID:{1}", objPostData.AgentID, objPostModel.AppId);

            log.Debug("Entering AfterGetData1 EncryptPostData Msg_Signature:{0} PostModel Timestamp:{1}  Nonce:{2}",
                objPostModel.Msg_Signature, objPostModel.Timestamp, objPostModel.Nonce);
            // log.Debug("objPostData is null?{0} ", objPostData==null);

            var objConfig = Innocellence.WeChat.Domain.Common.WeChatCommonService.GetWeChatConfig(objPostData.AgentID);
            if (objConfig == null)
            {
                log.Error("AfterGetData GetWeChatConfig get Error.  EncryptPostData AgentID:{0} PostModel AgentID:{1}", objPostData.AgentID, objPostModel.AppId);
            }
            else
            {
                objPostModel.CorpId = objConfig.WeixinCorpId;
                objPostModel.EncodingAESKey = objConfig.WeixinEncodingAESKey;
                objPostModel.Token = objConfig.WeixinToken;
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

        static WebRequestPost _wr = new WebRequestPost();
        /// <summary>
        /// Event事件类型请求之CLICK
        /// </summary>
        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            log.Debug("Entering Event事件类型请求之CLICK - " + requestMessage.EventKey);
            // 记录用户行为
            string errorMessage;
            // add permission check when user click the menu on winxin side
            if (!SmartphoneMenuPermissionManager.PermissionCheck(requestMessage.EventKey, requestMessage.FromUserName, requestMessage.AgentID, out errorMessage))
            {
                return NoPermissionResponse(errorMessage);
            }

            // 记录用户行为
            _wr.CallUserBehavior(requestMessage.EventKey, requestMessage.FromUserName, requestMessage.AgentID.ToString(), "", "", 4);

            switch (requestMessage.AgentID)
            {
                //case 6: // News
                //    {
                //        var articles = GetPushArticles(requestMessage.EventKey, 6);
                //        return CreateResponse(articles, _activityHost, "/News/ArticleInfo/wxdetail/");
                //    }
                //case 7: // HongTu
                //    {
                //        var articles = GetPushArticles(requestMessage.EventKey, 6);
                //        return CreateResponse(articles, _hongtuHost, "/News/ArticleInfo/wxdetail/");
                //    }
                //case 8: // Video
                //    {
                //        var articles = GetPushArticles(requestMessage.EventKey, 6);
                //        return CreateResponse(articles, _videoHost, "/News/ArticleInfo/wxdetail/");
                //    }
                //case 9: // Activity
                //    {
                //        var articles = GetPushArticles(requestMessage.EventKey, 6);
                //        return CreateResponse(articles, _activityHost, "/News/ArticleInfo/wxdetail/");
                //    }
                //case 14: // NSC
                //    {
                //        var articles = GetPushArticles(requestMessage.EventKey, 6);
                //        return CreateResponse(articles, _newsHost, "/News/ArticleInfo/wxdetail/");
                //    }
                //case 15: //SPP Communicate
                //    {
                //        var articles = GetPushArticles(requestMessage.EventKey, 6);
                //        return CreateResponse(articles, _newsHost, "/News/ArticleInfo/wxdetail/");
                //    }
                //case 16: // SalesTraining
                //    {
                //        var articles = GetPushArticles(requestMessage.EventKey, 6);
                //        return CreateResponse(articles, _newsHost, "/News/ArticleInfo/wxdetail/");
                //    }
                //case 17: // HR Employee Service
                //    {
                //        return ProcessClickRequest17(requestMessage.EventKey, requestMessage.FromUserName);
                //    }
                default:
                    {
                        LogManager.GetLogger(this.GetType()).Debug("Entering default2");

                        var menu = Infrastructure.Web.Domain.Service.CommonService.lstCategory.FirstOrDefault(a => a.CategoryCode == requestMessage.EventKey);
                        if (menu == null)
                        {
                            return null;
                        }
                        LogManager.GetLogger(this.GetType()).Debug("menu is ready");
                        string x = "";
                        //LogManager.GetLogger(this.GetType()).Debug(menu.Function.Replace('{', '【').Replace('}', '】'));
                        AppMenuView appMenu = new AppMenuView();
                        appMenu.ButtonReturnType = JsonHelper.FromJson<ButtonReturnType>(menu.Function);
                        if (appMenu == null || appMenu.ButtonReturnType == null || string.IsNullOrEmpty(appMenu.ButtonReturnType.ResponseMsgType))
                        {
                            LogManager.GetLogger(this.GetType()).Debug("appMenu parsed is NULL");

                            return null;
                        }
                        switch (appMenu.ButtonReturnType.ResponseMsgType.ToLower())
                        {
                            case "text":
                                var textMessage = this.CreateResponseMessage<ResponseMessageText>();
                                x = "";
                                LogManager.GetLogger(this.GetType()).Debug("appMenu.ButtonReturnType = " + appMenu.ButtonReturnType);
                                x = "";
                                LogManager.GetLogger(this.GetType()).Debug("appMenu.ButtonReturnType.Content = " + appMenu.ButtonReturnType.Content);
                                //x = string.Format("appMenu.ButtonReturnType.Button.name = {0}", appMenu.ButtonReturnType.Button.name);
                                LogManager.GetLogger(this.GetType()).Debug("ddd");
                                textMessage.Content = appMenu.ButtonReturnType.Content;
                                return textMessage;

                            case "news":
                                var articles = GetPushArticles(requestMessage.EventKey, 6);
                                return CreateResponse(articles, _newsHost, "/News/ArticleInfo/wxdetail/");
                        }
                        break;
                    }
            }

            return DefaultResponseMessage(requestMessage);
        }

        private List<ArticleInfoView> GetPushArticles(string eventkey, int takeCount = 6)
        {
            var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventkey)
                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
                .Take(takeCount).ToList();
            return articles;
        }

        private IResponseMessageBase NoPermissionResponse(string errorMessage)
        {
            var message = CreateResponseMessage<ResponseMessageText>();
            message.Content = errorMessage;//"对不起，您没有访问该菜单的权限!";
            return message;
        }


        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            var objConfig = WeChatCommonService.GetWeChatConfig(requestMessage.AgentID);
            responseMessage.Content = objConfig.WelcomeMessage;
            return responseMessage;
            //switch (requestMessage.AgentID)
            //{

            //    case 6:

            //        responseMessage.Content = "Welcome to Lilly China News.\r\n欢迎光临礼来中国快讯。";
            //        break;

            //    case 7:
            //        responseMessage.Content = "Welcome to Hong Tu.\r\n欢迎光临宏途。";
            //        break;

            //    case 8:
            //        responseMessage.Content = "Welcome to iVideo.\r\n欢迎光临微视频。";
            //        break;

            //    case 9:
            //        responseMessage.Content = "Welcome to iEvent.\r\n欢迎光临微活动。";
            //        break;
            //    case 14:
            //        responseMessage.Content = "Welcome to 2016 NSC.\r\n欢迎光临2016年会服务。";
            //        break;
            //    case 15:
            //        responseMessage.Content = "欢迎关注营销加油站！这里有最新最全的系统操作指南和流程指导，让你一手掌握！\r\nWelcome to Sales & Marketing Communication!";
            //        break;
            //    case 16:
            //        responseMessage.Content = "Welcome to Sales Training.\r\n欢迎光临销售培训。";
            //        break;
            //    case 17:
            //        responseMessage.Content = "盼星星盼月亮，小礼终于盼到你的关注啦！小礼企业号为您提供全方位员工服务！详情点击菜单栏哦。";
            //        break;
            //    default:
            //        responseMessage.Content = null;
            //        break;
            //}

        }

        private ResponseMessageNews CreateResponse(List<ArticleInfoView> articles, string host, string baseUrl)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageNews>();
            responseMessage.Articles = new List<Article>();
            foreach (var a in articles)
            {
                string imgUrl;
                //if (string.IsNullOrEmpty(a.ArticleContent))
                //{
                //    try
                //    {
                //        int? imgId = null;
                //        string imageUrl = null;

                //        foreach (var c in a.ArticleContentViews)
                //        {
                //            if (c.ImageID != null)
                //            {
                //                imgId = c.ImageID;

                //                // 这里处理imageUrl为_T的
                //                imageUrl = c.ImageUrl.Substring(0, c.ImageUrl.LastIndexOf('.')) + "_T.jpg";
                //                break;
                //            }
                //        }
                //        if (imgId == null)
                //        {
                //            LogManager.GetLogger(this.GetType()).Debug("Not found Image!");
                //            imgUrl = "http://wechatcms.lillyadmin.cn/Content/img/LillyLogoRed.png";
                //        }
                //        else
                //        {
                //            imgUrl = string.Format("{0}/Common/ThumbFile?id={1}&FileName={2}", host, imgId, imageUrl);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        LogManager.GetLogger(this.GetType()).Debug("Error in Creating news - \r\n" + a.ArticleTitle + "\r\n" + a.ImageUrl + "\r\n" + ex.Message);
                //        imgUrl = "http://wechatcms.lillyadmin.cn/Content/img/LillyLogoRed.png";
                //    }
                //}
                if (a.ThumbImageId == null)
                {
                    LogManager.GetLogger(this.GetType()).Debug("Not found Image!");
                    imgUrl = "http://wechatcms.lillyadmin.cn/Content/img/LillyLogoRed.png";
                }
                else
                {
                    imgUrl = string.Format("{0}/Common/PushFile?id={1}&FileName={2}", host, a.ThumbImageId, a.ThumbImageUrl);
                }

                var newArticle = new Article()
                {
                    Title = a.ArticleTitle,
                    Url = host + baseUrl + a.Id,
                    PicUrl = imgUrl,
                    Description = a.ArticleComment
                };
                LogManager.GetLogger(this.GetType()).Debug("Creating News - \r\n\tTitle: " + newArticle.Title + "\r\n\tUrl: " + newArticle.Url + "\r\n\tPush Image: " + newArticle.PicUrl);

                responseMessage.Articles.Add(newArticle);
            }
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_EnterAgentRequest(RequestMessageEvent_Enter_Agent requestMessage)
        {
            LogManager.GetLogger(this.GetType()).Debug("用户进入应用事件 - AppID:" + requestMessage.AgentID);
            //var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "LED欢迎您！感谢进入！";
            // HttpContext.Current.Session["Username"]="heying";

            // 记录用户行为
            _wr.CallUserBehavior(requestMessage.EventKey, requestMessage.FromUserName, requestMessage.AgentID.ToString(), "", "", 10);

            // 先判断是否第一次进入。
            // 从_weChatAppUser中获取当前用户
            string userId = requestMessage.FromUserName;
            bool isFirstJoin = false;
            var existingUser = _weChatAppUserService.Repository.Entities.Where(a => a.WeChatUserID == userId && a.WeChatAppID == requestMessage.AgentID).FirstOrDefault();

            CategoryType appType = (CategoryType)requestMessage.AgentID;


            isFirstJoin = (existingUser == null || existingUser == default(Innocellence.WeChat.Domain.Entity.WeChatAppUser));
            // 如果是第一次进入，需要将用户插入到数据库中
            if (isFirstJoin)
            {
                LogManager.GetLogger(this.GetType()).Debug("new WeChatAppUser - WeChatAppID=" + appType.ToDescription());
                // 将用户信息插入到数据库中
                _weChatAppUserService.Repository.Insert(new Innocellence.WeChat.Domain.Entity.WeChatAppUser()
                {
                    WeChatAppID = requestMessage.AgentID,
                    WeChatUserID = userId
                });

            }

            // 只有第一次推送。
            if (isFirstJoin)
            {
                List<ArticleInfoView> items;
                // 整理首次发送数据
                // TODO: 查找所有新闻的最新6条数据，组织图文推送
                switch (appType)
                {
                    case CategoryType.ArticleInfoCate:
                        {
                            items = _articleInfoService.GetList<ArticleInfoView>(a => a.ArticleStatus == "Published" && a.AppId == (int)CategoryType.ArticleInfoCate)
                               .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
                                .Take(6).ToList();
                            return CreateResponse(items, _newsHost, "/News/ArticleInfo/wxdetail/");
                        }
                    case CategoryType.HongtuCate:
                        {
                            items = _articleInfoService.GetList<ArticleInfoView>(a => a.ArticleStatus == "Published" && a.AppId == (int)CategoryType.HongtuCate)
                                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
                                .Take(6).ToList();
                            return CreateResponse(items, _hongtuHost, "/News/ArticleInfo/wxdetail/");
                        }
                    case CategoryType.VideoCate:
                        {
                            items = _articleInfoService.GetList<ArticleInfoView>(a => a.ArticleStatus == "Published" && a.AppId == (int)CategoryType.VideoCate)
                                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
                                .Take(5).ToList();
                            return CreateResponse(items, _videoHost, "/News/ArticleInfo/wxdetail/");
                        }
                    case CategoryType.ActivityCate:
                        {
                            items = _articleInfoService.GetList<ArticleInfoView>(a => a.ArticleStatus == "Published" && a.AppId == (int)CategoryType.ActivityCate)
                                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
                                .Take(6).ToList();
                            return CreateResponse(items, _activityHost, "/News/ArticleInfo/wxdetail/");
                        }
                    case CategoryType.SalesTrainingCate:
                        {
                            items = _articleInfoService.GetList<ArticleInfoView>(a => a.ArticleStatus == "Published" && a.AppId == (int)CategoryType.SalesTrainingCate && a.ArticleCateSub == "ST_ENTRY_PUSH")
                                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
                                .ToList();
                            return CreateResponse(items, _stHost, "/News/ArticleInfo/wxdetail/");
                        }
                    case CategoryType.HREServiceCate:
                        {
                            items = _articleInfoService.GetList<ArticleInfoView>(a => a.ArticleStatus == "Published" && a.AppId == (int)CategoryType.HREServiceCate && a.ArticleCateSub == "HRES_ENTRY_PUSH")
                                   .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
                                   .ToList();
                            return CreateResponse(items, _hresHost, "/News/ArticleInfo/wxdetail/");
                        }
                    case CategoryType.SPPCate:
                        {
                            items = _articleInfoService.GetList<ArticleInfoView>(a => a.ArticleStatus == "Published" && a.AppId == (int)CategoryType.SPPCate && a.ArticleCateSub == "SPP_ENTRY_PUSH")
                                   .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
                                   .ToList();
                            return CreateResponse(items, _sppHost, "/News/ArticleInfo/wxdetail/");
                        }
                    case CategoryType.NSCCate:
                        {
                            items = _articleInfoService.GetList<ArticleInfoView>(a => a.ArticleStatus == "Published" && a.AppId == (int)CategoryType.SPPCate && a.ArticleCateSub == "NSC_ENTRY_PUSH")
                                   .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
                                   .ToList();
                            return CreateResponse(items, _newsHost, "/News/ArticleInfo/wxdetail/");
                        }
                    default:
                        {
                            LogManager.GetLogger(this.GetType()).Debug("App Type not defined.");
                            items = _articleInfoService.GetList<ArticleInfoView>(a => a.ArticleStatus == "Published" && a.AppId == requestMessage.AgentID && a.ArticleCateSub.EndsWith("ENTRY_PUSH"))
                                   .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
                                   .ToList();
                            return CreateResponse(items, _newsHost, "/News/ArticleInfo/wxdetail/");
                        }
                }
            }

            return null;
        }

        public override QY.Entities.IResponseMessageBase DefaultResponseMessage(QY.Entities.IRequestMessageBase requestMessage)
        {

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "这是一条没有找到合适回复信息的默认消息。";

            //responseMessage.Content = "暂不支持自动回复。";

            //return null;
            return responseMessage;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {

            //记录用户行为
            _wr.CallUserBehavior("TEXT_EVENT", requestMessage.FromUserName, requestMessage.AgentID.ToString(),
                requestMessage.Content, "", 8);
            LogManager.GetLogger(this.GetType()).Debug("NO TEXT behavior ");
            return CreateNoResponseMessage(requestMessage.AgentID);
        }

        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            //记录用户行为
            _wr.CallUserBehavior("IMAGE_EVENT", requestMessage.FromUserName, requestMessage.AgentID.ToString(), requestMessage.MediaId, "", 8);

            LogManager.GetLogger(this.GetType()).Debug("OnImageRequest");
            LogManager.GetLogger(this.GetType()).Debug("AppId: {0} CreatedUserID: {1}", requestMessage.AgentID, requestMessage.FromUserName);
            SaveImage(requestMessage);

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "感谢您的参与。";

            return responseMessage;

        }

        private void SaveImage(RequestMessageImage requestMessage)
        {
            LogManager.GetLogger(this.GetType()).Debug("Begin SaveImage from Wechat Client");

            //WebClient webclient = new WebClient();
            //var picData = await webclient.DownloadDataTaskAsync(requestMessage.PicUrl);
            //string picType = requestMessage.PicUrl.Substring(requestMessage.PicUrl.LastIndexOf('.') + 1);

            //var img = ImageHelper.BytesToImage(picData);

            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    LogManager.GetLogger(this.GetType()).Debug("MediaApi.Get");
                    MediaApi.Get(GetToken(requestMessage.AgentID), requestMessage.MediaId, stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    //var imageBytes = stream.ToArray();
                    //var imageFromUser = ImageHelper.BytesToImage(imageBytes);

                    LogManager.GetLogger(this.GetType()).Debug("ImageUtility.MakeThumbnail");

                    string filename = USER_IMAGE_SAVE_FOLDER_PATH + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
                    string strPath = MapPath("/" + filename);

                    ImageUtility.MakeThumbnail(null, stream, strPath, 900, 0, "W", true);
                    //thumbnailImageFromUser.Save(strPath);

                    //LogManager.GetLogger(this.GetType()).Debug("thumbnailImageFromUser is NULL? - " + (thumbnailImageFromUser == null));

                    LogManager.GetLogger(this.GetType()).Debug("new ArticleImages()");
                    LogManager.GetLogger(this.GetType()).Debug("AppId: {0} CreatedUserID: {1}", requestMessage.AgentID, requestMessage.FromUserName);
                    var articleImage = new ArticleImages()
                    {
                        AppId = requestMessage.AgentID,
                        UploadedUserId = requestMessage.FromUserName,
                        ImageContent = System.IO.File.ReadAllBytes(strPath),
                        CreatedDate = DateTime.Now,
                        ImageName = filename
                    };

                    LogManager.GetLogger(this.GetType()).Debug("_articleImagesService.Insert");
                    _articleImagesService.Repository.Insert(articleImage);
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger(this.GetType()).Error(ex.Message + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace);
                }
            }

            LogManager.GetLogger(this.GetType()).Debug("End SaveImage from Wechat Client");
        }

        public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            //记录用户行为
            _wr.CallUserBehavior("VOICE_EVENT", requestMessage.FromUserName, requestMessage.AgentID.ToString(), requestMessage.MediaId, "", 8);
            LogManager.GetLogger(this.GetType()).Debug("NO VOICE behavior ");
            return CreateNoResponseMessage(requestMessage.AgentID);
        }

        public override IResponseMessageBase OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            _wr.CallUserBehavior("Pic_Photo_Or_Album", requestMessage.FromUserName, requestMessage.AgentID.ToString(), requestMessage.EventKey, "", 8);

            LogManager.GetLogger(this.GetType()).Debug("NO Photo behavior ");
            return CreateNoResponseMessage(requestMessage.AgentID);
        }

        public override IResponseMessageBase OnEvent_BatchJobResultRequest(RequestMessageEvent_Batch_Job_Result requestMessage)
        {
            LogManager.GetLogger(this.GetType()).Debug("RequestMessageEvent_Batch_Job_Result Enter");
            if (requestMessage.BatchJob != null && requestMessage.BatchJob.ErrCode == 0)
            {
                //根据返回的JobId查导入的Result.
                object ret = null;

                switch (requestMessage.BatchJob.JobType)
                {
                    case "sync_user":
                    case "replace_user":
                        var ret1 = MailListApi.BatchJobStatus<List<JobResultObjectUser>>(GetToken(0), requestMessage.BatchJob.JobId);
                        ret = ret1.result.OrderByDescending(t => t.errcode).ToList();
                        break;
                    case "invite_user":
                        var ret2 = MailListApi.BatchJobStatus<List<JobResultObjectInvite>>(GetToken(0), requestMessage.BatchJob.JobId);
                        ret = ret2.result.OrderByDescending(t => t.errcode).ToList();
                        break;
                    case "replace_party":
                        var ret3 = MailListApi.BatchJobStatus<List<JobResultObjectParty>>(GetToken(0), requestMessage.BatchJob.JobId);
                        ret = ret3.result.OrderByDescending(t => t.errcode).ToList();
                        break;
                }
                string json = JsonHelper.ToJson(ret);
                //errocode为0代表异步任务成功 更新表状态
                var jobLog = _batchJobLogService.GetLogByJobId(requestMessage.BatchJob.JobId);
                jobLog.Result = json;
                jobLog.Status = 1;
                _batchJobLogService.Repository.Update(jobLog);
            }
            else
            {
                string msg = requestMessage.BatchJob == null ? "BatchJobInfo is null" : requestMessage.BatchJob.ErrMsg;
                LogManager.GetLogger(this.GetType()).Debug("BatchUser Result Call Back - Error Msg=" + msg);
            }

            return new ResponseMessageBase();
        }

        private IResponseMessageBase CreateNoResponseMessage(int agentid)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            if (agentid == 17)
            {
                responseMessage.Content = "如有问题，请您在“服务”—“在线服务”中提交您的疑问。";
            }
            else
            {
                responseMessage.Content = "暂不支持回复功能。";
            }


            return responseMessage;
        }

        //TODO
        /// <summary>
        /// 用户图片获取并上传到服务器,数据库同步更新
        /// </summary>
        /// <param name="requestMessage">RequestMessageImage</param>
        private void ModifyUserImageInfo(RequestMessageImage requestMessage)
        {
            if (requestMessage == null || string.IsNullOrEmpty(requestMessage.PicUrl))
            {
                return;
            }

            WebClient client = new WebClient();

        }

        ///// <summary>
        ///// Menu request for News App
        ///// </summary>
        ///// <param name="eventKey"></param>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //private IResponseMessageBase ProcessClickRequest6(string eventKey, string userId)
        //{
        //    wr.CallUserBehavior(eventKey, userId, "6", "");

        //    switch (eventKey)
        //    {
        //        case "NEWS_CORP_NEWS":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == "NEWS_CORP_NEWS")
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseNews(articles, _newsHost, "/News/ArticleInfo/wxdetail/");
        //            }
        //        case "NEWS_DEV_REPORT":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == "NEWS_DEV_REPORT")
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseNews(articles, _newsHost, "/News/ArticleInfo/wxdetail/");
        //            }
        //        case "NEWS_CSR":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == "NEWS_CSR")
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseNews(articles, _newsHost, "/News/ArticleInfo/wxdetail/");
        //            }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Menu request for Hongtu App
        ///// </summary>
        ///// <param name="eventKey"></param>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //private IResponseMessageBase ProcessClickRequest7(string eventKey, string userId)
        //{
        //    wr.CallUserBehavior(eventKey, userId, "7", "");

        //    switch (eventKey)
        //    {
        //        case "HONGTU_LATEST":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == "HONGTU_LATEST")
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseVideo(articles, _hongtuHost, "/Hongtu/Hongtu/wxdetail/");
        //            }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Menu request for Video App
        ///// </summary>
        ///// <param name="eventKey"></param>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //private IResponseMessageBase ProcessClickRequest8(string eventKey, string userId)
        //{
        //    wr.CallUserBehavior(eventKey, userId, "8", "");
        //    switch (eventKey)
        //    {
        //        case "VIDEO_LILLY_VIDEO":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == "VIDEO_LILLY_VIDEO")
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseVideo(articles, _videoHost, "/Video/Video/wxdetail/");
        //            }
        //        case "VIDEO_PROJECT_EXHIBIT":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == "VIDEO_PROJECT_EXHIBIT")
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseVideo(articles, _videoHost, "/Video/Video/wxdetail/");
        //            }
        //        case "VIDEO_MORE":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == "VIDEO_MORE")
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    // TODO: 为了上线，用7个，等以后还需要改成6个
        //                    .Take(7).ToList();
        //                return CreateResponseVideo(articles, _videoHost, "/Video/Video/wxdetail/");
        //            }

        //    }

        //    return null;
        //}

        ///// <summary>
        ///// Menu request for Activity App
        ///// </summary>
        ///// <param name="eventKey"></param>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //private IResponseMessageBase ProcessClickRequest9(string eventKey, string userId)
        //{
        //    wr.CallUserBehavior(eventKey, userId, "9", "");

        //    var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventKey)
        //        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //        .Take(6).ToList();
        //    return CreateResponseActivity(articles, _activityHost, "/Activity/Activity/wxdetail/");

        //}

        ///// <summary>
        ///// Menu request for NSC App
        ///// </summary>
        ///// <param name="eventKey"></param>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //private IResponseMessageBase ProcessClickRequest14(string eventKey, string userId)
        //{
        //    wr.CallUserBehavior(eventKey, userId, "14", "");

        //    var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventKey)
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //    return CreateResponseNews(articles, _newsHost, "/News/ArticleInfo/wxdetail/");

        //    //switch (eventKey)
        //    //{
        //    //    case "NSC_GM&COO_INVITATION":   // 邀请函
        //    //        {
        //    //            var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventKey)
        //    //                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //    //                .Take(6).ToList();
        //    //            return CreateResponseNews(articles, _newsHost, "/News/ArticleInfo/wxdetail/");
        //    //        }
        //    //    case "NSC_CALENDAR":
        //    //        {

        //    //            var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventKey)
        //    //                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //    //                .Take(6).ToList();
        //    //            return CreateResponseNews(articles, _newsHost, "/News/ArticleInfo/wxdetail/");
        //    //        }
        //    //    case "NSC_REMINDER":
        //    //        {
        //    //            var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventKey)
        //    //                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //    //                .Take(6).ToList();
        //    //            return CreateResponseNews(articles, _newsHost, "/NSC/ArticleInfo/wxdetail/");
        //    //        }
        //    //    case "NSC_LILLYCHINA_MISSION_VISION":
        //    //        {
        //    //            var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventKey)
        //    //                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //    //                .Take(6).ToList();
        //    //            return CreateResponseNews(articles, _newsHost, "/NSC/ArticleInfo/wxdetail/");
        //    //        }
        //    //    case "NSC_ELCEC_WORD":
        //    //        {
        //    //            var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventKey)
        //    //                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //    //                .Take(6).ToList();
        //    //            return CreateResponseNews(articles, _newsHost, "/NSC/ArticleInfo/wxdetail/");
        //    //        }
        //    //    case "NSC_LIVE":
        //    //        {
        //    //            var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventKey)
        //    //                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //    //                .Take(6).ToList();
        //    //            return CreateResponseNews(articles, _newsHost, "/NSC/ArticleInfo/wxdetail/");
        //    //        }
        //    //    case "NSC_OUR_NSC":
        //    //        {
        //    //            var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventKey)
        //    //                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //    //                .Take(6).ToList();
        //    //            return CreateResponseNews(articles, _newsHost, "/NSC/ArticleInfo/wxdetail/");
        //    //        }
        //    //    case "NSC_MOMENT_OF_NSC":
        //    //        {
        //    //            var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventKey)
        //    //                .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //    //                .Take(6).ToList();
        //    //            return CreateResponseNews(articles, _newsHost, "/NSC/ArticleInfo/wxdetail/");
        //    //        }
        //    //}
        //    return null;
        //}

        ///// <summary>
        ///// Menu request for Sales and Marketing Communication App
        ///// </summary>
        ///// <param name="eventKey"></param>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //private IResponseMessageBase ProcessClickRequest15(string eventKey, string userId, int appId)
        //{
        //    //if (!SmartphoneMenuPermissionManager.PermissionCheck(eventKey, userId, appId))
        //    //{
        //    //    var message = CreateResponseMessage<ResponseMessageText>();
        //    //    message.Content = "对不起，您没有访问该菜单的权限!";
        //    //    return message;
        //    //}
        //    wr.CallUserBehavior(eventKey, userId, "15", "");

        //    switch (eventKey)
        //    {

        //        //case "SM_SALES_SPP":
        //        //    {

        //        //        var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1501)
        //        //            .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //        //            .Take(6).ToList();
        //        //        return CreateResponseSalesTraining(articles, _videoHost, "/SPP/ArticleInfo/wxdetail/");
        //        //    }
        //        //case "SM_SALES_BONUS":
        //        //    {

        //        //        var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1502)
        //        //            .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //        //            .Take(6).ToList();
        //        //        return CreateResponseSalesTraining(articles, _videoHost, "/SPP/ArticleInfo/wxdetail/");
        //        //    }
        //        case "SM_SALES_VEEVA":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1503)
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SPP/ArticleInfo/wxdetail/");
        //            }
        //        case "SM_SALES_RAPID":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1504)
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SPP/ArticleInfo/wxdetail/");
        //            }
        //        //modify by summer 2015-12-7
        //        //广播站
        //        case "SM_SALES_OTHER":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1505)
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SPP/ArticleInfo/wxdetail/");
        //            }
        //        case "SM_MARKET_HCP":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1511)
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SPP/ArticleInfo/wxdetail/");
        //            }
        //        case "SM_MARKET_SPEAKER":
        //            {


        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1512)
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SPP/ArticleInfo/wxdetail/");
        //            }

        //        case "SM_MARKET_VEEVA":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1514)
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SPP/ArticleInfo/wxdetail/");
        //            }
        //        case "SM_MARKET_FAQ":
        //            {

        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1513)
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SPP/ArticleInfo/wxdetail/");
        //            }
        //        case "SM_MARKET_OTHER":
        //            {
        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1515)
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SPP/ArticleInfo/wxdetail/");
        //            }
        //        default:
        //            return null;
        //    }
        //}

        ///// <summary>
        ///// Menu request for SalesTraining App
        ///// </summary>
        ///// <param name="eventKey"></param>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //private IResponseMessageBase ProcessClickRequest16(string eventKey, string userId)
        //{
        //    wr.CallUserBehavior(eventKey, userId, "16", "");
        //    switch (eventKey)
        //    {
        //        case "ST_RCOUR_VIEW":   // 核心课程概览
        //            {
        //                var articles =
        //                    _articleInfoService.GetList<ArticleInfoView>(
        //                        t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1601)
        //                        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                        .Take(6).ToList();
        //                return CreateResponse(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_RTRA_BAA":     // 培训前后
        //            {
        //                var articles =
        //                    _articleInfoService.GetList<ArticleInfoView>(
        //                        t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1602)
        //                        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                        .Take(6).ToList();
        //                return CreateResponse(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_RMICRO_FORUM":     // 微论坛
        //            {
        //                var articles =
        //                    _articleInfoService.GetList<ArticleInfoView>(
        //                        t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1603)
        //                        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                        .Take(6).ToList();
        //                return CreateResponse(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_RMICRO_SHARE":     // 微分享
        //            {
        //                var articles =
        //                    _articleInfoService.GetList<ArticleInfoView>(
        //                        t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1604)
        //                        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                        .Take(6).ToList();
        //                return CreateResponse(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_RLEARN_TYRANT":    // 学霸风采
        //            {
        //                var articles =
        //                    _articleInfoService.GetList<ArticleInfoView>(
        //                        t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1605)
        //                        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                        .Take(6).ToList();
        //                return CreateResponse(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_ECOUR_VIEW":       // 核心课程概览
        //            {
        //                var articles =
        //                    _articleInfoService.GetList<ArticleInfoView>(
        //                        t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1611)
        //                        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                        .Take(6).ToList();
        //                return CreateResponse(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_ETRA_BAA":     // 培训前后
        //            {
        //                var articles =
        //                    _articleInfoService.GetList<ArticleInfoView>(
        //                        t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1612)
        //                        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                        .Take(6).ToList();
        //                return CreateResponse(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_EMICRO_FORUM": // 微论坛
        //            {
        //                var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1608)
        //                    .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                    .Take(6).ToList();

        //                return CreateResponseSalesTraining(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_EMICRO_SHARE":     // 微分享
        //            {
        //                var articles =
        //                    _articleInfoService.GetList<ArticleInfoView>(
        //                        t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1613)
        //                        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                        .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_TRAIN_CALA":      // 培训月历
        //            {
        //                var articles =
        //                    _articleInfoService.GetList<ArticleInfoView>(
        //                        t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1621)
        //                        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                        .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_RDPAC_GUIDE":  // RDPAC指南
        //            {
        //                var articles =
        //                    _articleInfoService.GetList<ArticleInfoView>(
        //                        t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1622)
        //                        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                        .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_LEARN_GUIDE":  // My Learning指南
        //            {
        //                var articles =
        //                    _articleInfoService.GetList<ArticleInfoView>(
        //                        t => t.ArticleStatus == "Published" && t.ArticleCateSub == 1623)
        //                        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //                        .Take(6).ToList();
        //                return CreateResponseSalesTraining(articles, _videoHost, "/SalesTraining/ArticleInfo/wxdetail/");
        //            }
        //        case "ST_CONNECT_US":   // 联系我们
        //            {
        //                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //                responseMessage.Content = "联系我们的具体信息";

        //                return responseMessage;
        //            }
        //        default:
        //            return null;
        //    }

        //}

        /// <summary>
        /// Menu request for HRES APP
        /// </summary>
        /// <param name="eventKey"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        //private IResponseMessageBase ProcessClickRequest17(string eventKey, string userId)
        //{
        //    switch (eventKey)
        //    {
        //        case "HRES_SALARY_VACATION":
        //            {
        //                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //                responseMessage.Content = @"薪酬详情快速查询，休假出勤便捷管理，随时随地轻松登录，有效安排个人工作。<a href=""http://chinahw.lilly.com/"">薪酬休假管理平台</a>";

        //                return responseMessage;
        //            }
        //        case "HRES_WELFARE":
        //            {
        //                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //                responseMessage.Content = @"弹性福利明细，常用表单查询。您专属的福利管家，一键登录，乐享生活。<a href=""http://chinaflexbenefits.lilly.com/"">弹性福利平台</a>";
        //                return responseMessage;
        //            }
        //        case "HRES_STUDY":
        //            {
        //                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //                responseMessage.Content = @"我的学习我做主，使用电脑一键轻松登录学习平台。如用其他设备登录，只需输入您的用户名和密码，随时学习，so easy~<a href=""http://training.lilly.com/"">myLearning</a>";
        //                return responseMessage;
        //            }
        //        case "HRES_PERFORMANCE":
        //            {
        //                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //                responseMessage.Content = @"myPM可以帮助优秀的你，业绩表现更上一层楼。使用电脑一键轻松登录学习平台。如用其他设备登录，只需输入您的用户名和密码，随时学习，so easy~<a href=""http://mypm.lilly.com/"">myPM</a>";
        //                return responseMessage;
        //            }
        //        case "HRES_AWARD":
        //            {
        //                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //                // responseMessage.Content = @"<a href="""">奖励系统</a>。使用公司电脑将一键登录，若使用私人电脑或手机，请使用您的系统账号、密码登陆以上网址查询相关明细。";
        //                responseMessage.Content = "礼来全球员工奖励平台即将开启精彩之旅，敬请期待！全球化，无国界，视频互动，实时分享，用新颖有趣的方式表达感谢，用真诚的语言传递鼓励和认可，为小伙伴们点赞加油！";
        //                return responseMessage;
        //            }
        //        default:
        //            {
        //                var articles = GetPushArticles(eventKey);
        //                return CreateResponse(articles, _hresHost, "/News/ArticleInfo/wxdetail/");
        //            }

        //    }
        //}

        private static string GetToken(int appId)
        {
            var config = WeChatCommonService.GetWeChatConfig(appId);
            return AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
        }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
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


    }

    public class SmartphoneMenuPermissionManager
    {
        private static readonly ICacheManager _cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(SmartphoneMenuPermissionManager)));
        private static readonly ILogger log = LogManager.GetLogger(typeof(SmartphoneMenuPermissionManager));

        /// <summary>
        /// 
        /// </summary>
        //private static readonly IList<MenuGroup> _menuGroups = new List<MenuGroup> { new MenuGroup
        //{
        //    //SPP Communicate
        //    AppId = 15,
        //    MenuGroups = new List<SmartphoneMenuGroup>
        //    {
        //        new SmartphoneMenuGroup
        //        {
        //           GroupName = "SALES",
        //           Menus = new List<string>{"SM_SALES_SPP","SM_SALES_BONUS","SM_SALES_VEEVA","SM_SALES_RAPID","SM_SALES_OTHER"},
        //           //NeedTags = new List<WinXinTagEnum>{WinXinTagEnum.SF_DM,WinXinTagEnum.SF_NSD,WinXinTagEnum.SF_RSD,WinXinTagEnum.SF_REP,WinXinTagEnum.SPP_OPERATION},
        //           ErrorMessage = "对不起，您没有访问该菜单的权限!"
        //           },
        //           new SmartphoneMenuGroup
        //           {
        //               GroupName = "MARKET",
        //               Menus = new List<string>{"SM_MARKET_HCP","SM_MARKET_SPEAKER","SM_MARKET_RAPID","SM_MARKET_VEEVA","SM_MARKET_OTHER"},
        //               //NeedTags = new List<WinXinTagEnum>{WinXinTagEnum.SPP_MKT},
        //               ErrorMessage = "对不起，您没有访问该菜单的权限!"
        //           }
        //    }
        //},new MenuGroup
        //{
        //    //sales training
        //    AppId = 16,
        //    MenuGroups = new List<SmartphoneMenuGroup>
        //    {
        //        new SmartphoneMenuGroup
        //        {
        //            GroupName = "representation",
        //            Menus = new List<string>{"ST_RCOUR_VIEW","ST_RTRA_BAA","ST_RMICRO_FORUM","ST_RMICRO_SHARE","ST_RLEARN_TYRANT"},
        //            //NeedTags = new List<WinXinTagEnum>{WinXinTagEnum.SF_RSD,WinXinTagEnum.SF_DM,WinXinTagEnum.SF_REP,WinXinTagEnum.SF_NSD,WinXinTagEnum.SF_Trainer},
        //            ErrorMessage = "您如果需要了解代表的培训信息,请联系培训部!"
        //        },
        //        new SmartphoneMenuGroup
        //        {
        //            GroupName = "manger",
        //            Menus = new List<string>{"ST_ECOUR_VIEW","ST_ETRA_BAA","ST_EMICRO_FORUM","ST_EMICRO_SHARE"},
        //            //NeedTags = new List<WinXinTagEnum>{WinXinTagEnum.SF_DM,WinXinTagEnum.SF_RSD,WinXinTagEnum.SF_NSD,WinXinTagEnum.SF_Trainer},
        //            ErrorMessage = "如果您需要了解地区经理的相关信息,请联系您的主管!"
        //        }
        //    }
        //},new MenuGroup
        //{
        //    AppId = 18,
        //    MenuGroups = new List<SmartphoneMenuGroup>
        //    {
        //        new SmartphoneMenuGroup
        //        {
        //            GroupName = "sales",
        //            Menus = new List<string>{"SALES_NEWS","SALES_PERFORMANCE"},
        //            //NeedTags = new List<WinXinTagEnum>{WinXinTagEnum.HR,WinXinTagEnum.SV}
        //        },
        //        new SmartphoneMenuGroup
        //        {
        //            GroupName = "manager",
        //            Menus = new List<string>{"PERFORMANCE"},
        //            //NeedTags = new List<WinXinTagEnum>{WinXinTagEnum.HR}
        //        }
        //    }

        //}};

        public static bool PermissionCheck(string selectedMenu, string userId, int appId, out string errorMessage)
        {
            log.Debug("Entering PermissionCheck");
            errorMessage = null;
            //IList<MenuGroup> menuGroups;
            var categories = Infrastructure.Web.Domain.Service.CommonService.lstCategory;

            #region
            //try
            //{
            //    menuGroups = getAppMenuMapping();
            //}
            //catch (Exception ex)
            //{
            //    errorMessage = "服务器端发生异常,请联系管理员检查日志进行排查!";
            //    log.Error(ex, ex.StackTrace);
            //    return false;
            //}
            #endregion

            if (categories == null || categories.Count == 0)
            {
                //log.Debug("还没有在数据库中配置手机菜单权限的对应关系!");
                //errorMessage = "对不起,因为管理员还未分配菜单权限,您无法访问该菜单!";
                return true;
            }

            var apps = categories.Where(x => x.AppId == appId).ToList();
            if (apps.Count == 0)
            {
                return true;
            }

            log.Debug(string.Format("app id:{0}", appId));

            log.Debug(string.Format("click menu key:{0}", selectedMenu));

            var app = apps.Where(x => !string.IsNullOrEmpty(x.CategoryCode)).FirstOrDefault(a => a.CategoryCode.Equals(selectedMenu, StringComparison.CurrentCultureIgnoreCase));
            if (app == null || string.IsNullOrEmpty(app.Role))
            {
                return true;
            }

            var needTags = app.Role.Split(',');//.Where(x => x.Menus.Any(m => string.Compare(m, selectedMenu, StringComparison.CurrentCultureIgnoreCase) == 0)).SelectMany(x => x.NeedTags).ToList();

            if (!needTags.Any())
            {
                return true;
            }
            log.Debug(string.Format("need tags:{0}", needTags.Select(x => x).ToList().Join("   ||   ")));

            var tags = GetTagList(appId);

            //log.Debug(string.Format("all tags:{0}", tags.taglist.));
            //log.Debug("all tags(from cache):{0}", Environment.NewLine);
            //tags.taglist.ForEach(tag => log.Debug("tag id:{0},tag name:{1}", tag.tagid, tag.tagname));
            //log.Debug("===================================================");

            //needTags.Select(tag => (int)tag);
            var tagIds = tags.taglist.Where(x => needTags.Any(nt => nt == x.tagname)).Select(x => int.Parse(x.tagid)).ToList();
            log.Debug("need tag ids:{0}", tagIds.Join(" | "));

            var result = FindTargetMember(tagIds, userId, appId);

            if (!result)
            {
                errorMessage = app.NoRoleMessage;
                //menuGroups.First(x => x.AppId == appId).MenuGroups.First(y => y.Menus.Any(m => string.Compare(m, selectedMenu, StringComparison.CurrentCultureIgnoreCase) == 0)).ErrorMessage;
            }
            return FindTargetMember(tagIds, userId, appId);
        }

        private static GetTagListResult GetTagList(int appId)
        {
            return _cacheManager.Get("SmartphoneMenuPermissionManager.tagList", () => MailListApi.GetTagList(WeChatCommonService.GetWeiXinToken(appId)));
        }

        private static GetTagMemberResult GetTagMembers(int tagId, int appId)
        {
            //return _cacheManager.Get("SmartphoneMenuPermissionManager.Tagmember", () => MailListApi.GetTagMember(GetToken(), int.Parse(tagId)));
            return MailListApi.GetTagMember(WeChatCommonService.GetWeiXinToken(appId), tagId);
        }

        private static bool FindTargetMember(IEnumerable<int> tagIds, string userId, int appId)
        {
            log.Debug("current user id:{0}", userId);
            var user = WeChatCommonService.lstUser.First(x => x.userid == userId);

            return tagIds.Any(tagId =>
            {
                var tagMembers = GetTagMembers(tagId, appId);
                //log.Debug("members under current tag id:{0}", tagId);
                //tagMembers.ForEach(m => log.Debug("member id:{0},name:{1}", m.userid, m.name));
                return tagMembers.userlist.Any(x => x.userid == userId) || tagMembers.partylist.Any(y => user.department.Any(d => y == d));
            });
        }

        [Obsolete]
        private static IList<MenuGroup> getAppMenuMapping()
        {
            var configs = Infrastructure.Web.Domain.Service.CommonService.lstSysConfig;
            if (configs == null || configs.Count == 0)
            {
                return null;
            }

            var mappingConfig = configs.FirstOrDefault(x => x.ConfigCode == 15);
            return mappingConfig != null ? JsonHelper.FromJson<IList<MenuGroup>>(mappingConfig.ConfigValue) : null;
        }
    }

    public class SmartphoneMenuGroup
    {
        public string GroupName { get; set; }

        public IList<string> Menus { get; set; }

        public IList<string> NeedTags { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class MenuGroup
    {
        public int AppId { get; set; }

        public IList<SmartphoneMenuGroup> MenuGroups { get; set; }
    }

}