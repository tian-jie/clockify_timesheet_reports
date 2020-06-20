using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class MessageLogView : IViewModel
    {

        public MessageLogView()
        {
            SendTotalMembers = 0;
            SendMsgStatus = (int)SendMessageStatus.Sending;
            IsDeleted = false;
        }

        public int Id { get; set; }

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

        public string SendMsgStatusDisplayStr { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (MessageLog)model;
            this.Id = entity.Id;
            this.NewsIdList = entity.NewsIdList;
            this.AppId = entity.AppId;
            this.CreatedDate = entity.CreatedDate;
            this.IsDeleted = entity.IsDeleted;
            this.MsgContentType = entity.MsgContentType;
            this.CreatedUserName = entity.CreatedUserName;
            this.SendTotalMembers = entity.SendTotalMembers;
            this.SendMsgStatus = entity.SendMsgStatus;
            return this;
        }
    }
}
