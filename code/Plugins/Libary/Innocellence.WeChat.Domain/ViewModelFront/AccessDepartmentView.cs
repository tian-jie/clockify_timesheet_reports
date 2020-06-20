using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModelFront
{
    public class AccessDepartmentView
    {
        // 部门ID
        public Int32 id { get; set; }

        public Int32 parent { get; set; }

        public string level { get; set; }

        public string name { get; set; }

    }
}
