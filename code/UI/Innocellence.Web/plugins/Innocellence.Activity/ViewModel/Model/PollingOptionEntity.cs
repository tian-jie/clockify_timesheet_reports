using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Core;

namespace Innocellence.Activity.Contracts.Entity
{
    public class PollingOptionEntity : EntityBase<int>
    {
        public override int Id { get; set; }
        public string OptionIndex { get; set; }
        public string OptionName { get; set; }
        public int QuestionId { get; set; }
        public string Picture { get; set; }
        /// <summary>
        /// Function Json  {"type":"","isRequired":""}  
        /// type: 0-Normal 1-With input   isRequired: 0-false 1-true
        /// </summary>
        public string Type { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }
        public int? OrderIndex { get; set; }

        [ForeignKey("QuestionId")]
        public virtual PollingQuestionEntity PollingQuestionEntity { get; set; }
        public virtual ICollection<PollingResultEntity> PollingResults { get; set; }
    }
}
