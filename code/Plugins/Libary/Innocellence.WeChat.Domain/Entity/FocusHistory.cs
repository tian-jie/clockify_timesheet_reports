using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class FocusHistory : EntityBase<int>
    {
        public string UserId { get; set; }
        public DateTime CreatedTime { get; set; }
        public int? QrCodeSceneId { get; set; }
        public int Status { get; set; }
        public int? PeopleCount { get; set; }
        public int? PurePeopleCount { get; set; }
    }
}
