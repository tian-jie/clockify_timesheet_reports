using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView 
{
    public partial class ReimbursementView
	{
        public Int32 Id { get; set; }

        // ��Ⱥ/����
        public String InsuranceType { get; set; }
        // ������������ϸ��
        public String ReimbursementTreatment { get; set; }
        // ����(����)
        public String OutpatientStartPayLine { get; set; }
        // �ⶥ��(����)
        public String OutpatientEndPayLine { get; set; }
        // ����(סԺ)
        public String HospitalizationStartPayLine { get; set; }
        // �ⶥ��(סԺ)
        public String HospitalizationEndPayLine { get; set; }
        // ����ԭ������
        public String PolicyLink { get; set; }

	}
}
