using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.ImageTools;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.WeChat.Domain.ViewModelFront;
using Innocellence.Weixin;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.MP;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.AdvancedAPIs.GroupMessage;
using Innocellence.Weixin.MP.CommonAPIs;
using Innocellence.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Innocellence.WeChatMain.Common
{
    public class WechatCommonMP
    {


        private static ILogger log = LogManager.GetLogger(typeof(WechatCommon));


        /// <summary>
        /// 微信图文推送
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="article"></param>
        /// <param name="touser"></param>
        /// <param name="toparty"></param>
        /// <param name="totag"></param>
        public static SendResult PublishMessage(int appid, List<ArticleInfoView> articleList, string[] touser, string totag, string[] PreviewOpenids)
        {
            return SendMsg("news", "", articleList, totag, touser, PreviewOpenids);
        }



        public static SendResult SendMsgMP(List<ArticleInfoView> lstContent, SearchUserMPView searchCondition, string[] PreviewOpenids /*Preview use*/, bool isKefu = false)
        {
            string strTags = ""; //ToAll
            string[] userOpenIds = null;

            if (searchCondition.Group == null)
            {
                searchCondition.Group = -2;
            }
            if (searchCondition.Sex < 0 && searchCondition.Province == "-1")
            {
                if (searchCondition.Group == -2)
                {
                    // ToAll
                    strTags = "-1";
                }
                else
                {
                    // Send By Tag
                    strTags = searchCondition.Group.ToString();
                }
            }
            else
            {
                // Send By OpenId
                IWechatMPUserService _WechatMPUserService = EngineContext.Current.Resolve<IWechatMPUserService>();
                var objConfig = WeChatCommonService.GetWeChatConfigByID(lstContent[0].AppId.Value);
                userOpenIds = _WechatMPUserService.GetUserBySearchCondition(searchCondition, objConfig.AccountManageId.Value).Select(u => u.OpenId).ToArray();
            }

            string strType = ((WechatMessageLogType)lstContent[0].ContentType).ToString();
            return SendMsg(strType, "", lstContent, strTags, userOpenIds, PreviewOpenids, isKefu);
        }

        /// <summary>
        /// 服务号通用的发送消息
        /// </summary>
        /// <param name="strMsgType"></param>
        /// <param name="strContent"></param>
        /// <param name="lstContent"></param>
        /// <param name="strTags"></param>
        /// <param name="strOpenids"></param>
        /// <param name="PreviewOpenids"></param>
        /// <returns></returns>
        public static SendResult SendMsg(string strMsgType,
                                  string strContent, List<ArticleInfoView> lstContent, string strTags, string[] strOpenids, string[] PreviewOpenids, bool isKefu = false /*是否客服消息*/)
        {
            SendResult returnResult = null;
            WxJsonResult retCustom = null;
            var objConfig = WeChatCommonService.GetWeChatConfigByID(lstContent[0].AppId.Value);
            //log.Debug("strUser:{0} strDept:{1} strTags:{2} Msg:{3} APPID:{4}", strUser, strDept, strTags, strContent, iAppID);
            //  string strToken = Innocellence.Weixin.MP.CommonAPIs.AccessTokenContainer.GetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);
            switch (strMsgType)
            {
                case "text":

                    returnResult = MPSendMessage(objConfig, lstContent[0].NewsInfo.NewsContent, Weixin.MP.GroupMessageType.text, strTags, strOpenids, PreviewOpenids, "", "", isKefu);
                    break;
                case "news": //因为历史原因，news和mpnews对应反了.此处实际就是mpnews业务

                    List<NewsModel> articles = new List<NewsModel>();
                    string host = CommonService.GetSysConfig("Content Server", "");
                    if (host.EndsWith("/"))
                    {
                        host = host.Substring(0, host.Length - 1);
                    }
                    for (int i = 0; i < lstContent.Count; i++)
                    {
                        var item = lstContent[i];
                        var filePath = "";
                        if (0 == i)
                        {
                            //不存在就创建
                            filePath = WechatCommon.doGetFileCover(item.ImageCoverUrl, "_B");
                        }else
                        {
                            filePath = WechatCommon.doGetFileCover( item.ImageCoverUrl, "_T");
                        }


                        var ret0 = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(objConfig.WeixinAppId, objConfig.WeixinCorpSecret, Weixin.MP.UploadMediaFileType.thumb, HttpRuntime.AppDomainAppPath+ filePath);
                        var picUrl = (host + filePath).Replace("\\", "/");
                        var url = string.Format("{0}/MPNews/ArticleInfo/WxDetail/{1}?wechatid={2}&isPreview={3}", host, item.Id, item.AppId, PreviewOpenids == null ? false : true);
                        articles.Add(new NewsModel() { title = item.ArticleTitle, content = WechatCommonMP.ImageConvert(item.ArticleContent, objConfig.Id), thumb_url = picUrl, thumb_media_id = ret0.thumb_media_id, content_source_url = url });
                    }
                    var ret = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryNews(objConfig.WeixinAppId, objConfig.WeixinCorpSecret, 10000, articles.ToArray());
                    returnResult = MPSendMessage(objConfig, ret.media_id, Weixin.MP.GroupMessageType.mpnews, strTags, strOpenids, PreviewOpenids, "", "", isKefu);

                    break;
                case "mpnews":  //因为历史原因，news和mpnews对应反了。此处实际就是news业务

                    List<Article> articles1 = new List<Article>();
                    string host1 = CommonService.GetSysConfig("Content Server", "");
                    if (host1.EndsWith("/"))
                    {
                        host1 = host1.Substring(0, host1.Length - 1);
                    }
                    for (int i = 0; i < lstContent.Count; i++)
                    {
                        var item = lstContent[i];
                        var filePath = "";
                        if (0 == i)
                        {
                            //不存在就创建
                            filePath = WechatCommon.doGetFileCover( item.ImageCoverUrl, "_B");
                        }
                        else
                        {
                            filePath = WechatCommon.doGetFileCover(item.ImageCoverUrl, "_T");
                        }

                        //  var ret0 = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(objConfig.WeixinAppId, objConfig.WeixinCorpSecret, Weixin.MP.UploadMediaFileType.thumb, filePath);
                        var picUrl = (host1 + filePath).Replace("\\", "/");
                        var url = string.Format("{0}/MPNews/ArticleInfo/WxDetail/{1}?wechatid={2}&isPreview={3}", host1, item.Id, item.AppId, PreviewOpenids == null ? false : true);
                        articles1.Add(new Article() { Title = item.ArticleTitle, PicUrl = picUrl, Url = url, Description = item.ArticleComment });
                    }
                    // var ret1 = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryNews(objConfig.WeixinAppId, objConfig.WeixinCorpSecret, 10000, articles1.ToArray());

                    // retCustom=SendMsgKF(strMsgType, objConfig,strOpenids, articles1);

                    WxJsonResult retCustom1;
                    foreach (var openId in strOpenids)
                    {
                        retCustom1 = CustomApi.SendNews(objConfig.WeixinAppId, objConfig.WeixinCorpSecret, openId, articles1);
                        if (retCustom1.errcode != ReturnCode.请求成功)
                        {
                            retCustom = retCustom1;
                        }
                        else if (retCustom == null)
                        {
                            retCustom = retCustom1;
                        }
                    }

                    returnResult = new SendResult() { errcode = retCustom.errcode, errmsg = retCustom.errmsg };
                    //  returnResult = MPSendMessage(objConfig, ret1.media_id, Weixin.MP.GroupMessageType.mpnews, strTags, strOpenids, PreviewOpenids, "", "");
                    break;
                case "image":
                    lstContent[0].NewsInfo.MediaId = WechatCommon.DoNewsInfo(lstContent[0].NewsInfo, objConfig, AutoReplyContentEnum.IMAGE, lstContent[0].NewsInfo.ImageContent);
                    //WechatCommon.GetMediaInfo(AutoReplyContentEnum.IMAGE, lstContent[0].NewsInfo, objConfig.Id);
                    returnResult = MPSendMessage(objConfig, lstContent[0].NewsInfo.MediaId, Weixin.MP.GroupMessageType.image, strTags, strOpenids, PreviewOpenids, "", "", isKefu);
                    break;
                case "video":
                    //WechatCommon.GetMediaInfo(AutoReplyContentEnum.VIDEO, lstContent[0].NewsInfo, lstContent[0].NewsInfo.AppId);
                    lstContent[0].NewsInfo.MediaId = WechatCommon.DoNewsInfo(lstContent[0].NewsInfo, objConfig, AutoReplyContentEnum.VIDEO, lstContent[0].NewsInfo.VideoContent);
                    if (!isKefu)
                    {
                        var ret2 = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.GetOpenIdVideoMediaIdResult(objConfig.WeixinAppId, objConfig.WeixinCorpSecret, lstContent[0].NewsInfo.MediaId, lstContent[0].NewsInfo.NewsTitle, lstContent[0].NewsInfo.NewsComment);
                        lstContent[0].NewsInfo.MediaId = ret2.media_id;
                        lstContent[0].NewsInfo.MediaCreateTime = ret2.created_at;
                    }
                    MPSendMessage(objConfig, lstContent[0].NewsInfo.MediaId, Weixin.MP.GroupMessageType.video, strTags, strOpenids, PreviewOpenids, lstContent[0].NewsInfo.NewsTitle, lstContent[0].NewsInfo.NewsComment, isKefu);
                    break;
                case "voice":
                    //WechatCommon.GetMediaInfo(AutoReplyContentEnum.VOICE, lstContent[0].NewsInfo, lstContent[0].NewsInfo.AppId);
                    lstContent[0].NewsInfo.MediaId = WechatCommon.DoNewsInfo(lstContent[0].NewsInfo, objConfig, AutoReplyContentEnum.VOICE, lstContent[0].NewsInfo.SoundSrc);
                    returnResult = MPSendMessage(objConfig, lstContent[0].NewsInfo.MediaId, Weixin.MP.GroupMessageType.voice, strTags, strOpenids, PreviewOpenids, "", "", isKefu);
                    break;
            }
            return returnResult;
        }

        /// <summary>
        /// 发送客服消息
        /// </summary>
        /// <param name="strMsgType"></param>
        /// <param name="wechat"></param>
        /// <param name="strOpenids"></param>
        /// <param name="obj"></param>
        /// <param name="strTitle"></param>
        /// <param name="strComment"></param>
        /// <returns></returns>
        private static WxJsonResult SendMsgKF(Weixin.MP.GroupMessageType MsgType, SysWechatConfig wechat, string[] strOpenids, object obj, string strTitle = null, string strComment = null)
        {

            WxJsonResult Ret = null;

            foreach (var openId in strOpenids)
            {
                WxJsonResult returnResult = null;

                switch (MsgType)
                {
                    case GroupMessageType.text:

                        returnResult = CustomApi.SendText(wechat.WeixinAppId, wechat.WeixinCorpSecret, openId, (string)obj);
                        break;
                    //case GroupMessageType.mpnews:

                    //    returnResult = CustomApi.SendNews(wechat.WeixinAppId, wechat.WeixinCorpSecret, openId, (List<Article>)obj);
                    //    break;
                    case GroupMessageType.mpnews:
                        returnResult = CustomApi.SendMpNews(wechat.WeixinAppId, wechat.WeixinCorpSecret, openId, (string)obj);
                        break;
                    case GroupMessageType.image:
                        returnResult = CustomApi.SendImage(wechat.WeixinAppId, wechat.WeixinCorpSecret, openId, (string)obj);
                        break;
                    case GroupMessageType.video:
                        returnResult = CustomApi.SendVideo(wechat.WeixinAppId, wechat.WeixinCorpSecret, openId, (string)obj, strTitle, strComment);
                        break;
                    case GroupMessageType.voice:
                        returnResult = CustomApi.SendVoice(wechat.WeixinAppId, wechat.WeixinCorpSecret, openId, (string)obj);
                        break;
                }

                if (returnResult.errcode != ReturnCode.请求成功)
                {
                    Ret = returnResult;
                }
                else if (Ret == null)
                {
                    Ret = returnResult;
                }

            }

            return Ret;


        }


        private static SendResult MPSendMessage(SysWechatConfig wechat, string value,
            Weixin.MP.GroupMessageType type, string strTags, string[] strOpenids, string[] PreviewOpenids, string VideoTitle, string VideoContent, bool isKefu)
        {
            SendResult returnResult = null;

            //客服消息
            if (isKefu)
            {
                WxJsonResult retCustom = SendMsgKF(type, wechat, strOpenids, value, VideoTitle, VideoContent);

                return new SendResult() { errcode = retCustom.errcode, errmsg = retCustom.errmsg };
            }

            //预览消息
            if (PreviewOpenids != null)
            {
                foreach (var openid in PreviewOpenids)
                {
                    returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessagePreview(wechat.WeixinAppId, wechat.WeixinCorpSecret, type, value, openid);
                }
                return returnResult;
            }


            //其他正常消息
            if (strTags == "-1" || (null != strOpenids && strOpenids.Length == 1 && "@all".Equals(strOpenids[0]))) //ToAll
            {
                returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessageByGroupId(wechat.WeixinAppId, wechat.WeixinCorpSecret, "", value, type, true, false,10000);
            }
            else if (!string.IsNullOrEmpty(strTags)) //To Tags
            {
                returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessageByGroupId(wechat.WeixinAppId, wechat.WeixinCorpSecret, strTags, value, type, false, false, 10000);
            }
            else if (strOpenids.Length > 0) //To openids
            {
                //一次最多1W人，超过1W，分批发
                int iOpenidCount = strOpenids.Length;
                string[] TempOpenids ;

                for (int i = 0; i < Math.Ceiling(iOpenidCount / 5000d); i++)
                {
                    int iTo = 5000 * (i + 1) > iOpenidCount ? (iOpenidCount - 5000 * i) : 5000;
                    TempOpenids = new string[iTo];

                    Array.Copy(strOpenids, 5000 * i, TempOpenids, 0, iTo);

                    if (type == GroupMessageType.video) //腾讯接口视频单独处理
                    {
                        returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendVideoGroupMessageByOpenId(wechat.WeixinAppId, wechat.WeixinCorpSecret, VideoTitle, VideoContent, value, 10000, TempOpenids);
                    }
                    else
                    {
                        returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessageByOpenId(wechat.WeixinAppId, wechat.WeixinCorpSecret, type, value, 10000, TempOpenids);

                    }
                }
            }

            return returnResult;

        }


        ///// <summary>
        ///// 发送微信消息
        ///// </summary>
        ///// <param name="strMsgType">text,image,news</param>
        ///// <param name="strContent"></param>
        ///// <param name="lstContent"></param>
        ///// <returns></returns>
        //public static SendResult SendMsg(int iAppID, string strMsgType, string strContent, List<Article> lstContent)
        //{
        //    //MassResult objResult = null;
        //    string strDept = "", strUser = "", strTags = "";
        //    //  int IsSec = 0;

        //    GetAppInfoResult Ret = GetUserOrDept(iAppID);

        //    Ret.allow_userinfos.user.Where(a => a.status == "1").ToList().ForEach(a => { strUser += "|" + a.userid; });
        //    if (strUser.Length > 0) { strUser = strUser.Substring(1); }
        //    strDept = string.Join(",", Ret.allow_partys.partyid);
        //    strTags = string.Join(",", Ret.allow_tags.tagid);

        //    return SendMsg(iAppID, strMsgType, strUser, strDept, strTags, strContent, lstContent);

        //}

        //public static GetAppInfoResult GetUserOrDept(int iAppID)
        //{
        //    var objConfig = WeChatCommonService.GetWeChatConfigByID(iAppID);

        //    string strToken = AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);

        //    return AppApi.GetAppInfo(strToken, int.Parse( objConfig.WeixinAppId));
        //    // Ret.allow_userinfos
        //}

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
        /// 服务号消息内容中不能有外链的图片，需要转换
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="iAPPID"></param>
        /// <returns></returns>
        public static string ImageConvert(string strContent, int iAPPID)
        {

            List<string> lst = new List<string>();
            string regex = @"(<img.*?/>)";
            Regex listRegex = new Regex(regex, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            //得到匹配的数据集合
            MatchCollection mc = listRegex.Matches(strContent);
            foreach (Match a in mc)
            {
                var href = Regex.Match(a.Value, "src=\"(.*?)\"");
                if (href == null || href.Groups.Count < 2)
                {
                    continue;
                }

                if (lst.Contains(href.Groups[1].Value))
                {
                    continue;
                }
                lst.Add(href.Groups[1].Value);
                var file = href.Groups[1].Value.Replace(CommonService.GetSysConfig("Content Server", ""), "");

                var config = WeChatCommonService.GetWeChatConfigByID(iAPPID);

                var ret = MediaApi.UploadImg(config.WeixinAppId, config.WeixinCorpSecret, (HttpContext.Current == null ? HttpRuntime.AppDomainAppPath : HttpContext.Current.Request.PhysicalApplicationPath) + file, 10000 * 60);
                strContent = strContent.Replace(href.Groups[1].Value, ret.url);

            }

            return strContent;
        }




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

    }
}