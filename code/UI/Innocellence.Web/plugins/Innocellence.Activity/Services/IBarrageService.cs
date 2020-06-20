using Infrastructure.Core;
using Innocellence.Activity.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace Innocellence.Activity.Services
{
    public interface IBarrageService : IDependency, IBaseService<Barrage>
    {
        List<T> GetQList<T>(Expression<Func<Barrage, bool>> predicate) where T : IViewModel, new();
    }
}