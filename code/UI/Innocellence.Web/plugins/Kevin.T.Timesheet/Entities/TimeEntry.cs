using Infrastructure.Core;
using System;

namespace Kevin.T.Timesheet.Entities
{
    /// <summary>
    /// 填入的每个task的时间
    /// </summary>
    public class TimeEntry : EntityBase<int>
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public override int Id { get; set; }

        /// <summary>
        /// Clockify里的id字符串
        /// </summary>
        public virtual string Gid { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 用户id，用clockify里的userid字符串
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// 项目ID，clockify里的项目id字符串
        /// </summary>
        public virtual string ProjectId { get; set; }

        /// <summary>
        /// TaskId，clockify里的项目id字符串
        /// </summary>
        public virtual string TaskId { get; set; }

        /// <summary>
        /// 录入时间的那一天
        /// </summary>
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// 记录这个的时间，精确到小时，如1.5小时
        /// </summary>
        public virtual decimal TotalHours { get; set; }

        /// <summary>
        /// clockify里的workspace概念
        /// </summary>
        public virtual string WorkspaceId { get; set; }

        /// <summary>
        /// 是否锁定，锁定后不可修改
        /// </summary>
        public virtual bool IsLocked { get; set; }

        /// <summary>
        /// 冗余的Task里的isBilliable信息
        /// </summary>
        public virtual bool IsBillable { get; set; }

        /// <summary>
        /// 创建日期，框架直接调用
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// 创建人ID，框架直接调用
        /// </summary>
        public string CreatedUserID { get; set; }

        /// <summary>
        /// 创建人，框架直接调用
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 更新日期，框架直接调用
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// 更新人ID，框架直接调用
        /// </summary>
        public string UpdatedUserID { get; set; }

        /// <summary>
        /// 更新人，框架直接调用
        /// </summary>
        public string UpdatedUserName { get; set; }

        /// <summary>
        /// 逻辑删除，框架直接调用
        /// </summary>
        public bool? IsDeleted { get; set; }
    }
}
