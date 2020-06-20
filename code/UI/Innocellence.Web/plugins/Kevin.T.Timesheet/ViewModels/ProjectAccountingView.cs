using System;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.ModelsView
{
    /// <summary>
    /// 项目统计
    /// </summary>
    public partial class ProjectAccountingView
    {
        /// <summary>
        /// View用的ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 项目ID，或者项目号
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Clockify里的id字符串，项目ID
        /// </summary>
        public string ProjectGid { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 估算工时
        /// </summary>
        public decimal EstimatedSpentManHour { get; set; }

        /// <summary>
        /// 估算 带rate的工时，rate根据RoleTitle.Rate计算，projectAccountingByWeeks汇总
        /// </summary>
        public decimal EstimatedSpentManHourRate { get; set; }

        /// <summary>
        /// 已花费工时
        /// </summary>
        public decimal SpentManHour { get; set; }

        /// <summary>
        /// 已花费 带rate的工时，rate根据RoleTitle.Rate计算，projectAccountingByWeeks汇总
        /// </summary>
        public decimal SpentManHourRate { get; set; }

        // 第一层 by week
        // 第二层 by 员工
        public List<ProjectAccountingByWeek> ProjectAccountingByWeeks { get; set; }

        /// <summary>
        /// 所有数据by员工的汇总，不by week
        /// </summary>
        public List<TimeEntriesGroupByEmployeeView> TimeEntriesGroupByEmployeesView { get; set; }

    }

    /// <summary>
    /// 项目统计，每周的情况
    /// </summary>
    public class ProjectAccountingByWeek
    {
        /// <summary>
        /// 已花费工时，by周
        /// </summary>
        public decimal SpentManHour { get; set; }

        /// <summary>
        /// 已花费工时，by周,rate
        /// </summary>
        public decimal SpentManHourRate { get; set; }

        public List<ProjectAccountingByWeekByEmployee> projectAccountingByWeekByEmployees { get; set; }
    }

    /// <summary>
    /// 项目统计，每人的情况，by周
    /// </summary>
    public class ProjectAccountingByWeekByEmployee
    {
        /// <summary>
        /// 已花费工时，by周，by人
        /// </summary>
        public decimal SpentManHour { get; set; }

        /// <summary>
        /// 已花费工时，by周,by人,rate
        /// </summary>
        public decimal SpentManHourRate { get; set; }

    }
}
