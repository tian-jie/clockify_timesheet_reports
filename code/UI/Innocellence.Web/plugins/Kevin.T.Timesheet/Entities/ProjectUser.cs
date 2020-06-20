using Infrastructure.Core;
using System;

namespace Kevin.T.Timesheet.Entities
{
    /// <summary>
    /// 项目信息
    /// </summary>
    public class ProjectUser : EntityBase<int>
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public override int Id { get; set; }

        /// <summary>
        /// Clockify里的id字符串
        /// </summary>
        public virtual string ProjectGid { get; set; }

        /// <summary>
        /// Clockify里的id字符串
        /// </summary>
        public virtual string UserGid { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public virtual string EmployeeName { get; set; }

        /// <summary>
        /// 用户角色ID
        /// </summary>
        public int UserRoleTitleId { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public string UserRoleTitle { get; set; }

        /// <summary>
        /// Rate
        /// </summary>
        public decimal Rate { get; set; }

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
