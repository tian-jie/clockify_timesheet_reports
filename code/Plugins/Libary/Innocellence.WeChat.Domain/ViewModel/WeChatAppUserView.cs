using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView 
{
	//[Table("Logs")]
    public partial class WeChatAppUserView : IViewModel
	{
        public Int32 Id { get; set; }

        public String WeChatUserID { get; set; }
        public int WeChatAppID { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (WeChatAppUser)obj;
            Id = entity.Id;
            WeChatUserID = entity.WeChatUserID;
            WeChatAppID = entity.WeChatAppID;

            return this;
        }
	}
}
