using System.Collections.Generic;

namespace Kevin.T.Clockify.Data.Models
{
    public class TaskModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string projectId { get; set; }
        public List<string> assigneeIds { get; set; }
        public string assigneeId { get; set; }
        public string estimate { get; set; }
        public string status { get; set; }
        public string duration { get; set; }

    }
}
