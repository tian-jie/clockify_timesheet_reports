using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System.Collections.Generic;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class AccessDashboardConditionView
    {
        // XXX 相关治疗领域
        public string TherapeuticArea { get; set; }
        // 商品名
        public string TradeName { get; set; }
        // 剂型
        public string DosageForm { get; set; }
        // 规格
        public string Specification { get; set; }
        // 省
        public string Province { get; set; }
        // 市
        public string Municipalit { get; set; }

        // 相关治疗领域List
        public List<TherapeuticAreaView> TherapeuticAreaList { get; set; }
        // 商品名List
        public List<TradeNameView> TradeNameList { get; set; }
        // 剂型List
        public List<DosageFormView> DosageFormList { get; set; }
        // 省市情报
        public List<ProvinceWithCityView> ProvinceInfo { get; set; }
                
    }

    public class ProvinceWithCityView
    {
        public string ProvinceName { get; set; }

        public List<string> CityList { get; set; }

    }

    public class TherapeuticAreaView
    {
        public string TherapeuticArea { get; set; }

        public List<string> TradeNameList { get; set; }

    }

    public class TradeNameView
    {
        public string TradeName { get; set; }

        public List<string> DosageFormList { get; set; }

    }

    public class DosageFormView
    {
        public string DosageForm { get; set; }

        public List<string> SpecificationList { get; set; }

    }
}
