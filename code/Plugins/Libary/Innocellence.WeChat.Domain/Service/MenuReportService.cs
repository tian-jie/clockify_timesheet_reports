using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts.ViewModel;


namespace Innocellence.WeChat.Domain.Service
{
    public class MenuReportService : BaseService<MenuReport>, IMenuReportService
    {
        public List<MenuReportView> QueryTable(DateTime stardate, DateTime enddate, PageCondition pageCondition)
        {
            Expression<Func<MenuReport, bool>> menuReport = a => a.AccessDate >= stardate && a.AccessDate <= enddate;

            return GetList<MenuReportView>(menuReport, pageCondition);
        }


        public IList<MenuReport> QueryList(Expression<Func<MenuReport, bool>> func)
        {
            return Repository.Entities.Where(func).ToList();
        }


        public List<T> GetListByDate<T>(Expression<Func<MenuReport, bool>> predicate) where T : Infrastructure.Core.IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }
    }
}
