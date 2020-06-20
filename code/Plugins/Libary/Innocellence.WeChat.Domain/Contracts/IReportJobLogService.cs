using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IReportJobLogService : IDependency, IBaseService<ReportJobLog>
    {
        List<ReportJobLogView> QueryJobLogListForView(Expression<Func<ReportJobLog, bool>> func);

        IList<ReportJobLog> QueryJobLogList(Expression<Func<ReportJobLog, bool>> func);

        IQueryable<ReportJobLog> GetQueryable(Expression<Func<ReportJobLog, bool>> func);

        ReportJobLog CreateJobLog(string jobName, ReportJobLog entity = null);
    }
}
