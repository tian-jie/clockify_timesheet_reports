using System;
using Infrastructure.Core;

namespace Innocellence.Activity.Entity
{
    public class AnnualCheckinEntity : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public string LillyId { get; set; }
        public string Name { get; set; }
        public string EventNo { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        public string Expand { get; set; }
       
        /// <summary>
        /// 状态 1.uncheck  2.checked 
        /// </summary>
        public string Status { get; set; }

        public string CreatedUserId { get; set; }
        public string UpdatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
