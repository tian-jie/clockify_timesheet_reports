using System;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
    public class NewsPublishHistoryEntity : EntityBase<int>
    {
        public override int Id { get; set; }

        public int AppID { get; set; }

        public int NewsId { get; set; }

        public string NewsTitle { get; set; }

        public string ToUser { get; set; }

        public string ToDepartment { get; set; }

        public string ToTag { get; set; }

        public string SendStatus { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Type { get; set; }

        public string CreatedUserID { get; set; }
    }
}
