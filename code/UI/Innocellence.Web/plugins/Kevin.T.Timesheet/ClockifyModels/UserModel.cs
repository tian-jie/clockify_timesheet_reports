using System.Collections.Generic;

namespace Kevin.T.Clockify.Data.Models
{
    public class UserModel
    {
        public string id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string profilePicture { get; set; }
        public string activeWorkspace { get; set; }
        public string defaultWorkspace { get; set; }
        public string status { get; set; }

        public List<MembershipModel> memberships { get; set; }
    }
}
