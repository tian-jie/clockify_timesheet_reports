using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Innocellence.Activity.Model
{
	//[Table("News")]
    public  class BarrageExt : EntityBase<int>
	{
        public override Int32 Id { get; set; }

        public  string WeixinId { get; set; }

        public  string FeedBackContent { get; set; }

        public  string AppId { get; set; }

        public  Int32 Status { get; set; }

        public  DateTime? CreatedDate { get; set; }

        public  DateTime? UpdatedDate { get; set; }

        public  string WeixinName { get; set; }

        public  string FileName { get; set; }
        public  string Keyword { get; set; }
        public  Int32 BarrageId { get; set; }
	}
}
