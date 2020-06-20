using Infrastructure.Core;
using Infrastructure.Web.Domain.Service;
using Innocellence.Activity.Contracts.Entity;
using System;

namespace Innocellence.Activity.ModelsView
{
    public partial class EventEntityView : IViewModel
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public string PollingName { get; set; }
        public int AppId { get; set; }
        public Int32? PollingId { get; set; }
        public string RegisteredUrl { get; set; }
        public string CheckinUrl { get; set; }

        /// <summary>
        /// 活动的开始时间
        /// </summary>
        public DateTime StartedDateTime { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndedDateTime { get; set; }

        /// <summary>
        /// 允许报名的起始时间
        /// </summary>
        public DateTime ?RegisteredStartedDateTime { get; set; }

        /// <summary>
        /// 允许报名的结束时间
        /// </summary>
        public DateTime ?RegisteredEndedDateTime { get; set; }

        /// <summary>
        /// 签到开始时间
        /// </summary>
        public DateTime? CheckinStartedDateTime { get; set; }

        /// <summary>
        /// 签到结束时间
        /// </summary>
        public DateTime? CheckinEndedDateTime { get; set; }

        /// <summary>
        /// 活动创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }

        public bool? IsDeleted { get; set; }

        public string CreatedUserId { get; set; }

        /// <summary>
        /// 最大注册人数
        /// </summary>
        public int MaxUser { get; set; }

        /// <summary>
        /// 活动地点
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        //注册链接
        public string Registered { get; set; }
        //绝对地址
        public string AbsoluteUrl { get; set; }
        //取消原因
        public string CanceledReason { get; set; }
        //签到链接
        public string Checkin { get; set; }
        /// <summary>
        /// 1.NotStarted 2.Finished 3.OverMaxUser 4.RepeatRegistered 5.CanceledEvent 6.Continue 7.NoPolling
        /// </summary>
        public string Status { get; set; }

        public bool IsNeedRegisterBeforeCheckin { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (EventEntity)obj;
            Id = entity.Id;
            Name = entity.Name;
            PollingName = entity.PollingName;
            AppId = entity.AppId;
            PollingId = entity.PollingId.GetValueOrDefault();
            CreatedDate = entity.CreatedDate;
            RegisteredUrl = entity.RegisteredUrl;
            CheckinUrl = entity.CheckinUrl;
            StartedDateTime = entity.StartedDateTime;
            MaxUser = entity.MaxUser;
            Location = entity.Location;
            EndedDateTime = entity.EndedDateTime;
            CheckinStartedDateTime = entity.CheckinStartedDateTime;
            CheckinEndedDateTime = entity.CheckinEndedDateTime;
            IsDeleted = entity.IsDeleted;
            CreatedUserId = entity.CreatedUserId;
            Desc = entity.Desc;
            RegisteredStartedDateTime = entity.RegisteredStartedDateTime;
            RegisteredEndedDateTime = entity.RegisteredEndedDateTime;
            AbsoluteUrl = CommonService.GetSysConfig("WeChatUrl", "");
            CanceledReason = entity.CanceledReason;
            IsNeedRegisterBeforeCheckin = entity.IsNeedRegisterBeforeCheckin;
            return this;
        }
    }
}
