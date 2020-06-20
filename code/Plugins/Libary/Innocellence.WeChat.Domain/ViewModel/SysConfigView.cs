using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
	//[Table("SysConfig")]
	public partial class SysConfigView :IViewModel
	{
	
		public Int32 Id { get;set; }
 
		public  Int32? ConfigCode { get;set; }
		public  String ConfigValue { get;set; }
		public  String ConfigName { get;set; }
		public  DateTime? CreatedDate { get;set; }
		public  String CreatedUserID { get;set; }
		public  DateTime? UpdatedDate { get;set; }
		public  String UpdatedUserID { get;set; }
 
 
 
        public IViewModel ConvertAPIModel(object obj){
		var entity= (SysConfig)obj;
	    Id =entity.Id;
	    ConfigCode =entity.ConfigCode;
	    ConfigValue =entity.ConfigValue;
	    ConfigName =entity.ConfigName;
	    CreatedDate =entity.CreatedDate;
	    CreatedUserID =entity.CreatedUserID;
	    UpdatedDate =entity.UpdatedDate;
	    UpdatedUserID =entity.UpdatedUserID;
 
        return this;
        }
	}
}
