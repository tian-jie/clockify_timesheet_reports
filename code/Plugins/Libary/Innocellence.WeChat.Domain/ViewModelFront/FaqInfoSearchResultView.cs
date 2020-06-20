using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
    //[Table("Message")]
    public partial class FaqInfoSearchResultView
    {
        /// <summary>
        /// FAQ Search的关键字
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// FAQ Search的结果列表
        /// </summary>
        public List<FaqInfoView> List { get; set; }
        public string menuCode { get; set; }

    }
}
