using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModelFront
{
    public class AccessJsonView
    {
        // 调用接口执行后的message，如果成功，则为空
        public string message { get; set; }

        // 是否是某个应用的成员，这个主要是用于接口（根据userId获取用户信息）
        public bool isApplicationMember { get; set; }

        // 是否执行成功
        public bool success { get; set; }

        // 取得的详细信息
        public object item { get; set; }
    }
}
