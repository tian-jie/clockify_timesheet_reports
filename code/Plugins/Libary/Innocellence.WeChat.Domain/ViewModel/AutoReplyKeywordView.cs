using System;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
    public partial class AutoReplyKeywordView : IViewModel
    {

        public Int32 Id { get; set; }
        public Int32 AutoReplyId { get; set; }
        public Int32 SecondaryType { get; set; }
        public String Keyword { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (AutoReplyKeyword)obj;
            Id = entity.Id;
            AutoReplyId = entity.AutoReplyId;
            SecondaryType = entity.SecondaryType;
            Keyword = entity.Keyword;
            
            return this;
        }

        public AutoReplyKeyword ConvertToEntity()
        {
            var entity = new AutoReplyKeyword();
            entity.Id = Id;
            entity.AutoReplyId = AutoReplyId;
            entity.SecondaryType = SecondaryType;
            entity.Keyword = Keyword;

            return entity;
        }

    }
}
