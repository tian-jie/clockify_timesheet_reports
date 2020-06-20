using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("Feedback")]
    public partial class Feedback : EntityBase<int>
	{
        //[Id("Id",IsDbGenerated=false)]
        public override Int32 Id { get; set; }
 
		public  Int32? FeedbackTitle { get;set; }
		public  String FeedbackContent { get;set; }
		public  String FeedbackStatus { get;set; }
		public  DateTime? SendDate { get;set; }
		public  String ErrMsg { get;set; }
		public  DateTime? CreatedDate { get;set; }
		public  String CreatedUserID { get;set; }

        public String CreatedUserName { get; set; }

 
 
 
	}
}
