using System;
using System.Collections.Generic;
using Infrastructure.Core;

namespace Innocellence.Activity.Contracts.Entity
{
    public class PollingEntity : EntityBase<int>
    {

        public override int Id { get; set; }
        public Guid GuiId { get; set; }
        public string AppId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// PollingType 1.有奖问答 2.投票  3.活动
        /// </summary>
        public int? Type { get; set; }
        public string Status { get; set; }
        public string PollingHtml { get; set; }
        /// <summary>
        /// 只针对有奖问答
        /// </summary>
        public int? AwardNumber { get; set; }
        public decimal? StandardScore { get; set; }
        public string ReplyMessage { get; set; }

        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }

        public virtual ICollection<PollingQuestionEntity> PollingQuestions { get; set; }
    }
}
