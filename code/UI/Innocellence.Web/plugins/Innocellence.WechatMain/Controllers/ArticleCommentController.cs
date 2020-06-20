using Innocellence.Authentication.Controllers;
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
using Innocellence.WeChat.Domain.ModelsView;

namespace Innocellence.WeChatMain.Controllers
{
    public class ArticleCommentController : BaseController<ArticleComment, ArticleCommentView>
    {
        private readonly IArticleCommentThumbUpService _articleCommentThumbUpService;
        private readonly IArticleCommentService _articleCommentService;
        private readonly IWechatMPUserService _wechatMPUserService;
        private readonly IAddressBookService _addressBookService;
        private readonly IArticleInfoService _articleInfoService;

        private Dictionary<string, List<int>> userThumbUpMapping = new Dictionary<string, List<int>>();

        public ArticleCommentController(IArticleCommentService newsService,
            IArticleCommentThumbUpService articleCommentThumbUpService,
            IWechatMPUserService wechatMPUserService,
            IAddressBookService addressBookService,
            IArticleInfoService articleInfoService) : base(newsService)
        {
            this._articleCommentThumbUpService = articleCommentThumbUpService;
            this._articleCommentService = newsService;
            this._wechatMPUserService = wechatMPUserService;
            this._addressBookService = addressBookService;
            this._articleInfoService = articleInfoService;
        }

        // GET: ArticleComment
        public ActionResult Index()
        {
            return View();
        }

        public override List<ArticleCommentView> GetListEx(Expression<Func<ArticleComment, bool>> predicate, PageCondition con)
        {
            string articleInfoId = Request["articleId"];
            int articleId = -1;
            if (int.TryParse(articleInfoId, out articleId))
            {
                con.SortConditions.Add(new SortCondition("Id") { ListSortDirection = System.ComponentModel.ListSortDirection.Ascending });
                predicate = predicate.AndAlso(q => q.ArticleId != null && q.ArticleId == articleId && q.IsDeleted == false);
                bool isAddComment = false;
                if (bool.TryParse(Request["isAddComment"], out isAddComment) && isAddComment)
                {
                    string userOpenId = GetCurrentUserOpenID(Request["userInfoId"], Request["isMP"]);
                    if (!string.IsNullOrEmpty(userOpenId))
                    {
                        predicate = predicate.AndAlso(q => q.UserOpenId.Equals(userOpenId, StringComparison.OrdinalIgnoreCase));
                    }
                }
                var result = this._articleCommentService.GetList<ArticleCommentView>(predicate, con);
                AssembleCommentLists(result);
                return result;
            }
            return new List<ArticleCommentView>();
        }

        private void AssembleCommentLists(List<ArticleCommentView> result)
        {
            string currentUserOpenID = GetCurrentUserOpenID(Request["userInfoId"], Request["isMP"]);
            if (!string.IsNullOrEmpty(currentUserOpenID))
            {
                InitUserThumbUpMapping(currentUserOpenID);
                DateTime now = DateTime.Now;
                foreach (var item in result)
                {
                    //判断是否是当前user写的评论
                    if (currentUserOpenID.Equals(item.UserOpenId, StringComparison.OrdinalIgnoreCase))
                    {
                        item.CanDelete = true;
                    }
                    //判断当前user是否对评论点赞
                    item.HasThumbUp = userThumbUpMapping[currentUserOpenID].Contains(item.Id);
                    item.ThumbUpStyle = item.HasThumbUp ? "fa-thumbs-up" : "fa-thumbs-o-up";
                    item.DiffDateDisplayStr = GetDiffDateDisplayStr(item.CreatedDate, now);
                }
            }
        }

        private string GetDiffDateDisplayStr(DateTime? createdDate, DateTime now)
        {
            string displayStr = "刚刚";
            if (createdDate != null)
            {
                TimeSpan diff = now - createdDate.Value;
                if (diff.TotalDays >= 1)
                {
                    displayStr = string.Format("{0}天前", (int)Math.Floor(diff.TotalDays));
                }
                else if (diff.TotalHours >= 1)
                {
                    displayStr = string.Format("{0}小时前", (int)Math.Floor(diff.TotalHours));
                }
                else if (diff.TotalMinutes >= 10)
                {
                    displayStr = string.Format("{0}分钟前", (int)Math.Floor(diff.TotalMinutes));
                }
            }
            return displayStr;
        }

        private string GetCurrentUserOpenID(string userInfoIdStr, string isMPStr)
        {
            string openId = string.Empty;
            if (!string.IsNullOrEmpty(userInfoIdStr) && !string.IsNullOrEmpty(isMPStr))
            {
                bool isMP = false;
                if (bool.TryParse(isMPStr, out isMP))
                {
                    int userInfoId = -1;
                    if (int.TryParse(userInfoIdStr, out userInfoId))
                    {
                        if (isMP)
                        {
                            var user = _wechatMPUserService.Repository.GetByKey(userInfoId);
                            if (null != user && !string.IsNullOrEmpty(user.OpenId))
                            {
                                openId = user.OpenId;
                            }
                        }
                        else
                        {
                            var user = _addressBookService.Repository.GetByKey(userInfoId);
                            if (null != user && !string.IsNullOrEmpty(user.UserId))
                            {
                                openId = user.UserId;
                            }
                        }
                    }
                }
            }
            return openId;
        }

        private void InitUserThumbUpMapping(string currentUserOpenID)
        {
            if (!userThumbUpMapping.ContainsKey(currentUserOpenID))
            {
                List<int> hasThumbUpCommentIds = this._articleCommentThumbUpService.Repository.Entities.Where(e => e.CommentId != null && currentUserOpenID.Equals(e.UserOpenId, StringComparison.OrdinalIgnoreCase)).Select(e => (int)e.CommentId).ToList();
                userThumbUpMapping.Add(currentUserOpenID, hasThumbUpCommentIds);
            }
        }

        public JsonResult ThumbUp(int commentId, bool isThumbUp)
        {
            try
            {
                string userOpenId = GetCurrentUserOpenID(Request["userInfoId"], Request["isMP"]);
                if (!string.IsNullOrEmpty(userOpenId))
                {
                    _Logger.Debug("{0} {1} thumb up {2}", userOpenId, isThumbUp ? string.Empty : "cancle", commentId);
                    this._articleCommentThumbUpService.ThumbUp(commentId, userOpenId, isThumbUp);
                    this._articleCommentService.UpdateThumbsUpCount(commentId, isThumbUp);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
            }
            return null;
        }

        public ActionResult AddComment()
        {
            int userInfoId = -1;
            if (int.TryParse(Request["userInfoId"], out userInfoId) && userInfoId > 0)
            {
                int articleInfoId = -1;
                if (int.TryParse(Request["articleId"], out articleInfoId) && articleInfoId > 0)
                {
                    var articleInfo = this._articleInfoService.GetById<ArticleInfoView>(articleInfoId);
                    if (articleInfo != null)
                    {
                        ViewBag.ArticleTitle = articleInfo.ArticleTitle;
                    }
                    return View();
                }
            }
            return Redirect("/403.html");
        }

        public JsonResult DoAddComment(int articleId, string comment, bool isMP)
        {
            //userinfo id
            string userOpenId = GetCurrentUserOpenID(Request["userInfoId"], Request["isMP"]);
            if (!string.IsNullOrEmpty(userOpenId) && !string.IsNullOrEmpty(comment) && articleId > 0)
            {
                ArticleCommentView viewModel = new ArticleCommentView()
                {
                    ArticleId = articleId,
                    UserOpenId = userOpenId,
                    Comment = comment,
                    CanDelete = true,
                };
                if (isMP)
                {
                    var user = this._wechatMPUserService.GetUserByOpenId(userOpenId);
                    if (null != user)
                    {
                        viewModel.UserAvatar = user.HeadImgUrl;
                        viewModel.UserNickName = user.NickName;
                    }
                }
                else
                {
                    var user = this._addressBookService.GetMemberByUserId(userOpenId, true);
                    if (null != user)
                    {
                        viewModel.UserAvatar = user.Avatar;
                        viewModel.UserNickName = user.UserName;
                    }
                }
                int result = this._articleCommentService.InsertView(viewModel);
                return Json(new { data = viewModel }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult DoDeleteComment(int commentId)
        {
            if (commentId > 0)
            {
                this._articleCommentService.Repository.Delete(commentId);
            }
            return null;
        }
    }
}