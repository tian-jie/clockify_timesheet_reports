using System;
using System.Collections.Generic;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System.Linq.Expressions;
namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IMessageService : IDependency, IBaseService<Message>
    {
        List<T> GetList<T>(Expression<Func<Message, bool>> predicate) where T : IViewModel, new();

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
        bool CheckMessagePushRule(int appId,int AccountMangageID, IList<int> selectedDepartmentIds, IList<int> selectedTagIds, IList<string> selectedWeChatUserIDs, out CheckResult checkResult, bool isToAllUsers = false);
    }
}
