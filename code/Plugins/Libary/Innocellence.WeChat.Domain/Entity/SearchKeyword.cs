using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("NewsInfo")]
    public partial class SearchKeyword : EntityBase<int>
	{
        public Int32? AppId { get; set; }
        public Int32? Category { get; set; }

        public String Keyword { get; set; }

        public Int32? SearchCount { get; set; }
 
	}
}
