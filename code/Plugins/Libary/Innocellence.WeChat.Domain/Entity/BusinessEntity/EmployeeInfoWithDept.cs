using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class EmployeeInfoWithDept
    {
        public EmployeeInfoWithDept()
        {
            deptLvs = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                deptLvs.Add("");
            }
        }
        public string userid { get; set; }
        public string name { get; set; }
        public List<string> deptLvs { get; set; }
        public List<int> department { get; set; }
        public string position { get; set; }
        public string mobile { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string weixinid { get; set; }
        public string avatar { get; set; }
        public int status { get; set; }
        public Dictionary<string, string> tags { get; set; }
    }

}
