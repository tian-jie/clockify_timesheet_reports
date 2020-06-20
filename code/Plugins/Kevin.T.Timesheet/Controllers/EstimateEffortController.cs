using Infrastructure.Core.Logging;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Kevin.T.Timesheet.Controllers
{
    public class EstimateEffortController : BaseController<EstimateEffort, EstimateEffortView>
    {
        private ILogger Logger = LogManager.GetLogger("Timesheet");
        private IRoleTitleService _roleTitleService;
        private IEstimateEffortService _estimateEffortService;

        public EstimateEffortController(IEstimateEffortService estimateEffortService, IRoleTitleService roleTitleService)
            : base(estimateEffortService)
        {
            _estimateEffortService = estimateEffortService;
            _roleTitleService = roleTitleService;
        }

        public override ActionResult Index()
        {
            ViewBag.RoleTitles = _roleTitleService.All().ToList();
            return base.Index();
        }

        public override ActionResult GetList()
        {
            Expression<Func<EstimateEffort, bool>> predicate = m => true;
            PageCondition pageCondition = new PageCondition(1, 10);

            var list = GetListEx(predicate, pageCondition);

            var result = new { total = list.Count, rows = list };
            return Json(result, JsonRequestBehavior.AllowGet);
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
                Id = 999,
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