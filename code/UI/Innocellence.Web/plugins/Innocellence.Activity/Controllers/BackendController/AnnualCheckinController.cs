using Infrastructure.Utility.Data;
using Infrastructure.Utility.IO;
using Infrastructure.Web.UI;
using Innocellence.Activity.Entity;
using Innocellence.Activity.Services;
using Innocellence.Activity.ViewModel;
using Innocellence.WeChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Innocellence.Activity.Admin.Controllers
{
    public class AnnualCheckinController : BaseController<AnnualCheckinEntity, AnnualCheckinView>
    {
        public AnnualCheckinController(IAnnualCheckinService objService)
            : base(objService)
        {
            _objService = objService;
        }

        public override ActionResult Index()
        {
            return View();
        }

        public override List<AnnualCheckinView> GetListEx(Expression<Func<AnnualCheckinEntity, bool>> predicate, PageCondition ConPage)
        {
            string strSearch = Request["txtSearch"];
            string strNo = Request["txtEventNo"];

            if (!string.IsNullOrEmpty(strNo))
            {
                predicate = predicate.AndAlso(a => a.EventNo.Equals(strNo, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(strSearch))
            {
                predicate = predicate.AndAlso(a => a.Name.Contains(strSearch) || a.LillyId.Contains(strSearch));
            }

            //TODO:
            //predicate = predicate.AndAlso(a => a.AppId == AppId);

            var q = _BaseService.GetList<AnnualCheckinView>(predicate, ConPage).ToList();

            return q;
        }

        public override ActionResult Export()
        {
            string strSearch = Request["txtSearch"];
            string strNo = Request["txtEventNo"];

            Expression<Func<AnnualCheckinEntity, bool>> predicate = x => x.Id > 0;

            if (!string.IsNullOrEmpty(strNo))
            {
                predicate = predicate.AndAlso(a => a.EventNo.Equals(strNo, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(strSearch))
            {
                predicate = predicate.AndAlso(a => a.Name.Contains(strSearch) || a.LillyId.Contains(strSearch));
            }

            var reportList = _objService.GetList<AnnualCheckinView>(int.MaxValue, predicate).OrderByDescending(x => x.Id).ToList();

            return ExportToCsv(reportList);
        }

        private ActionResult ExportToCsv(List<AnnualCheckinView> list)
        {
            string[] headLine = { "Id", "LillyId", "Name", "CheckHotel", "MaterialNum", "Status", "EventNo", "UpdatedDate" };
            var csv = new CsvSerializer<AnnualCheckinView> { UseLineNumbers = false };
            var sRet = csv.SerializeStream(list, headLine);
            string fileName = "AnnualCheckin_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            return File(sRet, "text/comma-separated-values", fileName);
        }
    }
}