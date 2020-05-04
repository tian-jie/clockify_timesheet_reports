using Infrastructure.Core;
using System;

namespace Kevin.T.Timesheet.Entities
{
    /// <summary>
    /// 项目信息
    /// </summary>
    public class Project : EntityBase<int>
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
        /// 项目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 客户ID，外键client表的GID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// workspace ID，外键workspace表的GID
        /// </summary>
        public string WorkspaceId { get; set; }

        /// <summary>
        /// 颜色，无聊用的
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 项目是否有收入的项目
        /// </summary>
        public bool Billable { get; set; }

        /// <summary>
        /// 是否archive
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// 项目周期信息
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 是否公开项目
        /// </summary>
        public bool IsPublic { get; set; }

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
