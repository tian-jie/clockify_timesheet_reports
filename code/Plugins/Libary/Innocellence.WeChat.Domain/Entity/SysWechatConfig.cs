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

        // 标识微信号（企业号或者服务好）的ID，外键：关联表AccountManage
        public Int32? AccountManageId { get; set; }

        // TODO: 这个字段在AccountMange中已有，这里需要删除
        public String WeixinCorpId { get; set; }
        public String WeixinCorpSecret { get; set; }

        //是否企业号
        public Boolean? IsCorp { get; set; }

        public string AppSignKey { get; set; }

        //应用名称
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
