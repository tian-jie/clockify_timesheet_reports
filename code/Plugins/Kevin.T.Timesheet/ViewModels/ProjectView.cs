using Infrastructure.Core;
using System;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.ModelsView
{
    /// <summary>
    /// ��Ŀͳ��
    /// </summary>
    public partial class ProjectView : IViewModel
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
        /// ��Ŀ����
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// �ͻ�ID�����client���GID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// workspace ID�����workspace���GID
        /// </summary>
        public string WorkspaceId { get; set; }

        /// <summary>
        /// ��ɫ�������õ�
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// ��Ŀ�Ƿ����������Ŀ
        /// </summary>
        public bool Billable { get; set; }

        /// <summary>
        /// �Ƿ�archive
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// ��Ŀ������Ϣ
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// ��ע
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// �Ƿ񹫿���Ŀ
        /// </summary>
        public bool IsPublic { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            throw new NotImplementedException();
        }
    }
}
