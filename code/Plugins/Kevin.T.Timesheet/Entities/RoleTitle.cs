using Infrastructure.Core;
using System;

namespace Kevin.T.Timesheet.Entities
{
    /// <summary>
    /// 客户信息表
    /// </summary>
    public class RoleTitle : EntityBase<int>
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public override int Id { get; set; }

        /// <summary>
        /// 员工角色Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Rate，比率，项目上的比率
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
