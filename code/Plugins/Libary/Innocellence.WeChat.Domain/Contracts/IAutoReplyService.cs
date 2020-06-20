using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using System.Linq.Expressions;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Infrastructure.Utility.Data;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IAutoReplyService : IDependency, IBaseService<AutoReply>
    {
        List<T> GetList<T>(Expression<Func<AutoReply, bool>> predicate) where T : IViewModel, new();

        int Add(AutoReplyView view);

        int Update(AutoReplyView view);

        AutoReplyView GetDetail(int autoReplyId);

        //List<T> GetList<T>(Expression<Func<AutoReply, bool>> predicate, PageCondition page) where T : IViewModel, new();
    }
}
