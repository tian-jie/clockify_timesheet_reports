using System;
using System.Collections.Generic;
using Infrastructure.Core;
using System.ComponentModel;

namespace Innocellence.WeChat.Domain.Entity
{
    //[Table("NewsInfo")]
    public class ArticleInfo : EntityBase<int>
    {
        //[Id("Id",IsDbGenerated=true)]
        public override Int32 Id { get; set; }

        //[Column("NewsTitle",DbType=DBType.NVarChar,Length=2048,Precision=1024,IsNullable=true)]
        public String ArticleTitle { get; set; }

        //[Column("NewsTitleEN",DbType=DBType.VarChar,Length=1024,Precision=1024,IsNullable=true)]
        public String LanguageCode { get; set; }

        private String articleContent = string.Empty;

        //[Column("NewsContent",DbType=DBType.NVarChar,Length=-1,Precision=-1,IsNullable=true)]
        public String ArticleContent
        {
            get
            {
                return System.Web.HttpUtility.UrlDecode(articleContent);
            }
            set
            {
                articleContent = System.Web.HttpUtility.UrlEncode(value);
            }
        }

        public string ArticleContentEdit { get; set; }

        public String ArticleURL { get; set; }

        public String ImageCoverUrl { get; set; }

        //[Column("NewsContentEN",DbType=DBType.VarChar,Length=-1,Precision=-1,IsNullable=true)]
        public Guid? ArticleCode { get; set; }

        public int? GroupID { get; set; }

        public int? OrderID { get; set; }

        //[Column("NewsComment",DbType=DBType.NVarChar,Length=2048,Precision=1024,IsNullable=true)]
        public String ArticleComment { get; set; }

        //the category of the news
        //[Column("NewsCate",DbType=DBType.Int32,Length=4,Precision=10,IsNullable=true)]
        public Int32? AppId { get; set; }
        public String ArticleCateSub { get; set; }

        //0add 1publisthed
        //[Column("NewsStatus",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
        public String ArticleStatus { get; set; }

        //the count of the read
        //[Column("ReadCount",DbType=DBType.Int32,Length=4,Precision=10,IsNullable=true)]
        public Int32? ReadCount { get; set; }
        public Int32? ThumbsUpCount { get; set; }

        //[Column("IsDeleted",DbType=DBType.Boolean,Length=1,Precision=1,IsNullable=true)]
        public Boolean? IsDeleted { get; set; }

        //  public String ImageCoverUrl { get; set; }

        //public Int32? ThumbImageId { get; set; }
        //public String ThumbImageUrl { get; set; }


        public String ToDepartment { get; set; }
        public String ToTag { get; set; }
        public String ToUser { get; set; }
        public Int32? ArticleType { get; set; } //0新闻1消息


        //[Column("CreatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
        public DateTime? CreatedDate { get; set; }

        //[Column("CreatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
        public String CreatedUserID { get; set; }

        public String CreatedUserName { get; set; }

        //[Column("UpdatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
        public DateTime? UpdatedDate { get; set; }

        //[Column("UpdatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
        public String UpdatedUserID { get; set; }

        public String UpdatedUserName { get; set; }

        //[Column("PublishDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
        public DateTime? PublishDate { get; set; }

        public String Role { get; set; }

        //[Column("Previewers",DbType=DBType.nvarchar,Length=500,Precision=23,IsNullable=true)]
        public String Previewers { get; set; }

        //[Column("IsLike",DbType=DBType.Boolean,Length=1,Precision=1,IsNullable=true)]
        public Boolean? IsLike { get; set; }

        public Boolean? ShowLikeCount { get; set; }
        public Boolean? ShowReadCount { get; set; }
        public Boolean? IsWatermark { get; set; }
        public Boolean? NoShare { get; set; }

        public Boolean? NoCopy { get; set; }
        // public Boolean? NoShare { get; set; }

        public Boolean? IsTransmit { get; set; }
        public Boolean? IsPassingWeChatUserID { get; set; }
        public int? CategoryId { get; set; }

        /// <summary>
        /// 1消息 0新闻
        /// </summary>
        public Int32? ContentType { get; set; }

        /// <summary>
        /// 0 企业号内部人员/关注人员  1仅收到消息的人  2app可见范围内人员  3所有人
        /// </summary>
        public Int32? SecurityLevel { get; set; }

        //public virtual ICollection<ArticleThumbsUp> ArticleThumbsUps { get; set; }

        public DateTime? ScheduleSendTime { get; set; }

        public Boolean? ShowComment { get; set; }

        public DateTime? PreviewStartDate { get; set; }
    }

    public enum SecurityLevel
    {
        [Description("企业号内部人员/关注人员")]
        AllUserInAccoumentManagement = 0,

        [Description("仅收到消息的人")]
        JustReceivedUser = 1,

        [Description("app可见范围内人员")]
        AllUserInApp = 2,

        [Description("所有人")]
        AllPeople = 3,
    }
}
