using EntityFramework.Extensions;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.ModelsView;
using Innocellence.Activity.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Innocellence.Activity.Service
{
    public class PollingAnswerService : BaseService<PollingAnswerEntity>, IPollingAnswerService
    {

        public PollingAnswerService()
            : base("CAAdmin")
        {

        }
        public List<PollingResultCustomView> GetIsRight(Expression<Func<PollingAnswerEntity, bool>> predicate)
        {
            return Repository.Entities.Where(predicate)
                .ToList().Select(n => (PollingResultCustomView)(new PollingResultCustomView().ConvertAPIModel(n))).ToList();
        }
        public List<PollingResultCustomView> GetPersonScore(Expression<Func<PollingAnswerEntity, bool>> predicate)
        {
            var source = Repository.Entities.Where(predicate)
                .GroupBy(x => new { x.LillyId });

            source = source.OrderByDescending(m => m.Key.LillyId);
            var query = source.Select(x => new { x.Key, x.FirstOrDefault().Dept1, x.FirstOrDefault().Dept2, x.FirstOrDefault().Dept3, x.FirstOrDefault().Name, 
                sum = x.Where(y => y.Status).Sum(y =>  (decimal?)y.Score) })
               .Future();
            var list = query.ToList();

            return list.Select(y => new PollingAnswerEntity { LillyId = y.Key.LillyId, Score = y.sum ?? 0, Dept1 = y.Dept1, Dept2 = y.Dept2, Dept3 = y.Dept3, Name = y.Name }).ToList()
                .Select(n => (PollingResultCustomView)(new PollingResultCustomView().ConvertAPIModel(n))).ToList();

        }
        public List<PollingResultCustomView> Query(Expression<Func<PollingAnswerEntity, bool>> predicate, PageCondition con)
        {
            var source = Repository.Entities.Where(predicate)
                .GroupBy(x => new { x.LillyId });

            source = source.OrderByDescending(m => m.Key.LillyId);
            var allCountQuery = source.Select(x => x.Key.LillyId).FutureCount();

            var query = source.Select(x => new
            {
                x.Key,
                x.FirstOrDefault().Dept1,
                x.FirstOrDefault().Dept2,
                x.FirstOrDefault().Dept3,
                x.FirstOrDefault().Name,
                sum = x.Where(y => y.Status).Sum(y => (decimal?)y.Score)
            }).Skip((con.PageIndex - 1) * con.PageSize).Take(con.PageSize);

            var total = allCountQuery.Value;
            var list = query.ToList();


            con.RowCount = total;

            return list.Select(y => new PollingAnswerEntity { LillyId = y.Key.LillyId, Score = y.sum ??0, Dept1 = y.Dept1, Dept2 = y.Dept2, Dept3 = y.Dept3, Name = y.Name }).ToList()
                .Select(n => (PollingResultCustomView)(new PollingResultCustomView().ConvertAPIModel(n))).ToList();

        }
    }
}
