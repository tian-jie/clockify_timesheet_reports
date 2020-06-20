using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class ArticleComment : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public int? ArticleId { get; set; }
        public string UserOpenId { get; set; }
        public string UserAvatar { get; set; }
        public string UserNickName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ThumbsUpCount { get; set; }
        public string Comment { get; set; }
        public bool? IsDeleted { get; set; }

        public ArticleComment()
        {
            this.ThumbsUpCount = 0;
        }

    }
}
