using Infrastructure.Core;
using System;

namespace Innocellence.WeChatMeeting.Domain.Entity
{
    public class XXXX : EntityBase<int>
    {
        public override Int32 Id { get; set; }

        public Int32 AppId { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public Int32 PrimaryType { get; set; }

        public Boolean IsDeleted { get; set; }

        public String CreatedUserID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String UpdatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
