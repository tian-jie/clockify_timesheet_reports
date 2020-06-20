using Infrastructure.Core;
using System;

namespace Innocellence.Activity.Entity
{
    //[Table("News")]
    public class Barrage : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public string WeixinId { get; set; }
        public string FeedBackContent { get; set; }
        public string AppId { get; set; }
        public Int32 Status { get; set; }
        public Boolean? IsDisplay { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string WeixinName { get; set; }
        public string Keyword { get; set; }
        public Int32 SummaryId { get; set; }
        public string WeixinPic { get; set; }
      
    }
}
