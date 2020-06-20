using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
using Infrastructure.Core;

namespace Innocellence.Activity.Contracts.Entity
{
    public class EventProfileEntity : EntityBase<int>
    {
        public override int Id { get; set; }
        public string TypeCode { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int EventId { get; set; }
        public string ImgUrl { get; set; }
        public DateTime OperatedDateTime { get; set; }
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// For Annual SignIn
        /// </summary>
        public bool? IsDisplay1 { get; set; }
        public bool? IsDisplay2 { get; set; }
        public bool? IsDisplay3 { get; set; }

        //[ForeignKey("EventId")]
        //public virtual EventEntity Event { get; set; }
    }
}
