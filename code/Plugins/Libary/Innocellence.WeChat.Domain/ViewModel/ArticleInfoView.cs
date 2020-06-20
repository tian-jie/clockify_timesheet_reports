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

        [Description("����")]
        public String ArticleTitle { get; set; }

        [Description("ֵ")]
        public String LanguageCode { get; set; }

        [Description("��������")]
        public String ArticleComment { get; set; }

        [Description("��������")]
        public String ArticleContent { get; set; }

        [Description("�������ݱ༭")]
        public String ArticleContentEdit { get; set; }
        //[Required]
        //public String ArticleCateSub { get; set; }

        [Description("������������")]
        public List<ArticleContentView> ArticleContentViews { get; set; }

        [Description("�������ݶ���")]
        public String[] ArticleContents { get; set; }

        [Description("��Ƶ��ַ")]
        public String[] VideoUrl { get; set; }

        [Description("ͼƬ��ַ")]
        public String[] ImgUrl { get; set; }

        [Description("��ͼƬ��ַ")]
        public String[] ImgUrl_Old { get; set; }

        [Description("ͼƬID")]
        public String[] ImgID { get; set; }

        [Description("���´���")]
        public Guid? ArticleCode { get; set; }

        [Description("��ID")]
        public int? GroupID { get; set; }

        [Description("���ID")]
        public int? OrderID { get; set; }

        [Description("Ӧ�ó���ID")]
        public Int32? AppId { get; set; }

        [Description("������ַ")]
        public String ArticleURL { get; set; }

        [Description("ͼƬ������ַ")]
        public String ImageCoverUrl { get; set; }

        [Description("������")]
        public Int32? ThumbsUpCount { get; set; }

        [Description("ͼƬ����")]
        public String ImageName { get; set; }

        [Description("ͼƬ��ַ")]
        public String ImageUrl { get; set; }

        [Description("���")]
        public String ArticleCateName { get; set; }

        [Description("����״̬")]
        public String ArticleStatus { get; set; }

        [Description("�Ķ���")]
        public Int32? ReadCount { get; set; }

        [Description("ͼƬ����")]
        public byte[] ImageContent { get; set; }


        [Description("ͼƬ����_T")]
        public byte[] ImageContent_T { get; set; }

        [Description("ͼƬ����64λ����")]
        public string ImageContentBase64 { get; set; }

        [Description("ɾ��")]
        public Boolean? IsDeleted { get; set; }

        [Description("��")]
        public Boolean? IsLike { get; set; }

        [Description("����")]
        public Boolean? IsTransmit { get; set; }

        [Description("����΢���û�ID")]
        public Boolean? IsPassingWeChatUserID { get; set; }

        [Description("����ʱ��")]
        public DateTime? CreatedDate { get; set; }

        [Description("����ʱ��")]
        public DateTime? UpdatedDate { get; set; }

        [Description("����ʱ��")]
        public DateTime? PublishDate { get; set; }

        public string PublishDateFormatString { get; set; }

        [Description("�û���")]
        public String CreatedUserID { get; set; }
        public String CreatedUserName { get; set; }

        [Description("�����û���")]
        public String UpdatedUserID { get; set; }
        public String UpdatedUserName { get; set; }

        public IList<ArticleInfoView> List { get; set; }
        //   public String ImageCoverUrl { get; set; }

        [Description("����ͼƬID")]
        public Int32? ThumbImageId { get; set; }

        [Description("����ͼƬ��ַ")]
        public String ThumbImageUrl { get; set; }

        [Description("��ɫ")]
        public String Role { get; set; }
        //public String Previewers { get; set; }

        [Description("�����б�")]
        public List<string> PersonList { get; set; }

        [Description("���б�")]
        public List<int> GroupList { get; set; }

        [Description("��ǩ�б�")]
        public List<int> TagList { get; set; }

        [Description("Ӧ�ó�����")]
        public String APPName { get; set; }

        [Description("�ѵ���")]
        public bool IsThumbuped { get; set; }

        [Description("����")]
        public Boolean? ShowLikeCount { get; set; }

        [Description("��ʾ�Ķ�����")]
        public Boolean? ShowReadCount { get; set; }

        [Description("ˮӡ")]
        public Boolean? IsWatermark { get; set; }

        [Description("û�з���")]
        public Boolean? NoShare { get; set; }

        [Description("û�п���")]
        public Boolean? NoCopy { get; set; }

        [Description("��")]
        public int? Group { get; set; }

        [Description("�Ա�")]
        public int? Sex { get; set; }

        [Description("��Χ")]
        public string Province { get; set; }

        [Description("����")]
        public string City { get; set; }

        [Description("���ID")]
        public int? CategoryId { get; set; }

        [Description("�������")]
        public string CategoryName { get; set; }

        [Description("Ԥ��")]
        public String Previewers { get; set; }

        [Description("Լ�����޵�����")]
        public IList<ArticleThumbsUpView> ArticleThumbsUpViews { get; set; }

        [Description("����")]
        public String ToDepartment { get; set; }

        [Description("��ǩ")]
        public String ToTag { get; set; }

        [Description("�û�")]
        public String ToUser { get; set; }

        [Description("��������")]
        public Int32? ArticleType { get; set; } //0����1��Ϣ

        [Description("��������")]
        public Int32? ContentType { get; set; }

        [Description("����")]
        public string ContentTypeName { get; set; }

        //public virtual ICollection<ArticleThumbsUp> ArticleThumbsUps { get; set; }

        [Description("��ʱ����ʱ��")]
        public DateTime? ScheduleSendTime { get; set; }

        [Description("��ȫ����")]
        public Int32? SecurityLevel { get; set; }

        public NewsInfoView NewsInfo { get; set; }

        public string WxDetailUrl { get; set; }

        [Description("���Բ鿴����")]
        public bool CanReadComment { get; set; }

        /// <summary>
        /// ֻ�й�ע���û��ſ����������
        /// </summary>
        [Description("�����������")]
        public bool CanAddComment { get; set; }

        public Boolean? ShowComment { get; set; }

        [Description("Ԥ����ʼʱ��")]
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
