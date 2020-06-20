using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView 
{
    public partial class AccessDashboardView : IViewModel
	{
        public Int32 Id { get; set; }
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
        // 报销情报(职工)
        public ReimbursementView ReimbursementForStaff { get; set; }
        // 报销情报(城乡居民)
        public ReimbursementView ReimbursementForFarmer { get; set; }
        // 报销情报(门特门慢门诊大病特病)
        public ReimbursementSimpleView ReimbursementSimple { get; set; }
        /// <summary>
        /// 结果列表
        /// </summary>
        public List<AccessDashboardView> List { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (AccessDashboard)obj;
            Id = entity.Id;
            TherapeuticArea = entity.TherapeuticArea;
            ChemicalName = entity.ChemicalName;
            TradeName = entity.TradeName;
            DosageForm = entity.DosageForm;
            Specification = entity.Specification;
            ApprovaledText = entity.ApprovaledText;
            NRDLStatus = entity.NRDLStatus;
            ProductionEnterpriseName = entity.ProductionEnterpriseName;
            Adaptation = entity.Adaptation;
            Province = entity.Province;
            Municipalit = entity.Municipalit;
            ReimbursementRatio = entity.ReimbursementRatio;

            ReimbursementForStaff = new ReimbursementView();
            ReimbursementForStaff.Id = entity.Id;
            ReimbursementForStaff.InsuranceType = entity.InsuranceType;
            ReimbursementForStaff.ReimbursementTreatment = entity.ReimbursementTreatment;
            ReimbursementForStaff.OutpatientStartPayLine = entity.OutpatientStartPayLine;
            ReimbursementForStaff.OutpatientEndPayLine = entity.OutpatientEndPayLine;
            ReimbursementForStaff.HospitalizationStartPayLine = entity.HospitalizationStartPayLine;
            ReimbursementForStaff.HospitalizationEndPayLine = entity.HospitalizationEndPayLine;
            ReimbursementForStaff.PolicyLink = entity.PolicyLink;

            ReimbursementForFarmer = new ReimbursementView();
            ReimbursementForFarmer.Id = entity.Id;
            ReimbursementForFarmer.InsuranceType = entity.InsuranceTypeTwo;
            ReimbursementForFarmer.ReimbursementTreatment = entity.ReimbursementTreatmentTwo;
            ReimbursementForFarmer.OutpatientStartPayLine = entity.OutpatientStartPayLineTwo;
            ReimbursementForFarmer.OutpatientEndPayLine = entity.OutpatientEndPayLineTwo;
            ReimbursementForFarmer.HospitalizationStartPayLine = entity.HospitalizationStartPayLineTwo;
            ReimbursementForFarmer.HospitalizationEndPayLine = entity.HospitalizationEndPayLineTwo;
            ReimbursementForFarmer.PolicyLink = entity.PolicyLinkTwo;

            ReimbursementSimple = new ReimbursementSimpleView();
            ReimbursementSimple.Id = entity.Id;
            ReimbursementSimple.InsuranceType = entity.InsuranceTypeThree;
            ReimbursementSimple.CrowdType = entity.CrowdType;
            ReimbursementSimple.ReimbursementTreatment = entity.ReimbursementTreatmentThree;
            ReimbursementSimple.OutpatientStartPayLine = entity.OutpatientStartPayLineThree;
            ReimbursementSimple.OutpatientEndPayLine = entity.OutpatientEndPayLineThree;
            ReimbursementSimple.PolicyLink = entity.PolicyLinkThree;
            return this;
        }
	}
}
