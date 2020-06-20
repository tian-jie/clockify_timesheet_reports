using System;
using System.Net;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Innocellence.WeChat.Domain.ModelsView;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.QY.Entities;
using Infrastructure.Web.ImageTools;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts;
using System.IO;
using Infrastructure.Web.Domain.Service.Common;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChat.Controllers
{
    public partial class VideoController : WeChatBaseController<ArticleInfo, ArticleInfoView>
    {

        private IArticleInfoService _objService;

        public VideoController(IArticleInfoService objService)
            : base(objService)
        {
            _objService = objService;
            AppId = (int)CategoryType.VideoCate;
        }

        
        /// <summary>
        /// 微信前端网页
        /// </summary>
        /// <returns></returns>
        public ActionResult WxDetail(int id)
        {
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }

            var article = _objService.Repository.GetByKey(id);

            //控制未发布的和delete状态的文章不显示
            if (article.ArticleStatus != ConstData.STATUS_PUBLISH && article.IsDeleted.Value)
            {
                return Redirect("/ArticleInfo/invalid");
            }

            if (!string.IsNullOrEmpty(article.ArticleURL))
            {
                return Redirect(article.ArticleURL);
            }

            if (article == null)
            {
                throw new FileNotFoundException();
            }
            //记录用户行为
            ExecuteBehavior(article.AppId.Value, 1, "",id.ToString());
            var articleView = new ArticleInfoView().ConvertAPIModel(article);
            return View(articleView);
        }
    }
}
