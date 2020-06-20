using System;
using Infrastructure.Core;

namespace Innocellence.Activity.Entity
{
    public class AwardEntity : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        //FK with Polling
        public int PollingId { get; set; }
        public String Type { get; set; }
        public String SecurityCode { get; set; }
        public String Status { get; set; }
        public DateTime? AccessDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public String UpdatedUserID { get; set; }
        public Boolean? IsDeleted { get; set; }
    }
}
