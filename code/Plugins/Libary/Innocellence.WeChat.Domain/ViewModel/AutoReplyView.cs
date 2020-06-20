using System;
using System.Collections.Generic;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Newtonsoft.Json;
using Innocellence.WeChat.Domain.ViewModel;

namespace Innocellence.WeChat.Domain.ModelsView
{
    public partial class AutoReplyView : IViewModel
    {

        public AutoReplyView()
        {
            Keywords = new List<AutoReplyKeywordView>();
            Contents = new List<AutoReplyContentView>();
        }

        public Int32 Id { get; set; }
        public Int32 AppId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public String CreatedUserName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String UpdatedUserID { get; set; }
        public String UpdatedUserName { get; set; }

        public Int32 KeywordType { get; set; }

        public List<string> TextKeyword { get; set; }

        public List<int> MatchType { get; set; }

        public String KeywordTypeName { get; set; }

        public String AppName { get; set; }

        /// <summary>
        /// 所关联的Keyword, 需要从DB中获取
        /// </summary>
        public List<AutoReplyKeywordView> Keywords { get; set; }

        /// <summary>
        /// 所关联的自动回复, 需要从DB中获取
        /// </summary>
        public List<AutoReplyContentView> Contents { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (AutoReply)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            Name = entity.Name;
            Description = entity.Description;
            IsDeleted = entity.IsDeleted;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            CreatedUserName = entity.CreatedUserName;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            UpdatedUserName = entity.UpdatedUserName;
            KeywordType = entity.PrimaryType;
            return this;
        }

        public AutoReply ConvertToEntity()
        {
            var entity = new AutoReply();
            entity.Id = Id;
            entity.AppId = AppId;
            entity.Name = Name;
            entity.Description = Description;
            entity.PrimaryType = KeywordType;
            entity.CreatedUserID = CreatedUserID;
            entity.CreatedUserName = CreatedUserName;
            entity.CreatedDate = CreatedDate;
            entity.UpdatedUserID = UpdatedUserID;
            entity.UpdatedUserName = UpdatedUserName;
            entity.UpdatedDate = UpdatedDate;
            return entity;
        }

        public static AutoReplyView ConvertFromEntity(object obj)
        {
            var view = new AutoReplyView();
            var entity = (AutoReply)obj;
            view.Id = entity.Id;
            view.AppId = entity.AppId;
            view.Name = entity.Name;
            view.Description = entity.Description;
            view.IsDeleted = entity.IsDeleted;
            view.CreatedDate = entity.CreatedDate;
            view.CreatedUserID = entity.CreatedUserID;
            view.UpdatedDate = entity.UpdatedDate;
            view.UpdatedUserID = entity.UpdatedUserID;
            view.KeywordType = entity.PrimaryType;
            return view;
        }
    }
}
