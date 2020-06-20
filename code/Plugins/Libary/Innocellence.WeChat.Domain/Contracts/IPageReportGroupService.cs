using System.Linq.Expressions;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IPageReportGroupService : IDependency, IBaseService<PageReportGroup>
    {
        List<T> GetListByFromDate<T>(Expression<Func<PageReportGroup, bool>> predicate) where T : IViewModel, new();
    }

}
