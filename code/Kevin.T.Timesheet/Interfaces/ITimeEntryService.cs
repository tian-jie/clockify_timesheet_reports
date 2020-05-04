﻿using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.ModelsView;
using System;
using System.Collections.Generic;

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

        List<TimeEntriesGroupByEmployeeView> GetTimeEntriesByProjectGroupByEmployee(string projectGid);
    }
}
