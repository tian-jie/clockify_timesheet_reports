using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("Logs")]
    public partial class WeChatAppUser : EntityBase<int>
	{

        public override Int32 Id { get; set; }

        public String WeChatUserID { get; set; }
        public int WeChatAppID { get; set; }
 
	}
}
