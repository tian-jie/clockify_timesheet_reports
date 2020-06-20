using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Innocellence.WeChat.Domain.Services
{
    public partial class FlexBenefitService : BaseService<FlexBenefit>, IFlexBenefitService
    {
        public FlexBenefitService()
            : base("CAAdmin")
        {

        }

        public FlexBenefit GetFlexBenefitByConditions(string WeChatUserID, string accessYear)
        {
            //FlexBenefit flexBenefit = new FlexBenefit();
            if (string.IsNullOrEmpty(WeChatUserID) || string.IsNullOrEmpty(accessYear))
            {
                return null;
            }

            return Repository.Entities.Where(a => WeChatUserID.Equals(a.WeChatUserID) && accessYear.Equals(a.AccessYear)).Distinct().FirstOrDefault();
            //return flexBenefit;
        }
    }
}