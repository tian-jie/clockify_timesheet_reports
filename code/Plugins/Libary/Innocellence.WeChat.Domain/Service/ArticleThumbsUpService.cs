using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Service
{
    public class ArticleThumbsUpService : BaseService<ArticleThumbsUp>, IArticleThumbsUpService
    {
     
        public ArticleThumbsUpService()
            : base("CAAdmin")
        {
        }

    }
}
