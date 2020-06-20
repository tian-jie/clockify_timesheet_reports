using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using System.ComponentModel.DataAnnotations;

namespace Innocellence.WeChat.Domain.Entity
{
    //[Table("Category")]
    /// <summary>

    /// </summary>
    public partial class QuestionManage : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        //Ӧ��ID
        public Int32? AppId { get; set; }
        //���
        public string Category { get; set; }
       
        //������id
        public string CreatedUserId { get; set; }
        //������name
        public string QUserName { get; set; }

        //�ش���id
        public string UpdatedUserId { get; set; }
        //�ش���name
        public string AUsername { get; set; }

        //��������
        public string Question { get; set; }
        //�����
        public string Answer { get; set; }
        //����ʱ��
        public DateTime? CreatedDate { get; set; }
        //�޸�ʱ��
        public DateTime? UpdatedDate { get; set; }
        //����״̬
        public string Status { get; set; }
        //�Ķ�����
        public Int32? ReadCount { get; set; }
        //ɾ����ʶ
        public Boolean? IsDeleted { get; set; }
        public Int32? Satisfaction { get; set; }
    }
}
