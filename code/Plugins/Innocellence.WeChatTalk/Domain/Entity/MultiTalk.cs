using Infrastructure.Core;
using System;

namespace Innocellence.WeChatTalk.Domain.Entity
{
    public class MultiTalk : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public string OpenId { get; set; }
        public Int32 AppId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TextContent { get; set; }
        public Boolean IsDeleted { get; set; }
        public string MsgType { get; set; }

    }
}
