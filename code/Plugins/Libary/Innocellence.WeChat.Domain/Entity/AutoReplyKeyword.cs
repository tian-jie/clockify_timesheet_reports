using Infrastructure.Core;
using System;

namespace Innocellence.WeChat.Domain.Entity
{
    public class AutoReplyKeyword : EntityBase<int>
    {
        public override Int32 Id { get; set; }

        public Int32 AutoReplyId { get; set; }

        public Int32 SecondaryType { get; set; }

        public String Keyword { get; set; }

    }
}
