using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView 
{
	//[Table("Logs")]
    public partial class WechatUserView : IViewModel
	{
        public Int32 Id { get; set; }

        public String LanguageCode { get; set; }
        public String WeChatUserID { get; set; }
        public String WechatID { get; set; }
		public  DateTime? CreatedDate { get;set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (WechatUser)obj;
            Id = entity.Id;
            LanguageCode =entity.LanguageCode;
            WeChatUserID = entity.WeChatUserID;
            WechatID = entity.WechatID;
            CreatedDate = entity.CreatedDate;

            return this;
        }
	}
}
