using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Entity;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IFlexBenefitService : IDependency, IBaseService<FlexBenefit>
    {
        FlexBenefit GetFlexBenefitByConditions(string WeChatUserID, string accessYear);
    }
}
