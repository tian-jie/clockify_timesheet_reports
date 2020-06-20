using System.Collections.Generic;

namespace Kevin.T.Clockify.Data.Models
{
    public class MembershipModel
    {
        public string userId { get; set; }
        public string targetId { get; set; }
        public string membershipType { get; set; }
        public string membershipStatus { get; set; }

        public HourlyRateModel hourlyRate { get; set; }
    }
}
