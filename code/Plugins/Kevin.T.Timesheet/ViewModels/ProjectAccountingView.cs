using System;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.ModelsView
{
    /// <summary>
    /// ��Ŀͳ��
    /// </summary>
    public partial class ProjectAccountingView
    {
        /// <summary>
        /// View�õ�ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ��ĿID��������Ŀ��
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Clockify���id�ַ�������ĿID
        /// </summary>
        public string ProjectGid { get; set; }

        /// <summary>
        /// ��Ŀ����
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// ���㹤ʱ
        /// </summary>
        public decimal EstimatedSpentManHour { get; set; }

        /// <summary>
        /// ���� ��rate�Ĺ�ʱ��rate����RoleTitle.Rate���㣬projectAccountingByWeeks����
        /// </summary>
        public decimal EstimatedSpentManHourRate { get; set; }

        /// <summary>
        /// �ѻ��ѹ�ʱ
        /// </summary>
        public decimal SpentManHour { get; set; }

        /// <summary>
        /// �ѻ��� ��rate�Ĺ�ʱ��rate����RoleTitle.Rate���㣬projectAccountingByWeeks����
        /// </summary>
        public decimal SpentManHourRate { get; set; }

        // ��һ�� by week
        // �ڶ��� by Ա��
        public List<ProjectAccountingByWeek> ProjectAccountingByWeeks { get; set; }

        /// <summary>
        /// ��������byԱ���Ļ��ܣ���by week
        /// </summary>
        public List<TimeEntriesGroupByEmployeeView> TimeEntriesGroupByEmployeesView { get; set; }

    }

    /// <summary>
    /// ��Ŀͳ�ƣ�ÿ�ܵ����
    /// </summary>
    public class ProjectAccountingByWeek
    {
        /// <summary>
        /// �ѻ��ѹ�ʱ��by��
        /// </summary>
        public decimal SpentManHour { get; set; }

        /// <summary>
        /// �ѻ��ѹ�ʱ��by��,rate
        /// </summary>
        public decimal SpentManHourRate { get; set; }

        public List<ProjectAccountingByWeekByEmployee> projectAccountingByWeekByEmployees { get; set; }
    }

    /// <summary>
    /// ��Ŀͳ�ƣ�ÿ�˵������by��
    /// </summary>
    public class ProjectAccountingByWeekByEmployee
    {
        /// <summary>
        /// �ѻ��ѹ�ʱ��by�ܣ�by��
        /// </summary>
        public decimal SpentManHour { get; set; }

        /// <summary>
        /// �ѻ��ѹ�ʱ��by��,by��,rate
        /// </summary>
        public decimal SpentManHourRate { get; set; }

    }
}
