using System;
using Infrastructure.Core;
using Infrastructure.Utility.IO;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Contracts.ViewModel
{
    public class ArticleReportView : IViewModel
    {
        [CsvIgnore]
        public Int32 Id { get; set; }

        [CsvIgnore]
        public Int32 AppId { get; set; }

        public Int32 ArticleId { get; set; }

        [CsvIgnore]
        public string MenuKey { get; set; }

        [CsvIgnore]
        public string MenuName { get; set; }

        public string AppName { get; set; }

        public string ArticleTitle { get; set; }

        [CsvIgnore]
        public DateTime? CreatedDate { get; set; }

        public DateTime? AccessDate { get; set; }
        public int VisitorCount { get; set; }
        public int VisitTimes { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ArticleReport)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            ArticleId = entity.ArticleId;
            MenuKey = entity.MenuKey;
            MenuName = entity.MenuName;
            AppName = entity.AppName;
            ArticleTitle = entity.ArticleTitle;
            CreatedDate = entity.CreatedDate;
            AccessDate = entity.AccessDate;
            VisitorCount = entity.VisitorCount;
            VisitTimes = entity.VisitTimes;

            return this;
        }
    }
}
