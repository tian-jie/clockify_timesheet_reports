using Infrastructure.Core.Logging;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Kevin.T.Timesheet.Controllers
{
    public class TimesheetController : BaseController<TimeEntry, TimeEntryView>
    {
        private readonly ILogger Logger = LogManager.GetLogger("TimesheetController");
        private readonly ITimeEntryService _timeEntryService;
        private readonly ITimesheetService _timesheetService;
        private readonly IGroupService _groupService;
        private readonly IUserGroupService _userGroupService;
        private readonly IEmployeeService _employeeService;
        private readonly IProjectService _projectService;


        public TimesheetController(ITimeEntryService timeEntryService, ITimesheetService timesheetService,
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

        public override ActionResult Index()
        {
            var groups = _groupService.Repository.Entities.Where(a => a.IsDeleted != true).OrderBy(a => a.Name);
            ViewBag.Groups = groups;
            return View();
        }

        public ActionResult GetTimesheetThisweekByEmployee(string employeeId, int year = 2020, int week = 1)
        {
            // TODO: 应该获取当前的项目id，员工，查询这个员工所有的当周的录入情况，group by day，分别放到MON, TUE....中

            // 计算FirstWeek周期
            var yearFirstDay = new DateTime(DateTime.Now.Year, 1, 1);
            var firstDayofWeek = (int)yearFirstDay.DayOfWeek;
            DateTime weekFirstDay = DateTime.Now;
            DateTime weekLastDay = DateTime.Now;

            weekFirstDay = yearFirstDay.AddDays(-(firstDayofWeek == 0 ? 6 : firstDayofWeek - 1));
            var firstThursday = weekFirstDay.AddDays(3);

            if (yearFirstDay >= firstThursday)
            {
                weekFirstDay = weekFirstDay.AddDays(7);
            }

            var thisMonday = weekFirstDay.AddDays((week - 1) * 7);
            var nextMonday = thisMonday.AddDays(7);

            var timeEntries = _timeEntryService.GetTimeEntriesByEmployeeGroupByProject(employeeId, thisMonday, nextMonday);


            var result = new { total = timeEntries.Count, rows = timeEntries };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTimesheetThisweekByGroup(string groupId, int year = 0, int week = 0)
        {
            // 计算FirstWeek周期
            var yearFirstDay = new DateTime(DateTime.Now.Year, 1, 1);
            var firstDayofWeek = (int)yearFirstDay.DayOfWeek;
            DateTime weekFirstDay = DateTime.Now;
            DateTime weekLastDay = DateTime.Now;

            weekFirstDay = yearFirstDay.AddDays(-(firstDayofWeek == 0 ? 6 : firstDayofWeek - 1));
            var firstThursday = weekFirstDay.AddDays(3);

            if (yearFirstDay >= firstThursday)
            {
                weekFirstDay = weekFirstDay.AddDays(7);
            }

            var thisMonday = weekFirstDay.AddDays((week - 1) * 7);
            var nextMonday = thisMonday.AddDays(7);

            var timeEntries = _timeEntryService.GetTimeEntriesByGroupAndDuration(groupId, thisMonday, nextMonday);

            // 拿到所有的timeEntry后，根据每个人，组成当前week的数据
            var userTimeEntriesByUser = timeEntries.GroupBy(a => a.UserId);

            var usersRepo = _employeeService.Repository;
            var users = string.IsNullOrEmpty(groupId) ?
                                usersRepo.SqlQuery(
                $"select * from employee E where exists(select 1 from usergroup UG where UG.UserId = E.Gid) ")
:
            usersRepo.SqlQuery(
                $"select * from employee E where exists(select 1 from usergroup UG where UG.GroupId='{groupId}' and UG.UserId = E.Gid)");

            var timesheetByWeekViews = new List<TimesheetByWeekView>();
            // 把上面整理好的每个人每天的信息，整理到按周几放到的TimesheetByWeekView里。
            foreach (var ud in userTimeEntriesByUser)
            {
                var user = users.FirstOrDefault(a => a.Gid == ud.Key);
                var tv = new TimesheetByWeekView()
                {
                    UserId = ud.Key,
                    EmployeeName = user == null ? "" : user.Name,
                };

                foreach (var udd in ud.GroupBy(a => a.Date))
                {
                    var totalHoursByDate = udd.Sum(a => a.TotalHours);

                    var weekOfDay = udd.Key.DayOfWeek;
                    switch (weekOfDay)
                    {
                        case DayOfWeek.Monday:
                            tv.MondayTotalHours = totalHoursByDate;
                            break;
                        case DayOfWeek.Tuesday:
                            tv.TuesdayTotalHours = totalHoursByDate;
                            break;
                        case DayOfWeek.Wednesday:
                            tv.WednesdayTotalHours = totalHoursByDate;
                            break;
                        case DayOfWeek.Thursday:
                            tv.ThursdayTotalHours = totalHoursByDate;
                            break;
                        case DayOfWeek.Friday:
                            tv.FridayTotalHours = totalHoursByDate;
                            break;
                        case DayOfWeek.Saturday:
                            tv.SaturdayTotalHours = totalHoursByDate;
                            break;
                        case DayOfWeek.Sunday:
                            tv.SundayTotalHours = totalHoursByDate;
                            break;
                    }

                }
                timesheetByWeekViews.Add(tv);

            }

            // 添加其他没填的人
            foreach (var user in users)
            {
                var timesheetByWeekView = timesheetByWeekViews.FirstOrDefault(a => a.UserId == user.Gid);
                if (timesheetByWeekView == null)
                {
                    timesheetByWeekView = new TimesheetByWeekView()
                    {
                        UserId = user.Gid
                    };

                    timesheetByWeekViews.Add(timesheetByWeekView);
                }

                timesheetByWeekView.EmployeeName = user.Name;
            }
            GridRequest req = new GridRequest(Request);

            var result = new { total = timesheetByWeekViews.Count, rows = timesheetByWeekViews };
            return Json(result, JsonRequestBehavior.AllowGet);

            //return GetPageResult(timesheetByWeekViews, req);
        }

    }
}