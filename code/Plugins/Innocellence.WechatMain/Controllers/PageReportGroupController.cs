using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain;



namespace Innocellence.WeChatMain.Controllers
{
    public partial class PageReportGroupController : BaseController<PageReportGroup, PageReportGroupView>
    {
        private IPageReportService _pageReportService;
        public PageReportGroupController(IPageReportGroupService objService, IPageReportService pageReportService)
            : base(objService)
        {
            _pageReportService = pageReportService;
        }

        public override ActionResult Index()
        {
            var corpId = CommonService.GetSysConfig("WeixinCorpId", string.Empty);
            var lst = WeChatCommonService.lstSysWeChatConfig.Where(a => a.WeixinCorpId == corpId);
            ViewBag.Apps = lst;
            return View();
        }

        public override List<PageReportGroupView> GetListEx(Expression<Func<PageReportGroup, bool>> predicate, PageCondition ConPage)
        {

            var q = _objService.GetList<PageReportGroupView>(predicate, ConPage);

            var corpId = CommonService.GetSysConfig("WeixinCorpId", string.Empty);
            var allApplist = WeChatCommonService.lstSysWeChatConfig.Where(a => a.WeixinCorpId == corpId);

            q.ForEach(item =>
            {
                var appInfo = allApplist.FirstOrDefault(x => item.AppId.HasValue && x.WeixinAppId.Equals(item.AppId.Value.ToString()));
                item.AppName = appInfo != null ? appInfo.AppName : string.Empty;
            });

            return q;
        }

        public override bool BeforeAddOrUpdate(PageReportGroupView objModal, string Id)
        {
            bool validate = true;
            StringBuilder errMsg = new StringBuilder();

            if (!objModal.AppId.HasValue)
            {
                validate = false;
                errMsg.Append(T("Please change app name.<br/>"));
            }

            if (string.IsNullOrEmpty(objModal.GroupName))
            {
                validate = false;
                errMsg.Append(T("Please input group name.<br/>"));
            }

            if (string.IsNullOrEmpty(objModal.PageUrl))
            {
                validate = false;
                errMsg.Append(T("Please input page Url.<br/>"));
            }

            //string[] urlList = objModal.PageUrl.Split(',');
            //Regex reg = new Regex(@"(?<![\w@]+)((http|https)://)?(www.)?[a-z0-9\.]+(\.(com|net|cn|com\.cn|com\.net|net\.cn))(/[^\s\n]*)?");
            //foreach (string url in urlList)
            //{
            //    if (!reg.IsMatch(url))
            //    {
            //        validate = false;
            //        errMsg.Append(T("Please input true page Url.<br/>"));
            //        break;
            //    }
            //}

            if (!validate)
            {
                ModelState.AddModelError("Invalid Input", errMsg.ToString());
            }

            return validate;
        }

        [HttpPost]
        [ValidateInput(true)]
        public override JsonResult Post(PageReportGroupView objModal, string Id)
        {

            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id))
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(Id) || "0".Equals(Id))
            {
                DateTime now = DateTime.Now;
                objModal.CreatedDate = now;
                objModal.UpdatedDate = now;

                _BaseService.InsertView(objModal);
            }
            else
            {
                DateTime now = DateTime.Now;
                objModal.UpdatedDate = now;
                _BaseService.UpdateView(objModal);

            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportDetail()
        {
            return View();
        }


        public ActionResult GetPageReportList()
        {
            //need group id
            GridRequest req = new GridRequest(Request);
            Expression<Func<PageReport, bool>> predicate = FilterHelper.GetExpression<PageReport>(req.FilterGroup);
            int iCount = req.PageCondition.RowCount;
            var id = 0;


            if (Int32.TryParse(Request["groupId"], out id))
            {
                var appName = string.Empty;
                var pageGroup = _objService.Repository.GetByKey(id);
                var corpId = CommonService.GetSysConfig("WeixinCorpId", string.Empty);
                appName = WeChatCommonService.lstSysWeChatConfig.Where(a => a.WeixinCorpId == corpId).SingleOrDefault(p => p.WeixinAppId.Equals(pageGroup.AppId.ToString())).AppName;
                List<string> strs = new List<string>(pageGroup.PageUrl.ToLower().Split(','));
                predicate = predicate.AndAlso(a => a.Appid == pageGroup.AppId && strs.Contains(a.PageUrl.ToLower()));
                var list = _pageReportService.GetReportList(predicate, req.PageCondition);
                list.ForEach(item =>
                {
                    item.AppName = appName;
                    item.GroupName = pageGroup.GroupName;
                });
                return GetPageResult(list, req);
            }
            else
            {
                return GetPageResult(null, req);
            }



        }

    }

}
