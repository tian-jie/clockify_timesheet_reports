using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class AticleCommentThumbUpView : IViewModel
    {
        public int Id { get; set; }
        public int? CommentId { get; set; }
        public string UserOpenId { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (ArticleCommentThumbUp)model;
            AticleCommentThumbUpView result = new AticleCommentThumbUpView()
            {
                Id = entity.Id,
                CommentId = entity.CommentId,
                UserOpenId = entity.UserOpenId,
            };
            return result;
        }
    }
}
