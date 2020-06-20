using Infrastructure.Core;
using Innocellence.Activity.Entity;
using System;
using System.ComponentModel;


namespace Innocellence.Activity.Contracts.ViewModel
{
    public class BarrageView : IViewModel
    {

        public Int32 Id { get; set; }
        public string WeixinId { get; set; }

        [DescriptionAttribute("内容")]
        public string FeedBackContent { get; set; }

        public string FeedBackContentHidden { get; set; }
        public string AppId { get; set; }
        public Int32 Status { get; set; }
        public string StatusText { get; set; }
        public Int32 SummaryId { get; set; }
        public Boolean? IsDisplay { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [DescriptionAttribute("审批时间")]
        public DateTime? ApprovedDate { get; set; }

        [DescriptionAttribute("微信名")]
        public string WeixinName { get; set; }

        public string CreatedDateStr { get; set; }

        [DescriptionAttribute("关键字")]
        public string Keyword { get; set; }

        public string WeixinPic { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (Barrage)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            WeixinId = entity.WeixinId;
            WeixinName = entity.WeixinName;
            Keyword = entity.Keyword;
            Status = entity.Status;
            IsDisplay = entity.IsDisplay;
            CreatedDate = entity.CreatedDate;
            UpdatedDate = entity.UpdatedDate;
            ApprovedDate = entity.ApprovedDate;
            FeedBackContent = entity.FeedBackContent;
            WeixinPic = entity.WeixinPic;
            CreatedDateStr = entity.CreatedDate == null
                ? null
                : ((DateTime)entity.CreatedDate).ToString("MM/dd/yyyy") + "  " +
                  ((DateTime)entity.CreatedDate).ToShortTimeString();

            return this;
        }
    }
}