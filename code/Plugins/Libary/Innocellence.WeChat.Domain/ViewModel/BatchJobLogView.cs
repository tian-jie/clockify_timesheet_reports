using System;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Contracts.ViewModel
{
    public class BatchJobLogView : IViewModel
    {
        public int Id { get; set; }

        public string JobID { get; set; }

        public int? Status { get; set; }

        public string Type { get; set; }

        public string StatusDisplay { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Result { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (BatchJobLog)model;
            Id = entity.Id;
            JobID = entity.JobID;
            Status = entity.Status;
            StatusDisplay = entity.Status == 0 ? "未完成" : "已完成";
            Type = entity.Type;
            Result = entity.Result;
            CreatedDate = entity.CreatedDate;

            return this;
        }
    }
}
