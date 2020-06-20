using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMeeting.Domain.Entity
{
    public class MeetingInvitation : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public int? MeetingId { get; set; }
        public String Type { get; set; }
        public String UserId { get; set; }

        public String UserName { get; set; }
        public Int32? State { get; set; }
        public Boolean? CheckIn { get; set; }
        public DateTime? CheckInDt { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public DateTime? SignStartTime { get; set; }
        public DateTime? SignEndTime { get; set; }
        public String TimeClass { get; set; }
        public String OpenId { get; set; }
        public Boolean? IsDeleted { get; set; }
    }
}