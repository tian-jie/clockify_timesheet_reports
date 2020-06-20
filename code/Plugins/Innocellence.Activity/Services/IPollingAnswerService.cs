using Infrastructure.Core;
using Infrastructure.Utility.Data;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Innocellence.Activity.Services
{
    public interface IPollingAnswerService : IDependency, IBaseService<PollingAnswerEntity>
    {
        List<PollingResultCustomView> Query(Expression<Func<PollingAnswerEntity, bool>> predicate, PageCondition con);
        List<PollingResultCustomView> GetIsRight(Expression<Func<PollingAnswerEntity, bool>> predicate);
        List<PollingResultCustomView> GetPersonScore(Expression<Func<PollingAnswerEntity, bool>> predicate);
    }

}
