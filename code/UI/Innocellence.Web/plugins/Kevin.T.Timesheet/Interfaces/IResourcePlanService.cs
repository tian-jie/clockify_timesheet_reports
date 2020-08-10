﻿using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.ModelsView;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface IResourcePlanService : IDependency, IBaseService<ResourcePlan>
    {
        /// <summary>
        /// 上传excel模板，并且根据模板更新项目的resource plan
        /// </summary>
        /// <param name="excelFilename"></param>
        /// <param name="projectGid"></param>
        void UploadResourcePlan(string excelFilename, string projectGid);

        /// <summary>
        /// 取出某个project的所有resource plan
        /// </summary>
        /// <param name="projectGid"></param>
        List<ResourcePlan> GetResourcePlanByProject(string projectGid);

        /// <summary>
        /// 删除某个项目下的所有ResourcePlan
        /// </summary>
        /// <param name="projectGid"></param>
        void DeleteByProject(string projectGid);

        List<TimeEntriesGroupByEmployeeView> GetTimeEntriesByProjectGroupByEmployee(string projectGid);
    }
}
