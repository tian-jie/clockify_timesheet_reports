using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
	//[Table("Category")]
	public partial class CategoryView :IViewModel
	{
	
		public Int32 Id { get;set; }

        public Int32? IdEN { get; set; }
 
		public  Int32? CategoryCode { get;set; }
		public  String CategoryName { get;set; }
        public Int32? CategoryTypeCode { get; set; }
        public String CategoryNameCN { get; set; }
        public String Extra1 { get; set; }
        public String LanguageCode { get; set; }
		public  String CategoryDesc { get;set; }
		public  Int32? ParentCode { get;set; }
		public  Boolean? IsDeleted { get;set; }
		public  DateTime? CreatedDate { get;set; }
		public  String CreatedUserID { get;set; }
		public  DateTime? UpdatedDate { get;set; }
		public  String UpdatedUserID { get;set; }
 
 
 
        public IViewModel ConvertAPIModel(object obj){
		var entity= (Category)obj;
	    Id =entity.Id;
	    CategoryCode =entity.CategoryCode;
	    CategoryName =entity.CategoryName;
        CategoryTypeCode = entity.CategoryTypeCode;
        LanguageCode = entity.LanguageCode;
	    CategoryDesc =entity.CategoryDesc;
	    ParentCode =entity.ParentCode;
        Extra1 = entity.Extra1;
	    IsDeleted =entity.IsDeleted;
	    CreatedDate =entity.CreatedDate;
	    CreatedUserID =entity.CreatedUserID;
	    UpdatedDate =entity.UpdatedDate;
	    UpdatedUserID =entity.UpdatedUserID;
 
        return this;
        }
	}
}
