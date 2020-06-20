using System;
using System.Net;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Infrastructure.Utility.Data;
using Infrastructure.Core.Data;
using Infrastructure.Web.UI;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Contracts;

namespace Innocellence.Web.Controllers
{
    public class LogsController : BaseController<LogsModel, LogsView>
    {
        public LogsController(ILogsService objService)
            : base(objService)
        {
           
        }

        //初始化list页面
        public override List<LogsView> GetListEx(Expression<Func<LogsModel, bool>> predicate, PageCondition ConPage)
        {
            string date=Request["txtDate"];
            string logCate=Request["txtCateLog"];

            if (!string.IsNullOrEmpty(date)) { 
               DateTime dateTime=Convert.ToDateTime(date);
               DateTime dateAdd=dateTime.AddDays(1);
               predicate = predicate.AndAlso(a => a.CreatedDate >= dateTime && a.CreatedDate <= dateAdd);
            }

            if (!string.IsNullOrEmpty(logCate))
            {
                predicate = predicate.AndAlso(a => a.LogCate.Contains(logCate));
            }

            ConPage.SortConditions.Add(new SortCondition("Id", System.ComponentModel.ListSortDirection.Descending));

            var q = _BaseService.GetList<LogsView>(predicate, ConPage);

            return q.ToList();
        }

    }
}
