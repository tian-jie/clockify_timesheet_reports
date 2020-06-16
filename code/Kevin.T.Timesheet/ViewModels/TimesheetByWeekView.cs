using System;

namespace Kevin.T.Timesheet.ModelsView
{
    public partial class TimesheetByWeekView
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string EmployeeName { get; set; }

        public string ProjectGid { get; set; }

        public string TaskGid { get; set; }

        public string ProjectName { get; set; }

        public string TaskName { get; set; }

        public virtual decimal MondayTotalHours { get; set; }
        public virtual decimal TuesdayTotalHours { get; set; }
        public virtual decimal WednesdayTotalHours { get; set; }
        public virtual decimal ThursdayTotalHours { get; set; }
        public virtual decimal FridayTotalHours { get; set; }
        public virtual decimal SaturdayTotalHours { get; set; }
        public virtual decimal SundayTotalHours { get; set; }


    }
}
