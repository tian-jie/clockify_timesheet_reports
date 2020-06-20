using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
    //[Table("Message")]
    public partial class FaqInfoView : IViewModel
    {

        public Int32 Id { get; set; }
        public Int32? AppId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string ContentArea { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string ResourceEnternalLink { get; set; }
        public string KeyResourceAttachment { get; set; }
        public string TierTwo { get; set; }
        public DateTime? ValidThrough { get; set; }
        public string Customer { get; set; }
        public string Owner { get; set; }
        public string RequestStatus { get; set; }
        public string Status { get; set; }
        public string UrgencyStatus { get; set; }
        public bool? IsDeleted { get; set; }
        public int? ReadCount { get; set; }

        public string keyword { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (FaqInfo)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            Title = entity.Title;
            Category = entity.Category;
            ContentArea = entity.ContentArea;
            Question = entity.Question;
            Answer = entity.Answer;
            ResourceEnternalLink = entity.ResourceEnternalLink;
            KeyResourceAttachment = entity.KeyResourceAttachment;
            TierTwo = entity.TierTwo;
            ValidThrough = entity.ValidThrough;
            Customer = entity.Customer;
            Owner = entity.Owner;
            RequestStatus = entity.RequestStatus;
            Status = entity.Status;
            UrgencyStatus = entity.UrgencyStatus;
            ReadCount = entity.ReadCount;
            IsDeleted = entity.IsDeleted;

            return this;
        }
    }
}
