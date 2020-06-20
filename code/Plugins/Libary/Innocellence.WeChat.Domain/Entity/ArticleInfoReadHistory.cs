using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class ArticleInfoReadHistory : EntityBase<int>
    {
        public override Int32 Id { get; set; }

        public Int32? ArticleInfoId { get; set; }

        public string UserId { get; set; }
    }
}
