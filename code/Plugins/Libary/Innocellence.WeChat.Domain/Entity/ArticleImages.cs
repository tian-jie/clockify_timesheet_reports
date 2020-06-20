using Infrastructure.Core;
using System;
namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("NewsInfo")]
    public partial class ArticleImages : EntityBase<int>
	{
	
		//[Id("Id",IsDbGenerated=true)]
        public override Int32 Id { get; set; }
 

        //Title of the news
		//[Column("NewsTitle",DbType=DBType.NVarChar,Length=2048,Precision=1024,IsNullable=true)]
        public String ImageType { get; set; }


        public String ImageName { get; set; }

        //the category of the news
		//[Column("NewsCate",DbType=DBType.Int32,Length=4,Precision=10,IsNullable=true)]
        public Int32? ArticleID { get; set; }



        public byte[] ImageContent { get; set; }

        //
		//[Column("CreatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
		public DateTime? CreatedDate { get;set; }

        //
		//[Column("CreatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
		public String CreatedUserID { get;set; }

        //
        //[Column("AppId",DbType=DBType.Int32,Length=4,Precision=10,IsNullable=true)]
        public Int32? AppId { get; set; }

        public string UploadedUserId { get; set; }
	}
}
