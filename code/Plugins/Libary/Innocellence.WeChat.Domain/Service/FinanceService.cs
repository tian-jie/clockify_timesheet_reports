using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;

namespace Innocellence.WeChat.Domain.Service
{
    public class FinanceService : BaseService<FinanceQueryEntity>, IFinanceService
    {
        public FinanceService()
            : base("CAAdmin")
        {

        }

        public ResultEntity AddOrUpdate(IList<FinanceQueryEntity> list)
        {
            //list = list.GroupBy(x => x.TEANO).Select(x => x.OrderByDescending(y => y.ReceiveDate).First()).ToList();

            //var teaNumber = list.Select(x => x.TEANO).ToList();

            //var existList = Repository.Entities.Where(x => teaNumber.Contains(x.TEANO)).AsNoTracking().ToList();

            //var updateList = list.Where(x => existList.Any(y => String.Compare(y.TEANO, x.TEANO, StringComparison.OrdinalIgnoreCase) == 0)).ToList();

            //var insertList = list.Where(x => existList.All(y => String.Compare(y.TEANO, x.TEANO, StringComparison.OrdinalIgnoreCase) != 0)).ToList();

            //foreach (var item in updateList)
            //{
            //    var existItem = existList.First(x => x.TEANO == item.TEANO);
            //    item.Id = existItem.Id;
            //    Repository.Update(item);
            //}

            Repository.Insert(list.AsEnumerable());

            return new ResultEntity { InsertCount = list.Count };
        }

        public List<T> GetList<T>(Expression<Func<FinanceQueryEntity, bool>> predicate) where T : IViewModel, new()
        {
            var lst =Repository.Entities.Where(predicate).ToList().Select(n => (T) (new T().ConvertAPIModel(n))).ToList();
            return lst;
        }

        public List<FinanceEntityView> GetFinanceList(string WeChatUserID, PageCondition condition, out int totalRow)
        {
            var where = Repository.Entities.Where(a => a.WeChatUserID == WeChatUserID)
                .GroupBy(x => x.TEANO)
                .Select(x => x.OrderByDescending(y => y.ReceiveDate).ThenByDescending(y=>y.CreatedDate).FirstOrDefault());

            totalRow = where.Count();

            var list=where.OrderByDescending(x=>x.ReceiveDate).Skip((condition.PageIndex-1)*condition.PageSize).Take(condition.PageSize).ToList();

            return list.Select(n => (FinanceEntityView)new FinanceEntityView().ConvertAPIModel(n)).ToList();
        }
    }
}
