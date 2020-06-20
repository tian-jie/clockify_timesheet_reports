using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.CA.Service.Common
{
    /// <summary>
    /// Category Type, 具体数值和微信中的Application ID对应
    /// </summary>
    public enum CategoryType
    {
        [Description("News")]
        ArticleInfoCate = 6,

        [Description("Hongtu")]
        HongtuCate = 7,

        [Description("Video")]
        VideoCate = 8,

        [Description("Activity")]
        ActivityCate = 9,

        [Description("SurveyCate")]
        SurveyCate = 10,

        [Description("SalesTraining")]
        SalesTrainingCate = 16
    }

    public enum SysConfigCode
    {
        [Description("NewsTemplate")]
        NewsTemplate = 1,
        [Description("WeixinCorpId")]
        WeixinCorpId = 2,
        [Description("WeixinCorpSecret")]
        WeixinCorpSecret = 3
    }

    public enum StatusType
    {
        [Description("")]
        Other = -3,
        [Description("Saved")]
        Saved = -2,
        [Description("Rejected")]
        Rejected = -1,
        [Description("Rollbacked")]
        Rollbacked = 0,
        [Description("Submitted")]
        Submitted = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Published")]
        Published = 3
    }

}
