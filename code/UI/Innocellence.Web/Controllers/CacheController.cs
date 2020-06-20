using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Service.Common;
using Infrastructure.Web.Domain.Service;
using Innocellence.CA.Services;
using System.Runtime.Caching;

namespace Innocellence.Web.Controllers
{
    public class CacheController : BaseController<Category, CategoryView>
    {
        public readonly ICategoryService _categoryService;
        public readonly ISysConfigService _sysconfigService;

        public CacheController(ICategoryService categoryService,
            ISysConfigService sysconfigService)
            : base(categoryService)
        {
            _categoryService = categoryService;
            _sysconfigService = sysconfigService;
        }

        public override ActionResult Index()
        {
            var corpId = CommonService.GetSysConfig("WeixinCorpId", string.Empty);
          //  var lst = CommonService.lstSysWeChatConfig.Where(a => a.WeixinCorpId == corpId).ToList();

          //  ViewBag.Apps = lst;
            ViewBag.CateType = Request["CateType"];
            return base.Index();
        }

        //初始化list页面
        public override List<CategoryView> GetListEx(Expression<Func<Category, bool>> predicate, PageCondition ConPage)
        {
            string strCateType = Request["CateType"];
            string appId = Request["txtSubCate"];

            predicate = predicate.AndAlso(a => a.IsDeleted == false);

            if (!string.IsNullOrEmpty(appId))
            {
                int iCateType = int.Parse(appId);
                predicate = predicate.AndAlso(a => a.AppId == iCateType);
            }

            // ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

           List <CategoryView> cacheKeys = MemoryCache.Default.Select(kvp => new CategoryView() {  CategoryName=kvp.Key }).ToList();

            ConPage.RowCount = cacheKeys.Count;

            return cacheKeys;
        }

        public  ActionResult Clear(string strKey)
        {
            List<string> cacheKeys = MemoryCache.Default.Where(a=>a.Key== strKey).Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                System.Runtime.Caching.MemoryCache.Default.Remove(cacheKey);
            }

            return SuccessNotification("");

        }

        //Post方法
        public override JsonResult Post(CategoryView objModal, string Id)
        {
            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                _BaseService.InsertView(objModal);
            }
            else
            {
                _BaseService.UpdateView(objModal);
            }

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public override bool BeforeAddOrUpdate(CategoryView objModal, string Id)
        {
            bool validate = true;
            if (string.IsNullOrEmpty(objModal.CategoryName))
            {
                ModelState.AddModelError("titleEmpty", "标题不能为空");
                validate = false;
            }

            if (objModal.CategoryCode == null)
            {
                ModelState.AddModelError("codeEmpty", "CategoryCode不能为空");
                validate = false;
            }

            if (objModal.AppId == null)
            {
                ModelState.AddModelError("codeTypeEmpty", "CategoryTypeCode不能为空");
                validate = false;
            }
            return validate;
        }

        public override bool AfterDelete(string sIds)
        {
            // CommonService.ClearCache(1);
            return true;
        }
    }
}
