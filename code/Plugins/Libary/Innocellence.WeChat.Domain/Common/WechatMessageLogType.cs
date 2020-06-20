using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Common
{
    public enum WechatMessageLogType
    {
        Default = 0,
        text = 1,
        news = 2,
        image = 3,
        file = 4,
        video = 5,
        voice = 6,
    }
}
