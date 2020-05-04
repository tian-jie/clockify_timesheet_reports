using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using System;

namespace Kevin.T.Timesheet.ModelsView
{
    public partial class TimeEntryView : IViewModel
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
        public virtual string Description { get; set; }

        /// <summary>
        /// �û�id����clockify���userid�ַ���
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// ��ĿID��clockify�����Ŀid�ַ���
        /// </summary>
        public virtual string ProjectId { get; set; }

        /// <summary>
        /// TaskId��clockify�����Ŀid�ַ���
        /// </summary>
        public virtual string TaskId { get; set; }

        /// <summary>
        /// ¼��ʱ�����һ��
        /// </summary>
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// ��¼�����ʱ�䣬��ȷ��Сʱ����1.5Сʱ
        /// </summary>
        public virtual decimal TotalHours { get; set; }

        /// <summary>
        /// clockify���workspace����
        /// </summary>
        public virtual string WorkspaceId { get; set; }

        /// <summary>
        /// �Ƿ������������󲻿��޸�
        /// </summary>
        public virtual bool IsLocked { get; set; }

        /// <summary>
        /// �����Task���isBilliable��Ϣ
        /// </summary>
        public virtual bool IsBillable { get; set; }

        /// <summary>
        /// �������ڣ����ֱ�ӵ���
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// ������ID�����ֱ�ӵ���
        /// </summary>
        public string CreatedUserID { get; set; }

        /// <summary>
        /// �����ˣ����ֱ�ӵ���
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// �������ڣ����ֱ�ӵ���
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// ������ID�����ֱ�ӵ���
        /// </summary>
        public string UpdatedUserID { get; set; }

        /// <summary>
        /// �����ˣ����ֱ�ӵ���
        /// </summary>
        public string UpdatedUserName { get; set; }

        /// <summary>
        /// �߼�ɾ�������ֱ�ӵ���
        /// </summary>
        public bool? IsDeleted { get; set; }


        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (TimeEntry)obj;
            Id = entity.Id;
            Date = entity.Date;
            Description = entity.Description;
            Gid = entity.Gid;
            IsBillable = entity.IsBillable;
            IsLocked = entity.IsLocked;
            ProjectId = entity.ProjectId;
            TaskId = entity.TaskId;
            TotalHours = entity.TotalHours;
            UserId = entity.UserId;
            WorkspaceId = entity.WorkspaceId;

            return this;
        }
    }
}
