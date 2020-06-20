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

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IAutoReplyKeywordService : IDependency, IBaseService<AutoReplyKeyword>
    {
        List<T> GetList<T>(Expression<Func<AutoReplyKeyword, bool>> predicate) where T : IViewModel, new();
       
    }
}
