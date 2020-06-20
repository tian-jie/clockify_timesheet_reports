using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public partial class QrCodeMPItem : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public int? AppId { get; set; }
        public int? SceneId { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserID { get; set; }
        public bool? Deleted { get; set; }
        public int? RelatedUserId { get; set; }
    }
}
