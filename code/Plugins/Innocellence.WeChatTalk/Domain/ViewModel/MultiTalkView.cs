using Infrastructure.Core;
using Innocellence.WeChatTalk.Domain.Entity;
using System;

namespace Innocellence.WeChatTalk.Domain.ViewModel
{
    public class MultiTalkView : IViewModel
    {
        public Int32 Id { get; set; }
        public string OpenId { get; set; }
        public Int32 AppId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TextContent { get; set; }
        public Boolean IsDeleted { get; set; }
        public string ImgHeadUrl { get; set; }
        public string Name { get; set; }
        public string MsgType { get; set; }
        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (MultiTalk)model;
            Id = entity.Id;
            OpenId = entity.OpenId;
            AppId = entity.AppId;
            CreatedDate = entity.CreatedDate;
            TextContent = entity.TextContent;
            IsDeleted = entity.IsDeleted;
            MsgType = entity.MsgType;
            return this;
        }

    }
}
