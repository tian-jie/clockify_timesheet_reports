using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Binbin.Linq;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using WebBackgrounder;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.Common;

namespace Innocellence.WeChat.Domain.Service.job
{
    public class AppAccessReportJob : ICustomerJob
    {
        private readonly TimeSpan _interval;
        private readonly IReportJobLogService jobLogService = new ReportJobLogService();
        private readonly IUserBehaviorService userBehaviorService = new UserBehaviorService(new CodeFirstDbContext());
        private readonly IAppAccessReportService AppAccessReportService = new AppAccessReportService();
        private static readonly ILogger log = LogManager.GetLogger(typeof(AppAccessReportJob));
        private readonly string jobName = JobName.AppAccessReportJob.ToString();//"AppAccessReportJob";
        private bool _success = true;

        public AppAccessReportJob()
        {
            _interval = TimeSpan.FromHours(24);
        }

        public Task Execute()
        {
            return new Task(RunJob);
        }

        private void RunJob()
        {
            ReportJobLog jobLog = null;

            try
            {
                jobLog = jobLogService.CreateJobLog(jobName);
                ImportDataByTime(jobLog.DateFrom, jobLog.DateTo);
                jobLog.JobStatus = JobStatus.Success.ToString();
            }
            catch (InnocellenceException e)
            {
                if (jobLog != null)
                {
                    jobLog.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                    jobLog.JobStatus = JobStatus.Error.ToString();
                }

                log.Error<string>(string.Format("AppAccess report job error:{0}" + Environment.NewLine + "{1}", e.Message, e.StackTrace));
            }
            catch (Exception e)
            {
                if (jobLog != null)
                {
                    jobLog.ErrorMessage = e.Message;
                    jobLog.JobStatus = JobStatus.Error.ToString();
                }

                log.Error<string>(string.Format("AppAccess report job error:{0}" + Environment.NewLine + "{1}", e.Message, e.StackTrace));
            }
            finally
            {
                if (jobLog != null)
                {
                    jobLog.UpdatedDate = DateTime.Now;
                    jobLogService.Repository.Update(jobLog);
                }
            }
        }

        /// <summary>
        /// contentType 是 10
        /// </summary>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        private void ImportDataByTime(DateTime? fromDateTime, DateTime toDateTime)
        {
            IList<AppAccessReport> appAccessReportLogs = new List<AppAccessReport>();
            var predicate = PredicateBuilder.True<UserBehavior>();

            predicate = predicate.And(x => x.CreatedTime <= toDateTime && x.CreatedTime > fromDateTime);

            predicate = predicate.And(x => x.ContentType == 10);

            var logs = userBehaviorService.Repository.Entities.Where(predicate).ToList();//.AsParallel();


            logs.GroupBy(x => x.AppId).ToList().ForEach(appGroup => appGroup.GroupBy(x => x.CreatedTime.Date).ToList().ForEach(appAccess =>
           {
               var appAccessAction = appAccess.FirstOrDefault();
               if (appAccessAction == null) return;
               var app =
                   WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(
                       y => y.WeixinAppId == appAccessAction.AppId.ToString(CultureInfo.InvariantCulture));
               var appName = string.Empty;
               if (app == null)
               {
                   log.Info("没有找到app id 为{0}的app!", appAccessAction.AppId);
               }
               else
               {
                   appName = app.AppName;
               }
               var entity = new AppAccessReport
               {
                   AccessDate = appAccessAction.CreatedTime.Date,
                   AppId = appAccessAction.AppId,
                   AppName = appName,
                   CreatedDate = DateTime.Now,
                   AccessCount = appAccess.Count(),
                   AccessPerson = appAccess.GroupBy(x => x.UserId).Count()
               };
               appAccessReportLogs.Add(entity);
           }));
            AppAccessReportService.Repository.Insert(appAccessReportLogs.AsEnumerable());
        }

        public TimeSpan Interval
        {
            get { return _interval; }
        }

        public string Name
        {
            get { return "AppAccess Report Job"; }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.MaxValue; }
        }
        public void ManuallyRunJob()
        {
            RunJob();
        }

        public JobName JobName
        {
            get { return JobName.AppAccessReportJob; }
        }
        public bool Success
        {
            get { return _success; }
        }
    }
}
