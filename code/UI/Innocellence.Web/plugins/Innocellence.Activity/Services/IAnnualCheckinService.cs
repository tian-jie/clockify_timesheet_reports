using Infrastructure.Core;
using Innocellence.Activity.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace Innocellence.Activity.Services
{
    public interface IAnnualCheckinService : IDependency, IBaseService<AnnualCheckinEntity>
    {
        List<T> GetList<T>(Expression<Func<AnnualCheckinEntity, bool>> predicate) where T : IViewModel, new();
    }
}
