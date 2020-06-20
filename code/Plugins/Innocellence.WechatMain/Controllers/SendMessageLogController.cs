using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Contracts;
using Infrastructure.Utility.Data;
using System.Linq.Expressions;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.ModelsView;
using Newtonsoft.Json;

namespace Innocellence.WeChatMain.Controllers
{
    public class SendMessageLogController : BaseController<MessageLog, MessageLogView>
    {
        private IArticleInfoService _articleInfoService;

        public SendMessageLogController(IMessageLogService newsService, IArticleInfoService articleInfoService) : base(newsService)
        {
            this._articleInfoService = articleInfoService;
        }

        public override ActionResult Index()
        {
            return base.Index();
        }

        public override List<MessageLogView> GetListEx(Expression<Func<MessageLog, bool>> predicate, PageCondition con)
        {
            string txtDate = Request["txtDate"];
            int appId = int.Parse(Request["APPID"]);
            predicate = predicate.AndAlso(m => m.AppId == appId);

            if (!string.IsNullOrEmpty(txtDate))
            {
                DateTime dateTime = Convert.ToDateTime(txtDate);
                DateTime dateAdd = dateTime.AddDays(1);
                predicate = predicate.AndAlso(m => m.CreatedDate >= dateTime && m.CreatedDate <= dateAdd);
            }

            string TXTTYPE = Request["TXTTYPE"];
            if (!string.IsNullOrEmpty(TXTTYPE))
            {
                var contentType = (int)Enum.Parse(typeof(WechatMessageLogType), TXTTYPE, true);
                predicate = predicate.AndAlso(m => m.MsgContentType == contentType);
            }
            var result = base.GetListEx(predicate, con);
            result.ForEach(m =>
            {
                if (m.SendMsgStatus.HasValue)
                {
                    m.SendMsgStatusDisplayStr = ((SendMessageStatus)m.SendMsgStatus.Value).GetDescriptionByName();
                }
                else
                {
                    m.SendMsgStatusDisplayStr = SendMessageStatus.Success.GetDescriptionByName();
                }
            });
            return result;
        }

        public JsonResult GetItem(int id)
        {
            ArticleInfoView firstArticle = null;
            var item = this._objService.Repository.GetByKey(id);
            if (item != null && !string.IsNullOrEmpty(item.NewsIdList))
            {
                var newsIdList = item.NewsIdList.Split(',').ToList().Select(x => int.Parse(x)).ToList();
                if (newsIdList != null)
                {
                    List<NewsInfoView> newsInfoList = new List<NewsInfoView>();
                    bool isArticle = false;
                    List<dynamic> temp = typeof(WechatMessageLogType).GetAllItems();
                    var currentMsgContentType = temp.FirstOrDefault(w => item.MsgContentType.Value.Equals(w.Value));
                    for (int i = 0; i < newsIdList.Count; i++)
                    {
                        //包含被删除的图文
                        var article = this._articleInfoService.Repository.GetByKey(newsIdList[i]);
                        if (article != null)
                        {
                            var articleView = (ArticleInfoView)new ArticleInfoView().ConvertAPIModel(article);
                            if (i == 0)
                            {
                                firstArticle = articleView;
                            }
                            var newsInfo = (NewsInfoView)articleView.ConvertToNewsInfoView(article);
                            if (currentMsgContentType != null)
                            {
                                newsInfo.NewsCate = currentMsgContentType.Text;
                                isArticle = newsInfo.NewsCate.Equals("news");
                            }
                            newsInfoList.Add(newsInfo);
                        }
                    }
                    if (null != firstArticle)
                    {
                        firstArticle.ArticleCode = null;
                        if (isArticle)
                        {
                            firstArticle.ArticleContentEdit = JsonConvert.SerializeObject(newsInfoList);
                        }
                    }
                }
            }
            return Json(firstArticle, JsonRequestBehavior.AllowGet);
        }
    }
}