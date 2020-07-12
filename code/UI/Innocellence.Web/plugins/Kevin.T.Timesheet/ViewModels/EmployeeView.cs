using Infrastructure.Core;
using System;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.ModelsView
{
    /// <summary>
    /// ��Ŀͳ��
    /// </summary>
    public partial class EmployeeView : IViewModel
    {
        /// <summary>
        /// ����ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Clockify���id�ַ���
        /// </summary>
        public virtual string Gid { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Ա������
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
        /// Ա��ͷ��
        /// </summary>
        public string ProfilePicture { get; set; }

        /// <summary>
        /// Ĭ��workspace������clockify��ĸ���
        /// </summary>
        public string DefaultWorkspace { get; set; }

        /// <summary>
        /// Ա��״̬
        /// </summary>
        public string Status { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            throw new NotImplementedException();
        }
    }
}
