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
using Innocellence.WeChat.Domain.Common;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IAutoReplyContentService : IDependency, IBaseService<AutoReplyContent>
    {
        List<T> GetList<T>(Expression<Func<AutoReplyContent, bool>> predicate) where T : IViewModel, new();
        List<T> GetList<T>(int appId, string inputStr, int type) where T : IViewModel, new();
    }
}
