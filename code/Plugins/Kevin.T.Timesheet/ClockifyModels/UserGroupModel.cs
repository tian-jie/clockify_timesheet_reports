using System.Collections.Generic;

namespace Kevin.T.Clockify.Data.Models
{
    public class UserGroupModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string workspaceId { get; set; }
        public List<string> userIds { get; set; }
    }
}
