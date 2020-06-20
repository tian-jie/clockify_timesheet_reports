using System;
using System.Collections.Generic;
using System.Linq;
using Binbin.Linq;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Service.job
{
    public class ManualStartJobService : IManualStartJobService
    {
        private static readonly ManuallyJobManager _JobManager = new ManuallyJobManager();
        private readonly IReportJobLogService _reportJobLogService = new ReportJobLogService();
        //private static IList<JobName> jobLogs = new List<JobName> { JobName.AppAccessJob, JobName.ArticleReportJob, JobName.MenuReportJob };

        public IList<ReportJobLog> ManualStartJob(IList<JobName> runJobs)
        {
            foreach (var job in runJobs.Select(jobName => _JobManager.GetJob(jobName)).Where(job => job != null))
            {
                job.ManuallyRunJob();
            }

            var predicate = PredicateBuilder.False<ReportJobLog>();

            predicate = (from object job in Enum.GetValues(typeof(JobName)) select job.ToString()).Aggregate(predicate, (current, jobName) => current.Or(x => x.JobName == jobName));

            var status = JobStatus.Success.ToString();
            predicate = predicate.And(x => x.JobStatus == status);

            return _reportJobLogService.GetQueryable(predicate).OrderByDescending(x => x.CreatedDate).GroupBy(x => x.JobName).Select(x => x.FirstOrDefault()).ToList();
        }

    }
}
