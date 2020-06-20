using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;

using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView 
{
    public partial class AppAccessReportView : IViewModel
	{
        public int Id { get; set; }

        public DateTime? AccessDate { get; set; }

        public int? AppId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string AppName { get; set; }
        public int? AccessPerson { get; set; }
        public int? AccessCount { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (AppAccessReport)obj;
            Id = entity.Id;
            AccessDate = entity.AccessDate;
            AppId = entity.AppId;
            CreatedDate = entity.CreatedDate;
            AppName = entity.AppName;
            AccessPerson = entity.AccessPerson;
            AccessCount = entity.AccessCount;
            return this;
        }
	}
}
