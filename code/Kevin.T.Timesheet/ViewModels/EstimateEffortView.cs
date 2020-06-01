using Infrastructure.Core;
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
        public int ProjectId { get; set; }

        /// <summary>
        /// EmployeeId
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// EmployeeGId
        /// </summary>
        public string EmployeeGId { get; set; }

        /// <summary>
        /// RoleId
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// RoleTitle
        /// </summary>
        public int RoleTitle { get; set; }

        /// <summary>
        /// RoleRate�����ʣ���Ŀ�ϵı���
        /// </summary>
        public decimal RoleRate { get; set; }

        /// <summary>
        /// Effort�����ʣ���Ŀ�ϵı���
        /// </summary>
        public decimal Effort { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            throw new NotImplementedException();
        }
    }
}
