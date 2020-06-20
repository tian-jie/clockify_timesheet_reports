using Infrastructure.Core;
using Innocellence.WeChatMeeting.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMeeting.Domain.ViewModel
{
    public class MeetingInvitationView : IViewModel
    {
        public MeetingInvitationView() { }

        public  Int32 Id { get; set; }
        public Int32? MeetingId { get; set; }
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

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (MeetingInvitation)obj;
            Id = entity.Id;
            MeetingId = entity.MeetingId;
            Type = entity.Type;
            UserId = entity.UserId;
            UserName = entity.UserName;
            State = entity.State;
            CheckInDt = entity.CheckInDt;
            StartDateTime = entity.StartDateTime;
            EndDateTime = entity.EndDateTime;
            SignStartTime = entity.SignStartTime;
            SignEndTime = entity.SignEndTime;
            TimeClass = entity.TimeClass;
            OpenId = entity.OpenId;
            IsDeleted = entity.IsDeleted;

            return this;
        }


    }
}