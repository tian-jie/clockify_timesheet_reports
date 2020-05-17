using Infrastructure.Core.Logging;
using Infrastructure.Web.UI;
using Innocellence.Web.Controllers;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Kevin.T.Timesheet.Controllers
{
    public class ProjectAccountingController : BaseController<TimeEntry, TimeEntryView>
    {
        private readonly ILogger Logger = LogManager.GetLogger("TimesheetController");
        private readonly ITimeEntryService _timeEntryService;
        private readonly ITimesheetService _timesheetService;
        private readonly IGroupService _groupService;
        private readonly IUserGroupService _userGroupService;
        private readonly IEmployeeService _employeeService;
        private readonly IProjectService _projectService;


        public ProjectAccountingController(ITimeEntryService timeEntryService, ITimesheetService timesheetService,
            IGroupService groupService, IUserGroupService userGroupService,
            IEmployeeService employeeService,
            IProjectService projectService)
            : base(timeEntryService)
        {
            _timeEntryService = timeEntryService;
            _timesheetService = timesheetService;
            _groupService = groupService;
            _userGroupService = userGroupService;
            _employeeService = employeeService;
            _projectService = projectService;
        }

        [HttpGet]
        public override ActionResult Index()
        {
            return View();
        }

        private List<TimeEntriesGroupByEmployeeView> GetProjectEffortByEmployeePrivate(string projectGid)
        {
            // 获取项目信息
            var timeEntriesGroupByEmployeesView = _timeEntryService.GetTimeEntriesByProjectGroupByEmployee(projectGid);

            return timeEntriesGroupByEmployeesView;
        }

        [HttpPost]
        public override ActionResult GetList()
        {
            var req = new GridRequest(Request);

            var projectGid = Request["projectGid"];
            var projectAccountingInfo = GetProjectEffortByEmployeePrivate(projectGid);
            return GetPageResult(projectAccountingInfo, req);
        }

        [HttpPost]
        public ActionResult GetProjectAccountingInfo(string projectGid)
        {
            var timeEntriesGroupByEmployeesView = GetProjectEffortByEmployeePrivate(projectGid);
            var projectInfo = _projectService.GetProjectById(projectGid);
            var projectAccountingInfo = _projectService.GetProjectEstimatedEffortById(projectGid);

            projectAccountingInfo.Id = 1;
            projectAccountingInfo.ProjectGid = projectGid;
            projectAccountingInfo.ProjectId = projectInfo.Id;
            projectAccountingInfo.ProjectName = projectInfo.Name;
            projectAccountingInfo.TimeEntriesGroupByEmployeesView = timeEntriesGroupByEmployeesView;
            projectAccountingInfo.SpentManHour = timeEntriesGroupByEmployeesView.Sum(a => a.TotalHours);
            projectAccountingInfo.SpentManHourRate = timeEntriesGroupByEmployeesView.Sum(a => a.TotalHoursRate);


            return Json(projectAccountingInfo, JsonRequestBehavior.AllowGet);
        }

    }
}