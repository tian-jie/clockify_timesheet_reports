using Infrastructure.Core;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innocellence.Activity.Contracts.Entity
{
    public class PollingAnswerEntity : EntityBase<int>
    {
        public override int Id { get; set; }
        public int PollingId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string LillyId { get; set; }
        public string SelectAnswer { get; set; }
        public string RightAnswer { get; set; }
        public bool Status { get; set; }
        public decimal? Score { get; set; }
        public string Name { get; set; }
        public string Dept1 { get; set; }
        public string Dept2 { get; set; }
        public string Dept3 { get; set; }
        public Boolean? IsDeleted { get; set; }

    }
}
