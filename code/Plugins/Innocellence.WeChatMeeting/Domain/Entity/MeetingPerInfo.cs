using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMeeting.Domain.Entity
{
    public class MeetingPerInfo : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public String UserId { get; set; }
        public String Name { get; set; }
        public String ContactPhone { get; set; }
        public String Mailbox { get; set; }
        public String AssistantName { get; set; }
        public String AssistantPhone { get; set; }
        public String EmergencyContact { get; set; }
        public String EmergencyConPhone { get; set; }
        public String Remarks { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }
    }
}