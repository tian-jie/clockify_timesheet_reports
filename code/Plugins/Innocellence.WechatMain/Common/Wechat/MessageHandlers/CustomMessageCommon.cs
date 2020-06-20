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
using Infrastructure.Core.Logging;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Common;
using Infrastructure.Utility.Extensions;
using Infrastructure.Web.Domain.Service;
using System.Web.Configuration;
using System.Net;

using Innocellence.WeChat.Domain.Entity;
using Infrastructure.Web.ImageTools;
using System.Web.Hosting;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.Weixin.Entities;
using System.Text;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.ViewModel;
using System.Text;
using Innocellence.WeChat.Domain.Contracts;
using Newtonsoft.Json;
using Innocellence.Weixin.Helpers;
using Innocellence.WeChatMain.Common;
using System.Web;
using System.Data.Entity;

namespace Innocellence.Weixin.CommonService.MessageHandlers
{
    public class CustomMessageCommon
    {
        private static IArticleInfoService _articleInfoService = EngineContext.Current.Resolve<IArticleInfoService>();
        private IWeChatAppUserService _weChatAppUserService = EngineContext.Current.Resolve<IWeChatAppUserService>();// new WeChatAppUserService();
        private IBatchJobLogService _batchJobLogService = EngineContext.Current.Resolve<IBatchJobLogService>();// new BatchJobLogService();
        //  private static IAutoReplyContentService _autoReplyContentService = EngineContext.Current.Resolve<IAutoReplyContentService>();// new AutoReplyContentService();
        private static IAttachmentsItemService _attachmentsItemService = EngineContext.Current.Resolve<IAttachmentsItemService>();//new AttachmentsItemService();

        private static IAutoReplyContentService _AutoReplyContentService = EngineContext.Current.Resolve<IAutoReplyContentService>();

        private static string _newsHost = Infrastructure.Web.Domain.Service.CommonService.GetSysConfig("WeChatUrl", "");


        ////TODO
        //private ArticleImagesService _articleImagesService = new ArticleImagesService();
        //private IAddressBookService _addressBookServie = new AddressBookService();
        private const string USER_IMAGE_SAVE_FOLDER_PATH = @"/wxUserImage/";

        // private bool _isDebug = false;

        private static ILogger log { get { return LogManager.GetLogger("WeChat"); } }





        public static List<Article> CreateResponse(List<ArticleInfoView> articles, string host, string baseUrl)
        {
            // var responseMessage = this.CreateResponseMessage<ResponseMessageNews>();
            var Articles = new List<Article>();
            int i = 0;
            foreach (var a in articles)
            {
                //string imgUrl;

                //if (a.ThumbImageId == null)
                //{
                //    log.Error("Not found Image!");
                //    imgUrl = _newsHost + "/Content/img/LogoRed.png";
                //}
                //else
                //{
                //    imgUrl = string.Format("{0}{1}", _newsHost, a.ImageCoverUrl);
                //}

                var imgUrl = "";
                if (0 == i)
                {
                    //不存在就创建
                    imgUrl = WechatCommon.doGetFileCover( a.ImageCoverUrl, "_B");
                }
                else
                {
                    imgUrl = WechatCommon.doGetFileCover( a.ImageCoverUrl, "_T");
                }

                i++;

                var newArticle = new Article()
                {
                    Title = a.ArticleTitle,
                    Url = host + baseUrl + a.Id,
                    PicUrl = _newsHost+imgUrl,
                    Description = a.ArticleComment
                };
                log.Debug("Creating News - \r\n\tTitle: " + newArticle.Title + "\r\n\tUrl: " + newArticle.Url + "\r\n\tPush Image: " + newArticle.PicUrl);

                Articles.Add(newArticle);
            }
            return Articles;
        }







        #region 消息处理


        /// <summary>
        /// 获取视频回复
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Video CreateVideoResponseMessage(AutoReplyContentView content, string strCorpID)
        {

            if (content.IsNewContent.Value)
            {
                var video = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content)[0];
                return new Video()
                {
                    Title = video.NewsTitle,
                    Description = video.NewsComment,
                    MediaId = GetMediaIDByNewsInfo(video, content.Id),
                };
            }
            else
            {
                if (content.FileID != null)
                {

                    NewsInfoView news;
                    Innocellence.WeChatMain.Common.WechatCommon.GetMediaIDByFileID(content.FileID, _attachmentsItemService, strCorpID, out news);

                    return new Video()
                    {
                        Title = news.NewsTitle,
                        Description = news.NewsComment,
                        MediaId = news.MediaId,
                    };


                }
                else
                {
                    throw new Exception("do not found file id.");
                }
            }
            return null;
        }

        /// <summary>
        /// 获取语音回复
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Voice CreateVoiceResponseMessage(AutoReplyContentView content, string strCorpID)
        {
            // var voiceMsg = this.CreateResponseMessage<ResponseMessageVoice>();
            if (content.IsNewContent.Value)
            {
                var voice = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content)[0];
                return new Voice() { MediaId = GetMediaIDByNewsInfo(voice, content.Id) };
            }
            else
            {
                NewsInfoView news;
                return new Voice() { MediaId = Innocellence.WeChatMain.Common.WechatCommon.GetMediaIDByFileID(content.FileID, _attachmentsItemService, strCorpID, out news) };
            }
            return null;
        }

        /// <summary>
        /// 获取图片回复
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Image CreateImageResponseMessage(AutoReplyContentView content, string strCorpID)
        {
            //  var imageMsg = this.CreateResponseMessage<ResponseMessageImage>();
            if (content.IsNewContent.Value)
            {
                var image = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content)[0];
                return new Image() { MediaId = GetMediaIDByNewsInfo(image, content.Id) };
            }
            else
            {
                NewsInfoView news;
                return new Image() { MediaId = Innocellence.WeChatMain.Common.WechatCommon.GetMediaIDByFileID(content.FileID, _attachmentsItemService, strCorpID, out news) };
            }

            // return imageMsg;
        }

        /// <summary>
        /// 根据content获取回复内容，判断文件是否过期，过去需要重新传
        /// </summary>
        /// <param name="News"></param>
        /// <param name="iId"></param>
        /// <returns></returns>
        public static string GetMediaIDByNewsInfo(NewsInfoView News, int iId)
        {
            if (DateTimeHelper.GetDateTimeFromXml(News.MediaCreateTime).AddDays(3) < DateTime.Now)
            {

                log.Debug("GetMediaIDByNewsInfo start:  cate:{0}  AutoReplyID:{1}", News.NewsCate, iId);


                Innocellence.WeChatMain.Common.WechatCommon.GetMediaInfo((AutoReplyContentEnum)Enum.Parse(typeof(AutoReplyContentEnum), News.NewsCate.ToUpper()), News, News.AppId);

                var content = JsonConvert.SerializeObject(new List<NewsInfoView>() { News });
                AutoReplyContent model = new AutoReplyContent()
                {
                    Id = iId,
                    Content = content
                };

                _AutoReplyContentService.Repository.Update(model, new List<string>() { "Content" });

                return News.MediaId;
            }
            else
            {
                return News.MediaId;
            }


        }




        public static List<object> CreateNewsResponseMessage(AutoReplyContentView content, int appId, bool isCorp, bool isSafe, bool isAutoReply = false)
        {
            // var photoTextMsg = this.CreateResponseMessage<ResponseMessageNews>();
            var Articles = new List<object>();
            if (content.IsNewContent.Value)
            {
                var info = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content);
                var configs = Infrastructure.Web.Domain.Service.CommonService.lstSysConfig;
                // var config = configs.Where(a => a.ConfigName.Equals("Content Server", StringComparison.CurrentCultureIgnoreCase)).First();
                //var contentConfig = configs.Where(a => a.ConfigName.Equals("Content Server", StringComparison.CurrentCultureIgnoreCase)).First();
                string host = Infrastructure.Web.Domain.Service.CommonService.GetSysConfig("Content Server", "").TrimEnd('/');// config.ConfigValue;
                //if (host.EndsWith("/"))
                //{
                //    host = host.Substring(0, host.Length - 1);
                //}

                int ii = 0;
                foreach (var entity in info)
                {
                    if (ii == 0) //位置不同，缩略图的比例不一样
                    {
                        entity.ImageSrc = WechatCommon.doGetFileCover(entity.ImageSrc, "_B");
                    }
                    else
                    {
                        entity.ImageSrc = WechatCommon.doGetFileCover(entity.ImageSrc, "_T");
                    }

                    ii++;

                    var picUrl = host + entity.ImageSrc;
                    //var url = host + "/News/ArticleInfo/wxdetail/" + content.AutoReplyId + "?wechatid=" + appId;// "&subId=" + item.Id;
                    var url = string.Format("{0}/{1}/Message/GetNews?id={2}&wechatid={2}&type={3}&subId={4}", host, isCorp ? "news" : "mpnews",
                        content.AutoReplyId, appId, (int)NewsTypeEnum.AutoReply, entity.Id); //host + "/News/Message/GetNews?id=" + content.AutoReplyId + "&wechatid=" + appId + "&type=" + (int)NewsTypeEnum.AutoReply;
                    var newArticle = new Article()
                    {
                        Title = entity.NewsTitle,
                        Url = url,
                        PicUrl = picUrl,
                        Description = entity.NewsComment
                    };
                    Articles.Add(newArticle);
                }
            }
            else
            {
                List<ArticleInfoView> articleList = new List<ArticleInfoView>();
                if (content.SecondaryType == (int)AutoReplyNewsEnum.MANUAL)
                {
                    List<int> articleIds = content.NewsID.Trim(',').Split(',').ToList().Select(n => int.Parse(n)).ToList();
                    if (articleIds.Count > 0)
                    {
                        log.Debug("article count :{0}.", articleIds.Count);
                        var lst = ((DbSet<ArticleInfo>)_articleInfoService.Repository.Entities).AsNoTracking()
                            .Where(t => t.AppId == appId && articleIds.Contains(t.Id) && t.IsDeleted == false).ToList()
                            .Select(a => (ArticleInfoView)new ArticleInfoView().ConvertAPIModelListWithContent(a)).ToList();

                        //解决顺序问题
                        foreach (var aID in articleIds)
                        {
                            var al = lst.Find(a => a.Id == aID);
                            if (al != null)
                            {
                                articleList.Add(al);
                            }
                        }

                    }
                }
                else if (content.SecondaryType == (int)AutoReplyNewsEnum.LATEST)
                {
                    articleList.AddRange(((DbSet<ArticleInfo>)_articleInfoService.Repository.Entities).AsNoTracking()
                        .Where(t => t.AppId == appId && t.IsDeleted == false)
                        .OrderBy("Id", System.ComponentModel.ListSortDirection.Descending)
                        .Take(int.Parse(content.Content)).ToList()
                        .Select(a => (ArticleInfoView)new ArticleInfoView().ConvertAPIModelListWithContent(a)).ToList());
                };

                // articleList = articleList.Distinct().OrderByDescending(t => t.PublishDate).ToList();

                var token = WeChatCommonService.GetWeiXinToken(appId);
                int ii = 0;
                foreach (var a in articleList)
                {
                    log.Debug("Start ID:{0} ImageCoverUrl:{1} ",a.Id, a.ImageCoverUrl);

                    if (ii == 0) //位置不同，缩略图的比例不一样
                    {
                        a.ImageCoverUrl = WechatCommon.doGetFileCover(a.ImageCoverUrl, "_B");
                    }
                    else
                    {
                        a.ImageCoverUrl = WechatCommon.doGetFileCover(a.ImageCoverUrl, "_T");
                    }

                    ii++;


                    if (isSafe)
                    {
                        var newArticle = new MpNewsArticle()
                        {
                            title = a.ArticleTitle,
                            content_source_url = string.Format("{0}/{3}/ArticleInfo/wxdetail/{1}?wechatid={2}&isAutoReply={4}", _newsHost, a.Id, appId, isCorp ? "news" : "mpnews", isAutoReply ? 1 : 0),// _newsHost + "/News/ArticleInfo/wxdetail/" + a.Id + "?wechatid=" + appId,
                            //content = _newsHost + a.ImageCoverUrl,
                            digest = a.ArticleComment,
                            content = a.ArticleContent,// WechatCommonMP.ImageConvert(a.ArticleContent, appId),
                            author = a.CreatedUserID,
                            show_cover_pic = "0",
                            thumb_media_id = WechatCommon.GetMediaId(a.ImageCoverUrl, token)

                        };
                        log.Debug("Creating MPNews - \r\n\tTitle: " + newArticle.title + "\r\n\tUrl: " + newArticle.content_source_url + "\r\n\tPush Image: " + newArticle.thumb_media_id);
                        Articles.Add(newArticle);
                    }
                    else
                    {
                        var newArticle = new Article()
                        {
                            Title = a.ArticleTitle,
                            Url = string.Format("{0}/{3}/ArticleInfo/wxdetail/{1}?wechatid={2}&isAutoReply={4}", _newsHost, a.Id, appId, isCorp ? "news" : "mpnews", isAutoReply ? 1 : 0),// _newsHost + "/News/ArticleInfo/wxdetail/" + a.Id + "?wechatid=" + appId,
                            PicUrl = _newsHost + a.ImageCoverUrl.Replace("\\", "/").TrimStart('/'),
                            Description = a.ArticleComment
                        };
                        log.Debug("Creating News - \r\n\tTitle: " + newArticle.Title + "\r\n\tUrl: " + newArticle.Url + "\r\n\tPush Image: " + newArticle.PicUrl);
                        Articles.Add(newArticle);
                    }

                }
            }

            return Articles;
        }

        public static string CreateTextResponseMessage(AutoReplyContentView content)
        {
            // var testMsg = this.CreateResponseMessage<ResponseMessageText>();
            if (content.IsNewContent.Value)
            {
                var info = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content);
                return info[0].NewsContent;
            }
            else
            {
                return content.Content;
            }

            // LogManager.GetLogger(this.GetType()).Debug("Create text response :{0}", content);
            // return testMsg;
        }

        public static string CreateLinkResponseMessage(AutoReplyContentView content)
        {
            if (content.IsNewContent.Value)
            {
                var info = JsonConvert.DeserializeObject<List<NewsInfoView>>(content.Content);
                var url = info[0].NewsContent;
                var title = info[0].NewsTitle;
                return string.Format("<a href=\"{0}\">{1}</a>", url, title);
            }
            else
            {
                return content.Content;
            }
        }

        public static void SaveImage(MemoryStream stream, int iAPPID, string strUserID)
        {
            log.Debug("Begin SaveImage from Wechat Client");

            //WebClient webclient = new WebClient();
            //var picData = await webclient.DownloadDataTaskAsync(requestMessage.PicUrl);
            //string picType = requestMessage.PicUrl.Substring(requestMessage.PicUrl.LastIndexOf('.') + 1);

            //var img = ImageHelper.BytesToImage(picData);

            // var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.WeixinCorpId == requestMessage.ToUserName);

            using (stream)
            {
                try
                {


                    //var imageBytes = stream.ToArray();
                    //var imageFromUser = ImageHelper.BytesToImage(imageBytes);

                    log.Debug("ImageUtility.MakeThumbnail");

                    string filename = USER_IMAGE_SAVE_FOLDER_PATH + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
                    string strPath = MapPath("/" + filename);

                    ImageUtility.MakeThumbnail(null, stream, strPath, 900, 0, "W", 1, true);
                    //thumbnailImageFromUser.Save(strPath);

                    //LogManager.GetLogger(this.GetType()).Debug("thumbnailImageFromUser is NULL? - " + (thumbnailImageFromUser == null));

                    log.Debug("new ArticleImages()");
                    var articleImage = new ArticleImages()
                    {
                        AppId = iAPPID,
                        CreatedUserID = strUserID,
                        ImageContent = System.IO.File.ReadAllBytes(strPath),
                        CreatedDate = DateTime.Now,
                        ImageName = filename
                    };

                    log.Debug("_articleImagesService.Insert");

                    IArticleImagesService _articleImagesService = EngineContext.Current.Resolve<IArticleImagesService>();

                    _articleImagesService.Repository.Insert(articleImage);
                }
                catch (Exception ex)
                {
                    log.Error(ex, ex.Message);
                }
            }

            log.Debug("End SaveImage from Wechat Client");
        }




        #endregion




        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public static string MapPath(string path)
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
}


