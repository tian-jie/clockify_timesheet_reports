using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
	//[Table("Logs")]
	public partial class LogsView :IViewModel
	{
	
		public Int32 Id { get;set; }

        public String LogCate { get; set; }
		public  String LogContent { get;set; }
		public  Int32? LogFrom { get;set; }
		public  String LogSource { get;set; }
		public  DateTime? CreatedDate { get;set; }
		public  String CreatedUserID { get;set; }
		public  String CreatedUserName { get;set; }
 
 
 
        public IViewModel ConvertAPIModel(object obj){
		var entity= (Logs)obj;
	    Id =entity.Id;
	    LogCate =entity.LogCate;
	    LogContent =entity.LogContent;
	    LogFrom =entity.LogFrom;
	    LogSource =entity.LogSource;
	    CreatedDate =entity.CreatedDate;
	    CreatedUserID =entity.CreatedUserID;
	    CreatedUserName =entity.CreatedUserName;
 
        return this;
        }
	}
}
