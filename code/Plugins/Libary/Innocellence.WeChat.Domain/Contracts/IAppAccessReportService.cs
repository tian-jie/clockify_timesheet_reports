using System.Linq.Expressions;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IAppAccessReportService : IDependency, IBaseService<AppAccessReport>
    {
        List<T> GetListByFromDate<T>(Expression<Func<AppAccessReport, bool>> predicate) where T : IViewModel, new();
    }

}
