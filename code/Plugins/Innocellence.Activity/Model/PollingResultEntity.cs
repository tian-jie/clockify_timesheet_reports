using Infrastructure.Core;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innocellence.Activity.Contracts.Entity
{
    public class PollingResultEntity : EntityBase<int>
    {
        public override int Id { get; set; }
        public int PollingId { get; set; }
        public int QuestionId { get; set; }
        public int Answer { get; set; }
        public string AnswerText { get; set; }//填空题答案放入
        public int? AnswerPicture { get; set; }//备用
        public string UserId { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }

        public string QuestionName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }

        [ForeignKey("Answer")]
        public virtual PollingOptionEntity PollingOptionEntity { get; set; }
    }

    public class PollingResultTempEntity : EntityBase<int>
    {
        public override int Id { get; set; }
        public int PollingId { get; set; }
        public int QuestionId { get; set; }
        public int Answer { get; set; }
        public string AnswerText { get; set; }//填空题答案放入
        public int? AnswerPicture { get; set; }//备用
        public string UserId { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }

        public string QuestionName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }

        [ForeignKey("Answer")]
        public virtual PollingOptionEntity PollingOptionEntity { get; set; }
    }
}
