using System;
using Infrastructure.Core;

namespace Innocellence.Activity.Contracts.Entity
{
    public class EventEntity : EntityBase<int>
    {
        public override Int32 Id { get; set; }
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

        /// <summary>
        /// 允许报名的起始时间
        /// </summary>
        public DateTime ?RegisteredStartedDateTime { get; set; }

        /// <summary>
        /// 允许报名的结束时间
        /// </summary>
        public DateTime ?RegisteredEndedDateTime { get; set; }

        public string CanceledReason { get; set; }

        public bool IsNeedRegisterBeforeCheckin { get; set; }
    }
}
