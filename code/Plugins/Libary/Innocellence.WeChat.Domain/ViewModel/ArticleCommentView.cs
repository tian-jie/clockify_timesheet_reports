using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class ArticleCommentView : IViewModel
    {
        public Int32 Id { get; set; }
        public int? ArticleId { get; set; }
        public string UserOpenId { get; set; }
        public string UserAvatar { get; set; }
        public string UserNickName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ThumbsUpCount { get; set; }
        public string Comment { get; set; }
        public bool? IsDeleted { get; set; }
        public bool CanDelete { get; set; }
        public bool HasThumbUp { get; set; }
        public string ThumbUpStyle { get; set; }
        public string DiffDateDisplayStr { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (ArticleComment)model;
            ArticleCommentView result = new ArticleCommentView()
            {
                Id = entity.Id,
                ArticleId = entity.ArticleId,
                UserOpenId = entity.UserOpenId,
                UserAvatar = entity.UserAvatar,
                UserNickName = entity.UserNickName,
                CreatedDate = entity.CreatedDate,
                ThumbsUpCount = entity.ThumbsUpCount,
                Comment = entity.Comment,
                IsDeleted = entity.IsDeleted,
            };
            return result;
        }

        public ArticleCommentView()
        {
            this.ThumbsUpCount = 0;
            this.ThumbUpStyle = "fa-thumbs-o-up";
            this.DiffDateDisplayStr = "刚刚";
        }

    }
}
