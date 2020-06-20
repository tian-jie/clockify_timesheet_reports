using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class UserInfo : EntityBase<int>
    {
        public override Int32 Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WeChatUserID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Tel { get; set; }
    }
}
