using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChatMain.Controllers
{
    public partial class ArticleImagesController : BaseController<ArticleImages, ArticleImagesView>
    {

        public ArticleImagesController(IArticleImagesService objService, int appId)
            : base(objService)
        {
            _objService = objService;
            AppId = appId;
            ViewBag.AppId = AppId;
        }

        public ArticleImagesController(IArticleImagesService objService)
            : base(objService)
        {
            _objService = objService;
            AppId = (int)CategoryType.Undefined;
        }

        /// <summary>
        /// 修改为公共通用的模块
        /// </summary>
        /// <param name="appId">应用标识</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Index(int? appId)
        {
            AppId = appId.GetValueOrDefault();
            ViewBag.AppId = AppId;
            return View("../ArticleImages/Index");
        }

        /// <summary>
        /// 图片信息List取得
        /// </summary>
        /// <param name="predicate">条件简化查询语句</param>
        /// <param name="ConPage">排序方式</param>
        /// <returns>结果集</returns>
        public override List<ArticleImagesView> GetListEx(Expression<Func<ArticleImages, bool>> predicate, PageCondition ConPage)
        {
            string strCreateBy = Request["txtCreateBy"];
            string txtDate = Request["txtDate"];
            string appId = Request["AppId"];

            if (!string.IsNullOrEmpty(appId) && int.TryParse(appId, out AppId))
            {
                predicate = predicate.AndAlso(a => a.AppId == AppId);
            }

            if (!string.IsNullOrEmpty(txtDate))
            {
                DateTime dateTime = Convert.ToDateTime(txtDate);
                DateTime dateAdd = dateTime.AddDays(1);
                predicate = predicate.AndAlso(a => a.CreatedDate >= dateTime && a.CreatedDate <= dateAdd);
            }

            if (!string.IsNullOrEmpty(strCreateBy))
            {
                predicate = predicate.AndAlso(a => strCreateBy.Equals(a.UploadedUserId));
            }
            
            ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));
            var q = _BaseService.GetList<ArticleImagesView>(predicate, ConPage);
            return q;
        }
        

    }
}
