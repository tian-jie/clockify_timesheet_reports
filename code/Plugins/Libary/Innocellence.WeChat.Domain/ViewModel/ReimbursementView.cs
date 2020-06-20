using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView 
{
    public partial class ReimbursementView
	{
        public Int32 Id { get; set; }

        // 人群/险种
        public String InsuranceType { get; set; }
        // 报销待遇（详细）
        public String ReimbursementTreatment { get; set; }
        // 起付线(门诊)
        public String OutpatientStartPayLine { get; set; }
        // 封顶线(门诊)
        public String OutpatientEndPayLine { get; set; }
        // 起付线(住院)
        public String HospitalizationStartPayLine { get; set; }
        // 封顶线(住院)
        public String HospitalizationEndPayLine { get; set; }
        // 政策原文链接
        public String PolicyLink { get; set; }

	}
}
