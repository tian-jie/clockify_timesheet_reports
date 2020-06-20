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
        /// FAQ Search�Ĺؼ���
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// FAQ Search�Ľ���б�
        /// </summary>
        public List<FaqInfoView> List { get; set; }
        public string menuCode { get; set; }

    }
}
