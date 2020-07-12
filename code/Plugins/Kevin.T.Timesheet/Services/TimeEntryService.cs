using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.Entities;
using System.Collections.Generic;
using System;
using System.Linq;
using Kevin.T.Timesheet.ModelsView;

namespace Kevin.T.Timesheet.Services
{
    public class TimeEntryService : BaseService<TimeEntry>, ITimeEntryService
    {
        IUserGroupService _userGroupService;
        IProjectUserService _projectUserService;
        IProjectService _projectService;
        IProjectTaskService _projectTaskService;
        IEmployeeService _employeeService;
        IRoleTitleService _roleTitleService;

        public TimeEntryService(IUserGroupService userGroupService
            , IProjectUserService projectUserService
            , IProjectService projectService
            , IProjectTaskService projectTaskService
            , IEmployeeService employeeService
            , IRoleTitleService roleTitleService)
            : base("Timesheet")
        {
            _userGroupService = userGroupService;
            _projectUserService = projectUserService;
            _projectService = projectService;
            _projectTaskService = projectTaskService;
            _employeeService = employeeService;
            _roleTitleService = roleTitleService;
        }

        /// <summary>
        /// 根据给定的GroupId和指定的时间段，获取TimeEntry
        /// </summary>
        /// <param name="groupId">groupId，如果为空，则不过滤这个字段，全部查询</param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<TimeEntry> GetTimeEntriesByGroupAndDuration(string groupId, DateTime startDate, DateTime endDate)
        {
            List<TimeEntry> timeEntries;
            if (!string.IsNullOrEmpty(groupId))
            {
                var userIds = _userGroupService.Repository.Entities.Where(a => a.IsDeleted != true && a.GroupId == groupId).Select(a => a.UserId).ToList();

                timeEntries = Repository.Entities.Where(a => a.IsDeleted != true && userIds.Contains(a.UserId) && a.Date >= startDate && a.Date < endDate).ToList();
            }
            else
            {
                timeEntries = Repository.Entities.Where(a => a.IsDeleted != true && a.Date >= startDate && a.Date < endDate).ToList();
            }

            return timeEntries;
        }


        public List<TimeEntriesGroupByEmployeeView> GetTimeEntriesByProjectGroupByEmployee(string projectGid)
        {
            // 获取项目相关的员工
            // var projectUsers = _projectUserService.GetUserByProject(projectGid, null);
            var allUsers = _employeeService.AllEmployeesWithRole();
            var roleTitles = _roleTitleService.AllInternal();


            // 看这个projectGid，如果属于taskgid，就从taskgid过滤
            //var task = _projectTaskService.GetProjectTaskById(projectGid);

            // 再根据每个员工统计timeentry
            //var timeEntry = Repository.Entities.Where(a => a.IsDeleted != true);
            //if (task != null && task.Count > 0)
            //{
            //    timeEntry = timeEntry.Where(a => a.TaskId == projectGid);
            //}
            //else
            //{
            //    timeEntry = timeEntry.Where(a => a.ProjectId == projectGid);
            //}

            var sql = @"
                select * from TimeEntry TE where 
                exists(select 1 from Project P where P.Gid = TE.projectId and Gid = '{0}')
                or exists (select 1 from TaskToProjectMapping TPM where TPM.TaskGid = TE.TaskId and TPM.ProjectGid='{0}')";

            var timeEntry = Repository.SqlQuery(string.Format(sql, projectGid));


            var timeEntryByUsers = timeEntry.GroupBy(a => a.UserId);

            var timeEntriesGroupByEmployeesView = new List<TimeEntriesGroupByEmployeeView>();

            foreach (var a in timeEntryByUsers)
            {
                // var user = projectUsers.FirstOrDefault(b => b.UserGid == a.Key);

                var employee = allUsers.FirstOrDefault(b => b.Gid == a.Key);


                    timeEntriesGroupByEmployeesView.Add(new TimeEntriesGroupByEmployeeView()
                    {
                        UserId = a.Key,
                        TotalHours = a.Sum(b => b.TotalHours),
                        EmployeeRate = employee.RoleRate,
                        EmployeeRole = employee.RoleName,
                        TotalHoursRate = a.Sum(b => b.TotalHours) * employee.RoleRate,
                        EmployeeName = employee.Name
                    });
            }
            return timeEntriesGroupByEmployeesView;
        }

        public ProjectAccountingView GetTimeEntriesByProject(string projectGid)
        {
            var timeEntriesGroupByEmployeesView = GetTimeEntriesByProjectGroupByEmployee(projectGid);

            var projectInfo = _projectService.GetProjectById(projectGid);

            var projectAccountingView = new ProjectAccountingView()
            {
                ProjectGid = projectGid,
                ProjectId = projectInfo.Id,
                ProjectName = projectInfo.Name,
                SpentManHour = timeEntriesGroupByEmployeesView.Sum(a => a.TotalHours),
                SpentManHourRate = timeEntriesGroupByEmployeesView.Sum(a => a.TotalHoursRate)
            };

            return projectAccountingView;
        }

        public List<TimesheetByWeekView> GetTimeEntriesByEmployeeGroupByProject(string employeeId, DateTime startDate, DateTime endDate)
        {
            var timeEntries = Repository.Entities.Where(a => a.IsDeleted != true && a.Date >= startDate && a.Date < endDate && a.UserId == employeeId).ToList();
            var userTimeEntriesByUser = timeEntries.GroupBy(a => new { a.ProjectId, a.TaskId });

            var projects = _projectService.GetAllProjects();
            var tasks = _projectTaskService.GetAllProjectTasks();

            var timesheetByWeekViews = new List<TimesheetByWeekView>();
            // 把上面整理好的每个人每天的信息，整理到按周几放到的TimesheetByWeekView里。
            foreach (var ud in userTimeEntriesByUser)
            {
                var project = projects.FirstOrDefault(a => a.Gid == ud.Key.ProjectId);
                var task = tasks.FirstOrDefault(a => a.Gid == ud.Key.TaskId);
                var tv = new TimesheetByWeekView()
                {
                    ProjectName = project == null ? "N/A Project" : project.Name,
                    TaskName = task == null ? "/-" : " / " + task.Name,
                    ProjectGid = ud.Key.ProjectId,
                    TaskGid = ud.Key.TaskId
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

            return timesheetByWeekViews;
        }

        public List<EmployeeView> GetEmployeeByProjectTimeEntry(string projectGid)
        {
            //var employeeIds = Repository.Entities.Where(a => a.IsDeleted != true && a.ProjectId == projectGid).Select(a => a.UserId).Distinct();
            var sql = @"select * from TimeEntry TE
where exists(select 1 from TaskToProjectMapping TPM where TPM.ProjectGid = TE.ProjectId and TPM.ProjectGId = '{0}')
or exists(select 1 from TaskToProjectMapping TPM where TPM.TaskGid = TE.TaskId and TPM.ProjectGId = '{0}')";

            var employeeIds = Repository.SqlQuery(string.Format(sql, projectGid)).Select(a => a.UserId).Distinct();

            var employees = new List<EmployeeView>();
            var allEmployees = _employeeService.Repository.Entities.Where(a => a.IsDeleted != true);
            foreach (var employeeId in employeeIds)
            {
                var employee = allEmployees.FirstOrDefault(a => a.Gid == employeeId);
                var employeeView = new EmployeeView()
                {
                    Id = employee.Id,
                    Gid = employee.Gid,
                    Name = employee.Name,
                    DefaultWorkspace = employee.DefaultWorkspace,
                    Email = employee.Email,
                    ProfilePicture = employee.ProfilePicture,
                    Status = employee.Status
                };

                employees.Add(employeeView);
            }

            return employees;
        }
    }
}
