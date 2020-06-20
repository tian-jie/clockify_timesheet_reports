using Infrastructure.Core;
using System;

namespace Innocellence.WeChat.Domain.Entity
{
    public partial class ArticleCommentThumbUp : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public int? CommentId { get; set; }
        public string UserOpenId { get; set; }

    }
}
