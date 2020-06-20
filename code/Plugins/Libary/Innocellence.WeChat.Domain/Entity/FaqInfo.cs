using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class FaqInfo : EntityBase<int>
    {
        //[Id("Id",IsDbGenerated=true)]
        public Int32 AppId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ContentArea { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ResourceEnternalLink { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string KeyResourceAttachment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TierTwo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ValidThrough { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Customer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RequestStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UrgencyStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? ReadCount { get; set; }
    }
}
