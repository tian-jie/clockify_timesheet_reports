using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System.Linq;

namespace Innocellence.WeChat.Domain.Service
{
    public class ArticleCommentThumbUpService : BaseService<ArticleCommentThumbUp>, IArticleCommentThumbUpService
    {
        public ArticleCommentThumbUpService() : base("CAAdmin")
        {

        }

        public void ThumbUp(int commentId, string userOpenId, bool isThumbUp)
        {
            if (commentId > 0 && !string.IsNullOrEmpty(userOpenId))
            {
                if (isThumbUp)
                {
                    ArticleCommentThumbUp entity = new ArticleCommentThumbUp()
                    {
                        CommentId = commentId,
                        UserOpenId = userOpenId,
                    };
                    this.Repository.Insert(entity);
                }
                else
                {
                    var entity = this.Repository.Entities.FirstOrDefault(q => q.CommentId == commentId && q.UserOpenId.Equals(userOpenId, System.StringComparison.OrdinalIgnoreCase));
                    if (null != entity)
                    {
                        this.Repository.Delete(entity);
                    }
                }
            }
        }
    }
}
