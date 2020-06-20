using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Text;
using Infrastructure.Utility.IO;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChatMain.Controllers
{
    public class AppAccessReportController : BaseController<AppAccessReport, AppAccessReportView>
    {
        IAppAccessReportService _appAccessReportService = null;
        public AppAccessReportController(IAppAccessReportService objService)
            : base(objService)
        {
            _appAccessReportService = objService;
        }


        public override ActionResult Index()
        {
            string appid = Request["appId"];

            if (!string.IsNullOrEmpty(appid))
            {
                AppId = int.Parse(appid);
                @ViewBag.AppId = appid;
            }
            else
            {
                AppId = (int)CategoryType.Undefined;
                @ViewBag.AppId = AppId;
            }

            ViewBag.Apps = WeChatCommonService.GetAppList();

            return View();
        }

        public ActionResult AppIndex()
        {
            string strId = Request["Id"];

            if (string.IsNullOrEmpty(strId) ||
                !int.TryParse(strId, out AppId))
            {
                throw new Exception("你必须接受错误，这是请求方式的苦难！");
            }

            @ViewBag.AppId = strId;

            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest req = new GridRequest(Request);
            Expression<Func<AppAccessReport, bool>> predicate = FilterHelper.GetExpression<AppAccessReport>(req.FilterGroup);

            string strStartTime = Request["StartTime"];
            string strEndTime = Request["EndTime"];
            string appCate = Request["appCate"];

            if (!CheckDate(strStartTime, strEndTime))
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(appCate))
            {
                var appId = int.Parse(appCate);
                predicate = predicate.AndAlso(a => a.AppId == appId);//实现按AppId过滤
            }

            var list = GetListPrivate(ref predicate, req.PageCondition, strStartTime, strEndTime);

            return GetPageResult(list, req);
        }

        protected List<AppAccessReportView> GetListPrivate(
            ref Expression<Func<AppAccessReport, bool>> predicate, PageCondition conPage, string startTime = "", string endTime = "")
        {
            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime dtstartTime = Convert.ToDateTime(startTime);
                predicate = predicate.AndAlso(x => x.AccessDate >= dtstartTime);
            }

            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime dtendTime = Convert.ToDateTime(endTime);
                predicate = predicate.AndAlso(x => x.AccessDate <= dtendTime);
            }

            conPage.SortConditions.Add(new SortCondition("AccessDate", System.ComponentModel.ListSortDirection.Descending));

            var q = _BaseService.GetList<AppAccessReportView>(predicate, conPage);

            return q;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public override ActionResult Export()
        {
            string strStartTime = Request["StartTime"];
            string strEndTime = Request["EndTime"];
            string appCate = Request["appCate"];
            return ExportCSV(strStartTime, strEndTime, appCate);
        }
        public ActionResult ExportCSV(string startTime, string endTime, string appCate)
        {
            string errorMessage = string.Empty;

            Expression<Func<AppAccessReport, bool>> predicate = x => true;


            if (!CheckDate(startTime, endTime))
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime dtstartTime = Convert.ToDateTime(startTime);
                predicate = predicate.AndAlso(x => x.AccessDate >= dtstartTime);
            }

            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime dtendTime = Convert.ToDateTime(endTime);
                predicate = predicate.AndAlso(x => x.AccessDate <= dtendTime);
            }

            if (!string.IsNullOrEmpty(appCate))
            {
                var appId = int.Parse(appCate);
                predicate = predicate.AndAlso(a => a.AppId == appId);//实现按AppId过滤
            }

            List<AppAccessReportView> reportList = _appAccessReportService.GetListByFromDate<AppAccessReportView>(predicate);
            //List<AppAccessReportView> reportList = _BaseService.GetList<AppAccessReportView>(predicate, ConPage);
            //if (reportList.Count <= 0)
            //{
            //    return ErrorNotification("选择的时间段没有App访问信息!");
            //}

            return exportToCSV(reportList);

        }
        private ActionResult exportToCSV(List<AppAccessReportView> appAccessReportList)
        {
            string[] headLine = { "AccessDate", "AppName", "AccessPerson", "AccessCount" };
            var lst = appAccessReportList;
            CsvSerializer<AppAccessReportView> csv = new CsvSerializer<AppAccessReportView>();
            csv.UseLineNumbers = false;
            var ms = csv.SerializeStream(lst, headLine);

            return File(ms, "text/plain", string.Format("{0}.csv", "appAccessReport_" + DateTime.Now.ToString("yyyMMddHHmmss")));

        }
        /// <summary>
        /// 验证日期
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private bool CheckDate(string startTime, string endTime)
        {
            bool result = true;
            StringBuilder errMsg = new StringBuilder();
            if (string.IsNullOrEmpty(startTime))
            {
                result = false;
                errMsg.Append(T("起始日期不能为空!<br/>"));
            }

            if (string.IsNullOrEmpty(endTime))
            {
                result = false;

                errMsg.Append(T("截止日期不能为空!<br/>"));
            }


            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                DateTime dtstartTime = Convert.ToDateTime(startTime);
                DateTime dtendTime = Convert.ToDateTime(endTime);
                if (dtstartTime > dtendTime)
                {
                    result = false;
                    errMsg.Append(T("截止日期不能早于起始日期!<br/>"));
                }

                if (dtendTime.AddDays(-60) > dtstartTime)
                {
                    result = false;
                    errMsg.Append(T("起始日期与截止日期间隔不能超过两个月!<br/>"));
                }
            }
            if (!result)
            {
                ModelState.AddModelError("Invalid Input", errMsg.ToString());
            }
            return result;
        }

    }

}
