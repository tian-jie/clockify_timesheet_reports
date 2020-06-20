using System;

namespace Kevin.T.Timesheet.ModelsView
{
    public partial class TimeEntriesGroupByEmployeeView
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeRole { get; set; }

        public decimal EmployeeRate { get; set; }

        public virtual decimal TotalHours { get; set; }

        public virtual decimal TotalHoursRate { get; set; }

    }
}
