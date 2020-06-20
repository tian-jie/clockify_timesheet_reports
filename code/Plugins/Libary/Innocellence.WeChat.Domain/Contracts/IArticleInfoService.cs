using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.ViewModel;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IArticleInfoService : IDependency, IBaseService<ArticleInfo>
    {
        List<T> GetListByCode<T>(string Newscode) where T : IViewModel, new();
        //ArticleInfo GetByCateSub(string articleCateSub);
        T GetById<T>(int id) where T : IViewModel, new();

        List<T> GetList<T>(Expression<Func<ArticleInfo, bool>> predicate) where T : IViewModel, new();
        List<T> GetListWithContent<T>(Expression<Func<ArticleInfo, bool>> predicate, PageCondition con);

        List<NewsInfoView> GetListByNewsIDs(string strIDs);

        int UpdateArticleThumbsUp(int articleId, string userId, string type);

        string EncryptorWeChatUserID(string WeChatUserID);

        string DecryptWeChatUserID(string encryWeChatUserID);
     //   List<T> GetArticleList<T>(Expression<Func<ArticleInfo, bool>> predicate) where T : IViewModel, new();



        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="selectedDepartmentIds"></param>
        /// <param name="selectedTagIds"></param>
        /// <param name="selectedWeChatUserIDs"></param>
        /// <param name="checkResult"></param>
        /// <param name="isToAllUsers">如果是全员发, 把selectedWeChatUserIDs参数传入全部人员id</param>
        /// <returns></returns>
        bool CheckMessagePushRule(int appId, int AccountMangageID, IList<int> selectedDepartmentIds, IList<int> selectedTagIds, IList<string> selectedWeChatUserIDs, out CheckResult checkResult, bool isToAllUsers = false);

    }
}
