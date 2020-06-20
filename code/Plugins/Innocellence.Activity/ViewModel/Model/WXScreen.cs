using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Innocellence.Activity.Model
{
    //[Table("News")]
    public class WXScreen : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public string LillyId { get; set; }
      
        public Int32 AppId { get; set; }

        public DateTime? OperatedTime { get; set; }
        public Int32 EventId { get; set; }
      
    }
}
