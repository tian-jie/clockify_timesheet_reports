using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
    //[Table("Message")]
    public partial class MessageView : IViewModel
    {

        public Int32 Id { get; set; }
        public Int32? AppId { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public String URL { get; set; }

      //  public Guid? Code { get; set; }
        public String Comment { get; set; }
        public String Status { get; set; }
        public Int32? ReadCount { get; set; }
        public Int32? ThumbsUpCount { get; set; }

        public Boolean? IsLike { get; set; }
        public bool   IsDeleted { get; set; }
        public bool IsThumbuped { get; set; }
      //  public Int32? ThumbImageId { get; set; }
      //  public String ThumbImageUrl { get; set; }
        public DateTime? PublishDate { get; set; }
        public String Previewers { get; set; }
        public String toDepartment { get; set; }
        public String toTag { get; set; }
        public String toUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String UpdatedUserID { get; set; }

        public String APPName { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (Message)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            Title = entity.Title;
            Content = entity.Content;
            URL = entity.URL;
       //     Code = entity.Code;
            Comment = entity.Comment;
            Status = entity.Status;
            ReadCount = entity.ReadCount;
            ThumbsUpCount = entity.ThumbsUpCount;
         //   ThumbImageId = entity.ThumbImageId;
         //   ThumbImageUrl = entity.ThumbImageUrl;
            PublishDate = entity.PublishDate;
            Previewers = entity.Previewers;
            toDepartment = entity.toDepartment;
            toTag = entity.toTag;
            toUser = entity.toUser;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            IsDeleted = entity.IsDeleted;
            IsLike = entity.IsLike;

            return this;
        }
    }
}
