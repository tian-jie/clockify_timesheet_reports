using Infrastructure.Core;
using System;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.ModelsView
{
    /// <summary>
    /// 项目统计
    /// </summary>
    public partial class EmployeeView : IViewModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Clockify里的id字符串
        /// </summary>
        public virtual string Gid { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// RoleId
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// RoleName
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// RoleRate
        /// </summary>
        public decimal RoleRate { get; set; }

        /// <summary>
        /// 员工头像
        /// </summary>
        public string ProfilePicture { get; set; }

        /// <summary>
        /// 默认workspace，借用clockify里的概念
        /// </summary>
        public string DefaultWorkspace { get; set; }

        /// <summary>
        /// 员工状态
        /// </summary>
        public string Status { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            throw new NotImplementedException();
        }
    }
}
