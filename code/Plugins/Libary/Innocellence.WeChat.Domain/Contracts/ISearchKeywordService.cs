using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
namespace Innocellence.WeChat.Domain.Contracts
{
    public interface ISearchKeywordService : IDependency, IBaseService<SearchKeyword>
    {
        List<SearchKeywordView> GetSearchKeywordsByCategory(int appId, int topNum);
    }
}
