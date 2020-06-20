using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Innocellence.WeChatMain.Controllers
{
    public partial class TextReportController : BaseController<ReportJobLog, ReportJobLogView>
    {
        IManualStartJobService _manualStartJobService = null;
        IReportJobLogService _reportJobLogService = null;
        public TextReportController(IManualStartJobService manualStartJobService, IReportJobLogService reportJobLogService)
            : base(reportJobLogService)
        {
            _manualStartJobService = manualStartJobService;
            _reportJobLogService = reportJobLogService;
        }

        public override ActionResult Index()
        {
            return View();
        }

        public override List<ReportJobLogView> GetListEx(
            Expression<Func<ReportJobLog, bool>> predicate, PageCondition ConPage)
        {
            predicate = predicate.AndAlso(a => JobStatus.Success.ToString().Equals(a.JobStatus));

            List<ReportJobLogView> list = new List<ReportJobLogView>();

            ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

            var resultList = _reportJobLogService.QueryJobLogListForView(predicate);

            list = CreateJobList(resultList);

            ConPage.RowCount = 4;
            ConPage.PageIndex = 1;
            ConPage.PageSize = 4;
            
            return list;
        }

        /// <summary>
        /// 创建Job信息列表
        /// </summary>
        /// <param name="jobLoglist">JobReport列表</param>
        /// <returns>Job信息列表</returns>
        private List<ReportJobLogView> CreateJobList(List<ReportJobLogView> jobLoglist)
        {
            List<ReportJobLogView> result = new List<ReportJobLogView>();

            ReportJobLogView menuReportJob = CreateJobItem(jobLoglist, JobName.MenuReportJob, 1);
            ReportJobLogView appAccessJob = CreateJobItem(jobLoglist, JobName.AppAccessReportJob, 2);
            ReportJobLogView articleReportJob = CreateJobItem(jobLoglist, JobName.ArticleReportJob, 3);
            ReportJobLogView PageReportJob = CreateJobItem(jobLoglist, JobName.PageReportJob, 4);
            result.Add(menuReportJob);
            result.Add(appAccessJob);
            result.Add(articleReportJob);
            result.Add(PageReportJob);
            return result;
        }

        /// <summary>
        /// 创建Job信息
        /// </summary>
        /// <param name="JobLoglist">JobReport列表</param>
        /// <param name="jobName">Job名称</param>
        /// <param name="customId">自定义ID</param>
        /// <returns>Job信息</returns>
        private ReportJobLogView CreateJobItem(List<ReportJobLogView> jobLoglist, JobName jobName, int customId)
        {
            ReportJobLogView reportJobLogView = new ReportJobLogView();
            reportJobLogView = jobLoglist.Where(x => jobName.ToString().Equals(x.JobName)).OrderByDescending(
                x => x.CreatedDate).Distinct().FirstOrDefault();
            if (reportJobLogView == null)
            {
                reportJobLogView = new ReportJobLogView()
                {
                    Id = customId,
                    JobName = jobName.ToString(),
                    DateTo = "没有成功统计过的数据"
                };
            }
            return reportJobLogView;
        }

        [AllowAnonymous]
        public ActionResult JobImplement(string jobNameList)
        {
            if (string.IsNullOrEmpty(jobNameList))
            {
                return ErrorNotification("请选择至少一个待执行的JOB!");
            }

            string[] nameList = jobNameList.Split(',');

            if (nameList.Length == 0)
            {
                return ErrorNotification("没有提供正确的JOB处理信息");
            }

            List<JobName> nameEnum = new List<JobName>();

            foreach (string item in nameList)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                JobName jobType = (JobName)Enum.Parse(typeof(JobName), item);
                nameEnum.Add(jobType);
            }

            _manualStartJobService.ManualStartJob(nameEnum);

            return SuccessNotification("操作执行完毕!");
        }
    }
}