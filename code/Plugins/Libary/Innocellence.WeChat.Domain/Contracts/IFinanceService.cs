using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;


namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IFinanceService : IBaseService<FinanceQueryEntity>, IDependency
    {
        ResultEntity AddOrUpdate(IList<FinanceQueryEntity> list);
        List<T> GetList<T>(Expression<Func<FinanceQueryEntity, bool>> predicate) where T : IViewModel, new();
        List<FinanceEntityView> GetFinanceList(string WeChatUserID, PageCondition condition, out int totalRow);
    }

    public class ResultEntity
    {
        public int InsertCount { get; set; }

        public int UpdateCount { get; set; }
    }
   
}
