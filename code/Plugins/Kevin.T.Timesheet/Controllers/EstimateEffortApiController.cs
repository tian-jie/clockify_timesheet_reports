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
            PageCondition pageCondition = new PageCondition(1, 1000);

            var list = GetListEx(predicate, pageCondition);

            return Json(list.OrderBy(a=>a.RoleId).ToList(), JsonRequestBehavior.AllowGet);
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

            if (q.Count == 0)
            {
                // TODO: 如果数据为空的话，就动态创建一条完整的
                var externalRoles = _roleTitleService.AllExternal();
                var estimateRoles = new List<EstimateEffortView>();
                foreach (var r in externalRoles)
                {
                    estimateRoles.Add(new EstimateEffortView()
                    {
                        ProjectGid = strProjectGid,
                        Effort = 0,
                        ProjectId = 0,
                        RateEffort = 0,
                        RoleId = r.Id,
                        RoleRate = r.Rate,
                        RoleTitle = r.Title
                    });

                }

                SaveData(estimateRoles);

                q = _objService.GetList<EstimateEffortView>(predicate.AndAlso(x => x.IsDeleted != true), ConPage);
            }


            return q.ToList();
        }
    }
}