using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
    public class VoteResultEntity : EntityBase<int>
    {
        public override int Id { get; set; }

        public int AnwserId { get; set; }

        public int QuestionId { get; set; }

        public int VoteId { get; set; }

        public int VoteCount { get; set; }
    }
}
