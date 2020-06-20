using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModelFront
{
    public class SearchUserMPView
    {
        public int? Group { get; set; }
        public int? Sex { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
    }
}
