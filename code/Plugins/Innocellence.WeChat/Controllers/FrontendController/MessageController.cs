using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts;
using Infrastructure.Web.Domain.Service.Common;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.ViewModel;
using Newtonsoft.Json;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChat.Controllers
{
    public partial class MessageController : WeChatBaseController<Message, MessageView>
    {
        private IMessageService _objService;
        private IArticleThumbsUpService _articleThumbsUpService;
        private IWechatMessageLogService _messageLogService;
        private IWechatPreviewMessageLogService _previewMessageLogService;
        private IAutoReplyService _autoReplyService;

        public MessageController(IMessageService objService, IArticleThumbsUpService articleThumbsUpService, IWechatMessageLogService messageLogService, IAutoReplyService autoReplyService, IWechatPreviewMessageLogService previewMessageLogService)
            : base(objService)
        {
            _objService = objService;
            _articleThumbsUpService = articleThumbsUpService;
            _messageLogService = messageLogService;
            _autoReplyService = autoReplyService;
            _previewMessageLogService = previewMessageLogService;
        }

        /// <summary>
        /// 微信前端网页
        /// </summary>
        /// <returns></returns>
    
        public ActionResult WxDetailh5(int id)
        {
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }

            var article = _objService.Repository.GetByKey(id);
            var list = _articleThumbsUpService.Repository.Entities.Where(x => x.ArticleID == id && x.Type == ThumbupType.Message.ToString()).Select(x => new { articleId = x.ArticleID, userId = x.UserID, x.IsDeleted }).ToList();
            article.ThumbsUpCount = list.Count(x => x.IsDeleted != true);

            //控制未发布的和delete状态的文章不显示
            if (article == null || article.Status!=ConstData.STATUS_PUBLISH || article.IsDeleted)
            {
                return Redirect("../invalid");
            }
            //记录用户行为
            ExecuteBehavior(article.AppId.Value, 2, "",id.ToString());

            //Read count add by andrew 2016/2/3
            article.ReadCount++;
            _objService.Repository.Update(article, new List<string>() { "ReadCount" });

            if (!string.IsNullOrEmpty(article.URL))
            {
                return Redirect(article.URL);
            }

            var articleView = (MessageView)(new MessageView().ConvertAPIModel(article));
            articleView.IsThumbuped = list.Any(x => x.userId == ViewBag.WeChatUserID && x.IsDeleted != true);

            return View(articleView);
        }

       
        public ActionResult WxPreviewh5(int id)
        {

            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }

            var article = _objService.Repository.GetByKey(id);

            if (article == null || article.Status == ConstData.STATUS_PUBLISH || article.IsDeleted)
            {
                return Redirect("../invalid");
            }

            // 防止用户转发，全转为大写，忽略大小写
            if (!article.Previewers.ToUpper().Contains(ViewBag.WeChatUserID.ToUpper()))
            {
                return Redirect("/notauthed.html");
            }

            if (!string.IsNullOrEmpty(article.URL))
            {
                return Redirect(article.URL);
            }
            //记录用户行为
            ExecuteBehavior(article.AppId.Value, 2, "",id.ToString());
            var articleView = (MessageView)(new MessageView().ConvertAPIModel(article));
            return View(articleView);
        }
        
        public ActionResult invalid()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetNews(int id, int? subId, int type, bool? isPreview)
        {
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }
            NewsTypeEnum typeEnum = (NewsTypeEnum)type;
            switch (typeEnum)
            {
                case NewsTypeEnum.Message:
                    if (isPreview.HasValue && isPreview.Value)
                    {
                        var model = _previewMessageLogService.Repository.Entities.Where(a => a.Id == id && a.ContentType == (int)WechatMessageLogType.news).FirstOrDefault();
                        if (model != null)
                        {
                            var list = JsonConvert.DeserializeObject<List<NewsInfoView>>(model.Content);
                            ViewBag.Content = list.Where(a => a.Id == subId).First();
                            ViewBag.CreatedTime = model.CreatedTime.ToString("yyyy-MM-dd");
                        }
                    }
                    else
                    {
                        var model = _messageLogService.Repository.Entities.Where(a => a.Id == id && a.ContentType == (int)WechatMessageLogType.news).FirstOrDefault();
                        if (model != null)
                        {
                            var list = JsonConvert.DeserializeObject<List<NewsInfoView>>(model.Content);
                            ViewBag.Content = list.Where(a => a.Id == subId).First();
                            ViewBag.CreatedTime = model.CreatedTime.ToString("yyyy-MM-dd");
                        }
                    }
                    break;
                case NewsTypeEnum.AutoReply:
                    var view = new AutoReplyNewView();
                    view.Main = _autoReplyService.GetDetail(id);
                    var news = JsonConvert.DeserializeObject<List<NewsInfoView>>(view.Main.Contents[0].Content)[0];
                    ViewBag.Content = news;
                    ViewBag.CreatedTime = view.Main.CreatedDate.Value.ToString("yyyy-MM-dd");
                    break;
                default:
                    break;
            }
            return View();
        }
    }
}
