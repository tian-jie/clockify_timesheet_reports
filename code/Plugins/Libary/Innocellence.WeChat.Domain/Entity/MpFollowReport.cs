using Infrastructure.Core;
using System;

namespace Innocellence.WeChat.Domain.Entity
{
    public class MpFollowReport : EntityBase<int>
    {
        public string UserName { get; set; }

        public string Content { get; set; }

        public string ContentType { get; set; }

        public string CreatedTime { get; set; }

        public string CustomerNO { get; set; }

        public string CustomerRegisteredTime { get; set; }

        public dynamic ExtendObject { get; set; }
        public string FollowTag { get; set; }
    }
}
