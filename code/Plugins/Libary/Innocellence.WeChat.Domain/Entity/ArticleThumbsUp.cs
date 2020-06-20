using System;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("CourseAttention")]
    public partial class ArticleThumbsUp : EntityBase<int>
	{

        public  override Int32 Id { get; set; }
 
		public  String UserID { get;set; }
		public  String UserName { get;set; }
        public int ArticleID { get; set; }
		public  DateTime? CreatedDate { get;set; }
        //public  String CreatedUserID { get;set; }
		public  DateTime? UpdatedDate { get;set; }
        //public  String UpdatedUserID { get;set; }

        public bool? IsDeleted { get; set; }

        public string Type { get; set; }

        //[ForeignKey("ArticleID")]
        //public virtual ArticleInfo ArticleInfo { get; set; }
 
	}
}
