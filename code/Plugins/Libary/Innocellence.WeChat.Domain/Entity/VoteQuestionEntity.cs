using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
    public class VoteQuestionEntity : EntityBase<int>
    {
        //public VoteQuestionEntity()
        //{
        //    VoteAnwserEntities = new HashSet<VoteAnswerEntity>();
        //}

        public override int Id { get; set; }

        public string QuestionName { get; set; }

        public string QuestionType { set; get; }

        public virtual ICollection<VoteAnswerEntity> VoteAnwserEntities { get; set; }


        public int VoteId { get; set; }

        [ForeignKey("VoteId")]
        public virtual VoteEntity VoteEntity { get; set; }

        public bool IsRequired { get; set; }
    }
}
