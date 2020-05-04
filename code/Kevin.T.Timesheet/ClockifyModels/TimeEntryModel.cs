using System.Collections.Generic;

namespace Kevin.T.Clockify.Data.Models
{
    public class TimeEntryModel
    {
        public string id { get; set; }
        public string description { get; set; }
        public string userId { get; set; }
        public string projectId { get; set; }
        public List<TagModel> tags { get; set; }
        public List<string> customFieldValues { get; set; }
        public UserModel user { get; set; }
        public TaskModel task { get; set; }
        public ProjectModel project { get; set; }
        public TimeIntervalModel timeInterval { get; set; }
        public string workspaceId { get; set; }
        public float totalBillable { get; set; }
        public float totalBillableDecimal { get; set; }
        public HourlyRateModel hourlyRate { get; set; }
        public bool isLocked { get; set; }
    }
}
