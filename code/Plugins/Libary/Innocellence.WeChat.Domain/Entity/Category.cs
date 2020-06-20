using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("Category")]
    public partial class Category : EntityBase<int>
	{

        public override Int32 Id { get; set; }
 
		public  Int32? CategoryCode { get;set; }

        public Int32? CategoryTypeCode { get; set; }
		public  String CategoryName { get;set; }

        public String Extra1 { get; set; }
        public String LanguageCode { get; set; }
		public  String CategoryDesc { get;set; }
		public  Int32? ParentCode { get;set; }
		public  Boolean? IsDeleted { get;set; }
		public  DateTime? CreatedDate { get;set; }
		public  String CreatedUserID { get;set; }
		public  DateTime? UpdatedDate { get;set; }
		public  String UpdatedUserID { get;set; }
 
	}
}
