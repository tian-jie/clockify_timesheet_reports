using System;
using System.Collections.Generic;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IArticleReportService : IDependency, IBaseService<ArticleReport>
    {
        List<ArticleReportView> QueryTable(DateTime stardate, DateTime enddate, PageCondition pageCondition);
        List<T> GetListByDate<T>(Expression<Func<ArticleReport, bool>> predicate) where T : IViewModel, new();
    }
}
