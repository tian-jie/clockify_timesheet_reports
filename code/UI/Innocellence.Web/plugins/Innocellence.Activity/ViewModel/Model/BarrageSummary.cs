using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Innocellence.Activity.Entity
{
    //[Table("News")]
    public class BarrageSummary : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public string AppId { get; set; }
        public string Title { get; set; }
        public string Keyword { get; set; }
        public string ReturnText { get; set; }
        /// <summary>
        /// Barrage or Screen
        /// </summary>
        public string SummaryType { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }
        public Boolean? IsEnabled { get; set; }

        public enum SummaryTypes
        {
            Screen,
            Barrage
        }
    }
}
