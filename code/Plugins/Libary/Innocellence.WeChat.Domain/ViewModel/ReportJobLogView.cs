using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView 
{
    public partial class ReportJobLogView : IViewModel
	{
        public int Id { get; set; }

        public string JobName { get; set; }

        public string JobStatus { get; set; }

        public string ErrorMessage { get; set; }

        public String DateFrom { get; set; }

        public String DateTo { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (ReportJobLog)obj;
            Id = entity.Id;
            JobName = entity.JobName;
            JobStatus = entity.JobStatus;
            ErrorMessage = entity.ErrorMessage;
            DateFrom = string.Empty;
            if (entity.DateFrom != null)
            {
                DateFrom = entity.DateFrom.ToString("yyyy-MM-dd");
            }
            DateTo = string.Empty;
            if (entity.DateTo != null)
            {
                DateTo = entity.DateTo.ToString("yyyy-MM-dd");
            }
            
            CreatedDate = entity.CreatedDate;
            UpdatedDate = entity.UpdatedDate;

            return this;
        }
	}
}
