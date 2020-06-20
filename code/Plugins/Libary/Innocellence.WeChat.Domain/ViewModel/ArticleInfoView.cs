using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System.ComponentModel;

namespace Innocellence.WeChat.Domain.ModelsView
{
    //[Table("ArticleInfoView")]

    public partial class ArticleInfoView : IViewModel
    {
        

        public ArticleInfoView()
        {
            ArticleContentViews = new List<ArticleContentView>();
            List = new List<ArticleInfoView>();
        }

        public Int32 Id { get; set; }
        public int IsCorp { get; set; }

        [Description("标题")]
        public String ArticleTitle { get; set; }

        [Description("值")]
        public String LanguageCode { get; set; }

        [Description("文章评论")]
        public String ArticleComment { get; set; }

        [Description("文章内容")]
        public String ArticleContent { get; set; }

        [Description("文章内容编辑")]
        public String ArticleContentEdit { get; set; }
        //[Required]
        //public String ArticleCateSub { get; set; }

        [Description("文章内容评论")]
        public List<ArticleContentView> ArticleContentViews { get; set; }

        [Description("文章内容多条")]
        public String[] ArticleContents { get; set; }

        [Description("视频网址")]
        public String[] VideoUrl { get; set; }

        [Description("图片网址")]
        public String[] ImgUrl { get; set; }

        [Description("旧图片网址")]
        public String[] ImgUrl_Old { get; set; }

        [Description("图片ID")]
        public String[] ImgID { get; set; }

        [Description("文章代码")]
        public Guid? ArticleCode { get; set; }

        [Description("组ID")]
        public int? GroupID { get; set; }

        [Description("序号ID")]
        public int? OrderID { get; set; }

        [Description("应用程序ID")]
        public Int32? AppId { get; set; }

        [Description("文章网址")]
        public String ArticleURL { get; set; }

        [Description("图片覆盖网址")]
        public String ImageCoverUrl { get; set; }

        [Description("点赞数")]
        public Int32? ThumbsUpCount { get; set; }

        [Description("图片名字")]
        public String ImageName { get; set; }

        [Description("图片网址")]
        public String ImageUrl { get; set; }

        [Description("类别")]
        public String ArticleCateName { get; set; }

        [Description("文章状态")]
        public String ArticleStatus { get; set; }

        [Description("阅读数")]
        public Int32? ReadCount { get; set; }

        [Description("图片内容")]
        public byte[] ImageContent { get; set; }


        [Description("图片内容_T")]
        public byte[] ImageContent_T { get; set; }

        [Description("图片内容64位编码")]
        public string ImageContentBase64 { get; set; }

        [Description("删除")]
        public Boolean? IsDeleted { get; set; }

        [Description("像")]
        public Boolean? IsLike { get; set; }

        [Description("传输")]
        public Boolean? IsTransmit { get; set; }

        [Description("传递微信用户ID")]
        public Boolean? IsPassingWeChatUserID { get; set; }

        [Description("创建时间")]
        public DateTime? CreatedDate { get; set; }

        [Description("更新时间")]
        public DateTime? UpdatedDate { get; set; }

        [Description("激活时间")]
        public DateTime? PublishDate { get; set; }

        public string PublishDateFormatString { get; set; }

        [Description("用户名")]
        public String CreatedUserID { get; set; }
        public String CreatedUserName { get; set; }

        [Description("更新用户名")]
        public String UpdatedUserID { get; set; }
        public String UpdatedUserName { get; set; }

        public IList<ArticleInfoView> List { get; set; }
        //   public String ImageCoverUrl { get; set; }

        [Description("点赞图片ID")]
        public Int32? ThumbImageId { get; set; }

        [Description("点赞图片网址")]
        public String ThumbImageUrl { get; set; }

        [Description("角色")]
        public String Role { get; set; }
        //public String Previewers { get; set; }

        [Description("人物列表")]
        public List<string> PersonList { get; set; }

        [Description("组列表")]
        public List<int> GroupList { get; set; }

        [Description("标签列表")]
        public List<int> TagList { get; set; }

        [Description("应用程序名")]
        public String APPName { get; set; }

        [Description("已点赞")]
        public bool IsThumbuped { get; set; }

        [Description("像素")]
        public Boolean? ShowLikeCount { get; set; }

        [Description("显示阅读次数")]
        public Boolean? ShowReadCount { get; set; }

        [Description("水印")]
        public Boolean? IsWatermark { get; set; }

        [Description("没有分享")]
        public Boolean? NoShare { get; set; }

        [Description("没有拷贝")]
        public Boolean? NoCopy { get; set; }

        [Description("组")]
        public int? Group { get; set; }

        [Description("性别")]
        public int? Sex { get; set; }

        [Description("范围")]
        public string Province { get; set; }

        [Description("城市")]
        public string City { get; set; }

        [Description("类别ID")]
        public int? CategoryId { get; set; }

        [Description("类别名称")]
        public string CategoryName { get; set; }

        [Description("预览")]
        public String Previewers { get; set; }

        [Description("约束点赞的文章")]
        public IList<ArticleThumbsUpView> ArticleThumbsUpViews { get; set; }

        [Description("部门")]
        public String ToDepartment { get; set; }

        [Description("标签")]
        public String ToTag { get; set; }

        [Description("用户")]
        public String ToUser { get; set; }

        [Description("文章类型")]
        public Int32? ArticleType { get; set; } //0新闻1消息

        [Description("内容类型")]
        public Int32? ContentType { get; set; }

        [Description("类型")]
        public string ContentTypeName { get; set; }

        //public virtual ICollection<ArticleThumbsUp> ArticleThumbsUps { get; set; }

        [Description("定时发送时间")]
        public DateTime? ScheduleSendTime { get; set; }

        [Description("安全级别")]
        public Int32? SecurityLevel { get; set; }

        public NewsInfoView NewsInfo { get; set; }

        public string WxDetailUrl { get; set; }

        [Description("可以查看评论")]
        public bool CanReadComment { get; set; }

        /// <summary>
        /// 只有关注的用户才可以添加评论
        /// </summary>
        [Description("可以添加评论")]
        public bool CanAddComment { get; set; }

        public Boolean? ShowComment { get; set; }

        [Description("预览开始时间")]
        public DateTime? PreviewStartDate { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ArticleInfo)obj;
            Id = entity.Id;
            ArticleTitle = entity.ArticleTitle;
            LanguageCode = entity.LanguageCode;
            ArticleComment = entity.ArticleComment;
            ArticleContent = entity.ArticleContent;
            ArticleContentEdit = entity.ArticleContentEdit;
            ArticleCode = entity.ArticleCode;
            ArticleURL = entity.ArticleURL;
            ThumbsUpCount = entity.ThumbsUpCount;
            ImageCoverUrl = entity.ImageCoverUrl;

            GroupID = entity.GroupID;
            OrderID = entity.OrderID;
            AppId = entity.AppId;
            ArticleStatus = entity.ArticleStatus;
            ReadCount = entity.ReadCount;
            //ArticleCateSub = entity.ArticleCateSub;
            IsDeleted = entity.IsDeleted;
            IsLike = entity.IsLike;

            NoCopy = entity.NoCopy;
            NoShare = entity.NoShare;
            IsWatermark = entity.IsWatermark;
            ShowReadCount = entity.ShowReadCount;
            ShowLikeCount = entity.ShowLikeCount;

            IsTransmit = entity.IsTransmit;
            IsPassingWeChatUserID = entity.IsPassingWeChatUserID;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            CreatedUserName = entity.CreatedUserName;
            UpdatedUserName = entity.UpdatedUserName;
            PublishDate = entity.PublishDate;
            Role = entity.Role;
            CategoryId = entity.CategoryId;
            //Previewers = entity.Previewers;

            ArticleType = entity.ArticleType;
            ToDepartment = entity.ToDepartment;
            ToTag = entity.ToTag;
            ToUser = entity.ToUser;
            ContentType = entity.ContentType;
            Previewers = entity.Previewers;
            SecurityLevel = entity.SecurityLevel;
            ScheduleSendTime = entity.ScheduleSendTime;
            ShowComment = entity.ShowComment;
            PreviewStartDate = entity.PreviewStartDate;

            if (!string.IsNullOrEmpty(entity.ArticleContentEdit))
            {
                ArticleContentViews = Infrastructure.Utility.Data.JsonHelper.FromJson<List<ArticleContentView>>(entity.ArticleContentEdit);
            }
            else
            {
                ArticleContentViews = new List<ArticleContentView>();
            }


            //try
            //{
            //    // ArticleContentStructures = Infrastructure.Utility.Data.JsonHelper.FromJson<List<ArticleContentStructure>>(entity.ArticleContent);
            //}
            //catch (Exception ex)
            //{
            //    string s = ex.Message;
            //}
            return this;
        }



        public IViewModel ConvertAPIModelList(object obj)
        {
            var entity = (ArticleInfo)obj;
            Id = entity.Id;
            ArticleTitle = entity.ArticleTitle;
            LanguageCode = entity.LanguageCode;
            ArticleComment = entity.ArticleComment;
            // ArticleContent = entity.ArticleContent;
            ArticleCode = entity.ArticleCode;
            ArticleURL = entity.ArticleURL;
            ThumbsUpCount = entity.ThumbsUpCount;
            ImageCoverUrl = entity.ImageCoverUrl;

            GroupID = entity.GroupID;
            OrderID = entity.OrderID;

            Previewers = entity.Previewers;
            AppId = entity.AppId;
            ArticleStatus = entity.ArticleStatus;
            ReadCount = entity.ReadCount;
            //ArticleCateSub = entity.ArticleCateSub;
            IsDeleted = entity.IsDeleted;
            IsLike = entity.IsLike;

            NoCopy = entity.NoCopy;
            NoShare = entity.NoShare;
            IsWatermark = entity.IsWatermark;
            ShowReadCount = entity.ShowReadCount;
            ShowLikeCount = entity.ShowLikeCount;

            IsTransmit = entity.IsTransmit;
            IsPassingWeChatUserID = entity.IsPassingWeChatUserID;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            CreatedUserName = entity.CreatedUserName;
            UpdatedUserName = entity.UpdatedUserName;
            PublishDate = entity.PublishDate;
            Role = entity.Role;
            CategoryId = entity.CategoryId;

            ArticleType = entity.ArticleType;
            ToDepartment = entity.ToDepartment;
            ToTag = entity.ToTag;
            ToUser = entity.ToUser;
            ContentType = entity.ContentType;
            Previewers = entity.Previewers;

            SecurityLevel = entity.SecurityLevel;
            ScheduleSendTime = entity.ScheduleSendTime;
            ShowComment = entity.ShowComment;
            IsLike = entity.IsLike;
            ShowReadCount = entity.ShowReadCount;

            return this;
        }

        public IViewModel ConvertAPIModelListWithContent(object obj)
        {
            this.ConvertAPIModelList(obj);
            var entity = (ArticleInfo)obj;
            this.ArticleContent = entity.ArticleContent;
            this.ArticleContentEdit = entity.ArticleContentEdit;
            return this;
        }

        public IViewModel ConvertToNewsInfoView(object obj)
        {
            var entity = (ArticleInfo)obj;
            return new NewsInfoView()
            {
                AppId = entity.AppId.Value,
                // City=entity.ci
                ImageContent = entity.ImageCoverUrl,
                ImageSrc = entity.ImageCoverUrl,
                NewsCate = "",
                NewsComment = entity.ArticleComment,
                NewsContent = entity.ArticleContent,
                NewsTitle = entity.ArticleTitle,
                Id = entity.Id,
                IsLike = entity.IsLike,
                ShowComment = entity.ShowComment,
                ShowReadCount = entity.ShowReadCount,

            };



        }

        public override bool Equals(object obj)
        {
            try
            {
                ArticleInfoView y = obj as ArticleInfoView;
                if (obj != null)
                {
                    return this.Id == y.Id && this.AppId == y.AppId;
                }
            }
            catch (Exception)
            {

            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
