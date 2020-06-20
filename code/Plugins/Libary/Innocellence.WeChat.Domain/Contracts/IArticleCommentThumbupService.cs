using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IArticleCommentThumbUpService : IDependency, IBaseService<ArticleCommentThumbUp>
    {
        void ThumbUp(int commentId, string userOpenId, bool isThumbUp);
    }
}
