using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface ITimeEntryService : IDependency, IBaseService<TimeEntry>
    {
        /// <summary>
        /// 根据给定的GroupId和指定的时间段，获取TimeEntry
        /// </summary>
        /// <param name="groupId">groupId，如果为空，则不过滤这个字段，全部查询</param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        List<TimeEntry> GetTimeEntriesByGroupAndDuration(string groupId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 统计项目花费信息，by到每一个成员
        /// </summary>
        /// <param name="projectGid"></param>
        /// <returns></returns>
        List<TimeEntriesGroupByEmployeeView> GetTimeEntriesByProjectGroupByEmployee(string projectGid);

        /// <summary>
        /// 统计项目花费信息
        /// </summary>
        /// <param name="projectGid"></param>
        /// <returns></returns>
        ProjectAccountingView GetTimeEntriesByProject(string projectGid);

        /// <summary>
        /// 获取员工在这段时间内的每个项目的花费时间
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        List<TimesheetByWeekView> GetTimeEntriesByEmployeeGroupByProject(string employeeId, DateTime startDate, DateTime endDate);
    }
}
