using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Entity
{
    [NotMapped]
    public class ReportExtention : UserBehavior
    {
        /// <summary>
        /// 出报表时需要冗余图文标题
        /// </summary>
        [NotMapped]
        public string Title { get; set; }

        [NotMapped]
        public int NewsId { get; set; }

        [NotMapped]
        public string MenuName { get; set; }

        [NotMapped]
        public string MenuKey { get; set; }
    }
}
