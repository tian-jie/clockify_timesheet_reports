using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.Activity.Entity;
using Innocellence.Activity.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Innocellence.Activity.Service
{
    public class BarrageService : BaseService<Barrage>, IBarrageService
    {
        public BarrageService()
            : base("CAAdmin")
        {

        }

        public List<T> GetQList<T>(Expression<Func<Barrage, bool>> predicate) where T : IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).AsNoTracking().ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }

    }
}