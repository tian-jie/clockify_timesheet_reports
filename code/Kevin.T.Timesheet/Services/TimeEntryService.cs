﻿using Infrastructure.Core.Data;
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

        public TimeEntryService(IUserGroupService userGroupService
            , IProjectUserService projectUserService
            , IProjectService projectService)
            : base("Timesheet")
        {
            _userGroupService = userGroupService;
            _projectUserService = projectUserService;
            _projectService = projectService;
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
            var projectUsers = _projectUserService.GetUserByProject(projectGid, null);
            var allUsers = _projectUserService.Repository.Entities;

            // 再根据每个员工统计timeentry
            var timeEntryByUsers = Repository.Entities.Where(a => a.IsDeleted != true && a.ProjectId == projectGid).GroupBy(a => a.UserId);
            var timeEntriesGroupByEmployeesView = new List<TimeEntriesGroupByEmployeeView>();

            foreach (var a in timeEntryByUsers)
            {
                var user = projectUsers.FirstOrDefault(b => b.UserGid == a.Key);

                if (user == null)
                {
                    timeEntriesGroupByEmployeesView.Add(new TimeEntriesGroupByEmployeeView()
                    {
                        UserId = a.Key,
                        TotalHours = a.Sum(b => b.TotalHours),
                        EmployeeRate = 1,
                        EmployeeRole = "N/A",
                        TotalHoursRate = a.Sum(b => b.TotalHours),
                        EmployeeName = allUsers == null ? "Unknown???" : allUsers.FirstOrDefault(b => b.UserGid == a.Key).EmployeeName
                    });
                }
                else
                {
                    timeEntriesGroupByEmployeesView.Add(new TimeEntriesGroupByEmployeeView()
                    {
                        UserId = a.Key,
                        TotalHours = a.Sum(b => b.TotalHours),
                        EmployeeRate = user.Rate,
                        EmployeeRole = user.UserRoleTitle,
                        TotalHoursRate = user.Rate * a.Sum(b => b.TotalHours),
                        EmployeeName = user.EmployeeName
                    });
                }
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
    }
}
