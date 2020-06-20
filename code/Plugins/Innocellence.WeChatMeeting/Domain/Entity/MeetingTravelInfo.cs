using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMeeting.Domain.Entity
{
    public class MeetingTravelInfo : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public int? MeetingId { get; set; }
        public String UserId { get; set; }
        public String UserName { get; set; }
        public String Mode { get; set; }
        public String Flight_Train { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public String ExpectArrivalTime { get; set; }
        public String Type { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String Remarks { get; set; }
        public Boolean? IsDeleted { get; set; }

    }
}