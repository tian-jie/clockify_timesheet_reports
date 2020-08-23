using System.Collections.Generic;

namespace Kevin.T.Clockify.Data.Models
{
    public class TimeEntryModelV2
    {
        public string _id { get; set; }

        public bool isLocked { get; set; }

        public bool billable { get; set; }

        public string description { get; set; }

        public string projectColor { get; set; }

        public string projectId { get; set; }

        public string taskId { get; set; }

        public string projectName { get; set; }

        public string userId { get; set; }

        public string userName { get; set; }

        public string userEmail { get; set; }

        public TimeIntervalModel timeInterval { get; set; }

    }
}
