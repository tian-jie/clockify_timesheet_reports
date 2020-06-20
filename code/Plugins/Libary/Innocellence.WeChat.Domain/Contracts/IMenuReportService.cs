using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IMenuReportService : IDependency, IBaseService<MenuReport>
    {
        List<MenuReportView> QueryTable(DateTime stardate, DateTime enddate, PageCondition pageCondition);

        IList<MenuReport> QueryList(Expression<Func<MenuReport, bool>> func);

        List<T> GetListByDate<T>(Expression<Func<MenuReport, bool>> predicate) where T : IViewModel, new();
    }
}
