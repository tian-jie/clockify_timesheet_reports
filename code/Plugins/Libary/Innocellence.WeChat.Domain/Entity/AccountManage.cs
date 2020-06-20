using System;
using System.Collections.Generic;
using Infrastructure.Core;
namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("NewsInfo")]
    public class AccountManage : EntityBase<int>
	{
		//[Id("Id",IsDbGenerated=true)]
        public override Int32 Id { get; set; }
        
        public String AccountName { get; set; }
       
        public String AccountDescription { get; set; }

        public String AccountLogo { get; set; }

        public String CorpId { get; set; }

        public string QrCode { get; set; }

        public int? AccountType { get; set; }
      
		//[Column("CreatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
        public DateTime? CreatedDate { get; set; }

		//[Column("CreatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
        public string CreatedUserID { get; set; }

		//[Column("UpdatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
        public DateTime? UpdatedDate { get; set; }

		//[Column("UpdatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
        public string UpdatedUserID { get; set; }

        public Boolean? IsDeleted { get; set; }

    }
}
