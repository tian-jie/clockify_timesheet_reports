using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.ViewModel;
using System.Diagnostics;
using Infrastructure.Utility.Data;

namespace Innocellence.WeChat.Domain.Service
{
    public class WechatUserRequestMessageTagService : BaseService<WechatUserRequestMessageTag>, IWechatUserRequestMessageTagService
    {
        public WechatUserRequestMessageTagService()
            : base("CAADMIN")
        {
        }
    }
}
