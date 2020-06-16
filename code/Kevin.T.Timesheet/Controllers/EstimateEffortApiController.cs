using Infrastructure.Core.Logging;
using Innocellence.Web.Controllers;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using Infrastructure.Utility.Data;
using System.Linq.Expressions;
using Infrastructure.Web.UI;
using Newtonsoft.Json;

namespace Kevin.T.Timesheet.Controllers
{
    public class EstimateEffortApiController : BaseController<EstimateEffort, EstimateEffortView>
    {
        private ILogger Logger = LogManager.GetLogger("Timesheet");
        private IRoleTitleService _roleTitleService;
        private IEstimateEffortService _estimateEffortService;

        public EstimateEffortApiController(IEstimateEffortService estimateEffortService, IRoleTitleService roleTitleService)
            : base(estimateEffortService)
        {
            _estimateEffortService = estimateEffortService;
            _roleTitleService = roleTitleService;
        }

        public override ActionResult Index()
        {
            throw new InvalidOperationException();
        }

        public override ActionResult GetList()
        {
            throw new InvalidOperationException();

            Expression<Func<EstimateEffort, bool>> predicate = m => true;
            PageCondition pageCondition = new PageCondition(1, 10);

            var list = GetListEx(predicate, pageCondition);

            var result = new { total = list.Count, rows = list };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetData()
        {
            Expression<Func<EstimateEffort, bool>> predicate = m => true;
            PageCondition pageCondition = new PageCondition(1, 10);

            var list = GetListEx(predicate, pageCondition);

            var result = new List<string>();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveCellChangedData(List<EstimateEffortView> estimateEffortViews)
        {
            foreach (var estimateEffortView in estimateEffortViews)
            {
                if (estimateEffortView.Id == 0)
                {
                    _estimateEffortService.InsertView(estimateEffortView);
                }
                else
                {
                    _estimateEffortService.UpdateView(estimateEffortView);
                }
            }

            return Json(new { status = 200 }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SaveData(List<EstimateEffortView> estimateEffortViews)
        {
            foreach (var estimateEffortView in estimateEffortViews)
            {
                if (estimateEffortView.Id == 0)
                {
                    _estimateEffortService.InsertView(estimateEffortView);
                }
                else
                {
                    _estimateEffortService.UpdateView(estimateEffortView);
                }
            }

            return Json(new { status = 200 }, JsonRequestBehavior.AllowGet);
        }

        public override List<EstimateEffortView> GetListEx(Expression<Func<EstimateEffort, bool>> predicate, PageCondition ConPage)
        {
            string strProjectGid = Request["ProjectGid"];

            if (!string.IsNullOrEmpty(strProjectGid))
            {
                predicate = predicate.AndAlso(x => x.ProjectGid == strProjectGid);
            }

            var q = _objService.GetList<EstimateEffortView>(predicate.AndAlso(x => x.IsDeleted != true), ConPage);

            // 添加一个汇总
            var summary = new EstimateEffortView()
            {
                Id = int.MaxValue,
                ProjectGid = strProjectGid,
                ProjectId = 0,
                Effort = q.Sum(a => a.Effort),
                RoleTitle = "汇总",
                RateEffort = q.Sum(a => a.Effort * a.RoleRate)
            };

            q.Add(summary);

            return q.ToList();
        }
    }
}