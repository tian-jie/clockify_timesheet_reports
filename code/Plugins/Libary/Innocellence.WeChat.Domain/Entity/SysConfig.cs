using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("SysConfig")]
    public partial class SysConfig : EntityBase<int>
	{
	
		//[Id("Id",IsDbGenerated=true)]
        public override Int32 Id { get; set; }
 

        //the code of the Config
		//[Column("ConfigCode",DbType=DBType.Int32,Length=4,Precision=10,IsNullable=true)]
		public Int32? ConfigCode { get;set; }

        //the value of the config which find by configcode
		//[Column("ConfigValue",DbType=DBType.VarChar,Length=128,Precision=128,IsNullable=true)]
		public String ConfigValue { get;set; }

        //the name of the config
		//[Column("ConfigName",DbType=DBType.VarChar,Length=512,Precision=512,IsNullable=true)]
		public String ConfigName { get;set; }

        //
		//[Column("CreatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
		public DateTime? CreatedDate { get;set; }

        //
		//[Column("CreatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
		public String CreatedUserID { get;set; }

        //
		//[Column("UpdatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
		public DateTime? UpdatedDate { get;set; }

        //
		//[Column("UpdatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
		public String UpdatedUserID { get;set; }
 
 
 
	}
}
