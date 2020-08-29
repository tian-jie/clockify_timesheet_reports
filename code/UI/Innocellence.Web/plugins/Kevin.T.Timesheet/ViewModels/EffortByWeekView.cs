namespace Kevin.T.Timesheet.ModelsView
{
    /// <summary>
    /// 项目统计
    /// </summary>
    public partial class EffortByWeekView
    {
        /// <summary>
        /// Year
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Week
        /// </summary>
        public int Week { get; set; }

        /// <summary>
        /// TotalHours
        /// </summary>
        public decimal TotalHours { get; set; }

        /// <summary>
        /// TotalHoursRate
        /// </summary>
        public decimal TotalHoursRate { get; set; }

    }
}
