using Infrastructure.Core;
using Innocellence.Activity.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Script.Serialization;

namespace Innocellence.Activity.ViewModel
{
    public partial class AnnualCheckinView : IViewModel
    {
        public Int32 Id { get; set; }
        public string LillyId { get; set; }
        [DescriptionAttribute("姓名")]
        public string Name { get; set; }
        public string Expand { get; set; }
        /// <summary>
        /// 入住酒店
        /// </summary>
        [DescriptionAttribute("入住酒店")]
        public string CheckHotel { get; set; }

        /// <summary>
        /// 衣服尺寸
        /// </summary>
        [DescriptionAttribute("衣服尺寸")]
        public string MaterialNum { get; set; }
         [DescriptionAttribute("活动编号")]
        public string EventNo { get; set; }

        /// <summary>
        /// 状态 1.uncheck  2.checked 
        /// </summary>
        [DescriptionAttribute("状态")]
        public string Status { get; set; }

        public string CreatedUserId { get; set; }

        public string UpdatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        [DescriptionAttribute("领取时间")]
        public DateTime? UpdatedDate { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (AnnualCheckinEntity)obj;
            Id = entity.Id;
            LillyId = entity.LillyId;
            Name = entity.Name;
            var js = new JavaScriptSerializer();
            var jsonResult = js.Deserialize<object>(entity.Expand);
            var fullResult = jsonResult as Dictionary<string, object>;
            if (fullResult != null)
            {
                CheckHotel = fullResult["入住酒店"] as string;
                MaterialNum = fullResult["衣服尺寸"] as string;
            }
            Status = entity.Status == "Checked" ? "已领取" : "未领取";
            EventNo = entity.EventNo;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserId = entity.UpdatedUserId;
            return this;
        }
    }
}
