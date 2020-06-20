using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView 
{
    public partial class FlexBenefitView : IViewModel
	{
        public Int32 Id { get; set; }

        public String WeChatUserID { get; set; }

        public String GlobalID { get; set; }

        public String CostText { get; set; }

        public String RiskFee { get; set; }

        public String ExaminationFee { get; set; }

        public String EnjoyFee { get; set; }

        public String Summary { get; set; }

        public String AccessYear { get; set; }  

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (FlexBenefit)obj;
            Id = entity.Id;
            WeChatUserID = entity.WeChatUserID;
            GlobalID = entity.GlobalID;
            CostText = entity.CostText;

            if (entity.RiskFee.HasValue && entity.RiskFee > 0)
            {
                RiskFee = entity.RiskFee.ToString();
            }
            else
            {
                RiskFee = "-";
            }

            if (entity.ExaminationFee.HasValue && entity.ExaminationFee > 0)
            {
                ExaminationFee = entity.ExaminationFee.ToString();
            }
            else
            {
                ExaminationFee = "-";
            }

            if (entity.EnjoyFee.HasValue && entity.EnjoyFee > 0)
            {
                EnjoyFee = entity.EnjoyFee.ToString();
            }
            else
            {
                EnjoyFee = "-";
            }

            Summary = entity.Summary;
            AccessYear = entity.AccessYear;
            return this;
        }
	}
}
