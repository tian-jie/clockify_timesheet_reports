using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;


namespace Innocellence.WeChat.Domain.Services
{
    public partial class AccessDashboardService : BaseService<AccessDashboard>, IAccessDashboardService
    {
        public AccessDashboardService()
            : base("CAAdmin")
        {

        }

        public List<AccessDashboardConditionView> GetAllAccessDashboards()
        {
            return Repository.Entities.Select(x => new AccessDashboardConditionView() { 
                TherapeuticArea = x.TherapeuticArea,
                TradeName = x.TradeName,
                DosageForm = x.DosageForm,
                Specification = x.Specification,
                Province = x.Province,
                Municipalit = x.Municipalit
            }).ToList();
        }

        public AccessDashboardConditionView GetAccessDashboardConditions()
        {
            AccessDashboardConditionView result = new AccessDashboardConditionView();
            var allAccessDashboardList = Repository.Entities.ToList();

            if (!allAccessDashboardList.Any())
            {
                return null;
            }

            result.TherapeuticAreaList = new List<TherapeuticAreaView>();
            result.TradeNameList = new List<TradeNameView>();
            result.DosageFormList = new List<DosageFormView>();
            result.ProvinceInfo = new List<ProvinceWithCityView>();

            var therapeuticAreaList = allAccessDashboardList.Select(x => x.TherapeuticArea.Trim()).Distinct().ToList();
            therapeuticAreaList.ForEach(therapeuticArea =>
            {
                // 治疗领域 -> 商品名称级联作成
                TherapeuticAreaView tav = new TherapeuticAreaView();
                tav.TherapeuticArea = therapeuticArea;
                tav.TradeNameList = allAccessDashboardList.Where(x => x.TherapeuticArea.Contains(therapeuticArea)).Select(x => x.TradeName.Trim()).Distinct().ToList();
                result.TherapeuticAreaList.Add(tav);               
            });

            var tradeNameList = allAccessDashboardList.Select(x => x.TradeName.Trim()).Distinct().ToList();
            tradeNameList.ForEach(tradeName =>
            {
                // 商品名称 -> 剂型级联作成
                TradeNameView tnv = new TradeNameView();
                tnv.TradeName = tradeName;
                tnv.DosageFormList = allAccessDashboardList.Where(x => x.TradeName.Contains(tradeName)).Select(x => x.DosageForm.Trim()).Distinct().ToList();
                result.TradeNameList.Add(tnv);
            });

            var dosageFormList = allAccessDashboardList.Select(x => x.DosageForm.Trim()).Distinct().ToList();
            dosageFormList.ForEach(dosageForm =>
            {
                // 剂型 -> 规格级联作成
                DosageFormView dfv = new DosageFormView();
                dfv.DosageForm = dosageForm;
                dfv.SpecificationList = allAccessDashboardList.Where(x => x.DosageForm.Contains(dosageForm)).Select(x => x.Specification.Trim()).Distinct().ToList();
                result.DosageFormList.Add(dfv);
            });
            
            var provinceList = allAccessDashboardList.Select(x => x.Province.Trim()).Distinct().ToList();
            provinceList.ForEach(item =>
            {
                // 省 -> 城市级联作成
                ProvinceWithCityView info = new ProvinceWithCityView();
                info.ProvinceName = item;
                info.CityList = allAccessDashboardList.Where(x => x.Province.Contains(item)).Select(x => x.Municipalit.Trim()).Distinct().ToList();
                result.ProvinceInfo.Add(info);
            });

            return result;
        }


        public List<T> GetSearchResult<T>(Expression<Func<AccessDashboard, bool>> predicate) where T : IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }
    }
}