using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMeeting.Domain.Entity
{
    public class Meeting : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public String PersonArray { get; set; }
        public String Request { get; set; }
        public String Optional { get; set; }
        public String TimeSlot { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public String Location { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public DateTime? SignStartTime { get; set; }
        public DateTime? SignEndTime { get; set; }
        public String Owner { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String CreatedUser { get; set; }
        public Boolean? IsSignUp { get; set; }
        public DateTime? SignUpStartTime { get; set; }
        public DateTime? SignUpEndTime { get; set; }
        public Boolean? IsLimitPerNumber { get; set; }
        public Int32? AccountType { get;set;}
        public String PublicNumAppId { get; set; }
        public String EnterpriseCorpId { get; set; }
        public String EnterpriseAppId { get; set; }
        public Boolean? IsDeleted { get; set; }

       
        
    }
}