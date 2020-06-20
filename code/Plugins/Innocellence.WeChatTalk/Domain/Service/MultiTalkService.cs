using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChatTalk.Domain.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Innocellence.WeChatTalk.Domain.Service
{
    public partial class MultiTalkService : BaseService<MultiTalk>, IMultiTalkService
    {
        public MultiTalkService(
            )
        : base("CAAdmin")
        {
        }
       
    }
}