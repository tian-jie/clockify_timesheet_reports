using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;

namespace Innocellence.WeChat.Domain.Service
{
    public class ReportJobLogService : BaseService<ReportJobLog>, IReportJobLogService
    {
       
        public List<ReportJobLogView> QueryJobLogListForView(Expression<Func<ReportJobLog, bool>> func)
        {
            return Repository.Entities.Where(func).AsNoTracking().ToList().Select(
                n => (ReportJobLogView)(new ReportJobLogView().ConvertAPIModel(n))).ToList();
        }

        public IList<ReportJobLog> QueryJobLogList(Expression<Func<ReportJobLog, bool>> func)
        {
            return Repository.Entities.Where(func).AsNoTracking().ToList();
        }

        public IQueryable<ReportJobLog> GetQueryable(Expression<Func<ReportJobLog, bool>> func)
        {
            return Repository.Entities.Where(func).AsNoTracking();
        }

        public ReportJobLog CreateJobLog(string jobName, ReportJobLog entity = null)
        {
            //手动启动时创建
            if (entity != null)
            {
                Repository.Insert(entity);
                return entity;
            }

            var newEnity = new ReportJobLog { JobStatus = JobStatus.Running.ToString(), JobName = jobName, CreatedDate = DateTime.Now };
            var success = JobStatus.Success.ToString();

            var jobEntity = GetQueryable(x => x.JobName == jobName && x.JobStatus == success).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

            if (jobEntity != null)
            {
                newEnity.DateFrom = jobEntity.DateTo;

                //TODO:check
                newEnity.DateTo = DateTime.Now.Date;
            }
            else
            {
                //1953
                newEnity.DateFrom = new DateTime(1953, 1, 1);
                newEnity.DateTo = DateTime.Now.Date;
            }

            Repository.Insert(newEnity);

            return newEnity;
        }

    }

    public enum JobStatus
    {
        Error,
        Running,
        Success,
    }
}
