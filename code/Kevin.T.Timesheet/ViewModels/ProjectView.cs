using Infrastructure.Core;
using System;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.ModelsView
{
    /// <summary>
    /// 项目统计
    /// </summary>
    public partial class ProjectView : IViewModel
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

        public IViewModel ConvertAPIModel(object model)
        {
            throw new NotImplementedException();
        }
    }
}
