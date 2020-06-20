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
    public class ArticleCommentService : BaseService<ArticleComment>, IArticleCommentService
    {
        public void UpdateThumbsUpCount(int commentId, bool isThumbUp)
        {
            this.Repository.SqlExcute(string.Format("Update ArticleComment Set ThumbsUpCount=ThumbsUpCount{0} Where Id = {1}", isThumbUp ? "+1" : "-1", commentId));
        }

        public ArticleCommentService() : base("CAAdmin") { }
    }
}
