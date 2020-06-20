using Infrastructure.Core;
using System;

namespace Innocellence.WeChat.Domain.Entity
{
    public class WechatUserRequestMessageLog : EntityBase<int>
    {
        public override int Id { get; set; }

        public int AppID { get; set; }
        public string TagId { get; set; }
        /// <summary>
        /// request.FromUserName
        /// response.ToUserName
        /// </summary>
        public String UserID { get; set; }

        /// <summary>
        /// 根据UserID 从通讯录中读取
        /// </summary>
        public String UserName { get; set; }

        public String Content { get; set; }

        /// <summary>
        /// WechatUserMessageLogContentType
        /// </summary>
        public int ContentType { get; set; }

        public DateTime CreatedTime { get; set; }

        public bool? IsAutoReply { get; set; }

        public Boolean? HasReaded { get; set; }

        public long? Duration { get; set; }
    }
}
