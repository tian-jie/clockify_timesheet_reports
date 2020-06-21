using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using System;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.ModelsView
{
    /// <summary>
    /// 项目统计
    /// </summary>
    public partial class ProjectUserView : IViewModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }

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

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (ProjectUser)model;
            Id = entity.Id;
            ProjectGid = entity.ProjectGid;
            UserGid = entity.UserGid;
            EmployeeName = entity.EmployeeName;
            UserRoleTitleId = entity.UserRoleTitleId;
            UserRoleTitle = entity.UserRoleTitle;
            Rate = entity.Rate;

            return this;
        }
    }
}
