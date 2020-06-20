using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.ImageTools;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.Weixin;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.Exceptions;
using Innocellence.Weixin.Helpers;
using Innocellence.Weixin.QY;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.AdvancedAPIs.App;
using Innocellence.Weixin.QY.AdvancedAPIs.Mass;
using Innocellence.Weixin.QY.AdvancedAPIs.Media;
using Innocellence.Weixin.QY.CommonAPIs;
using Innocellence.Weixin.QY.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMain.Common
{
    public class WechatCommon
    {

        private static ILogger log { get { return LogManager.GetLogger("WeChat"); } }

        /// <summary>
        /// 微信图文推送
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="article"></param>
        /// <param name="touser"></param>
        /// <param name="toparty"></param>
        /// <param name="totag"></param>
        public static MassResult PublishMessage(int appid, List<ArticleInfoView> articleList, string touser, string toparty, string totag, bool isPreview = false)
        {
            MassResult objResult = null;
            //LogManager.GetLogger(typeof(WechatCommon)).Debug("Publishing Article - "
            //    + "\r\n\tTitle: " + article.Title
            //    + "\r\n\tUrl: " + article.Url
            //    + "\r\n\tPush Image: " + article.PicUrl
            //    + "\r\n\tPush To: *" + touser + "*,*" + toparty + "*,*" + totag + "*"
            //    );

            if (touser == "@all")
            {
                // 只要touser=="@all"了，就忽略其他参数
                objResult = WechatCommon.SendMsg(appid, "news", "@all", "", "", "", articleList);
            }
            else if (string.IsNullOrEmpty(toparty) && string.IsNullOrEmpty(totag))
            {
                // 选择的组 和 tag 都没选的话，看touser
                if (string.IsNullOrEmpty(touser))
                {
                    // user也没选的话，touser就是@all
                    touser = "@all";
                }
                objResult = WechatCommon.SendMsg(appid, "news", touser, "", "", "", articleList, isPreview);
            }
            else
            {
                // 否则的话，现在的情况是touser!="@all"，三个条件组合使用
                objResult = WechatCommon.SendMsg(appid, "news", touser, toparty, totag, "", articleList, isPreview);
            }

            return objResult;
        }

        public static MassResult SendMsg(int iAppID, string strMsgType, string strUser, string strDept, string strTags, string strContent, List<ArticleInfoView> lstContent, bool isPreview = false)
        {
            var objConfig = WeChatCommonService.GetWeChatConfigByID(iAppID);
            if (objConfig.IsCorp.HasValue && !objConfig.IsCorp.Value)
            {
                var ret = WechatCommonMP.SendMsg(strMsgType, strContent, lstContent, strTags, strUser.Split(','), null);

                return new MassResult() { errmsg = ret.errmsg, errcode = (ReturnCode_QY)ret.errcode };
            }
            else
            {
                int isSafe = 0;
                if (lstContent != null && lstContent.Count > 0)
                {
                    if (lstContent[0].NewsInfo != null && lstContent[0].NewsInfo.isSecurityPost.HasValue && lstContent[0].NewsInfo.isSecurityPost.Value)
                    {
                        isSafe = 1;
                    }
                }
                return SendMsgQY(iAppID, strMsgType, strUser, strDept, strTags, strContent, lstContent, isSafe, isPreview);
            }

        }
        public static MassResult SendMsgQY(int iAppID, string strMsgType, string strUser, string strDept, string strTags, string strContent, List<ArticleInfoView> lstContent, int IsSec, bool isPreview = false)
        {
            MassResult objResult = null;
            try
            {
                var objConfig = WeChatCommonService.GetWeChatConfigByID(iAppID);
                log.Warn("SendMsgQY strUser:{0} strDept:{1} strTags:{2} Msg:{3} APPID:{4} strMsgType:{5} IsSec:{6} MsgCount:{7}", strUser, strDept, strTags, strContent, iAppID, strMsgType, IsSec, lstContent.Count);
                string strToken = (objConfig.IsCorp != null && !objConfig.IsCorp.Value) ? Innocellence.Weixin.MP.CommonAPIs.AccessTokenContainer.GetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret) : AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);
                var news = lstContent[0].NewsInfo;
                //保密图文消息需要用mpnews发送
                strMsgType = strMsgType == "news" && IsSec == 1 ? "mpnews" : strMsgType;
                switch (strMsgType)
                {
                    case "text":
                        objResult = MassApi.SendText(strToken, strUser, strDept, strTags, objConfig.WeixinAppId.ToString(), lstContent[0].ArticleContent, IsSec);
                        break;
                    case "image":
                        news.MediaId = DoNewsInfo(news, objConfig, AutoReplyContentEnum.IMAGE, news.ImageContent);
                        objResult = MassApi.SendImage(strToken, strUser, strDept, strTags, objConfig.WeixinAppId.ToString(), news.MediaId, IsSec);
                        break;
                    case "news":
                        #region news
                        var wechatBaseUrl = CommonService.GetSysConfig("WeChatUrl", "");
                        var lstArticle = new List<Article>();

                        int i = 0;

                        foreach (var objModel in lstContent)
                        {
                            if (i == 0) //位置不同，缩略图的比例不一样
                            {
                                objModel.ImageCoverUrl = doGetFileCover(objModel.ImageCoverUrl, "_B");
                            }
                            else
                            {
                                objModel.ImageCoverUrl = doGetFileCover(objModel.ImageCoverUrl, "_T");
                            }
                            i++;
                            lstArticle.Add(new Article()
                            {
                                Title = objModel.ArticleTitle,
                                Description = objModel.ArticleComment,
                                // PicUrl = aiv.ThumbImageId == null ? wechatBaseUrl+"/Content/img/LogoRed.png" : string.Format("{0}/Common/PushFile?id={1}&FileName={2}", wechatBaseUrl, aiv.ThumbImageId, aiv.ThumbImageUrl),
                                //PicUrl = objModel.ImageCoverUrl == null ? wechatBaseUrl + "Content/img/LogoRed.png" : string.Format("{0}{1}", wechatBaseUrl, objModel.ImageCoverUrl),

                                PicUrl = string.Format("{0}{1}", wechatBaseUrl, objModel.ImageCoverUrl).Replace("/\\", "/").Replace("\\", "/"),
                                Url = string.Format("{0}/News/ArticleInfo/WxDetail/{1}?wechatid={2}&isPreview={3}", wechatBaseUrl, objModel.Id, objModel.AppId, isPreview)

                            });
                        }

                        //var lstArticle = lstContent.Select(objModel => new Article()
                        //{
                        //    Title = objModel.ArticleTitle,
                        //    Description = objModel.ArticleComment,
                        //    // PicUrl = aiv.ThumbImageId == null ? wechatBaseUrl+"/Content/img/LogoRed.png" : string.Format("{0}/Common/PushFile?id={1}&FileName={2}", wechatBaseUrl, aiv.ThumbImageId, aiv.ThumbImageUrl),
                        //    //PicUrl = objModel.ImageCoverUrl == null ? wechatBaseUrl + "Content/img/LogoRed.png" : string.Format("{0}{1}", wechatBaseUrl, objModel.ImageCoverUrl),



                        //    PicUrl = string.Format("{0}{1}", wechatBaseUrl, objModel.ImageCoverUrl).Replace("/\\", "/").Replace("\\", "/"),
                        //    Url = string.Format("{0}/News/ArticleInfo/WxDetail/{1}?wechatid={2}&isPreview={3}", wechatBaseUrl, objModel.Id, objModel.AppId, isPreview)

                        //}).ToList();
                        objResult = MassApi.SendNews(strToken, strUser, strDept, strTags, objConfig.WeixinAppId.ToString(), lstArticle, IsSec);
                        #endregion
                        break;
                    case "mpnews":
                        #region mpnews
                        var wechatBaseUrl1 = CommonService.GetSysConfig("WeChatUrl", "");
                        var lstMpArticle = new List<MpNewsArticle>();
                        int ii = 0;
                        foreach (var objModel in lstContent)
                        {
                            if (ii == 0) //位置不同，缩略图的比例不一样
                            {
                                objModel.ImageCoverUrl = doGetFileCover(objModel.ImageCoverUrl, "_B");
                            }
                            else
                            {
                                objModel.ImageCoverUrl = doGetFileCover(objModel.ImageCoverUrl, "_T");
                            }
                            ii++;
                            lstMpArticle.Add(new MpNewsArticle()
                            {
                                title = objModel.ArticleTitle,
                                digest = objModel.ArticleComment,
                                content = objModel.ArticleContent,
                                author = objModel.UpdatedUserID,
                                show_cover_pic = "1",
                                thumb_media_id = GetMediaId(objModel.ImageCoverUrl, strToken),

                                // PicUrl = aiv.ThumbImageId == null ? wechatBaseUrl+"/Content/img/LogoRed.png" : string.Format("{0}/Common/PushFile?id={1}&FileName={2}", wechatBaseUrl, aiv.ThumbImageId, aiv.ThumbImageUrl),
                                //PicUrl = objModel.ImageCoverUrl == null ? wechatBaseUrl + "Content/img/LogoRed.png" : string.Format("{0}{1}", wechatBaseUrl, objModel.ImageCoverUrl),
                                //  = string.Format("{0}{1}", wechatBaseUrl, objModel.ImageCoverUrl),
                                content_source_url = string.Format("{0}/News/ArticleInfo/WxDetail/{1}?wechatid={2}&isPreview={3}", wechatBaseUrl1, objModel.Id, objModel.AppId, isPreview)

                            });
                        }
                        objResult = MassApi.SendMpNews(strToken, strUser, strDept, strTags, objConfig.WeixinAppId.ToString(), lstMpArticle, IsSec);
                        #endregion
                        break;
                    case "video":
                        //WechatCommon.GetMediaInfo(AutoReplyContentEnum.VIDEO, news, news.AppId);
                        news.MediaId = DoNewsInfo(news, objConfig, AutoReplyContentEnum.VIDEO, news.VideoContent);
                        objResult = MassApi.SendVideo(strToken, strUser, strDept, strTags, objConfig.WeixinAppId.ToString(), news.MediaId, news.NewsTitle, news.NewsComment, IsSec);
                        //NewsToAttachments(news, "video", news.VideoContent);
                        break;
                    case "file":
                        // WechatCommon.GetMediaInfo(AutoReplyContentEnum.FILE, news, news.AppId);
                        news.MediaId = DoNewsInfo(news, objConfig, AutoReplyContentEnum.FILE, news.FileSrc);
                        objResult = MassApi.SendFile(strToken, strUser, strDept, strTags, objConfig.WeixinAppId.ToString(), news.MediaId, IsSec);
                        // NewsToAttachments(news, "file", news.FileSrc);
                        break;
                    case "voice":
                        // WechatCommon.GetMediaInfo(AutoReplyContentEnum.VOICE, news, news.AppId);
                        news.MediaId = DoNewsInfo(news, objConfig, AutoReplyContentEnum.VOICE, news.SoundSrc);
                        objResult = MassApi.SendVoice(strToken, strUser, strDept, strTags, objConfig.WeixinAppId.ToString(), news.MediaId, IsSec);
                        //NewsToAttachments(news, "voice", news.SoundSrc);
                        break;
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
            //在更新MsgLog时保持原来的错误处理, 即不进行任何处理
            finally
            {
                UpdateMsgLog(lstContent, objResult);
            }
            return objResult;
        }

        private static void UpdateMsgLog(List<ArticleInfoView> lstContent, MassResult sendResult)
        {
            try
            {
                if (lstContent != null)
                {
                    string newsIdList = string.Join(",", lstContent.Select(a => a.Id));
                    if (!string.IsNullOrEmpty(newsIdList))
                    {
                        MessageLogService msgLogService = new MessageLogService();
                        var needUpdateMsgLogs = msgLogService.Repository.Entities.Where(m => newsIdList.Equals(m.NewsIdList, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (needUpdateMsgLogs != null)
                        {
                            if (sendResult != null)
                            {
                                foreach (var needUpdateMsgLog in needUpdateMsgLogs)
                                {
                                    needUpdateMsgLog.SendMsgStatus = sendResult.errcode == ReturnCode_QY.请求成功 ? (int)SendMessageStatus.Success : (int)SendMessageStatus.Failed;
                                    needUpdateMsgLog.SendTotalMembers = GetAllMemberCount(lstContent);
                                }
                            }
                            else
                            {
                                needUpdateMsgLogs.ForEach(m => m.SendMsgStatus = (int)SendMessageStatus.Failed);
                            }
                            msgLogService.Repository.Update(needUpdateMsgLogs);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("更新消息列表失败");
                log.Error(e);
            }
        }

        private static int GetAllMemberCount(List<ArticleInfoView> lstContent)
        {
            int allMembersCount = 0;
            try
            {
                if (lstContent != null && lstContent.Count > 0)
                {
                    var lstArticles = lstContent[0];
                    List<string> allMember = new List<string>();
                    if (!string.IsNullOrEmpty(lstArticles.ToDepartment) || !string.IsNullOrEmpty(lstArticles.ToTag))
                    {
                        AddressBookService addressBookService = new AddressBookService();
                        var userInfos = addressBookService.Repository.Entities.Where(address => address.EmployeeStatus == "A" && address.DeleteFlag != 1).ToList();
                        if (!string.IsNullOrEmpty(lstArticles.ToUser))
                        {
                            allMember.AddRange(lstArticles.ToUser.Split(','));
                        }
                        if (!string.IsNullOrEmpty(lstArticles.ToDepartment))
                        {
                            var allDepartment = lstArticles.ToDepartment.Split(',');
                            foreach (var d in allDepartment)
                            {
                                var inDepartmentMem = userInfos.Where(a => a.Department.Replace("[", ",").Replace("]", ",").Contains("," + d + ",")).ToList();
                                allMember.AddRange(inDepartmentMem.Select(mem => mem.UserId));
                            }
                            log.Debug("Send to Department {0}", lstArticles.ToDepartment);
                        }
                        if (!string.IsNullOrEmpty(lstArticles.ToTag))
                        {
                            var allTag = lstArticles.ToTag.Split(',');
                            foreach (var t in allTag)
                            {
                                var inTagMem = userInfos.Where(a => a.TagList.Replace("[", ",").Replace("]", ",").Contains("," + t + ",")).ToList();
                                allMember.AddRange(inTagMem.Select(mem => mem.UserId));
                            }
                            log.Debug("Send to Tag {0}", lstArticles.ToTag);
                        }
                        allMembersCount = allMember.Distinct().Count();
                        log.Debug("Send to members {0}", allMembersCount);
                    }
                    else
                    {
                        allMembersCount = lstArticles.ToUser.Split(',').Count();
                        log.Debug("Only Send to user {0},Count{1}", lstArticles.ToUser, allMembersCount);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return allMembersCount;
        }

        public static string DoNewsInfo(NewsInfoView news, SysWechatConfig config, AutoReplyContentEnum type, string strFileName)
        {
            string mediaId = null;
            if (news.materialId.HasValue && news.materialId.Value > 0) //有素材
            {
                IAttachmentsItemService _attachmentsItemService = EngineContext.Current.Resolve<IAttachmentsItemService>();

                NewsInfoView news1;
                mediaId = WechatCommon.GetMediaIDByFileID(news.materialId, _attachmentsItemService, config.WeixinCorpId, out news1);

            }
            else
            {
                mediaId = WechatCommon.GetMediaInfo(type, news, news.AppId);

                NewsToAttachments(news, type.ToString().ToLower(), strFileName);
            }
            return mediaId;
        }

        public static string doGetFileCover(string strPath, string strPad)
        {
            string strTo = strPath.Insert(strPath.LastIndexOf('.'), strPad);

            //不存在就创建
            if (!File.Exists(HttpRuntime.AppDomainAppPath + strTo))
            {
                log.Debug("文件【{0}】不存在！正在重新生成", HttpRuntime.AppDomainAppPath + strTo);
                ImageUtility.MakeThumbnail(HttpRuntime.AppDomainAppPath + strPath, null, HttpRuntime.AppDomainAppPath + strTo, 450, 250, "Cut");
            }

            log.Debug("strTo:{0}", strTo);

            return strTo;
        }

        /// <summary>
        /// 消息内容存储到素材表
        /// </summary>
        /// <param name="news"></param>
        /// <param name="uploadFileType"></param>
        /// <param name="strFileName"></param>
        private static void NewsToAttachments(NewsInfoView news, string uploadFileType, string strFileName)
        {
            IAttachmentsItemService _attachmentsItemService = EngineContext.Current.Resolve<IAttachmentsItemService>();
            string strFullName = (HttpContext.Current == null ? HttpRuntime.AppDomainAppPath : HttpContext.Current.Request.PhysicalApplicationPath) + strFileName;

            AttachmentsItemPostProperty p = new AttachmentsItemPostProperty()
            {
                SaveFullName = Path.GetFileName(strFullName),
                ServerPath = HttpRuntime.AppDomainAppPath,
                FileName = news.NewsTitle,
                TargetFilePath = strFileName.Replace(Path.GetFileName(strFullName), "").Replace("//", "/").Trim('/'),
                UploadFileType = uploadFileType,
                AppId = news.AppId,
                Description = news.NewsComment,
                VideoCoverSrc = string.IsNullOrEmpty(news.ImageSrc) ? news.ImageContent : news.ImageSrc,
                UserName = _attachmentsItemService.Repository.LoginUserName,// User.Identity.Name,
                MediaId = news.MediaId,
                // MediaExpireTime=
                //ViewId = pid,
            };
            SysAttachmentsItem itemView = _attachmentsItemService.ThumbImageAndInsertIntoDB(p);
        }


        public static string GetMediaId(string strPath, string strToken)
        {
            // var saveFileName = Path.GetFileName(strPath.Replace("_t", "")); //不发缩略图
            // var targetFilePath = strPath.Substring(0, strPath.LastIndexOf(saveFileName));
            //  targetFilePath = targetFilePath.Trim('\\', '/');
            //var saveDir = Path.Combine(HttpContext.Current.Server.MapPath("~/"), targetFilePath);


            //   var fi = new FileInfo(HttpRuntime.AppDomainAppPath + strPath.Replace("_t", ""));
            var fi = new FileInfo(HttpRuntime.AppDomainAppPath + strPath);
            Dictionary<string, Stream> dic = new Dictionary<string, Stream>();

            dic.Add(fi.Name, fi.OpenRead());
            return MediaApi.Upload(strToken, UploadMediaFileType.image, dic, "", 50 * 10000).media_id;
        }
        /// <summary>
        /// 发送微信消息
        /// </summary>
        /// <param name="strMsgType">text,image,news</param>
        /// <param name="strContent"></param>
        /// <param name="lstContent"></param>
        /// <returns></returns>
        public static MassResult SendMsg(int iAppID, string strMsgType, string strContent, List<ArticleInfoView> lstContent)
        {
            //MassResult objResult = null;
            string strDept = "", strUser = "", strTags = "";
            //  int IsSec = 0;

            GetAppInfoResult Ret = GetUserOrDept(iAppID);

            Ret.allow_userinfos.user.Where(a => a.status == "1").ToList().ForEach(a => { strUser += "|" + a.userid; });
            if (strUser.Length > 0) { strUser = strUser.Substring(1); }
            strDept = string.Join(",", Ret.allow_partys.partyid);
            strTags = string.Join(",", Ret.allow_tags.tagid);

            return SendMsg(iAppID, strMsgType, strUser, strDept, strTags, strContent, lstContent);

        }

        /// <summary>
        /// 发送微信消息.自动
        /// </summary>
        /// <param name="iAppID"></param>
        /// <param name="strMsgType"></param>
        /// <param name="strContent"></param>
        /// <param name="lstContent"></param>
        /// <returns></returns>
        public static MassResult SendMsgToUser(int iAppID, string strMsgType, string strContent, List<ArticleInfoView> lstContent)
        {
            MassResult ret;
            var objConfig = WeChatCommonService.GetWeChatConfigByID(iAppID);
            if (objConfig.IsCorp.HasValue && objConfig.IsCorp.Value)
            {
                ret = SendMsg(iAppID, strMsgType, lstContent[0].ToUser.Replace(",", "|"), lstContent[0].ToDepartment.Replace(",", "|"), lstContent[0].ToTag.Replace(",", "|"), strContent, lstContent);
            }
            else
            {
                var retMP = WechatCommonMP.SendMsgMP(lstContent,
                    new WeChat.Domain.ViewModelFront.SearchUserMPView()
                    {
                        Group = lstContent[0].NewsInfo.Group,
                        City = lstContent[0].NewsInfo.City,
                        Province = lstContent[0].NewsInfo.Province,
                        Sex = lstContent[0].NewsInfo.Sex
                    }, null);
                ret = new MassResult() { errcode = (ReturnCode_QY)retMP.errcode, errmsg = retMP.errmsg };
            }

            return ret;

        }

        public static GetAppInfoResult GetUserOrDept(int iAppID)
        {
            var objConfig = WeChatCommonService.GetWeChatConfigByID(iAppID);

            string strToken = AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);

            return AppApi.GetAppInfo(strToken, int.Parse(objConfig.WeixinAppId));
            // Ret.allow_userinfos
        }

        //public static List<Article> SendNews(List<NewsInfo> lst)
        //{
        //    var lstContent = new List<Article>();
        //    Article art = new Article();
        //    NewsInfo news = lst.Find(a => a.LanguageCode == ConstData.LAN_CN);
        //    NewsInfo newsEN = lst.Find(a => a.LanguageCode == ConstData.LAN_EN);

        //    art.Title = newsEN.NewsTitle + news.NewsTitle;
        //    art.Description =newsEN.NewsContent+"\n"+news.NewsContent;
        //    art.PicUrl = string.Format("{0}Common/file?Id={1}&filename={2}&ImgType=1",
        //        WebConfigurationManager.AppSettings["WebUrl"], news.Id, news.ImageName);
        //    art.Url = string.Format("{0}NewsInfo/Detail?NewsCode={1}", WebConfigurationManager.AppSettings["WebUrl"], news.NewsCode);
        //   // art.Description = art.PicUrl;
        //    lstContent.Add(art);

        //    return lstContent;
        //}

        //public static List<Article> SendCourse(List<TrainingCourse> lst)
        //{
        //    var lstContent = new List<Article>();
        //    Article art = new Article();
        //    TrainingCourse news = lst.Find(a => a.LanguageCode == ConstData.LAN_CN);
        //    TrainingCourse newsEN = lst.Find(a => a.LanguageCode == ConstData.LAN_EN);

        //    art.Title = newsEN.CourseName + news.CourseName;
        //    art.Description = newsEN.CourseComment + "\n" + news.CourseComment;
        //    art.PicUrl = "Common/file?Id=" + news.Id + "&filename=/Content/img/Course1.png&ImgType=1";
        //    art.Url = string.Format("{0}/Course/Detail?CourseCode={1}", WebConfigurationManager.AppSettings["WebUrl"], news.CourseCode);
        //    lstContent.Add(art);

        //    return lstContent;
        //}

        /// <summary>
        /// 关注课程的人，提前一天提醒
        /// </summary>
        /// <returns></returns>
        public static bool AutoSendMsg()
        {
            //try
            //{
            //    TrainingCourseService ser = new TrainingCourseService();
            //    DateTime dt = DateTime.Now.Date;
            //    var lst = ser.GetListToday(dt.AddDays(-1), dt).Select(a =>
            //        new TrainingCourse
            //        {
            //            CourseCode = a.CourseCode,
            //            LanguageCode = a.LanguageCode,
            //            CourseName = a.CourseName,
            //            CourseComment = a.CourseComment,
            //            Id = a.Id,
            //            CreatedUserID = a.CreatedUserID
            //        }).GroupBy(a => a.CourseCode);


            //    foreach (var a in lst)
            //    {
            //        var lstContent = new List<TrainingCourse>();
            //        lstContent.AddRange(a.ToList());

            //        var lstSend = SendCourse(lstContent);

            //        // SendMsg(iAppID,"news", lstContent[0].CreatedUserID, "", "", null, lstSend);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogManager.GetLogger(typeof(WechatCommon)).Error("Auto SendMsg Error!", ex);
            //}
            return true;
        }


        public static string GetMediaInfo(AutoReplyContentEnum cate, NewsInfoView news, int iAppID)
        {
            string mediaId = null;
            string saveFileName = null;
            string targetFilePath = null;
            string saveDir = null;
            FileInfo fi = null;
            dynamic ret = null;


            //上次的素材还没过期
            if (!string.IsNullOrEmpty(news.MediaId) && news.MediaCreateTime > 0)
            {
                if (DateTimeHelper.GetDateTimeFromXml(news.MediaCreateTime).AddDays(3) > DateTime.Now)
                {
                    return news.MediaId;
                }
            }


            Dictionary<string, Stream> dic = new Dictionary<string, Stream>();

            var objConfig = WeChatCommonService.GetWeChatConfigByID(iAppID);

            bool isCrop = (objConfig.IsCorp == null || objConfig.IsCorp.Value);

            string strToken = (objConfig.IsCorp != null && !objConfig.IsCorp.Value) ? "" : Innocellence.Weixin.QY.CommonAPIs.AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);


            log.Debug("GetMediaInfo start:  cate:{0}  iAppID:{1}", cate, iAppID);

            switch (cate)
            {
                case AutoReplyContentEnum.IMAGE:
                    //saveFileName = Path.GetFileName(news.ImageContent.Replace("_t", "")); //不发缩略图
                    //targetFilePath = news.ImageContent.Substring(0, news.ImageContent.LastIndexOf(saveFileName));
                    //targetFilePath = targetFilePath.Trim('\\', '/');
                    //saveDir = Path.Combine(HttpContext.Current.Server.MapPath("~/"), targetFilePath);
                    //fi = new FileInfo(Path.Combine(saveDir, saveFileName));

                    fi = new FileInfo(HttpRuntime.AppDomainAppPath + news.ImageContent.Replace("_t", ""));

                    if (isCrop)
                    {
                        dic.Add(fi.Name, fi.OpenRead());
                        ret = MediaApi.Upload(strToken, UploadMediaFileType.image, dic, "", 50 * 10000);
                    }
                    else
                    {
                        ret = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret,
                            Innocellence.Weixin.MP.UploadMediaFileType.image, fi.OpenRead(), fi.FullName, 50 * 10000);
                    }

                    news.MediaId = ret.media_id;
                    mediaId = ret.media_id;
                    break;
                case AutoReplyContentEnum.VOICE:
                    //saveFileName = Path.GetFileName(news.SoundSrc);
                    //targetFilePath = news.SoundSrc.Substring(0, news.SoundSrc.LastIndexOf(saveFileName));
                    //targetFilePath = targetFilePath.Trim('\\', '/');
                    //saveDir = Path.Combine(HttpContext.Current.Server.MapPath("~/"), targetFilePath);
                    //fi = new FileInfo(Path.Combine(saveDir, saveFileName));

                    fi = new FileInfo(HttpRuntime.AppDomainAppPath + news.SoundSrc.Replace("_t", ""));

                    dic.Add(fi.Name, fi.OpenRead());
                    try
                    {
                        if (isCrop)
                        {
                            ret = MediaApi.Upload(strToken, UploadMediaFileType.voice, dic, "", 50 * 10000);
                        }
                        else
                        {
                            ret = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret,
                                Innocellence.Weixin.MP.UploadMediaFileType.voice, fi.OpenRead(),
                                fi.FullName, 50 * 10000);
                        }
                    }
                    catch (ErrorJsonResultException ex)
                    {
                        if (ex.JsonResult.errcode == Weixin.ReturnCode.语音播放时间超过限制)
                        {
                            throw new Exception("语音播放时间超过限制，请上传播放长度不超过60秒的语音文件。");
                        }
                        else
                        {
                            throw;
                        }
                    }
                    news.MediaId = ret.media_id;
                    mediaId = ret.media_id;
                    break;
                case AutoReplyContentEnum.VIDEO:
                    //saveFileName = Path.GetFileName(news.VideoContent);
                    //targetFilePath = news.VideoContent.Substring(0, news.VideoContent.LastIndexOf(saveFileName));
                    //targetFilePath = targetFilePath.Trim('\\', '/');
                    //saveDir = Path.Combine(HttpContext.Current.Server.MapPath("~/"), targetFilePath);
                    //fi = new FileInfo(Path.Combine(saveDir, saveFileName));

                    fi = new FileInfo(HttpRuntime.AppDomainAppPath + news.VideoContent.Replace("_t", ""));


                    if (isCrop)
                    {
                        dic.Add(fi.Name, fi.OpenRead());
                        ret = MediaApi.Upload(strToken, UploadMediaFileType.video, dic, "", 50 * 10000);
                    }
                    else
                    {
                        ret = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret,
                           Innocellence.Weixin.MP.UploadMediaFileType.video, fi.OpenRead(), fi.FullName, 50 * 10000);
                    }
                    news.MediaId = ret.media_id;
                    mediaId = ret.media_id;
                    break;
                case AutoReplyContentEnum.FILE:
                    //saveFileName = Path.GetFileName(news.FileSrc);
                    //targetFilePath = news.FileSrc.Substring(0, news.FileSrc.LastIndexOf(saveFileName));
                    //targetFilePath = targetFilePath.Trim('\\', '/');
                    //saveDir = Path.Combine(HttpContext.Current.Server.MapPath("~/"), targetFilePath);
                    //fi = new FileInfo(Path.Combine(saveDir, saveFileName));

                    fi = new FileInfo(HttpRuntime.AppDomainAppPath + news.FileSrc.Replace("_t", ""));
                    string extention = fi.Name.Substring(fi.Name.LastIndexOf('.'));
                    string fileName = string.IsNullOrEmpty(news.RealFileName) ? news.NewsTitle : news.RealFileName;
                    string displayName = fileName.EndsWith(extention) ? fileName : fileName + extention;
                    if (isCrop)
                    {
                        dic.Add(displayName, fi.OpenRead());
                        ret = MediaApi.Upload(strToken, UploadMediaFileType.file, dic, "", 50 * 10000);

                    }
                    else
                    {
                        //服务号暂时不支持推送文件.
                        //ret = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret, Innocellence.Weixin.MP.UploadMediaFileType.voice, fi.OpenRead(), Path.Combine(saveDir, saveFileName), 50 * 10000);
                    }
                    news.MediaId = ret.media_id;
                    mediaId = ret.media_id;
                    break;
                default:
                    break;
            }

            if (ret != null)
            {
                news.MediaCreateTime = ret.created_at;


                log.Debug("GetMediaInfo end:  media_id:{0} ", ret.media_id);
            }
            return mediaId;

        }


        /// <summary>
        /// 根据资源ID获取回复信息，如果过期重新上传
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        public static string GetMediaIDByFileID(int? fileID, IAttachmentsItemService _attachmentsItemService, string CorpId, out NewsInfoView news)
        {
            if (fileID == null)
            {
                throw new Exception("do not found file id.");
            }
            else
            {

                log.Debug("GetMediaIDByFileID start:   fileID:{0}", fileID);

                //
                var fileInfo = _attachmentsItemService.GetById<AttachmentsItemView>((int)fileID);

                if (fileInfo != null)
                {
                    news = new NewsInfoView()
                    {
                        NewsContent = "",
                        NewsTitle = fileInfo.AttachmentTitle,
                        ImageContent = fileInfo.AttachmentUrl,
                        VideoContent = fileInfo.AttachmentUrl,
                        FileSrc = fileInfo.AttachmentUrl,
                        SoundSrc = fileInfo.AttachmentUrl,
                        MediaId = fileInfo.MediaId,
                        NewsComment = fileInfo.Description
                    };
                }
                else
                {
                    news = new NewsInfoView();
                }

                if (fileInfo != null && (string.IsNullOrEmpty(fileInfo.MediaId) || !fileInfo.MediaExpireTime.HasValue || fileInfo.MediaExpireTime.Value < DateTime.Now))
                {
                    var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.WeixinCorpId == CorpId);


                    AutoReplyContentEnum cate = AutoReplyContentEnum.FILE;
                    if (fileInfo.Type == (int)AttachmentsType.IMAGE)
                    {
                        cate = AutoReplyContentEnum.IMAGE;
                        if (fileInfo.FileSize > 2 * 1024 * 1024)
                        {
                            throw new Exception("图片文件不能大于2M！");
                        }

                    }
                    else if (fileInfo.Type == (int)AttachmentsType.VIDEO)
                    {
                        cate = AutoReplyContentEnum.VIDEO;
                        if (fileInfo.FileSize > 10 * 1024 * 1024)
                        {
                            throw new Exception("视频文件不能大于10M！");
                        }
                    }
                    else if (fileInfo.Type == (int)AttachmentsType.AUDIO)
                    {
                        cate = AutoReplyContentEnum.VOICE;
                        if (fileInfo.FileSize > 2 * 1024 * 1024)
                        {
                            throw new Exception("声音文件不能大于210M！");
                        }
                    }
                    else if (fileInfo.Type == (int)AttachmentsType.FILE)
                    {
                        cate = AutoReplyContentEnum.FILE;
                        if (fileInfo.FileSize > 20 * 1024 * 1024)
                        {
                            throw new Exception("普通文件不能大于20M！");
                        }
                    }


                    news.NewsCate = cate.ToString().ToLower();
                    news.AppId = config.Id;


                    Innocellence.WeChatMain.Common.WechatCommon.GetMediaInfo(cate, news, config.Id);

                    _attachmentsItemService.UpdateMediaId(fileInfo.Id, news.MediaId, DateTimeHelper.GetDateTimeFromXml(news.MediaCreateTime));

                    log.Debug("GetMediaIDByFileID end  GetNewMediaId:   fileID:{0} MediaId:{1}", fileID, news.MediaId);

                    return news.MediaId;

                }
                else
                {
                    log.Debug("GetMediaIDByFileID end  Use Old MediaId:   fileID:{0}   MediaId:{1}", fileID, news.MediaId);

                    return fileInfo == null ? string.Empty : fileInfo.MediaId;
                }



            }
        }



    }
}