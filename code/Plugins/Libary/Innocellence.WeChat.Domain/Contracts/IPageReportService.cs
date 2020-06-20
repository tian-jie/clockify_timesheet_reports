using System.Linq.Expressions;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using Innocellence.WeChat.Domain.ModelsView;
using Infrastructure.Utility.Data;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IPageReportService : IDependency, IBaseService<PageReport>
    {
        List<T> GetListByFromDate<T>(Expression<Func<PageReport, bool>> predicate) where T : IViewModel, new();
        List<PageReportView> GetReportList(Expression<Func<PageReport, bool>> predicate,
              PageCondition con);
    }

}
