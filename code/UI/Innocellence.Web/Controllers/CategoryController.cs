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

namespace Innocellence.Web.Controllers
{
    public class CategoryController : BaseController<Category, CategoryView>
    {
        public readonly ICategoryService _categoryService;
        public readonly ISysConfigService _sysconfigService;

        public CategoryController(ICategoryService categoryService,
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

            var q = _BaseService.GetList<CategoryView>(predicate, ConPage);

            return q.ToList();
        }

        public override ActionResult Get(string id)
        {
            CategoryView modelView = new CategoryView();

            if (!string.IsNullOrEmpty(Request["id"]) && Request["id"] != "0")
            {
                //根据courseId拿lst
                var model = _objService.Repository.Entities.Where(a => a.CategoryCode == id).FirstOrDefault();
                if (model != null)
                {
                    modelView.Id = model.Id;
                    modelView.CategoryName = model.CategoryName;
                    modelView.CategoryCode = model.CategoryCode;
                    modelView.AppId = model.AppId;
                }
                else
                {
                    ModelState.AddModelError("NotFind", T("This data is not find"));
                }

            }

            return Json(modelView, JsonRequestBehavior.AllowGet);

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
