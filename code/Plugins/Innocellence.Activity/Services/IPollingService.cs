using Infrastructure.Core;
using Infrastructure.Utility.Data;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.Model;
using Innocellence.Activity.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Innocellence.Activity.Services
{
    public interface IPollingService : IDependency, IBaseService<PollingEntity>
    {
        List<PollingView> QueryList(Expression<Func<PollingEntity, bool>> predicate);
        PollingView GetPollingView(int pollingId);
        PollingView GetPollingView(string guid);
        PollingView GetPollingVote(int pollingId, string LillyId);
        PollingView GetPollingDetailView(int pollingId);
        PollingScreenView GetPollingScreenData(int pollingid, int questionid);
        IList<PollingResultEntity> GetPollingResultByPollingId(int pollingID, string lillyId = null);
        IList<PollingResultTempView> GetPollingResultTempByLillyId(int pollingID, string lillyId);
        string ConvertAbcToYn(string abc, int len = 10);

        List<PollingResultCustomView> PollingResult(int pollingId, IList<PollingResultEntity> pollingResultEntity, PollingView pollingView);

        List<PollingResultScoreView> PollingResultScore(int pollingId, PageCondition pageCondition);

        int InsertOrUpdateView<T>(T objModalSrc);
        //int GetScore(int pollingId, string lillyid);
        PollingView GetPersonCount(int pollingId, string lillyid = null);

        
        /// <summary>
        /// return score
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        decimal InsertResult(PollingResultView view);

        /// <summary>
        /// get polling sort info
        /// </summary>
        /// <param name="pollingIds"></param>
        /// <returns></returns>
        IList<SortViewModel> GetSorting(IList<int> pollingIds);

        int ClonePolling(int pollingId);
      
    }

}
