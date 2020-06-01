using Infrastructure.Core.Logging;
using Innocellence.Web.Controllers;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;

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
            return View();
        }

    }
}