using Infrastructure.Core;
using Infrastructure.Utility.Data;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.Service;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Innocellence.Activity.Services
{
    public interface IPollingResultService : IDependency, IBaseService<PollingResultEntity>
    {
        int InsertView(PollingResultView objModalSrc);
        int UpdateView(PollingResultView objModalSrc);
        int GetUserCount(int id, string lillyid);
        List<PollingResultEntity> GetPollingReslutViews(int id, string lillyid);
        List<T> GetListExport<T>(Expression<Func<PollingResultEntity, bool>> predicate) where T : IViewModel, new();

        int DeleteByQuestionId(int id);
        int DeleteByOptionId(int id);

        List<PollingResultEntity> GetList(int polling, string lillyid = null);

        IList<PollingResultEntity> GetGroupPagingList(Expression<Func<PollingResultEntity, bool>> predicate,
            PageCondition con);

        IList<PollingResultEntity> GetGroupPagingByUserId(Expression<Func<PollingResultEntity, bool>> predicate,
            PageCondition con);

        IList<PollingResultEntity> GetBatchResultsByPollingId(int id, int batchSize, QueryableGroup queryableGroup,
            Expression<Func<PollingResultEntity, dynamic>> selectExpression);
        void GetUserList(List<PollingResultExportView> lists);
    }
}
