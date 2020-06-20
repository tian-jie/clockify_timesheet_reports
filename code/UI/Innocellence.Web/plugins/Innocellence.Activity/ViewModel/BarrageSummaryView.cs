
using Infrastructure.Core;
using Innocellence.Activity.Entity;
using System;
using System.ComponentModel;
using System.Text;

namespace Innocellence.Activity.Contracts.ViewModel
{
    public class BarrageSummaryView : IViewModel
    {

        public Int32 Id { get; set; }
        public string AppId { get; set; }

        [DescriptionAttribute("标题")]
        public string Title { get; set; }

        [DescriptionAttribute("关键字")]
        public string Keyword { get; set; }
        public string KeywordStr { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }

        [DescriptionAttribute("创建日期")]
        public DateTime? CreatedDate { get; set; }
        [DescriptionAttribute("更新日期")]
        public DateTime? UpdatedDate { get; set; }

        public string CreatedDateStr { get; set; }
        public string RollUrl { get; set; }
        public string NotRollUrl { get; set; }
        public string SummaryType { get; set; }
        public string QuestionTypeStr { get; set; }
        public string QRCodeDisplay { get; set; }
        public string ReturnText { get; set; }
        public Boolean? IsDeleted { get; set; }
        public Boolean? IsEnabled { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (BarrageSummary)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            Title = entity.Title;
            Keyword = entity.Keyword;
            KeywordStr = System.Web.HttpUtility.UrlEncode(entity.Keyword, Encoding.UTF8);
            CreatedUserID = entity.CreatedUserID;
            CreatedDate = entity.CreatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            UpdatedDate = entity.UpdatedDate;
            IsDeleted = entity.IsDeleted;
            IsEnabled = entity.IsEnabled;
            ReturnText = entity.ReturnText;
            CreatedDateStr = entity.CreatedDate == null
                ? null
                : ((DateTime)entity.CreatedDate).ToString("MM/dd/yyyy") + "  " +
                  ((DateTime)entity.CreatedDate).ToShortTimeString();
            SummaryType = entity.SummaryType;

            return this;
        }
    }
}