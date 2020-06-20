using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
    public partial class FeedBackView : IViewModel
    {
        public Int32 Id { get; set; }
        public String Content { get; set; }
        public String MenuCode { get; set; }
        public String FeedBackUserId { get; set; }
        public DateTime FeedBackDateTime { get; set; }
        public Int32 AppID { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (FeedBackEntity)obj;
            Id = entity.Id;
            Content = entity.Content;
            MenuCode = entity.MenuCode;
            FeedBackUserId = entity.FeedBackUserId;
            FeedBackDateTime = entity.FeedBackDateTime;
            AppID = entity.AppID;
            return this;
        }
    }
}
