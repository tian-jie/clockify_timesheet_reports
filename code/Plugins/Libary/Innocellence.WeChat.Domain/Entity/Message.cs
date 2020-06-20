using System;
using System.Collections.Generic;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("Message")]
    public partial class Message : EntityBase<int>
	{

        public Int32? AppId { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public String URL { get; set; }

     //   public Guid? Code { get; set; }
        public String Comment { get; set; }
        public String Status { get; set; }
        public Int32? ReadCount { get; set; }
        public Int32? ThumbsUpCount { get; set; }
        public Boolean? IsLike { get; set; }
        public bool IsDeleted { get; set; }
        public Int32? ThumbImageId { get; set; }
        public String ThumbImageUrl { get; set; }
        public DateTime? PublishDate { get; set; }
        public String Previewers { get; set; }
        public String toDepartment { get; set; }
        public String toTag { get; set; }
        public String toUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String UpdatedUserID { get; set; }
	}


    public class CheckResult
    {
        public IList<ErrorDesc> ErrorDepartments { get; set; }

        public IList<ErrorDesc> ErrorTags { get; set; }

        public IList<string> ErrorUsers { get; set; }
    }

    public class ErrorDesc
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
