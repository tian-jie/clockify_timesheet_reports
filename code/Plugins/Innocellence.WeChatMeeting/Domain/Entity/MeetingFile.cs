using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMeeting.Domain.Entity
{
    public class MeetingFile : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public Int32? MeetingId { get; set; }
        public String FileName { get; set; }
        public String FilePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }

    }
}