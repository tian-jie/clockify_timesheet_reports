using Infrastructure.Core;
using System;

namespace Innocellence.WeChat.Domain.Entity
{
    public class WechatUserRequestMessageTag : EntityBase<int>
    {
        public override int Id { get; set; }

        public int AppID { get; set; }

        public String TagName { get; set; }

        public Boolean IsDeleted { get; set; }

        public String CreatedUserID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String UpdatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
