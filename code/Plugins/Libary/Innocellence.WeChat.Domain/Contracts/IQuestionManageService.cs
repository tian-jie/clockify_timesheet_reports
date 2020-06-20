using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IQuestionManageService : IDependency, IBaseService<QuestionManage>
    {
        List<T> GetListByStatus<T>(int appid,string Status) where T : IViewModel, new();
        List<T> GetListByQUserId<T>(int appid, string QUserId,string category) where T : IViewModel, new();
        List<T> GetQuestionList<T>(Expression<Func<QuestionManage, bool>> predicate) where T : IViewModel, new();
        List<string> GetStatusList(int appid);
    }
}
