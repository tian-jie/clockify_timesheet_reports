using Infrastructure.Core.Logging;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain;
using Kevin.T.Timesheet.Common;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System;
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
        private readonly IResourcePlanService _resourcePlanService;


        public ProjectAccountingController(ITimeEntryService timeEntryService, ITimesheetService timesheetService,
            IGroupService groupService, IUserGroupService userGroupService,
            IEmployeeService employeeService,
            IProjectService projectService,
            IResourcePlanService resourcePlanService)
            : base(timeEntryService)
        {
            _timeEntryService = timeEntryService;
            _timesheetService = timesheetService;
            _groupService = groupService;
            _userGroupService = userGroupService;
            _employeeService = employeeService;
            _projectService = projectService;
            _resourcePlanService = resourcePlanService;
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

            var today = DateTime.Now.Date;
            // 获取今天，以及今天的w和y
            var year = today.YearOfWeekOfYear();
            var week = today.WeekOfYear();

            // 计算本周一
            var weekDay = today.DayOfWeek;

            var monday = today.AddDays(-(int)weekDay + 1);
            // 计算AC
            projectAccountingInfo.ActualCostByWeek = _timeEntryService.GetActualEffortByWeek(projectGid, monday);
            decimal lastTotalHours = 0;
            decimal lastTotalHoursRate = 0;
            // 每周的数据要加上前一周的数据
            foreach (var acbw in projectAccountingInfo.ActualCostByWeek)
            {
                acbw.TotalHours += lastTotalHours;
                acbw.TotalHoursRate += lastTotalHoursRate;

                lastTotalHours = acbw.TotalHours;
                lastTotalHoursRate = acbw.TotalHoursRate;
            }

            // 计算EC
            lastTotalHours = 0;
            lastTotalHoursRate = 0;
            projectAccountingInfo.EstimateToCompletionByWeek = _resourcePlanService.GetBudgetByWeek(projectGid, year, week);
            foreach (var etcbw in projectAccountingInfo.EstimateToCompletionByWeek)
            {
                etcbw.TotalHours += lastTotalHours;
                etcbw.TotalHoursRate += lastTotalHoursRate;

                lastTotalHours = etcbw.TotalHours;
                lastTotalHoursRate = etcbw.TotalHoursRate;
            }

            // 计算ETC
            projectAccountingInfo.EAC = lastTotalHoursRate;
            // 数据怎么给eChart做折线图

            return Json(projectAccountingInfo, JsonRequestBehavior.AllowGet);
        }

    }
}