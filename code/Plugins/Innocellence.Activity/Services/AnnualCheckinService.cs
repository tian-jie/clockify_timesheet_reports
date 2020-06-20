using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.Activity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;



namespace Innocellence.Activity.Services
{
    public class AnnualCheckinService : BaseService<AnnualCheckinEntity>, IAnnualCheckinService
    {
        public AnnualCheckinService()
            : base("CAAdmin")
        {

        }

        public List<T> GetList<T>(Expression<Func<AnnualCheckinEntity, bool>> predicate) where T : IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }
    }
}
