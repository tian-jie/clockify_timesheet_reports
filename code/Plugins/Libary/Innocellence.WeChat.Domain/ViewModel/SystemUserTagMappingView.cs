using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class SystemUserTagMappingView
    {
        public  int Id { get; set; }

        public int TagId { get; set; }

        public string UserOpenid { get; set; }

        public bool IsDeleted { get; set; }
    }
}
