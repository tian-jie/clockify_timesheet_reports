using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Web.Mvc;
namespace Innocellence.WeChat.Controllers
{
    public partial class FeedbackController : WeChatBaseController<FeedBackEntity, FeedBackView>
    {
        public FeedbackController(IFeedbackService objService)
            : base(objService)
        {
        }

        public ActionResult WxDetail(string appId, string menuCode)
        {
            //如果未关注企业号
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }

            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(menuCode) ||
                !int.TryParse(appId, out AppId))
            {
                return Redirect("/notauthed.html");
            }

            if (((CategoryType)AppId) != CategoryType.JohnLAccessChina)
            {
                return Redirect("/notauthed.html");
            }

            //记录用户行为
            ExecuteBehavior(int.Parse(appId), 7, menuCode);
            ViewBag.MenuCode = menuCode;
            ViewBag.AppID = appId;
            return View();
        }

     
        public ActionResult WxCommonDetail(string appId, string menuCode)
        {
            //如果未关注企业号
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }

            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(menuCode) ||
                !int.TryParse(appId, out AppId))
            {
                return Redirect("/notauthed.html");
            }

            //if (((CategoryType)AppId) != CategoryType.ACC)
            //{
            //    return Redirect("/notauthed.html");
            //}

            //记录用户行为
            ExecuteBehavior(int.Parse(appId), 7, menuCode);
            ViewBag.MenuCode = menuCode;
            ViewBag.AppID = appId;
            return View();
        }

        //public ActionResult WxITDetail(string appId, string menuCode)
        //{
        //    //如果未关注企业号
        //    if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
        //    {
        //        return Redirect("/notauthed.html");
        //    }

        //    if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(menuCode) ||
        //        !int.TryParse(appId, out AppId))
        //    {
        //        return Redirect("/notauthed.html");
        //    }

        //    if (((CategoryType)AppId) != CategoryType.ITHelpdeskCate)
        //    {
        //        return Redirect("/notauthed.html");
        //    }

        //    //记录用户行为
        //    ExecuteBehavior(int.Parse(appId), 7, "IT_FEEDBACK");
        //    ViewBag.MenuCode = menuCode;
        //    ViewBag.AppID = appId;
        //    return View();
        //}

        [HttpPost]
        public ActionResult Create(FeedBackView feedBackView)
        {
            string strContent = feedBackView.Content;
            string WeChatUserID = ViewBag.WeChatUserID;

            if (string.IsNullOrEmpty(strContent) || strContent.Trim().Length < 5)
            {
                return Json(new { str = "Insert Failed.", msg = "请输入您的问题，并且至少5个字符哦。" });
            }

            if (strContent.Length > 500)
            {
                return Json(new { str = "Insert Failed.", msg = "您的问题太长啦，老板日理万机，没空看哦。" });
            }
            
            if (string.IsNullOrEmpty(WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }

            feedBackView.FeedBackUserId = WeChatUserID;
            feedBackView.FeedBackDateTime = DateTime.Now;

            _BaseService.InsertView(feedBackView);

            return Json(new { str = "Insert Success.", msg = "用户反馈已成功提交!" });

        }
    }
}