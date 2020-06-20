using System.Collections.Generic;

namespace Kevin.T.Clockify.Data.Models
{
    public class ProjectModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string clientId { get; set; }
        public string clientName { get; set; }
        public string workspaceId { get; set; }
        public string color { get; set; }
        public EstimateModel estimate { get; set; }
        public bool billable { get; set; }
        public bool archived { get; set; }
        public string duration { get; set; }
        public MembershipModel membership { get; set; }
        public string note { get; set; }
        public bool template { get; set; }

        public bool Public { get; set; }

    }

    public class EstimateModel
    {

    }
}
