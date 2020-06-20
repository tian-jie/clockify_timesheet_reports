using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class MessageLog : EntityBase<int>
    {
        public override int Id { get; set; }

        public string NewsIdList { get; set; }

        public int? AppId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public bool? IsDeleted { get; set; }

        /// <summary>
        /// WechatMessageLogType
        /// </summary>
        public int? MsgContentType { get; set; }

        public string CreatedUserName { get; set; }

        public int? SendTotalMembers { get; set; }

        public int? SendMsgStatus { get; set; }
    }

    public enum SendMessageStatus
    {
        [Description("发送中")]
        Sending = 0,
        [Description("成功")]
        Success = 1,
        [Description("失败")]
        Failed = 2,
    }
}
