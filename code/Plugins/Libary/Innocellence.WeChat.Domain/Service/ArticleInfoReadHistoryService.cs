using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Service
{
    public class ArticleInfoReadHistoryService : BaseService<ArticleInfoReadHistory>, IArticleInfoReadHistoryService
    {
        public ArticleInfoReadHistoryService() : base("CAAdmin") { }

        public bool HasReaded(int articleInfoId, string userId)
        {
            var q = this.Repository.Entities.FirstOrDefault(h => h.ArticleInfoId.HasValue && h.ArticleInfoId.Value.Equals(articleInfoId) && h.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));
            return q != null;
        }
    }
}
