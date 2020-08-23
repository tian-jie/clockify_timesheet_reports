using System.Collections.Generic;

namespace Kevin.T.Clockify.Data.Models
{
    public class TimeEntryResponseModelV2
    {
        public List<ClockifyResponsePageInfo> totals { get; set; }


        public List<TimeEntryModelV2> timeEntries { get; set; }

    }

}
