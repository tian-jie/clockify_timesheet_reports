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
            // 获取项目列表
            var projects = _projectService.GetAllProjects();
            ViewBag.Projects = projects;
            var projectAccountingInfo = new ProjectAccountingView()
            {
                ProjectGid = "-",
                ProjectId = -1,
                ProjectName = "-",
                SpentManHour = -1,
                SpentManHourRate = -1
            };

            ViewBag.ProjectInfo = projectAccountingInfo;

            return View();
        }

        [HttpPost]
        public override ActionResult GetList()
        {
            var projectGid = Request["projectGid"];
            var req = new GridRequest(Request);

            if (string.IsNullOrEmpty(projectGid))
            {
                return GetPageResult(new List<TimeEntriesGroupByEmployeeView>(), req);
            }
            // 获取项目信息
            var projectInfo = _projectService.GetProjectById(projectGid);

            var timeEntriesGroupByEmployeesView = _timeEntryService.GetTimeEntriesByProjectGroupByEmployee(projectGid);
            var id = 1;
            var projectAccountingInfo = new ProjectAccountingView()
            {
                Id = id++,
                ProjectGid = projectGid,
                ProjectId = projectInfo.Id,
                ProjectName = projectInfo.Name,
                TimeEntriesGroupByEmployeesView = timeEntriesGroupByEmployeesView,
                SpentManHour = timeEntriesGroupByEmployeesView.Sum(a => a.TotalHours),
                SpentManHourRate = timeEntriesGroupByEmployeesView.Sum(a => a.TotalHoursRate)
            };

            ViewBag.ProjectInfo = projectAccountingInfo;


            return GetPageResult(timeEntriesGroupByEmployeesView, req);
        }

    }
}