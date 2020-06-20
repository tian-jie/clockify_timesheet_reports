using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("Logs")]
    public partial class Logs : EntityBase<int>
	{

        public  override Int32 Id { get; set; }
 
		public  string LogCate { get;set; }
		public  String LogContent { get;set; }
		public  Int32? LogFrom { get;set; }
		public  String LogSource { get;set; }
		public  DateTime? CreatedDate { get;set; }
		public  String CreatedUserID { get;set; }
		public  String CreatedUserName { get;set; }
 
	}
}
