using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Entity;
using System.Linq.Expressions;
using Innocellence.WeChat.Domain.ViewModel;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IAccessDashboardService : IDependency, IBaseService<AccessDashboard>
    {
        AccessDashboardConditionView GetAccessDashboardConditions();

        List<AccessDashboardConditionView> GetAllAccessDashboards();

        List<T> GetSearchResult<T>(Expression<Func<AccessDashboard, bool>> predicate) where T : IViewModel, new();
    }
}
