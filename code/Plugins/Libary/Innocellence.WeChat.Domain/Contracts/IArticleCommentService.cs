using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IArticleCommentService : IDependency, IBaseService<ArticleComment>
    {
        void UpdateThumbsUpCount(int commentId, bool isThumbUp);
    }
}
