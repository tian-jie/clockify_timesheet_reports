using System;
using Infrastructure.Core;

namespace qyAPITest.Controllers
{
    public class BlankEntity : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public string Name { get; set; }

        public int AppId { get; set; }

    }
}
