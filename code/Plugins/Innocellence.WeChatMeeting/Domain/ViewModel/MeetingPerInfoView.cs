using Infrastructure.Core;
using Innocellence.WeChatMeeting.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMeeting.Domain.ViewModel
{
    public class MeetingPerInfoView : IViewModel
    {
        public MeetingPerInfoView() { }

        public  Int32 Id { get; set; }
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

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (MeetingPerInfo)obj;
            Id = entity.Id;
            UserId = entity.UserId;
            Name = entity.Name;
            ContactPhone = entity.ContactPhone;
            Mailbox = entity.Mailbox;
            AssistantName = entity.AssistantName;
            AssistantPhone = entity.AssistantPhone;
            EmergencyContact = entity.EmergencyContact;
            EmergencyConPhone = entity.EmergencyConPhone;
            Remarks = entity.Remarks;
            CreatedDate = entity.CreatedDate;
            IsDeleted = entity.IsDeleted;

            return this;
        }
    }
}