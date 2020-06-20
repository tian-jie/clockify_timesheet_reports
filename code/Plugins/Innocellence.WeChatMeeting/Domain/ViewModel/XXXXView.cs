using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChatMeeting.Domain.ViewModel
{
    //[Table("ArticleInfoView")]

    public partial class XXXXView : IViewModel
    {

        public XXXXView()
        {

        }

        public Int32 Id { get; set; }
        public int IsCorp { get; set; }
        public String ArticleTitle { get; set; }
        public String LanguageCode { get; set; }
        public String ArticleComment { get; set; }
        public String ArticleContent { get; set; }
        public String ArticleContentEdit { get; set; }
        //[Required]
        //public String ArticleCateSub { get; set; }

     
        public Int32? ThumbImageId { get; set; }
        public String ThumbImageUrl { get; set; }
        public String Role { get; set; }
        //public String Previewers { get; set; }
        public List<string> PersonList { get; set; }
        public List<int> GroupList { get; set; }
        public List<int> TagList { get; set; }
        public String APPName { get; set; }
        public bool IsThumbuped { get; set; }


        public Boolean? ShowLikeCount { get; set; }
        public Boolean? ShowReadCount { get; set; }
        public Boolean? IsWatermark { get; set; }
        public Boolean? NoShare { get; set; }

        public Boolean? NoCopy { get; set; }

        public int? Group { get; set; }
        public int? Sex { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

   

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ArticleInfo)obj;
            Id = entity.Id;
            ArticleTitle = entity.ArticleTitle;
            LanguageCode = entity.LanguageCode;
            ArticleComment = entity.ArticleComment;
            ArticleContent = entity.ArticleContent;
          
            Role = entity.Role;
            CategoryId = entity.CategoryId;
            //Previewers = entity.Previewers;

            return this;
        }

      

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
