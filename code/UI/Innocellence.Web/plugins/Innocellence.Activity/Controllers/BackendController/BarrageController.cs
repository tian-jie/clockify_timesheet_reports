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
    public class BarrageController : BaseController<Barrage, BarrageView>
    {
        public BarrageController(IBarrageService objService)
            : base(objService)
        {
            _objService = objService;
        }

        public ActionResult Check()
        {
            ViewBag.appid = Request["appid"];
            ViewBag.summaryId = Request["summaryId"];
            ViewBag.summaryType = Request["summaryType"];

            return View();
        }

        public override List<BarrageView> GetListEx(Expression<Func<Barrage, bool>> predicate, PageCondition ConPage)
        {
            var content = Request["FeedBackContent"];

            if (!string.IsNullOrEmpty(content))
            {
                predicate = predicate.AndAlso(a => a.FeedBackContent.Contains(content));
            }

            ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Ascending));

            var q = _objService.GetList<BarrageView>(predicate, ConPage);
            q.ForEach(t => { t.FeedBackContent = QyMessageHandlers.QyCustomMessageHandler.ConvertEmotion(t.FeedBackContent); });
            return q;
        }

        public override ActionResult Export()
        {

            Expression<Func<Barrage, bool>> predicate = x => x.Id > 0;

            var content = Request["FeedBackContent"];
            if (!string.IsNullOrEmpty(content))
            {
                predicate = predicate.AndAlso(a => a.FeedBackContent.Contains(content));
            }
            if (!string.IsNullOrEmpty(Request["SummaryId"]))
            {
                var id = int.Parse(Request["SummaryId"]);
                predicate = predicate.AndAlso(a => a.SummaryId == id);
            }
            if (!string.IsNullOrEmpty(Request["AppId"]))
            {
                var appid = Request["AppId"];
                predicate = predicate.AndAlso(a => a.AppId == appid);
            }

            var reportList = _objService.Repository.Entities.Where(predicate).ToList().
                Select(n => (BarrageView)(new BarrageView().ConvertAPIModel(n))).
                ToList().OrderBy(x => x.CreatedDate).ToList();

            return ExportToCsv(reportList);
        }

        private ActionResult ExportToCsv(List<BarrageView> list)
        {
            string[] headLine = { "Id", "FeedBackContent", "ApprovedDate", "WeixinName", "Keyword" };
            var csv = new CsvSerializer<BarrageView> { UseLineNumbers = false };
            var sRet = csv.SerializeStream(list, headLine);
            string fileName = "Barrage_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            return File(sRet, "text/comma-separated-values", fileName);
        }

        //Category 转换
        public JsonResult StatusChange(int appid, int summaryid)
        {
            var barrage = _objService.Repository.GetByKey(summaryid);

            if (barrage == null)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            barrage.Status = barrage.Status == 1 ? 0 : 1;
            if (barrage.Status == 0)
            {
                barrage.ApprovedDate = null;
            }
            else
            {
                barrage.ApprovedDate = DateTime.Now;
            }
            _objService.Repository.Update(barrage, new List<string>() { "Status", "ApprovedDate" });
            return SuccessNotification("Success");
        }

    }
}