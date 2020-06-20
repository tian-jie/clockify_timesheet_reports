using System;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
    public class FeedBackEntity : EntityBase<int>
    {
        public override int Id { get; set; }

        public string Content { get; set; }

        public string MenuCode { get; set; }

        public string FeedBackUserId { get; set; }

        public DateTime FeedBackDateTime { get; set; }

        public int AppID { get; set; }
    }
}
