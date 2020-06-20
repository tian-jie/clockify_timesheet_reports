using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Core;

namespace Innocellence.Activity.Contracts.Entity
{
    public class PollingQuestionEntity : EntityBase<int>
    {
        public override int Id { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// 题型 0.多选 1.单选 2.最多选几项 99.填空
        /// </summary>
        public int Type { set; get; }
        public int PollingId { get; set; }
        public bool IsRequired { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 只针对有奖问答
        /// </summary>
        public string RightAnswers { get; set; }
        public decimal? Score { get; set; }
        public int? OrderIndex { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }

        [ForeignKey("PollingId")]
        public virtual PollingEntity PollingEntity { get; set; }
        public virtual ICollection<PollingOptionEntity> PollingOptionEntities { get; set; }
    }
}
