using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("Logs")]
    public partial class SysWechatConfig : EntityBase<int>
	{

        public override Int32 Id { get; set; }

        public String WeixinToken { get; set; }
        public String WeixinEncodingAESKey { get; set; }

        // ��ʶ΢�źţ���ҵ�Ż��߷���ã���ID�������������AccountManage
        public Int32? AccountManageId { get; set; }

        // TODO: ����ֶ���AccountMange�����У�������Ҫɾ��
        public String WeixinCorpId { get; set; }
        public String WeixinCorpSecret { get; set; }

        //�Ƿ���ҵ��
        public Boolean? IsCorp { get; set; }

        public string AppSignKey { get; set; }

        //Ӧ������
        public string AppName { get; set; }
        public String WeixinAppId { get; set; }

        public String AccessToken { get; set; }
        public DateTime? AccessTokenExpireTime { get; set; }

        public String WelcomeMessage { get; set; }

        public string ProductID { get; set; }

        public DateTime? CreatedDate { get; set; }

        //
        //[Column("CreatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
        public String CreatedUserID { get; set; }

        public String CoverUrl { get; set; }

        //
        //[Column("UpdatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
        public DateTime? UpdatedDate { get; set; }

        //
        //[Column("UpdatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
        public String UpdatedUserID { get; set; }

        public Boolean? IsDeleted { get; set; }

 
	}
}
