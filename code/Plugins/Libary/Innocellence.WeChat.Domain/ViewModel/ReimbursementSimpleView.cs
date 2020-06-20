using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView 
{
    public partial class ReimbursementSimpleView
	{
        public Int32 Id { get; set; }

        // ����
        public String InsuranceType { get; set; }
        // ��Ⱥ
        public String CrowdType { get; set; }
        // ������������ϸ��
        public String ReimbursementTreatment { get; set; }
        // ����(����)
        public String OutpatientStartPayLine { get; set; }
        // �ⶥ��(����)
        public String OutpatientEndPayLine { get; set; }
        // ����ԭ������
        public String PolicyLink { get; set; }
	}
}
