using System;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Innocellence.WeChat.Domain.ModelsView
{
    public partial class AutoReplyContentView : IViewModel
    {

        public Int32 Id { get; set; }
        public Int32? AutoReplyId { get; set; }
        public Int32? PrimaryType { get; set; }
        public Int32? SecondaryType { get; set; }
        public Boolean? IsEncrypt { get; set; }
        public String Content { get; set; }
        public String NewsID { get; set; }
        public Int32? FileID { get; set; }
        public string MediaId { get; set; }
        public List<int> UserTags { get; set; }

        public List<int> MessageTags { get; set; }

        public List<int> UserGroups { get; set; }

        public string InterfaceLink { get; set; }

        public bool? IsNewContent { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (AutoReplyContent)obj;
            Id = entity.Id;
            AutoReplyId = entity.AutoReplyId;
            PrimaryType = entity.PrimaryType;
            SecondaryType = entity.SecondaryType;
            IsEncrypt = entity.IsEncrypt;
            Content = entity.Content;
            NewsID = entity.NewsID;
            FileID = entity.FileID;
            MediaId = entity.MediaId;
            IsNewContent = entity.IsNewContent;
            InterfaceLink = entity.InterfaceLink;
            UserTags = JsonConvert.DeserializeObject<List<int>>(entity.UserTags ?? "[]");
            UserGroups = JsonConvert.DeserializeObject<List<int>>(entity.UserGroups ?? "[]");
            MessageTags = JsonConvert.DeserializeObject<List<int>>(entity.MessageTags ?? "[]");
            return this;
        }

        public AutoReplyContent ConvertToEntity()
        {
            var entity = new AutoReplyContent();
            entity.Id = Id;
            entity.PrimaryType = PrimaryType;
            entity.AutoReplyId = AutoReplyId;
            entity.SecondaryType = SecondaryType;
            entity.IsEncrypt = IsEncrypt;
            entity.Content = Content;
            entity.NewsID = NewsID;
            entity.FileID = FileID;
            entity.MediaId = MediaId;
            entity.IsNewContent = IsNewContent;
            entity.UserGroups = JsonConvert.SerializeObject(UserGroups);
            entity.MessageTags = JsonConvert.SerializeObject(MessageTags);
            entity.UserTags = JsonConvert.SerializeObject(UserTags);
            entity.InterfaceLink = InterfaceLink;
            return entity;
        }

    }
}
