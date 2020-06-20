using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Entity
{

    public partial class FlexBenefit : EntityBase<int>
	{

        public override Int32 Id { get; set; }

        public String WeChatUserID { get; set; }

        public String GlobalID { get; set; }

        public String CostText { get; set; }

        public Decimal? RiskFee { get; set; }

        public Decimal? ExaminationFee { get; set; }

        public Decimal? EnjoyFee { get; set; }

        public String Summary { get; set; }

        public String AccessYear { get; set; }  
       
	}
}
