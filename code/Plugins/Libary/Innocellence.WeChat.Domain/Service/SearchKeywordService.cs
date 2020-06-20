using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Service
{
    public class SearchKeywordService : BaseService<SearchKeyword>, ISearchKeywordService
    {
        public SearchKeywordService(IUnitOfWork unitOfWork)
            : base("CAAdmin")
        {
        }

       public List<SearchKeywordView> GetSearchKeywordsByCategory(int appId, int topNum = -1)
        {
            var lst = Repository.Entities.Where(a => a.AppId == appId);
            if(topNum != -1)
            {
                lst = lst.OrderByDescending(a=>a.SearchCount).Take(topNum);
            }

            return lst.ToList().Select(n => (SearchKeywordView)new SearchKeywordView().ConvertAPIModel(n)).ToList();
        }

    }
}
