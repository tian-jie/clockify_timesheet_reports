using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Extensions;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 业务实现——App访问量统计
    /// </summary>
    public partial class PageReportService : BaseService<PageReport>, IPageReportService
    {
       
        public PageReportService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            
        }

        public PageReportService()
            
        {

        }

        public List<T> GetListByFromDate<T>(Expression<Func<PageReport, bool>> predicate) where T : IViewModel, new()
        {
            //var lst = Repository.Entities.Where(predicate).DistinctBy(p => new { p.AccessDate }).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            var lst = Repository.Entities.Where(predicate).OrderByDescending(p => new { p.AccessDate }).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }

        public List<PageReportView> GetReportList(Expression<Func<PageReport, bool>> predicate,
              PageCondition con)
        {
            int iTotal = con.RowCount;

            var q = from p in Repository.Entities.Where(predicate) group p by p.AccessDate into g select new PageReportView  { AccessDate= g.Key, TotalVisitorCount = g.Sum(p => p.VisitorCount), TotalVisitTimes = g.Sum(p => p.VisitTimes) };


            con.RowCount = q.Count();

            return q.OrderBy(a=>a.AccessDate).Skip((con.PageIndex - 1) * con.PageSize).Take(con.PageSize).ToList();
        }
    }
}