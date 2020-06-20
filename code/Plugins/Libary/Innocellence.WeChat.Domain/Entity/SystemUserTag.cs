using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class SystemUserTag : EntityBase<int>
    {
        public override int Id { get; set; }

        public int? AccountManageId { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}
