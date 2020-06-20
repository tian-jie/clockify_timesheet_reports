using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class WechatUserRequestMessageLogView : IViewModel
    {

        public int Id { get; set; }

        public int AppID { get; set; }

        public String UserID { get; set; }
        [Description("用户名")]
        public String UserName { get; set; }
        [Description("内容")]
        public String Content { get; set; }

        public int ContentType { get; set; }
        [Description("发布时间")]
        public DateTime CreatedTime { get; set; }

        public Boolean? HasReaded { get; set; }

        public string PhotoUrl { get; set; }

        public string Mobile { get; set; }

        public string Department { get; set; }

        public string TagId { get; set; }

        public bool IsCrop { get; set; }
        [Description("内容类型")]
        public string ContentTypeDisplayStr { get; set; }

        public string EmployeeNo { get; set; }

        public string AppLogo { get; set; }

        public bool? IsAutoReply { get; set; }

        public long? Duration { get; set; }
        [Description("状态")]
        public string HasReadedString { 
            get 
            {
                if (HasReaded.HasValue && HasReaded.Value)
                {
                    return "已读";
                }
                else
                {
                    return "未读";
                }
            } 
        }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (WechatUserRequestMessageLog)obj;
            Id = entity.Id;
            AppID = entity.AppID;
            UserID = entity.UserID;
            UserName = entity.UserName;
            Content = entity.Content;
            ContentType = entity.ContentType;
            CreatedTime = entity.CreatedTime;
            TagId = entity.TagId;
            IsAutoReply = IsAutoReply;
            HasReaded = entity.HasReaded;
            Duration = entity.Duration;
            return this;
        }

        public WechatUserRequestMessageLog ConvertToEntity()
        {
            var entity = new WechatUserRequestMessageLog();
            entity.Id = Id;
            entity.AppID = AppID;
            entity.UserID = UserID;
            entity.UserName = UserName;
            entity.Content = Content;
            entity.ContentType = ContentType;
            entity.CreatedTime = CreatedTime;
            entity.TagId = TagId;
            entity.IsAutoReply = IsAutoReply;
            entity.HasReaded = HasReaded;
            entity.Duration = Duration;
            return entity;
        }
    }
}
