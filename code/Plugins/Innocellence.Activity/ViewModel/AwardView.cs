using Infrastructure.Core;
using Innocellence.Activity.Entity;
using System;

namespace Innocellence.Activity.ModelsView
{
    public partial class AwardView : IViewModel
    {
        public Int32 Id { get; set; }
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
        
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (AwardEntity)obj;
            Id = entity.Id;
            PollingId = entity.PollingId;
            Type = entity.Type;
            SecurityCode = entity.SecurityCode;
            Status = entity.Status;
            AccessDate = entity.AccessDate;

            return this;
        }
    }
}
