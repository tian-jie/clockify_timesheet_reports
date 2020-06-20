using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;

namespace Innocellence.WeChat.Domain.ModelsView
{
    public class QuestionImagesView: IViewModel
    {
        public Int32 Id { get; set; }

        public String ImageType { get; set; }
        
        public String ImageName { get; set; }

        public Int32? QuestionID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String CreatedUserID { get; set; }

        public Int32? AppId { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (QuestionImages)obj;
            Id = entity.Id;
            ImageType = entity.ImageType;
            ImageName = entity.ImageName;
            QuestionID = entity.QuestionID;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            AppId = entity.AppId;

            return this;
        }
    }
}
