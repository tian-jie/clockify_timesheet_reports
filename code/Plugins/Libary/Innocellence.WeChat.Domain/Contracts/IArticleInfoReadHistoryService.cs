using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IArticleInfoReadHistoryService : IDependency, IBaseService<ArticleInfoReadHistory>
    {
        bool HasReaded(int articleInfoId, string userId);
    }
}
