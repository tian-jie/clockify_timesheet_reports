using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
	//[Table("NewsInfo")]
	public partial class ToolsView :IViewModel
	{
	
		public  Int32 Id { get;set; }
        public String ToolsName { get; set; }

        public Int32? FileSize { get; set; }
        public byte[] ZipContent { get; set; }
		public  Boolean? IsDeleted { get;set; }

		public  DateTime? CreatedDate { get;set; }
		public  String CreatedUserID { get;set; }
		public  DateTime? UpdatedDate { get;set; }
		public  String UpdatedUserID { get;set; }

        public IViewModel ConvertAPIModel(object obj){
            if(obj==null){return null;}
		var entity= (Tools)obj;
	    Id =entity.Id;
        ToolsName = entity.ToolsName;
        FileSize = entity.FileSize;
        ZipContent = entity.ZipContent;
	    CreatedDate =entity.CreatedDate;
	    CreatedUserID =entity.CreatedUserID;
        return this;
        }
	}
}
