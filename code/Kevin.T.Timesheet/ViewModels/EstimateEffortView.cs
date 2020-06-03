using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using System;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.ModelsView
{
    /// <summary>
    /// ��Ŀͳ��
    /// </summary>
    public partial class EstimateEffortView : IViewModel
    {
        /// <summary>
        /// ����ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ProjectGid
        /// </summary>
        public string ProjectGid { get; set; }

        /// <summary>
        /// ProjectId
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// EmployeeId
        /// </summary>
        public int? EmployeeId { get; set; }

        /// <summary>
        /// EmployeeGId
        /// </summary>
        public string EmployeeGId { get; set; }

        /// <summary>
        /// RoleId
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// RoleTitle
        /// </summary>
        public string RoleTitle { get; set; }

        /// <summary>
        /// RoleRate�����ʣ���Ŀ�ϵı���
        /// </summary>
        public decimal RoleRate { get; set; }

        /// <summary>
        /// RateEffort
        /// </summary>
        public decimal RateEffort { get; set; }

        /// <summary>
        /// Effort�����ʣ���Ŀ�ϵı���
        /// </summary>
        public decimal Effort { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (EstimateEffort)obj;
            Id = entity.Id;
            ProjectGid = entity.ProjectGid;
            ProjectId = entity.ProjectId;
            EmployeeId = entity.EmployeeId;
            EmployeeGId = entity.EmployeeGId;
            RoleId = entity.RoleId;
            RoleTitle = entity.RoleTitle;
            RoleRate = entity.RoleRate;
            Effort = entity.Effort;

            RateEffort = entity.Effort * entity.RoleRate;
            return this;
        }
    }
}
