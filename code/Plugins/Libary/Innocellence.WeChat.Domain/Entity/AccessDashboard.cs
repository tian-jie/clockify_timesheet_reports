using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
namespace Innocellence.WeChat.Domain.Entity
{
    public partial class AccessDashboard : EntityBase<int>
    {
        public override int Id { get; set; }
        // XXX 相关治疗领域
        public String TherapeuticArea { get; set; }
        // 化学名
        public String ChemicalName { get; set; }
        // 商品名
        public String TradeName { get; set; }
        // 剂型
        public String DosageForm { get; set; }
        // 规格
        public String Specification { get; set; }
        // 生产注册批文
        public String ApprovaledText { get; set; }
        // NRDL现状
        public String NRDLStatus { get; set; }
        // 生产企业名称
        public String ProductionEnterpriseName { get; set; }
        // 适应症
        public String Adaptation { get; set; }
        // 省
        public String Province { get; set; }
        // 地市
        public String Municipalit { get; set; }
        // 报销比例
        public String ReimbursementRatio { get; set; }

        // 人群/险种(职工)
        public String InsuranceType { get; set; }
        // 报销待遇（详细）(职工)
        public String ReimbursementTreatment { get; set; }
        // 起付线(门诊)(职工)
        public String OutpatientStartPayLine { get; set; }
        // 封顶线(门诊)(职工)
        public String OutpatientEndPayLine { get; set; }
        // 起付线(住院)(职工)
        public String HospitalizationStartPayLine { get; set; }
        // 封顶线(住院)(职工)
        public String HospitalizationEndPayLine { get; set; }
        // 政策原文链接(职工)
        public String PolicyLink { get; set; }

        // 人群/险种(城乡居民)
        public String InsuranceTypeTwo { get; set; }
        // 报销待遇（详细）(城乡居民)
        public String ReimbursementTreatmentTwo { get; set; }
        // 起付线(门诊)(城乡居民)
        public String OutpatientStartPayLineTwo { get; set; }
        // 封顶线(门诊)(城乡居民)
        public String OutpatientEndPayLineTwo { get; set; }
        // 起付线(住院)(城乡居民)
        public String HospitalizationStartPayLineTwo { get; set; }
        // 封顶线(住院)(城乡居民)
        public String HospitalizationEndPayLineTwo { get; set; }
        // 政策原文链接(城乡居民)
        public String PolicyLinkTwo { get; set; }

        // 险种(门特门慢门诊大病特病）
        public String InsuranceTypeThree { get; set; }
        // 人群(门特门慢门诊大病特病）
        public String CrowdType { get; set; }
        // 报销待遇（详细）(门特门慢门诊大病特病）
        public String ReimbursementTreatmentThree { get; set; }
        // 起付线(门诊)(门特门慢门诊大病特病）
        public String OutpatientStartPayLineThree { get; set; }
        // 封顶线(门诊)(门特门慢门诊大病特病）
        public String OutpatientEndPayLineThree { get; set; }
        // 政策原文链接(门特门慢门诊大病特病）
        public String PolicyLinkThree { get; set; }
    }
}
