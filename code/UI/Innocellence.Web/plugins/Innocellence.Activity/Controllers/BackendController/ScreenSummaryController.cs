using Infrastructure.Utility.Data;
using Infrastructure.Utility.IO;
using Infrastructure.Web.UI;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.Entity;
using Innocellence.Activity.Services;
using Innocellence.WeChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;


namespace Innocellence.Activity.Controllers
{
    public class ScreenSummaryController : BaseController<BarrageSummary, BarrageSummaryView>
    {
        private static object _lock=new Object();

        public ScreenSummaryController(IBarrageSummaryService objService)
            : base(objService)
        {
            _objService = objService;
        }

        public override ActionResult Index()
        {
            ViewBag.appid = Request["Appid"];
            return View();
        }

        public override List<BarrageSummaryView> GetListEx(Expression<Func<BarrageSummary, bool>> predicate, PageCondition ConPage)
        {
            var title = Request["Title"];
            var appid = Request["AppId"];
            var summaryType = Request["type"];

            predicate = predicate.AndAlso(a => a.IsDeleted != true && a.AppId == appid && a.SummaryType.Equals(summaryType));

            if (!string.IsNullOrEmpty(title))
            {
                predicate = predicate.AndAlso(a => a.Title.Contains(title) || a.Keyword.Contains(title));
            }

            ConPage.SortConditions.Add(new SortCondition("CreatedDate",
                System.ComponentModel.ListSortDirection.Descending));

            var q = _objService.GetList<BarrageSummaryView>(predicate, ConPage);
            return q;
        }

        public override ActionResult Export()
        {

            Expression<Func<BarrageSummary, bool>> predicate = x => x.Id > 0;

            var title = Request["Title"];
            var appid = Request["AppId"];
            var type = Request["SummaryType"];

            predicate = predicate.AndAlso(a => a.IsDeleted != true && a.AppId == appid);

            if (!string.IsNullOrEmpty(title))
            {
                predicate = predicate.AndAlso(a => a.Title.Contains(title) || a.Keyword.Contains(title));
            }

            if (!string.IsNullOrEmpty(type))
            {
                predicate = predicate.AndAlso(a => a.SummaryType.Equals(type));
            }

            var reportList = _objService.Repository.Entities.Where(predicate).ToList().
                Select(n => (BarrageSummaryView)(new BarrageSummaryView().ConvertAPIModel(n))).
                ToList().OrderByDescending(x => x.CreatedDate).ToList();

            return ExportToCsv(reportList);
        }

        private ActionResult ExportToCsv(List<BarrageSummaryView> list)
        {
            string[] headLine = { "Id", "Title", "Keyword", "CreatedDate" };
            var csv = new CsvSerializer<BarrageSummaryView> { UseLineNumbers = false };
            var sRet = csv.SerializeStream(list, headLine);
            string fileName = "ScreenSummary_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            return File(sRet, "text/comma-separated-values", fileName);
        }

        public override ActionResult Edit(string id)
        {
            ViewBag.appid = Request["appId"];
            var obj = GetObject(id);

            return View(obj);
        }

        //Post方法
        [HttpPost]
        [ValidateInput(false)]
        public override JsonResult Post(BarrageSummaryView objModal, string Id)
        {
            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            InsertOrUpdate(objModal, Id);

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        protected void InsertOrUpdate(BarrageSummaryView objModal, string Id)
        {
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                objModal.IsEnabled = false;
                objModal.SummaryType = BarrageSummary.SummaryTypes.Screen.ToString();
                _BaseService.InsertView(objModal);
            }
            else
            {
                var lst = new List<string>() { "Title", "Keyword", "ReturnText"};
                _BaseService.UpdateView(objModal, lst);
            }
        }

        public JsonResult ChangeBarrageStatus(string id, string appid)
        {
            int iRet = 0;
            var objModel = _BaseService.Repository.GetByKey(int.Parse(id));

            if (!objModel.IsEnabled.GetValueOrDefault())
            {
                objModel.IsEnabled = true;
                lock (_lock)
                {
                    var obj=_objService.Repository.Entities.FirstOrDefault(a => a.AppId == appid && a.IsDeleted == false &&
                                                                    a.IsEnabled != false && a.Keyword.Equals(objModel.Keyword));
                    if (obj != null)
                    {
                        return SuccessNotification("关键字重复，请确保微信弹幕和提问屏幕中没有已启用的同名关键字！");
                    }
                }
            }
            else
            {
                objModel.IsEnabled =false;
            }

            iRet = _BaseService.Repository.Update(objModel, new List<string>() { "IsEnabled" });

            if (iRet > 0)
            {
                return SuccessNotification("操作成功");
            }
            else
            {
                return ErrorNotification("操作失败");
            }
        }
    }
}