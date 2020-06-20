using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
    public class VoteAnswerEntity : EntityBase<int>
    {
        //public VoteAnswerEntity()
        //{
        //    VoteResults = new HashSet<VoteResultEntity>();
        //}

        public override int Id { get; set; }

        public string Name { get; set; }

        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual VoteQuestionEntity VoteQuestionEntity { get; set; }

        public virtual ICollection<VoteResultEntity> VoteResults { get; set; }
    }
}
