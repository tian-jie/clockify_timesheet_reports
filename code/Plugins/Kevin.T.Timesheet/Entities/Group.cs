using Infrastructure.Core;
using System;

namespace Kevin.T.Timesheet.Entities
{
    /// <summary>
    /// 公司大部门表
    /// </summary>
    public class Group : EntityBase<int>
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public override int Id { get; set; }
        
        /// <summary>
        /// Clockify里的id字符串
        /// </summary>
        public string Gid { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// workspaceid
        /// </summary>
        public string WorkspaceId { get; set; }

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
