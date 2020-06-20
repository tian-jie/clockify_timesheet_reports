using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
	//[Table("CourseAttention")]
    public partial class ArticleThumbsUpView : IViewModel
	{
	
		public Int32 Id { get;set; }
 
		public  String UserID { get;set; }
		public  String UserName { get;set; }
        public int ArticleID { get; set; }
		public  DateTime? CreatedDate { get;set; }
        //public  String CreatedUserID { get;set; }
		public  DateTime? UpdatedDate { get;set; }
        //public  String UpdatedUserID { get;set; }
 
	//	public  TrainingCourseView TrainingCourse { get;set; }
 
 
        public IViewModel ConvertAPIModel(object obj){
            var entity = (ArticleThumbsUp)obj;
	    Id =entity.Id;
	    UserID =entity.UserID;
	    UserName =entity.UserName;
        ArticleID = entity.ArticleID;
	    CreatedDate =entity.CreatedDate;
        //CreatedUserID =entity.CreatedUserID;
	    UpdatedDate =entity.UpdatedDate;
        //UpdatedUserID =entity.UpdatedUserID;
 
        return this;
        }
	}
}
