﻿using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Service.Common;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Innocellence.WeChat.Controllers
{
    public partial class ArticleInfoController : WeChatBaseController<ArticleInfo, ArticleInfoView>
    {
        private IArticleInfoService _objService;
        private IArticleThumbsUpService _articleThumbsUpService;
        private IMessageService _messageService;
        private IAddressBookService _addressBookService;
        private IArticleInfoReadHistoryService _articleInfoReadHistoryService;

        //记录用户行为
        //   Innocellence.WeChat.Domain.Common.WebRequestPost wr = new Innocellence.WeChat.Domain.Common.WebRequestPost();
        public ArticleInfoController(IArticleInfoService objService,
            IArticleThumbsUpService articleThumbsUpService,
            IMessageService messageService,
            IAddressBookService addressBookService,
            IArticleInfoReadHistoryService articleInfoReadHistoryService
           )
            : base(objService)
        {
            _objService = objService;
            _messageService = messageService;
            _articleThumbsUpService = articleThumbsUpService;
            _addressBookService = addressBookService;
            _articleInfoReadHistoryService = articleInfoReadHistoryService;
        }

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnBaseActionExecuting(filterContext);
        //}

        public override List<ArticleInfoView> GetListEx(Expression<Func<ArticleInfo, bool>> predicate, PageCondition ConPage)
        {
            int appId = default(int);
            int cate = default(int);
            if (int.TryParse(Request["wechatid"], out appId) && int.TryParse(Request["strSubCate"], out cate))
            {
                predicate = predicate.AndAlso(a => a.AppId == appId && a.CategoryId == cate && a.IsDeleted == false && a.ArticleType == 0 && a.PublishDate != null);
                if (!string.IsNullOrEmpty(Request["searchword"]))
                {
                    var searchWord = Request["searchword"];
                    predicate = predicate.AndAlso(a => a.ArticleTitle.Contains(searchWord) || a.ArticleContent.Contains(searchWord));
                }
                var q = _objService.GetList<ArticleInfoView>(predicate, ConPage);
                q.ForEach(a =>
                {
                    a.ImageCoverUrl = a.ImageCoverUrl.Replace(".", "_B.");
                    if (a.PublishDate.HasValue)
                    {
                        a.PublishDateFormatString = ((DateTime)a.PublishDate).ToString("yyyy-MM-dd");
                    }
                });
                return q.ToList();
            }
            return null;
        }

        public override ActionResult List()
        {
            ViewBag.AppId = Request["wechatid"];
            ViewBag.StrSubCate = Request["strSubCate"];
            return View();
        }

        public ActionResult WxDetail(int id, bool isPreview = false)
        {
            //由于存在重复的App（58与10为同一个App），此处将58替换为10
            //暂时保证 PRD环境可运行
            //此代码可在稳定运行数月后删除
            int appId = AppId == 58 ? 10 : AppId;
            _Logger.Debug("app id :{0}", appId);
            var article = _objService.Repository.Entities.FirstOrDefault(a => a.Id == id && a.AppId == appId && !a.IsDeleted.Value);
            if (article == null || (!isPreview && article.ArticleStatus != ConstData.STATUS_PUBLISH))
            {
                ///return Redirect("../invalid");
                ///
                return View(new ArticleInfoView() { ArticleTitle = "错误！", ArticleContent = "文章不存在！可能已经删除或者未发布！" });
            }
            //var list = _articleThumbsUpService.Repository.Entities.Where(x => x.ArticleID == id && x.Type == ThumbupType.Article.ToString()).Select(x => new { articleId = x.ArticleID, userId = x.UserID, x.IsDeleted }).ToList();

            //article.ThumbsUpCount = list.Count(x => x.IsDeleted != true);

            //第一次跳转到该Action 后需要再次跳转到OAuth
            //第二次跳转到该Action 后再进行验证
            //当redirectUrl 为string.empty 时, 认为验证通过
            string redirecturl = base.ExecuteAuthorityFilterForNews(article, true);
            if (!string.IsNullOrEmpty(redirecturl))
            {
                return Redirect(redirecturl);
            }
            //if (article.ArticleType == 1 && isPreview)
            //{
            //    article = null;
            //}

            if (isPreview)
            {
                var dateOfUpdate = (DateTime)article.PreviewStartDate;
                TimeSpan timeBetweenNowPreview = DateTime.Now.Subtract(dateOfUpdate);
                int sumHours = timeBetweenNowPreview.Hours;

                if (sumHours > 12)
                {
                    return RedirectToAction("overtime", new { wechatid = appId });
                }
            }

            if (article.PublishDate == null)
            {
                article.PublishDate = article.CreatedDate;
            }

            //有异步
            ExecuteBehavior(article.AppId.Value, 1, "", id.ToString());

            //勉强异步一下吧
            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(ViewBag.WeChatUserID))
                {
                    _Logger.Debug("begin to check has readed :{0}, {1}", article.Id, ViewBag.WeChatUserID);
                    if (!_articleInfoReadHistoryService.HasReaded(article.Id, ViewBag.WeChatUserID))
                    {
                        _articleInfoReadHistoryService.InsertView(new ArticleInfoReadHistoryView() { UserId = ViewBag.WeChatUserID, ArticleInfoId = article.Id });
                        _Logger.Debug("begin to add read count");
                        new ArticleInfoService().Repository.SqlExcute("update articleinfo set ReadCount=ReadCount+1 where id=" + article.Id.ToString());
                    }
                }
                else
                {
                    new ArticleInfoService().Repository.SqlExcute("update articleinfo set ReadCount=ReadCount+1 where id=" + article.Id.ToString());
                }
            });

            if (!string.IsNullOrEmpty(article.ArticleURL))
            {
                if (!article.IsPassingWeChatUserID.GetValueOrDefault()) return Redirect(article.ArticleURL);

                var builder = new UriBuilder(article.ArticleURL);
                var query = HttpUtility.ParseQueryString(builder.Query);
                query.Add("WeChatUserID", _objService.EncryptorWeChatUserID(ViewBag.WeChatUserID));
                _Logger.Debug(string.Format("url {0}", builder.ToString()));
                builder.Query = query.ToString();
                return Redirect(builder.ToString());
            }

            var articleView = new ArticleInfoView().ConvertAPIModel(article);

            var view = (ArticleInfoView)articleView;
            string WeChatUserID = ViewBag.WeChatUserID;
            view.IsThumbuped = _articleThumbsUpService.Repository.Entities.Any(x => x.UserID == WeChatUserID && x.ArticleID == id && x.IsDeleted != true);

            var appName = WeChatCommonService.lstSysWeChatConfig.Find(a => a.Id == appId);
            if (appName != null)
            {
                view.APPName = appName.AppName;
            }
            else
            {
                view.APPName = string.Empty;

            }
            //此处需要添加判断
            //判断文章允许支持点赞
            if (view.ShowComment.HasValue && view.ShowComment.Value)
            {
                var user = _addressBookService.GetMemberByUserId(WeChatUserID);
                if (null != user)
                {
                    ViewBag.UserInfoId = user.Id;
                    view.CanReadComment = true;
                    if (user.Status != null && user.Status.Value == 1  //关注
                        && "A".Equals(user.EmployeeStatus, StringComparison.OrdinalIgnoreCase)) //在职
                    {
                        view.CanAddComment = true;
                    }
                }
            }
            return View(view);
        }

        public ActionResult WxPreview(int id)
        {
            // 根据id去取文章
            var article = _objService.Repository.GetByKey(id);

            if (article == null || article.ArticleStatus == ConstData.STATUS_PUBLISH || article.IsDeleted.Value)
            {
                return Redirect("../invalid");
            }

            // 防止用户转发，全转为大写，忽略大小写
            if (!article.Previewers.ToUpper().Contains(ViewBag.WeChatUserID.ToUpper()))
            {
                return Redirect("/notauthed.html");
            }

            if (!string.IsNullOrEmpty(article.ArticleURL))
            {
                return Redirect(article.ArticleURL);
            }
            //记录用户行为
            ExecuteBehavior(article.AppId.Value, 1, "", id.ToString());

            var articleView = new ArticleInfoView().ConvertAPIModel(article);
            return View(articleView);
        }

        [LogTypeFilter(0, "")]
        public ActionResult invalid()
        {
            return View();
        }
        public ActionResult overtime()
        {
            return View();
        }
        //点赞实现
        public JsonResult GetThumbupCount()
        {

            if (objLoginInfo == null || string.IsNullOrEmpty(objLoginInfo.UserName))
            {
                return ErrorNotification("请先关注公众号！");
            }
            string aritcleId = Request["articleId"];
            string type = Request["type"];
            var articleid = 0;
            if (!string.IsNullOrEmpty(aritcleId))
            {
                articleid = int.Parse(aritcleId);
            }

            var num = _objService.UpdateArticleThumbsUp(articleid, objLoginInfo.WeChatUserID, type);

            return Json(new { count = num }, JsonRequestBehavior.AllowGet);
        }

        //微信网页
        //public ActionResult WxLink(string CateSub)
        //{
        //    //wr.CallUserBehavior(Request.FilePath, ViewBag.WeChatUserID, "17", CateSub, Request.Path, 5);
        //    if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
        //    {
        //        return Redirect("/notauthed.html");
        //    }

        //    var article = ((IArticleInfoService)_objService).GetByCateSub(CateSub);


        //    //控制未发布的和delete状态的文章不显示
        //    if (article == null || article.ArticleStatus != ConstData.STATUS_PUBLISH)
        //    {
        //        return Redirect("../ArticleInfo/invalid");
        //    }
        //    //记录用户行为
        //    ExecuteBehavior(article.AppId.Value, 5, CateSub);
        //    //Read count
        //    article.ReadCount++;
        //    _objService.Repository.Update(article, new List<string>() { "ReadCount" });
        //    if (!string.IsNullOrEmpty(article.ArticleURL))
        //    {
        //        return Redirect(article.ArticleURL);
        //    }

        //    var articleView = (ArticleInfoView)(new ArticleInfoView().ConvertAPIModel(article));
        //    return View("../ArticleInfo/wxLink", articleView);
        //}

        //SPP--支持帮助--联系我们=
        //public ActionResult SPPContactWxLink()
        //{
        //    string CateSub = "SM_CONTACT_US";

        //    return WxLink(CateSub);
        //}

        //SPP--支持帮助--使用指南
        //public ActionResult SPPNewPersonWxLink()
        //{
        //    string CateSub = "SM_BRAND_NEW";

        //    return WxLink(CateSub);
        //}

        //SPP--支持帮助--关于我们
        //public ActionResult SPPAboutUSWxLink()
        //{
        //    string CateSub = "SM_ABOUT_US";

        //    return WxLink(CateSub);
        //}

        ////SPP--支持帮助--用户反馈
        //public ActionResult SPPFeedbackWxLink()
        //{
        //    string CateSub = "SM_USER_FEEDBACK";

        //    return WxLink(CateSub);
        //}

        //HR--服务--热线电话
        //public ActionResult HRTELWxLink()
        //{
        //    string CateSub = "HRES_HOTLINE";

        //    return WxLink(CateSub);
        //}

        //IT--IT信息--政策流程
        //public ActionResult ITPOLICYWxLink()
        //{
        //    string CateSub = "IT_POLICY";

        //    return WxLink(CateSub);
        //}

        public ActionResult DecryptWeChatUserID(string WeChatUserID)
        {
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }
            var id = _objService.DecryptWeChatUserID(WeChatUserID);
            _Logger.Debug(string.Format("url testing. WeChatUserID:{0}", id));
            return Json(new { ok = "ok" }, JsonRequestBehavior.AllowGet);
        }
    }
}