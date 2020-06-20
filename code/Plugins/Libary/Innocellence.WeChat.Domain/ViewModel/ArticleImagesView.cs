using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;

namespace Innocellence.WeChat.Domain.ModelsView
{
    public class ArticleImagesView : IViewModel
    {
        public Int32 Id { get; set; }

        public String ImageType { get; set; }
        
        public String ImageName { get; set; }

        public Int32? ArticleID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String CreatedUserID { get; set; }

        public Int32? AppId { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ArticleImages)obj;
            Id = entity.Id;
            ImageType = entity.ImageType;
            ImageName = entity.ImageName;
            ArticleID = entity.ArticleID;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.UploadedUserId;
            AppId = entity.AppId;

            return this;
        }
    }
}
