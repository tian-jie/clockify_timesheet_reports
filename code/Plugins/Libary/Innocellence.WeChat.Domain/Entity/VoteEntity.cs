using System;
using System.Collections.Generic;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
    public class VoteEntity : EntityBase<int>
    {
        //public VoteEntity()
        //{
        //    VoteQuestionEntities = new HashSet<VoteQuestionEntity>();
        //}

        public override int Id { get; set; }

        public string VoteName { get; set; }

        public string CreatUserId { get; set; }

        public DateTime CreatDateTime { get; set; }

        public string VoteType { get; set; }

        /// <summary>
        /// 调查的状态
        /// </summary>
        public string VoteStatus { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public virtual ICollection<VoteQuestionEntity> VoteQuestions { get; set; }
    }
}
