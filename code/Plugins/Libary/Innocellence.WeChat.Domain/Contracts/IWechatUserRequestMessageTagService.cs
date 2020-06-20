using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IWechatUserRequestMessageTagService : IDependency, IBaseService<WechatUserRequestMessageTag>
    {
        
    }
}
