using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Entity
{

    public partial class FileManage : EntityBase<int>
    {
        public override Int32 Id { get; set; }

        public String FileName { get; set; }

        public String FileType { get; set; }

        public String FileSize { get; set; }

        public String Url { get; set; }

        public String FilePath { get; set; }

        public String OriginalName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String CreatedUserID { get; set; }

        public String MediaID { get; set; }

        public String Description { get; set; }

        public Boolean IsDeleted { get; set; }

    }
}
