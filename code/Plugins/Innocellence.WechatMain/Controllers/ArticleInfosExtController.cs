
using System.Web.Mvc;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.CommonAPIs;
using Innocellence.Weixin.QY.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Web.UI;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using System;
using Infrastructure.Utility.IO;
using Infrastructure.Utility.Filter;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChatMain.Common;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.Weixin.Entities;

namespace Innocellence.WeChatMain.Controllers
{

    

  

    public partial class AdminAppController : ArticleInfoController
    {
        private static string wechatBaseUrl = CommonService.GetSysConfig("WeChatUrl", "");
        private IArticleInfoService _objService;
        public AdminAppController(IArticleInfoService objService, IArticleThumbsUpService articleThumbsUpService)
            : base(objService,  articleThumbsUpService)
        {
            _objService = objService;
        }

        public List<Category> rtn_list(List<Category> lstCate)
        {
            //获取所有的category
            List<Category> lstAll = CommonService.GetCategory(false).ToList();
            List<Category> lstnew = new List<Category>();
            lstnew.AddRange(lstCate);
            lstCate.ForEach(item =>
                {
                    var parent = lstAll.SingleOrDefault(a => a.Id == item.ParentCode);
                    if (parent != null)
                    {
                        if (!lstnew.Contains(parent))
                        {
                            lstnew.Add(parent);
                        }
                    }
                });

         
            //追加APP为一级节点
            var mixCategoryList = AppStructReconstitution(lstnew);

            // return lstnew.OrderBy(a => a.CategoryOrder).ToList();
            return mixCategoryList.OrderBy(a => a.CategoryOrder).ToList();
        }

        private List<Category> AppStructReconstitution(List<Category> outAppCategoryList)
        {
            List<Category> asrList = new List<Category>();

            if (outAppCategoryList == null || outAppCategoryList.Count == 0)
            {
                return asrList;
            }
            // 获取可用APP列表
            var corpId = CommonService.GetSysConfig("WeixinCorpId", string.Empty);
            var allApplist = WeChatCommonService.lstSysWeChatConfig.Where(a => a.WeixinCorpId == corpId).ToList();

            var appIdList = outAppCategoryList.Select(x => x.AppId).Distinct();

            var appList = allApplist.Where(x => appIdList.Contains(int.Parse(x.WeixinAppId))).ToList();
            // App内容改写为伪Category内容
            appList.ForEach(item =>
                {
                    Category category = new Category
                    {
                        // appId的负值作为Id,用于作为真Category一级菜单的父节点Id
                        Id = -Convert.ToInt32(item.WeixinAppId),
                        CategoryCode = string.Empty,
                        CategoryName = item.AppName,
                        ParentCode = 0,
                        // Id列作为排序条件
                        CategoryOrder = item.Id
                    };

                    if (!asrList.Contains(category))
                    {
                        asrList.Add(category);
                    }

                });

            outAppCategoryList.ForEach(item =>
                {
                    Category category = new Category();
                    category.Id = item.Id;
                    category.CategoryCode = item.CategoryCode;
                    category.CategoryName = item.CategoryName;
                    category.ParentCode = item.ParentCode;
                    category.CategoryOrder = item.CategoryOrder;
                    // 原一级节点的父节点修改为APP变换后的伪category的id
                    if (item.ParentCode == 0)
                    {
                        category.ParentCode = -item.AppId;
                    }

                    if (!asrList.Contains(category))
                    {
                        asrList.Add(category);
                    }
                });

            return asrList;
        }

        //[ActionName("AdminAppIndex")]
        public override ActionResult Index(int? appId)
        {
            List<Category> lstCate = CommonService.GetCategory(false).Where(a => a.IsAdmin.Value).ToList();
            //获取所有的category


            ViewBag.lstCate = rtn_list(lstCate);

            return View("../ArticleInfo/AdminIndex");
        }

        [HttpGet]
        public override ActionResult getTreeData(int? appid)
        {
            List<Category> lstCate = CommonService.GetCategory(false).Where(a => a.IsAdmin.Value).ToList();


            var listReturn = EasyUITreeData.GetTreeData(rtn_list(lstCate), "Id", "CategoryName", "ParentCode");

            return Json(listReturn, JsonRequestBehavior.AllowGet);

        }

        public override ActionResult Edit(string id, int? appId)
        {
            List<Category> lstCate = CommonService.GetCategory(false).Where(a => a.IsAdmin.Value).ToList();
            //获取所有的category


            ViewBag.lstCate = rtn_list(lstCate);

            var obj = GetObject(id);
            ViewBag.cateId = obj.CategoryId;
            //ViewBag.cateId = string.Empty;
            //if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(obj.ArticleCateSub))
            //{
            //    var cateInfo = CommonService.lstCategory.FirstOrDefault(a =>
            //        obj.ArticleCateSub.Equals(a.CategoryCode) && a.AppId == obj.AppId);

            //    ViewBag.cateId = cateInfo != null ? cateInfo.Id.ToString() : string.Empty;
            //}

            return View("../ArticleInfo/Edit", obj);
        }

        //public override List<ArticleInfoView> GetListEx(Expression<System.Func<ArticleInfo, bool>> predicate, PageCondition ConPage)
        //{
        //    string strTitle = Request["txtArticleTitle"];
        //    //string strNewsCate = AppId.ToString();
        //    string txtDate = Request["txtDate"];
        //    string strSubCate = Request["txtSubCate"];

        //    predicate = predicate.AndAlso(a => a.IsDeleted == false);

        //    if (!string.IsNullOrEmpty(strTitle))
        //    {
        //        predicate = predicate.AndAlso(a => a.ArticleTitle.Contains(strTitle));
        //    }

        //    if (!string.IsNullOrEmpty(txtDate))
        //    {
        //        DateTime dateTime = Convert.ToDateTime(txtDate);
        //        DateTime dateAdd = dateTime.AddDays(1);
        //        predicate = predicate.AndAlso(a => a.PublishDate >= dateTime && a.PublishDate <= dateAdd);
        //    }

        //    if (!string.IsNullOrEmpty(strSubCate))
        //    {
        //        // ID变更为CategoryCode
        //        //var cateInfo = CommonService.lstCategory.FirstOrDefault(a => a.Id == Convert.ToInt32(strSubCate)
        //        //    && a.IsDeleted == false);

        //        //string categoryCode = cateInfo == null ? string.Empty : cateInfo.CategoryCode;
        //        //predicate = predicate.AndAlso(a => a.ArticleCateSub == strSubCate);
        //        predicate = predicate.AndAlso(a => a.CategoryId == Convert.ToInt32(strSubCate));

        //    }
        //    var codes = (from item in CommonService.GetCategory(false).Where(a => a.IsAdmin.Value ).ToList() select item.CategoryCode).ToList();
        //    predicate = predicate.AndAlso(a => codes.Contains(a.ArticleCateSub) && !string.IsNullOrEmpty(a.ArticleCateSub));
          
        //    ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

        //    var q = _BaseService.GetList<ArticleInfoView>(predicate, ConPage);

        //    // CategoryCode转变换CategoryName
        //    var lstCate = CommonService.GetCategory(false);

        //    q.ForEach(item =>
        //    {
        //        Category category = lstCate.Where(x => !string.IsNullOrEmpty(x.CategoryCode) &&
        //      x.CategoryCode.Equals(item.ArticleCateSub)).Distinct().FirstOrDefault();
        //        item.ArticleCateSub = category == null ? string.Empty : category.CategoryName;
        //        var app =
        //         WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(
        //             y => y.WeixinAppId == item.AppId.ToString());
        //        item.APPName = app == null ? string.Empty : app.AppName;
        //    }
        //        );
           
        //    return q;

        //}

        //[HttpPost]
        //[ValidateInput(false)]
        //public override JsonResult Post(ArticleInfoView objModal, string Id)
        //{
        //    var appId = GetAppID(int.Parse(objModal.ArticleCateSub));
        //    if (appId == null)
        //    {
        //        ModelState.AddModelError("ArticleCateSub", @"The app of under you selected category does not exist!");
        //    }
        //    else
        //    {
        //        objModal.AppId = appId;
        //    }

        //    return base.Post(objModal, Id);
        //}

        public override ActionResult Export()
        {
            var req = new GridRequest(Request);
            var profilePredicate = FilterHelper.GetExpression<ArticleInfo>(req.FilterGroup);
            string strTitle = Request["txtArticleTitle"];
            //string strNewsCate = AppId.ToString();
            string txtDate = Request["txtDate"];
            string strSubCate = Request["txtSubCate"];

            profilePredicate = profilePredicate.AndAlso(a => a.IsDeleted == false);

            if (!string.IsNullOrEmpty(strTitle))
            {
                profilePredicate = profilePredicate.AndAlso(a => a.ArticleTitle.Contains(strTitle));
            }

            if (!string.IsNullOrEmpty(txtDate))
            {
                DateTime dateTime = Convert.ToDateTime(txtDate);
                DateTime dateAdd = dateTime.AddDays(1);
                profilePredicate = profilePredicate.AndAlso(a => a.PublishDate >= dateTime && a.PublishDate <= dateAdd);
            }

            if (!string.IsNullOrEmpty(strSubCate))
            {
                // ID变更为CategoryCode
                //var cateInfo = CommonService.lstCategory.FirstOrDefault(a => a.Id == Convert.ToInt32(strSubCate)
                //    && a.IsDeleted == false);

                //string categoryCode = cateInfo == null ? string.Empty : cateInfo.CategoryCode;
                //predicate = predicate.AndAlso(a => a.ArticleCateSub == strSubCate);
                profilePredicate = profilePredicate.AndAlso(a => a.CategoryId == Convert.ToInt32(strSubCate));

            }
            var ids = (from item in CommonService.GetCategory(false).Where(a => a.IsAdmin.Value).ToList() select item.Id).ToList();
            profilePredicate = profilePredicate.AndAlso(a => a.CategoryId != null && ids.Contains(a.CategoryId.Value));

            var list = _objService.GetList<ArticleInfoView>(0, profilePredicate, new List<SortCondition>() { new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending) });

            return exportToCSV(list);
        }
        //[HttpPost]
        //[ValidateInput(false)]
        //public  override  JsonResult WxPreview( ArticleInfoView objModal)
        //{
        //    var appId = GetAppID(int.Parse(objModal.ArticleCateSub));
        //    if (appId == null)
        //    {
        //        ModelState.AddModelError("ArticleCateSub", @"The app of under you selected category does not exist!");
        //    }
        //    else
        //    {
        //        objModal.AppId = appId;
        //    }
        //    return base.WxPreview(objModal);
        //}

        private static int? GetAppID(int categoryId)
        {
            var category = CommonService.lstCategory.FirstOrDefault(x => x.Id == categoryId);
            return category != null ? category.AppId : null;
        }
        private ActionResult exportToCSV(List<ArticleInfoView> ReportList)
        {
            string[] headLine = { "Id", "ArticleTitle", "ReadCount", "PublishDate" };
            CsvSerializer<ArticleInfoView> csv = new CsvSerializer<ArticleInfoView>();
            csv.UseLineNumbers = false;
            var sRet = csv.SerializeStream(ReportList, headLine);
            string fileName = "System_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            return File(sRet, "text/comma-separated-values", fileName);
        }
    }

    
}
