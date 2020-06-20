using System;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
    public class WeChatEmotion : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public string Target { get; set; }
        
    }
}
