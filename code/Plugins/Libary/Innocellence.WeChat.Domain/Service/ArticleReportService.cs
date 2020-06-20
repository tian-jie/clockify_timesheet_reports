using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Service
{
    public class ArticleReportService : BaseService<ArticleReport>, IArticleReportService
    {
        //public ArticleReportService(IUnitOfWork unitOfWork)
        //    : base(unitOfWork)
        //{

        //}

        public List<ArticleReportView> QueryTable(DateTime stardate, DateTime enddate, PageCondition pageCondition)
        {
            Expression<Func<ArticleReport, bool>> predicate = a => a.AccessDate >= stardate && a.AccessDate <= enddate;

            return GetList<ArticleReportView>(predicate, pageCondition);
        }
        
        public List<T> GetListByDate<T>(Expression<Func<ArticleReport, bool>> predicate) where T : IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }

    }
}
