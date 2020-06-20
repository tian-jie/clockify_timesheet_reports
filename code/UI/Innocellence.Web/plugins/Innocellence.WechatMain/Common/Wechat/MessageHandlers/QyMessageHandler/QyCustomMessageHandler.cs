/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：QyCustomMessageHandler.cs
    文件功能描述：自定义QyMessageHandler
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Utility;
using Infrastructure.Utility.Data;
using Innocellence.Weixin.CommonService.QyMessageHandler;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.Entities;
using Innocellence.Weixin.QY.MessageHandlers;
using Infrastructure.Core.Logging;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Common;
using Infrastructure.Utility.Extensions;
using Infrastructure.Web.Domain.Service;
using System.Web.Configuration;
using System.Net;
using Innocellence.Weixin.QY.CommonAPIs;
using Innocellence.WeChat.Domain.Entity;
using Infrastructure.Web.ImageTools;
using System.Web.Hosting;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.Entities;
using System.Text;
using ResponseMessageBase = Innocellence.Weixin.Entities.ResponseMessageBase;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.ViewModel;
using System.Text;
using Innocellence.WeChat.Domain.Contracts;
using Newtonsoft.Json;
using Innocellence.Weixin.Helpers;
using Innocellence.Weixin.CommonService.MessageHandlers;
using System.Transactions;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.UI;
using Newtonsoft.Json.Linq;

namespace Innocellence.Weixin.CommonService.QyMessageHandlers
{
    public class QyCustomMessageHandler : QyMessageHandler<QyCustomMessageContext>
    {
        private IArticleInfoService _articleInfoService = EngineContext.Current.Resolve<IArticleInfoService>();
        private IWeChatAppUserService _weChatAppUserService = EngineContext.Current.Resolve<IWeChatAppUserService>();// new WeChatAppUserService();
        private IBatchJobLogService _batchJobLogService = EngineContext.Current.Resolve<IBatchJobLogService>();// new BatchJobLogService();
        private IAutoReplyContentService _autoReplyContentService = EngineContext.Current.Resolve<IAutoReplyContentService>();// new AutoReplyContentService();
        private IAttachmentsItemService _attachmentsItemService = EngineContext.Current.Resolve<IAttachmentsItemService>();//new AttachmentsItemService();

        private IAutoReplyContentService _AutoReplyContentService = EngineContext.Current.Resolve<IAutoReplyContentService>();

        private string _newsHost = Infrastructure.Web.Domain.Service.CommonService.GetSysConfig("WeChatUrl", "");


        private static object objLockUser = new object();


        //TODO
        private ArticleImagesService _articleImagesService = new ArticleImagesService();
        private IAddressBookService _addressBookServie = EngineContext.Current.Resolve<IAddressBookService>();
        private const string USER_IMAGE_SAVE_FOLDER_PATH = @"/wxUserImage/";

        // private bool _isDebug = false;

        private ILogger log { get { return LogManager.GetLogger("WeChat"); } }

        public QyCustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, bool isDebug = false)
            : base(inputStream, postModel, maxRecordCount, isDebug)
        {
            log.Debug("maxRecordCount is" + maxRecordCount);
            // this.actAfterGetData = AfterGetData;

            // this.actAfterDecryptData = AfterDecryptData;

            //log = LogManager.GetLogger("WeChat");
            //_isDebug = isDebug;

            log.Debug("Entering QyCustomMessageHandler debug: " + isDebug.ToString());
        }

        public override void AfterGetData(EncryptPostData objPostData, PostModel objPostModel)
        {
            log.Debug("Entering AfterGetData0 EncryptPostData AgentID:{0} PostModel AppId:{1}", objPostData.AgentID, objPostModel.AppId);

            log.Debug("Entering AfterGetData1 EncryptPostData Msg_Signature:{0} PostModel Timestamp:{1}  Nonce:{2}",
                objPostModel.Msg_Signature, objPostModel.Timestamp, objPostModel.Nonce);
            // log.Debug("objPostData is null?{0} ", objPostData==null);

            var objConfig = Innocellence.WeChat.Domain.Common.WeChatCommonService.GetWeChatConfigByID(int.Parse(objPostModel.AppId));
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


        #region Event
        //  static WebRequestPost _wr = new WebRequestPost();
        /// <summary>
        /// Event事件类型请求之CLICK
        /// </summary>
        public override List<IResponseMessageBase> OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            log.Debug("Entering Event事件类型请求之CLICK - " + requestMessage.EventKey);
            log.Debug("Request from :{0}, to :{1}", requestMessage.FromUserName, requestMessage.ToUserName);
            // 记录用户行为
            //string errorMessage;
            //// add permission check when user click the menu on winxin side
            //if (!SmartphoneMenuPermissionManager.PermissionCheck(requestMessage.EventKey, requestMessage.FromUserName, requestMessage.AgentID, out errorMessage))
            //{
            //    return NoPermissionResponse(errorMessage);
            //}

            // 记录用户行为
            // _wr.CallUserBehavior(requestMessage.EventKey, requestMessage.FromUserName, requestMessage.AgentID.ToString(), "", "", 4);


            //log.Debug("Entering default2");

            return CreateResponseMessage(requestMessage.EventKey, (int)AutoReplyKeywordEnum.MENU);

        }

        //private List<ArticleInfoView> GetPushArticles(string eventkey, int takeCount = 6)
        //{
        //    var articles = _articleInfoService.GetList<ArticleInfoView>(t => t.ArticleStatus == "Published" && t.ArticleCateSub == eventkey)
        //        .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
        //        .Take(takeCount).ToList();
        //    return articles;
        //}

        private List<IResponseMessageBase> NoPermissionResponse(string errorMessage)
        {
            var message = CreateResponseMessage<ResponseMessageText>();
            message.Content = errorMessage;//"对不起，您没有访问该菜单的权限!";
            return new List<IResponseMessageBase>() { message };
        }

        /// <summary>
        /// 关注事件。关注时需要导入用户信息到用户表。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override List<IResponseMessageBase> OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            log.Debug("request user id:{0} current app id:{1}", requestMessage.FromUserName, requestMessage.AgentID);

            var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.WeixinCorpId == requestMessage.ToUserName);

            var userId = requestMessage.FromUserName;


            lock (objLockUser) //不lock的话，会插入多次
            {
                var user = _addressBookServie.GetMemberByUserId(userId);

                //同步用户
                WeChatCommonService.SyncUserFromWechat(userId, ref user, config);
            }

            log.Debug("update user {0} success.", userId);

            //清除缓存
            WeChatCommonService.ClearMemberLst(config.AccountManageId.Value);


            ////企业号没有关注回复
            //var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            ////   var objConfig = WeChatCommonService.GetWeChatConfigByID(config.Id);
            //responseMessage.Content = config.WelcomeMessage;
            //return new List<IResponseMessageBase>() { responseMessage };
            return CreateResponseMessage(null, (int)AutoReplyKeywordEnum.ALL, requestMessage.FromUserName);
        }

        public override List<IResponseMessageBase> OnEvent_UnSubscribeRequest(RequestMessageEvent_UnSubscribe requestMessage)
        {
            log.Debug("request user id:{0} current app id:{1}", requestMessage.FromUserName, requestMessage.AgentID);
            var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.WeixinCorpId == requestMessage.ToUserName);
            //微信后台修改可见范围时会触发其子应用对应的取消关注事件
            //因此我们认为只有当企业小助手的取消关注事件触发时, 用户才对整个企业号取消关注
            if (config != null && "企业小助手".Equals(config.AppName))
            {
                var userId = requestMessage.FromUserName;
                var user = _addressBookServie.GetMemberByUserId(userId);
                if (user != null)
                {
                    user.Status = 4;
                    var columnList = new List<string> { "Status" };
                    _addressBookServie.Repository.Update(user, columnList);
                }
                log.Debug("update user {0} success.", userId);
                //清除缓存
                WeChatCommonService.ClearMemberLst(config.AccountManageId.Value);

                _weChatAppUserService.Repository.Delete(u => u.WeChatUserID == userId && u.WeChatAppID == requestMessage.AgentID);
            }

            return base.OnEvent_UnSubscribeRequest(requestMessage);
        }

        //private List<IResponseMessageBase> CreateResponse(List<ArticleInfoView> articles, string host, string baseUrl)
        //{
        //    var responseMessage = this.CreateResponseMessage<ResponseMessageNews>();
        //    responseMessage.Articles = new List<Article>();
        //    foreach (var a in articles)
        //    {
        //        string imgUrl;

        //        if (a.ThumbImageId == null)
        //        {
        //            log.Error("Not found Image!");
        //            imgUrl = _newsHost + "/Content/img/LogoRed.png";
        //        }
        //        else
        //        {
        //            imgUrl = string.Format("{0}{1}", _newsHost, a.ImageCoverUrl);
        //        }

        //        var newArticle = new Article()
        //        {
        //            Title = a.ArticleTitle,
        //            Url = host + baseUrl + a.Id,
        //            PicUrl = imgUrl,
        //            Description = a.ArticleComment
        //        };
        //        log.Debug("Creating News - \r\n\tTitle: " + newArticle.Title + "\r\n\tUrl: " + newArticle.Url + "\r\n\tPush Image: " + newArticle.PicUrl);

        //        responseMessage.Articles.Add(newArticle);
        //    }
        //    return new List<IResponseMessageBase>() { responseMessage };
        //}

        public override List<IResponseMessageBase> OnEvent_EnterAgentRequest(RequestMessageEvent_Enter_Agent requestMessage)
        {
            log.Debug("用户进入应用事件 - AppID:" + requestMessage.AgentID);


            // 先判断是否第一次进入。
            // 从_weChatAppUser中获取当前用户
            string userId = requestMessage.FromUserName;
            bool isFirstJoin = false;


            //如果设置了首次进入就直接回复
            var ret = CreateResponseMessage(null, (int)AutoReplyKeywordEnum.FirstEnterEvent);
            if (ret != null && ret.Count > 0)
            {
                var existingUser = _weChatAppUserService.Repository.Entities.Where(a => a.WeChatUserID == userId && a.WeChatAppID == requestMessage.AgentID).FirstOrDefault();

                CategoryType appType = (CategoryType)requestMessage.AgentID;
                isFirstJoin = (existingUser == null || existingUser == default(Innocellence.WeChat.Domain.Entity.WeChatAppUser));
                // 如果是第一次进入，需要将用户插入到数据库中
                if (isFirstJoin)
                {
                    log.Debug("First enter WeChatAppUser - WeChatAppID=" + appType.ToDescription());
                    // 将用户信息插入到数据库中
                    _weChatAppUserService.Repository.Insert(new Innocellence.WeChat.Domain.Entity.WeChatAppUser()
                    {
                        WeChatAppID = requestMessage.AgentID,
                        WeChatUserID = userId
                    });

                    return ret;
                }
            }


            //如果没有设置首次进入
            return CreateResponseMessage(null, (int)AutoReplyKeywordEnum.EnterEvent); ;
        }

        public override List<IResponseMessageBase> DefaultResponseMessage(IRequestMessageBase requestMessage)
        {

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "这是一条没有找到合适回复信息的默认消息。";

            //responseMessage.Content = "暂不支持自动回复。";

            //return null;
            return new List<IResponseMessageBase>() { responseMessage };
        }

        public override List<IResponseMessageBase> OnTextRequest(RequestMessageText requestMessage)
        {

            //记录用户行为
            // _wr.CallUserBehavior("TEXT_EVENT", requestMessage.FromUserName, requestMessage.AgentID.ToString(),
            //    requestMessage.Content, "", 8);
            log.Debug("handle text requst :{0}", requestMessage.Content);
            log.Debug("Request from :{0}, to :{1}", requestMessage.FromUserName, requestMessage.ToUserName);
            try
            {
                return CreateResponseMessage(requestMessage.Content, (int)AutoReplyKeywordEnum.TEXT);
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return CreateNoResponseMessage(requestMessage.AgentID);
        }

        public override List<IResponseMessageBase> OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            log.Debug("Latitude: {0}, Longitude :{1}, Precision:{2}", requestMessage.Latitude, requestMessage.Longitude, requestMessage.Precision);
            //return CreateResponseMessage(string.Empty, (int)AutoReplyKeywordEnum.LOCATION); to do
            return DefaultResponseMessage(requestMessage);
        }



        public override List<IResponseMessageBase> OnEvent_ScancodePushRequest(RequestMessageEvent_Scancode_Push requestMessage)
        {
            log.Debug("requestMessage :{0}", requestMessage.EventKey);
            log.Debug("scan result :{0}, type :{1}", requestMessage.ScanCodeInfo.ScanResult, requestMessage.ScanCodeInfo.ScanType);
            return CreateResponseMessage(AutoReplyMenuEnum.SCAN_PUSH_EVENT.ToString(), (int)AutoReplyKeywordEnum.MENU);
        }

        public override List<IResponseMessageBase> OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            log.Debug("requestMessage :{0}", requestMessage.EventKey);
            log.Debug("scan result :{0}, type :{1}", requestMessage.ScanCodeInfo.ScanResult, requestMessage.ScanCodeInfo.ScanType);
            return CreateResponseMessage(AutoReplyMenuEnum.SCAN_WITH_PROMPT.ToString(), (int)AutoReplyKeywordEnum.MENU);
        }

        public override List<IResponseMessageBase> OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            //  _wr.CallUserBehavior("Pic_Photo_Or_Album", requestMessage.FromUserName, requestMessage.AgentID.ToString(), requestMessage.EventKey, "", 8);

            log.Debug("NO Photo behavior ");
            return CreateNoResponseMessage(requestMessage.AgentID);
        }

        public override List<IResponseMessageBase> OnEvent_BatchJobResultRequest(RequestMessageEvent_Batch_Job_Result requestMessage)
        {
            log.Debug("RequestMessageEvent_Batch_Job_Result Enter.jobid:{0}", requestMessage.BatchJob.JobId);
            if (requestMessage.BatchJob != null && requestMessage.BatchJob.ErrCode == 0)
            {
                //根据返回的JobId查导入的Result.
                object ret = null;

                var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.WeixinCorpId == requestMessage.ToUserName);

                switch (requestMessage.BatchJob.JobType)
                {
                    case "sync_user":
                    case "replace_user":
                        var ret1 = MailListApi.BatchJobStatus<List<JobResultObjectUser>>(GetToken(config.Id), requestMessage.BatchJob.JobId);
                        ret = ret1.result.OrderByDescending(t => t.errcode).ToList();
                        break;
                    case "invite_user":
                        var ret2 = MailListApi.BatchJobStatus<List<JobResultObjectInvite>>(GetToken(config.Id), requestMessage.BatchJob.JobId);
                        ret = ret2.result.OrderByDescending(t => t.errcode).ToList();
                        break;
                    case "replace_party":
                        var ret3 = MailListApi.BatchJobStatus<List<JobResultObjectParty>>(GetToken(config.Id), requestMessage.BatchJob.JobId);
                        ret = ret3.result.OrderByDescending(t => t.errcode).ToList();
                        break;
                }
                string json = JsonHelper.ToJson(ret);
                //errocode为0代表异步任务成功 更新表状态
                var jobLog = _batchJobLogService.GetLogByJobId(requestMessage.BatchJob.JobId);
                if (jobLog != null)
                {
                    jobLog.Result = json;
                    jobLog.Status = 1;
                    _batchJobLogService.Repository.Update(jobLog);
                }
                else
                {
                    log.Error("BatchJob 不存在！jobid:{0}", requestMessage.BatchJob.JobId);
                }

            }
            else
            {
                string msg = requestMessage.BatchJob == null ? "BatchJobInfo is null" : requestMessage.BatchJob.ErrMsg;
                log.Debug("BatchUser Result Call Back - Error Msg={0}", msg);
            }

            return new List<IResponseMessageBase>() { new ResponseMessageBase() };
        }

        #endregion

        #region 消息处理
        public List<IResponseMessageBase> CreateResponseMessage(string inputStr, int type, string userId = null)
        {
            int appId = int.Parse(_postModel.AppId);
            log.Debug("keyword :{0}, type :{1}.", inputStr, type);
            List<IResponseMessageBase> responseMsgList = new List<IResponseMessageBase>();
            var contents = _autoReplyContentService.GetList<AutoReplyContentView>(appId, inputStr, type);
            if (null != contents && contents.Count > 0)
            {
                foreach (var content in contents)
                {
                    IResponseMessageBase responseMsg = null;
                    try
                    {
                        switch (content.PrimaryType)
                        {
                            case (int)AutoReplyContentEnum.LINK: //回复a标签,其实就是Text
                                responseMsg = CreateLinkResponseMessage(content);
                                break;
                            case (int)AutoReplyContentEnum.TEXT:
                                responseMsg = CreateTextResponseMessage(content);
                                break;
                            case (int)AutoReplyContentEnum.VOICE:
                                responseMsg = CreateVoiceResponseMessage(content);
                                break;
                            case (int)AutoReplyContentEnum.VIDEO:
                                responseMsg = CreateVideoResponseMessage(content);
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
                        //if (type == (int)AutoReplyKeywordEnum.SubscribeEvent && content.UserTags != null && content.UserTags.Count > 0)
                        //{
                        //    if (ValidateUserNeedAddToTag(content.AutoReplyId, userId))
                        //    {
                        //        AddUserToTag(content.UserTags, userId);
                        //    }
                        //}
                    }
                    catch (Exception e)
                    {
                        log.Error(e, "Lost some auto reply content.");

                    }
                    if (responseMsg != null)
                    {
                        //保密消息相关的设置
                        if (responseMsg is ResponseMessageBaseWechat)
                        {
                            (responseMsg as ResponseMessageBaseWechat).isSafe = (content.IsEncrypt.HasValue ? content.IsEncrypt.Value : false);
                            (responseMsg as ResponseMessageBaseWechat).APPID = appId;
                            log.Debug("Response safe :{0}", (responseMsg as ResponseMessageBaseWechat).isSafe);
                        }

                        log.Debug("Response from :[{0}]; to :[{1}]", responseMsg.FromUserName, responseMsg.ToUserName);
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
                //responseMsgList.Add(this.CreateResponseMessage<ResponseMessageText>());
                return responseMsgList;
            }
        }

        /// <summary>
        /// 在组装FilterGroup时,对于嵌套多层的Group不做处理
        /// 目前只当做只有一层Group,所有过滤条件均在Rules中
        /// 且所有Rule之间为And关系
        /// Department, TagList与User的属性之间取交集
        /// </summary>
        /// <param name="autoReplyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool ValidateUserNeedAddToTag(int? autoReplyId, string userId)
        {
            try
            {
                bool result = false;
                if (null != autoReplyId && !string.IsNullOrWhiteSpace(userId))
                {
                    log.Debug("autoReply:{0}, fromUser:{1}", autoReplyId, userId);
                    AutoReplyKeywordService keywordService = new AutoReplyKeywordService();
                    var keyword = keywordService.Repository.Entities.FirstOrDefault(k => k.AutoReplyId == autoReplyId);
                    if (null != keyword && !string.IsNullOrWhiteSpace(keyword.Keyword))
                    {
                        log.Debug("Keyword:{0}", keyword.Keyword);
                        FilterGroup map = JsonConvert.DeserializeObject<FilterGroup>(keyword.Keyword);
                        if (null != map && null != map.Rules)
                        {
                            List<FilterGroup> listFilter = new List<FilterGroup>();
                            ReAssembleFilterGroup(map, listFilter);
                            log.Debug("map rule count:{0}, filters count:{1}", map.Rules.Count, listFilter.Count);
                            var expression = FilterHelper.GetExpression<SysAddressBookMember>(map);
                            expression = expression.AndAlso(u => u.UserId.Equals(userId));
                            AddressBookService addressBookService = new AddressBookService();
                            var user = addressBookService.Repository.Entities.FirstOrDefault(expression);
                            if (null != user)
                            {
                                log.Debug("match user:{0}", user.UserName);
                                #region user符合除了Department与TagList以外的条件
                                result = true;
                                foreach (var group in listFilter)
                                {
                                    foreach (var rule in group.Rules)
                                    {
                                        bool hasValue = CheckUserValueAndKeyword(rule, user);
                                        //只需要检查And关系
                                        if (group.Operate == FilterOperate.And)
                                        {
                                            result = result && hasValue;
                                        }
                                    }
                                }
                                #endregion
                            }
                            else//暂时用不到
                            {
                                log.Debug("no match user.");
                                #region 如果user不符合, 且Department与TagList与其他条件是Or 的关系
                                foreach (var group in listFilter)
                                {
                                    //只需要检查Or的关系
                                    if (group.Operate == FilterOperate.Or)
                                    {
                                        user = addressBookService.GetMemberByUserId(userId, true);
                                        if (null != user)
                                        {
                                            foreach (var rule in group.Rules)
                                            {
                                                bool hasValue = CheckUserValueAndKeyword(rule, user);
                                                result = result || hasValue;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return false;
        }

        private bool CheckUserValueAndKeyword(FilterRule rule, SysAddressBookMember user)
        {
            if (null != rule && null != user)
            {
                List<string> valueInUser = null;
                JArray keywordArray = null;
                switch (rule.Field)
                {
                    case "Department":
                        if (!string.IsNullOrWhiteSpace(user.Department))
                        {
                            valueInUser = JsonConvert.DeserializeObject<List<string>>(user.Department);
                            keywordArray = rule.Value as JArray;
                        }
                        break;
                    case "TagList":
                        if (!string.IsNullOrWhiteSpace(user.TagList))
                        {
                            valueInUser = JsonConvert.DeserializeObject<List<string>>(user.TagList);
                            keywordArray = rule.Value as JArray;
                        }
                        break;
                    default:
                        return false;
                }
                return ValidateUserValueAndKeyword(valueInUser, keywordArray);
            }
            return false;
        }

        private bool ValidateUserValueAndKeyword(List<string> valueInUser, JArray keywordArray)
        {
            if (null != valueInUser && null != keywordArray)
            {
                List<string> keywordList = new List<string>();
                foreach (var item in keywordArray)
                {
                    keywordList.Add(item.Value<string>());
                }
                return valueInUser.Intersect(keywordList).Count() > 0;
            }
            return false;
        }

        private void ReAssembleFilterGroup(FilterGroup map, List<FilterGroup> listFilter)
        {
            CheckFieldInRuleAndRemoveRule(map, listFilter, "Department", "TagList");
            if (map.Groups != null && map.Groups.Count > 0)
            {
                foreach (var item in map.Groups)
                {
                    ReAssembleFilterGroup(item, listFilter);
                }
            }
        }

        private void CheckFieldInRuleAndRemoveRule(FilterGroup map, List<FilterGroup> listFilter, params string[] args)
        {
            if (null != map && null != listFilter && null != args)
            {
                foreach (var fieldName in args)
                {
                    if (map.Rules.Any(f => f.Field.Equals(fieldName)))
                    {
                        FilterGroup departmentFilter = new FilterGroup()
                        {
                            Rules = new List<FilterRule>() { map.Rules.FirstOrDefault(f => f.Field.Equals(fieldName)) },
                            Operate = map.Operate,
                        };
                        listFilter.Add(departmentFilter);
                        map.Rules.Remove(map.Rules.FirstOrDefault(f => f.Field.Equals(fieldName)));
                    }
                }
            }
        }

        private bool CheckEquals(object x, object y)
        {
            if (null != x && null != y)
            {
                //忽略类型不同,存在Int32与Int64的问题,因此选择直接ToString比较字符串
                //if (x.GetType().Equals(y))
                //{
                return x.ToString().Equals(y.ToString(), StringComparison.OrdinalIgnoreCase);
                //}
            }
            return false;
        }

        private void AddUserToTag(List<int> userTags, string userId)
        {
            AddressBookService _addressBookService = new AddressBookService();
            string accessToken = GetToken();
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                var result = 0;
                StringBuilder sb = new StringBuilder();
                foreach (var tagId in userTags)
                {
                    _addressBookService.addMemberTag(userId, tagId);
                    var ret = MailListApi.AddTagMember(accessToken, tagId, new string[] { userId });
                    result += (int)ret.errcode;
                    if (result != 0)
                    {
                        sb.AppendLine(ret.errmsg);
                    }
                }
                if (result == (int)ReturnCode_QY.请求成功)
                {
                    transactionScope.Complete();
                }
                else
                {
                    throw new Exception(sb.ToString());
                }
            }
        }

        private string GetToken()
        {
            var config = WeChatCommonService.GetWeChatConfigByID(int.Parse(_postModel.AppId));
            if (null != config)
            {
                AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取视频回复
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private IResponseMessageBase CreateVideoResponseMessage(AutoReplyContentView content)
        {
            var videoMsg = this.CreateResponseMessage<ResponseMessageVideo>();

            videoMsg.Video = CustomMessageCommon.CreateVideoResponseMessage(content, _postModel.CorpId);

            //if (content.IsNewContent.Value)
            //{
            //    var video = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content)[0];
            //    videoMsg.Video = new Video()
            //    {
            //        Title = video.NewsTitle,
            //        Description = video.NewsComment,
            //        MediaId = GetMediaIDByNewsInfo(video, content.Id),
            //    };
            //}
            //else
            //{
            //    if (content.FileID != null)
            //    {

            //        NewsInfoView news;
            //        Innocellence.WeChatMain.Common.WechatCommon.GetMediaIDByFileID(content.FileID, _attachmentsItemService, _postModel.CorpId, out news);

            //        videoMsg.Video = new Video()
            //        {
            //            Title = news.NewsTitle,
            //            Description = news.NewsComment,
            //            MediaId = news.MediaId,
            //        };


            //    }
            //    else
            //    {
            //        throw new Exception("do not found file id.");
            //    }
            //}
            return videoMsg;
        }

        /// <summary>
        /// 获取语音回复
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private IResponseMessageBase CreateVoiceResponseMessage(AutoReplyContentView content)
        {
            var voiceMsg = this.CreateResponseMessage<ResponseMessageVoice>();
            //if (content.IsNewContent.Value)
            //{
            //    var voice = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content)[0];
            //    voiceMsg.Voice = new Voice() { MediaId = GetMediaIDByNewsInfo(voice, content.Id) };
            //}
            //else
            //{
            //    NewsInfoView news;
            //    voiceMsg.Voice = new Voice() { MediaId = Innocellence.WeChatMain.Common.WechatCommon.GetMediaIDByFileID(content.FileID, _attachmentsItemService, _postModel.CorpId, out news) };
            //}

            voiceMsg.Voice = CustomMessageCommon.CreateVoiceResponseMessage(content, _postModel.CorpId);
            return voiceMsg;
        }

        /// <summary>
        /// 获取图片回复
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private IResponseMessageBase CreateImageResponseMessage(AutoReplyContentView content)
        {
            var imageMsg = this.CreateResponseMessage<ResponseMessageImage>();
            //if (content.IsNewContent.Value)
            //{
            //    var image = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content)[0];
            //    imageMsg.Image = new Image() { MediaId = GetMediaIDByNewsInfo(image, content.Id) };
            //}
            //else
            //{
            //    NewsInfoView news;
            //    imageMsg.Image = new Image() { MediaId = Innocellence.WeChatMain.Common.WechatCommon.GetMediaIDByFileID(content.FileID, _attachmentsItemService, _postModel.CorpId, out news) };
            //}

            imageMsg.Image = CustomMessageCommon.CreateImageResponseMessage(content, _postModel.CorpId);

            return imageMsg;
        }

        ///// <summary>
        ///// 根据content获取回复内容，判断文件是否过期，过去需要重新传
        ///// </summary>
        ///// <param name="News"></param>
        ///// <param name="iId"></param>
        ///// <returns></returns>
        //private string GetMediaIDByNewsInfo(NewsInfoView News, int iId)
        //{
        //    if (DateTimeHelper.GetDateTimeFromXml(News.MediaCreateTime).AddDays(3) < DateTime.Now)
        //    {

        //        log.Debug("GetMediaIDByNewsInfo start:  cate:{0}  AutoReplyID:{1}", News.NewsCate, iId);


        //        Innocellence.WeChatMain.Common.WechatCommon.GetMediaInfo((AutoReplyContentEnum)Enum.Parse(typeof(AutoReplyContentEnum), News.NewsCate.ToUpper()), News, News.AppId);

        //        var content = JsonConvert.SerializeObject(new List<NewsInfoView>() { News });
        //        AutoReplyContent model = new AutoReplyContent()
        //        {
        //            Id = iId,
        //            Content = content
        //        };

        //        _AutoReplyContentService.Repository.Update(model, new List<string>() { "Content" });

        //        return News.MediaId;
        //    }
        //    else
        //    {
        //        return News.MediaId;
        //    }


        //}




        private IResponseMessageBase CreateNewsResponseMessage(AutoReplyContentView content, int appId)
        {

            if (content.IsEncrypt.HasValue && content.IsEncrypt.Value)
            {
                var photoTextMsg = this.CreateResponseMessage<ResponseMessageMpNews>();
                var list = CustomMessageCommon.CreateNewsResponseMessage(content, appId, true, content.IsEncrypt.HasValue ? content.IsEncrypt.Value : false, true);
                photoTextMsg.MpNewsArticles = list.Select(c => (MpNewsArticle)c).ToList();
                return photoTextMsg;
            }
            else
            {
                var photoTextMsg = this.CreateResponseMessage<ResponseMessageNews>();
                var list = CustomMessageCommon.CreateNewsResponseMessage(content, appId, true, content.IsEncrypt.HasValue ? content.IsEncrypt.Value : false, true);
                photoTextMsg.Articles = list.Select(c => (Article)c).ToList();
                return photoTextMsg;
            }



            //photoTextMsg.Articles = new List<Article>();
            //if (content.IsNewContent.Value)
            //{
            //    var info = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content)[0];
            //    var configs = Infrastructure.Web.Domain.Service.CommonService.lstSysConfig;
            //    var config = configs.Where(a => a.ConfigName.Equals("Content Server", StringComparison.CurrentCultureIgnoreCase)).First();
            //    var contentConfig = configs.Where(a => a.ConfigName.Equals("Content Server", StringComparison.CurrentCultureIgnoreCase)).First();
            //    string host = config.ConfigValue;
            //    if (host.EndsWith("/"))
            //    {
            //        host = host.Substring(0, host.Length - 1);
            //    }
            //    string contentHost = contentConfig.ConfigValue;
            //    if (contentHost.EndsWith("/"))
            //    {
            //        contentHost = contentHost.Substring(0, contentHost.Length - 1);
            //    }
            //    var picUrl = contentHost + info.ImageSrc;
            //    //var url = host + "/News/ArticleInfo/wxdetail/" + content.AutoReplyId + "?wechatid=" + appId;// "&subId=" + item.Id;
            //    var url = host + "/News/Message/GetNews?id=" + content.AutoReplyId + "&wechatid=" + appId + "&type=" + (int)NewsTypeEnum.AutoReply;
            //    var newArticle = new Article()
            //    {
            //        Title = info.NewsTitle,
            //        Url = url,
            //        PicUrl = picUrl,
            //        Description = info.NewsComment
            //    };
            //    photoTextMsg.Articles.Add(newArticle);
            //}
            //else
            //{
            //    List<ArticleInfoView> articleList = new List<ArticleInfoView>();
            //    if (content.SecondaryType == (int)AutoReplyNewsEnum.MANUAL)
            //    {
            //        List<int> articleIds = content.NewsID.Trim(',').Split(',').ToList().Select(n => int.Parse(n)).ToList();
            //        if (articleIds.Count > 0)
            //        {
            //            log.Debug("article count :{0}.", articleIds.Count);
            //            articleList.AddRange(_articleInfoService.GetList<ArticleInfoView>(t => t.AppId == appId && t.ArticleStatus == "Published" && articleIds.Contains(t.Id)));
            //        }
            //    }
            //    else if (content.SecondaryType == (int)AutoReplyNewsEnum.LATEST)
            //    {
            //        articleList.AddRange(_articleInfoService.GetList<ArticleInfoView>(t => t.AppId == appId && t.ArticleStatus == "Published")
            //            .OrderBy("PublishDate", System.ComponentModel.ListSortDirection.Descending)
            //            .Take(int.Parse(content.Content)).ToList());
            //    };
            //    articleList = articleList.Distinct().OrderByDescending(t => t.PublishDate).ToList();
            //    foreach (var a in articleList)
            //    {
            //        var newArticle = new Article()
            //        {
            //            Title = a.ArticleTitle,
            //            Url = _newsHost + "/News/ArticleInfo/wxdetail/" + a.Id + "?wechatid=" + appId,
            //            PicUrl = _newsHost + a.ImageCoverUrl,
            //            Description = a.ArticleComment
            //        };
            //        log.Debug("Creating News - \r\n\tTitle: " + newArticle.Title + "\r\n\tUrl: " + newArticle.Url + "\r\n\tPush Image: " + newArticle.PicUrl);
            //        photoTextMsg.Articles.Add(newArticle);
            //    }
            //}


        }

        private IResponseMessageBase CreateLinkResponseMessage(AutoReplyContentView content)
        {
            var testMsg = this.CreateResponseMessage<ResponseMessageText>();
            testMsg.Content = CustomMessageCommon.CreateLinkResponseMessage(content);
            return testMsg;
        }


        private IResponseMessageBase CreateTextResponseMessage(AutoReplyContentView content)
        {
            var testMsg = this.CreateResponseMessage<ResponseMessageText>();
            //if (content.IsNewContent.Value)
            //{
            //    var info = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content);
            //    testMsg.Content = info[0].NewsContent;
            //}
            //else
            //{
            //    testMsg.Content = content.Content;
            //}

            testMsg.Content = CustomMessageCommon.CreateTextResponseMessage(content);

            log.Debug("Create text response :{0}", content);
            return testMsg;
        }

        public override List<IResponseMessageBase> OnImageRequest(RequestMessageImage requestMessage)
        {
            //记录用户行为
            //  _wr.CallUserBehavior("IMAGE_EVENT", requestMessage.FromUserName, requestMessage.AgentID.ToString(), requestMessage.MediaId, "", 8);

            //log.Debug("OnImageRequest", requestMessage.);
            log.Debug("OnImageRequest :{0}  {1}", requestMessage.MediaId, requestMessage.PicUrl);
            if (requestMessage.AgentID == (int)CategoryType.NSCCate || requestMessage.AgentID == 20)
            {
                SaveImage(requestMessage);
            }


            return CreateResponseMessage(string.Empty, (int)AutoReplyKeywordEnum.IMAGE);

            //Dictionary<string, Stream> dic = new Dictionary<string, Stream>();
            //{
            //    var stream = System.IO.File.OpenRead(requestMessage.PicUrl);
            //    dic.Add("test", stream);
            //}

            //var result = MediaApi.Upload(GetToken(requestMessage.AgentID), QY.UploadMediaFileType.image, dic, "");

            //log.Debug("result:" + result.media_id);
            //log.Debug("result error code :" + result.errcode);
            //log.Debug("result error msg :" + result.errmsg);
            var response = this.CreateResponseMessage<ResponseMessageImage>();
            response.Image = new Image() { MediaId = requestMessage.MediaId };
            return new List<IResponseMessageBase>() { response };
            //只为默认回复.
            return CreateResponseMessage(string.Empty, (int)AutoReplyKeywordEnum.IMAGE);

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "感谢您的参与。";

            return new List<IResponseMessageBase>() { responseMessage };

        }

        private void SaveImage(RequestMessageImage requestMessage)
        {
            log.Debug("Begin SaveImage from Wechat Client");

            //WebClient webclient = new WebClient();
            //var picData = await webclient.DownloadDataTaskAsync(requestMessage.PicUrl);
            //string picType = requestMessage.PicUrl.Substring(requestMessage.PicUrl.LastIndexOf('.') + 1);

            //var img = ImageHelper.BytesToImage(picData);

            var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.WeixinCorpId == requestMessage.ToUserName);

            using (MemoryStream stream = new MemoryStream())
            {

                log.Debug("MediaApi.Get");
                MediaApi.Get(GetToken(config.Id), requestMessage.MediaId, stream);
                stream.Seek(0, SeekOrigin.Begin);

                CustomMessageCommon.SaveImage(stream, config.Id, requestMessage.FromUserName);

            }
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    try
            //    {
            //        log.Debug("MediaApi.Get");
            //        MediaApi.Get(GetToken(config.Id), requestMessage.MediaId, stream);
            //        stream.Seek(0, SeekOrigin.Begin);

            //        //var imageBytes = stream.ToArray();
            //        //var imageFromUser = ImageHelper.BytesToImage(imageBytes);

            //        log.Debug("ImageUtility.MakeThumbnail");

            //        string filename = USER_IMAGE_SAVE_FOLDER_PATH + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
            //        string strPath = MapPath("/" + filename);

            //        ImageUtility.MakeThumbnail(null, stream, strPath, 900, 0, "W", 0, true);
            //        //thumbnailImageFromUser.Save(strPath);

            //        //log.Debug("thumbnailImageFromUser is NULL? - " + (thumbnailImageFromUser == null));

            //        log.Debug("new ArticleImages()");
            //        var articleImage = new ArticleImages()
            //        {
            //            AppId = requestMessage.AgentID,
            //            CreatedUserID = requestMessage.FromUserName,
            //            ImageContent = System.IO.File.ReadAllBytes(strPath),
            //            CreatedDate = DateTime.Now,
            //            ImageName = filename
            //        };

            //        log.Debug("_articleImagesService.Insert");
            //        _articleImagesService.Repository.Insert(articleImage);
            //    }
            //    catch (Exception ex)
            //    {
            //        log.Error(ex.Message + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace);
            //    }
            //}

            log.Debug("End SaveImage from Wechat Client");
        }

        public override List<IResponseMessageBase> OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            //记录用户行为
            //  _wr.CallUserBehavior("VOICE_EVENT", requestMessage.FromUserName, requestMessage.AgentID.ToString(), requestMessage.MediaId, "", 8);
            log.Debug("NO VOICE behavior :{0}", requestMessage.Format);
            return CreateResponseMessage(requestMessage.MediaId, (int)AutoReplyKeywordEnum.AUDIO);
        }

        public override List<IResponseMessageBase> OnLocationRequest(RequestMessageLocation requestMessage)
        {
            log.Debug("requestMessage x:{0}, y:{1} from user {2}", requestMessage.Location_X, requestMessage.Location_Y, requestMessage.FromUserName);
            return CreateResponseMessage(string.Empty, (int)AutoReplyKeywordEnum.LOCATION);
        }

        public override List<IResponseMessageBase> OnVideoRequest(RequestMessageVideo requestMessage)
        {
            return CreateResponseMessage(string.Empty, (int)AutoReplyKeywordEnum.VIDEO);
        }

        public override List<IResponseMessageBase> OnShortVideoRequest(RequestMessageShortVideo requestMessage)
        {
            return CreateResponseMessage(string.Empty, (int)AutoReplyKeywordEnum.VIDEO);
        }


        private List<IResponseMessageBase> CreateNoResponseMessage(int agentid)
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


            return new List<IResponseMessageBase>() { responseMessage };
        }


        #endregion

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


        private static string GetToken(int iWeChatID)
        {
            var config = WeChatCommonService.GetWeChatConfigByID(iWeChatID);
            return AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
        }

        ///// <summary>
        ///// Maps a virtual path to a physical disk path.
        ///// </summary>
        ///// <param name="path">The path to map. E.g. "~/bin"</param>
        ///// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        //public virtual string MapPath(string path)
        //{
        //    if (HostingEnvironment.IsHosted)
        //    {
        //        //hosted
        //        return HostingEnvironment.MapPath(path);
        //    }

        //    //not hosted. For example, run in unit tests
        //    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //    path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
        //    return Path.Combine(baseDirectory, path);
        //}


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

            var config = WeChatCommonService.GetWeChatConfigByID(appId);

            var user = WeChatCommonService.lstUser(config.AccountManageId.Value).First(x => x.userid == userId);

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