using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Infrastructure.Utility.IO;
using Infrastructure.Web.UI;
using Innocellence.WeChatMain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChatMain.Controllers
{
    public partial class WechatFollowReportController : BaseController<WechatFollowReport, WechatFollowReportView>
    {
        IWechatFollowReportService _wechatFollowReportService = null;

        public WechatFollowReportController(IWechatFollowReportService wechatFollowReportService)
            : base(wechatFollowReportService)
        {
            _wechatFollowReportService = wechatFollowReportService;
        }

        public override ActionResult Index()
        {
            return View();
        }

        public override ActionResult GetList()
        {
            string errorMessage = string.Empty;
            string strStartTime = Request["txtStartTime"];
            string strEndTime = Request["txtEndTime"];
            if (!CheckDate(strStartTime, strEndTime, out errorMessage))
            {
                return ErrorNotification(errorMessage);
            }
            
            GridRequest req = new GridRequest(Request);
            Expression<Func<WechatFollowReport, bool>> predicate = FilterHelper.GetExpression<WechatFollowReport>(req.FilterGroup);
            int iCount = req.PageCondition.RowCount;

            var list = GetListEx(predicate, req.PageCondition);


            return GetPageResult(list, req);
        }

        public override List<WechatFollowReportView> GetListEx(
            Expression<Func<WechatFollowReport, bool>> predicate, PageCondition ConPage)
        {
            string strStartTime = Request["txtStartTime"];
            string strEndTime = Request["txtEndTime"];
            List<WechatFollowReportView> list = new List<WechatFollowReportView>();

            if (string.IsNullOrEmpty(strStartTime) || string.IsNullOrEmpty(strEndTime))
            {
                return list;
            }

            list = GetListPrivate(ref predicate, ConPage, strStartTime, strEndTime);
            
            return list;
        }

        protected List<WechatFollowReportView> GetListPrivate(
            ref Expression<Func<WechatFollowReport, bool>> predicate, PageCondition ConPage, string startTime = "", string endTime = "")
        {
            
            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime dtstartTime = Convert.ToDateTime(startTime);
                predicate = predicate.AndAlso(x => x.StatisticsDate >= dtstartTime);                
            }

            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime dtendTime = Convert.ToDateTime(endTime);
                predicate = predicate.AndAlso(x => x.StatisticsDate <= dtendTime);
            }

            ConPage.SortConditions.Add(new SortCondition("StatisticsDate", System.ComponentModel.ListSortDirection.Descending));

            var q = _BaseService.GetList<WechatFollowReportView>(predicate, ConPage);

            return q;
        }

        /// <summary>
        /// 日期验证
        /// </summary>
        /// <param name="startTime">起始日期</param>
        /// <param name="endTime">截止日期</param>
        /// <param name="errorMessage">验证失败时的错误信息</param>
        /// <returns></returns>
        private bool CheckDate(string startTime, string endTime, out string errorMessage)
        {
            bool result = true;
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(startTime))
            {
                result = false;
                errorMessage = "起始日期不能为空!";
                throw new Exception(errorMessage);
            }

            if (string.IsNullOrEmpty(endTime))
            {
                result = false;
                errorMessage = "截止日期不能为空!";
                throw new Exception(errorMessage);
            } 
            
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                DateTime dtstartTime = Convert.ToDateTime(startTime);
                DateTime dtendTime = Convert.ToDateTime(endTime);
                if (dtstartTime > dtendTime)
                {
                    result = false;
                    errorMessage = "截止日期不能早于起始日期!";
                    throw new Exception(errorMessage);
                }

                if (dtendTime.AddDays(-60) > dtstartTime)
                {
                    result = false;
                    errorMessage = "起始日期与截止日期间隔不能超过两个月!";
                    throw new Exception(errorMessage);
                }
            }
            return result;
        }

        public override ActionResult Export()
        {
            string strStartTime = Request["txtStartTime"];
            string strEndTime = Request["txtEndTime"];
            return ExportCSV(strStartTime, strEndTime);
        }

        /// <summary>
        /// CSV文件出力
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">终了时间</param>
        /// <returns></returns>
        public ActionResult ExportCSV(string startTime, string endTime)
        {
            string errorMessage = string.Empty;

            Expression<Func<WechatFollowReport, bool>> predicate = x => true;

            if (!CheckDate(startTime, endTime, out errorMessage))
            {
                return ErrorNotification(errorMessage);
            }

            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime dtstartTime = Convert.ToDateTime(startTime);
                predicate = predicate.AndAlso(x => x.StatisticsDate >= dtstartTime);
            }

            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime dtendTime = Convert.ToDateTime(endTime);
                predicate = predicate.AndAlso(x => x.StatisticsDate <= dtendTime);
            }

            List<WechatFollowReportView> reportList = _wechatFollowReportService.GetListByFromStatisticsDate<WechatFollowReportView>(predicate);

            //if (reportList.Count <= 0)
            //{
            //    throw new Exception("选择的时间段没有关注信息!");
            //}

            return exportToCSV(reportList);
        }

        private ActionResult exportToCSV(List<WechatFollowReportView> wechatFollowReportList)
        {
            string[] headLine = { "StatisticsDate", "FollowCount", "UnFollowCount" };
            CsvSerializer<WechatFollowReportView> csv = new CsvSerializer<WechatFollowReportView>();
            csv.UseLineNumbers = false;
            string sRet = csv.Serialize(wechatFollowReportList, headLine);

            byte[] bits = System.Text.Encoding.UTF8.GetBytes(sRet); 
            string fileName = "WeChatFollowReport_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            return File(bits, "text/comma-separated-values", fileName);            
        }

        [AllowAnonymous]
        public ActionResult ReportGenerate()
        {
            WechatFollowReportType result = WechatFollowReportCommon.WechatFollowReportWork(AccountManageID);

            if (result == WechatFollowReportType.Success)
            {
                return SuccessNotification("数据统计成功！");
            }
            else if (result == WechatFollowReportType.AlreadyCreated)
            {
                return ErrorNotification("今天已经进行过统计，请明天再试！");
            }
            else
            {
                return ErrorNotification("系统发生错误！无法进行统计！");
            }

        }

    }

}
