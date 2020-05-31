using Infrastructure.Core;
using System;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.ModelsView
{
    /// <summary>
    /// ��Ŀͳ��
    /// </summary>
    public partial class ProjectUserView : IViewModel
    {
        /// <summary>
        /// ����ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Clockify���id�ַ���
        /// </summary>
        public virtual string ProjectGid { get; set; }

        /// <summary>
        /// Clockify���id�ַ���
        /// </summary>
        public virtual string UserGid { get; set; }

        /// <summary>
        /// Ա������
        /// </summary>
        public virtual string EmployeeName { get; set; }

        /// <summary>
        /// �û���ɫID
        /// </summary>
        public int UserRoleTitleId { get; set; }

        /// <summary>
        /// �û���ɫ
        /// </summary>
        public string UserRoleTitle { get; set; }

        /// <summary>
        /// Rate
        /// </summary>
        public decimal Rate { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            throw new NotImplementedException();
        }
    }
}
