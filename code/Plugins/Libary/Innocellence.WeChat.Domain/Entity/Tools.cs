using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("NewsInfo")]
    public partial class Tools : EntityBase<int>
	{
	
		//[Id("Id",IsDbGenerated=true)]
        public override Int32 Id { get; set; }

        public String ToolsName { get; set; }

        //Course Delete flag
		//[Column("IsDeleted",DbType=DBType.Boolean,Length=1,Precision=1,IsNullable=true)]
		public Boolean? IsDeleted { get;set; }

        //
		//[Column("ImageID",DbType=DBType.VarChar,Length=1024,Precision=1024,IsNullable=true)]
        public byte[] ZipContent { get; set; }

        public Int32? FileSize { get; set; }

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
