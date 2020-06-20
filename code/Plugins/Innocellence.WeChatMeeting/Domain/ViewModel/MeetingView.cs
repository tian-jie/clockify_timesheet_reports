using Infrastructure.Core;
using Innocellence.WeChatMeeting.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMeeting.Domain.ViewModel
{
    public partial class MeetingView : IViewModel
    {
        public MeetingView() {
            MeetingViews = new List<MeetingView>();
            List = new List<MeetingView>();
        }
        public  Int32 Id { get; set; }
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
        public Int32? AccountType { get; set; }
        public String PublicNumAppId { get; set; }
        public String EnterpriseCorpId { get; set; }
        public String EnterpriseAppId { get; set; }
        public Boolean? IsDeleted { get; set; }

        public IList<MeetingView> List { get; set; }
        public List<MeetingView> MeetingViews { get; set; }

        public virtual List<String> SendToGroup { get; set; }
        public virtual List<String> SendToPerson { get; set; }
        public virtual List<String> SendToPersonName { get; set; }

        public virtual List<String> SendToGroupName { get; set; }
        public virtual List<String> SendToTag { get; set; }
        public virtual List<String> SendToTagName { get; set; }

        public virtual String SaveFullName { get; set; }
        public virtual String FileName { get; set; }
        public virtual String TargetFilePath { get; set; }
        public virtual String UploadFileType { get; set; }

        //我的会议
        public virtual String IsPerInfo { get; set; }//是否填写报名信息
        public virtual String IsTravel { get; set; }//是否填写行程信息
        public virtual String STime { get; set; }//会议开始时间
        public virtual String ETime { get; set; }//会议结束时间
        public virtual String SsTime { get; set; }//签到开始时间
        public virtual String SeTime { get; set; }//签到结束时间
        public virtual String TimeClass { get; set; }


        



        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (Meeting)obj;
            Id = entity.Id;
            PersonArray = entity.PersonArray;
            Request = entity.Request;
            Optional = entity.Optional;
            TimeSlot = entity.TimeSlot;
            Title = entity.Title;
            Content = entity.Content;
            Location = entity.Location;
            StartDateTime = entity.StartDateTime;
            EndDateTime = entity.EndDateTime;
            SignStartTime = entity.SignStartTime;
            SignEndTime = entity.SignEndTime;
            Owner = entity.Owner;
            CreatedDate = entity.CreatedDate;
            CreatedUser = entity.CreatedUser;
            IsSignUp = entity.IsSignUp;
            SignUpStartTime = entity.SignUpStartTime;
            SignUpEndTime = entity.SignUpEndTime;
            IsLimitPerNumber = entity.IsLimitPerNumber;
            AccountType = entity.AccountType;
            PublicNumAppId = entity.PublicNumAppId;
            EnterpriseCorpId = entity.EnterpriseCorpId;
            EnterpriseAppId = entity.EnterpriseAppId;
            IsDeleted = entity.IsDeleted;

            return this;
        }



    }
}