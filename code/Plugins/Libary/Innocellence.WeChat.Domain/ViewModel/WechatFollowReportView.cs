using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView 
{
    public partial class WechatFollowReportView : IViewModel
	{
        public Int32 Id { get; set; }

        public String StatisticsDate { get; set; }

        public Int32? FollowCount { get; set; }

        public Int32? UnFollowCount { get; set; }

        public DateTime? CreatedDate { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (WechatFollowReport)obj;
            Id = entity.Id;
            StatisticsDate = entity.StatisticsDate.HasValue ? 
                Convert.ToDateTime(entity.StatisticsDate).ToString("yyyy-MM-dd") : string.Empty;
            FollowCount = entity.FollowCount;
            UnFollowCount = entity.UnFollowCount;
            CreatedDate = entity.CreatedDate;

            return this;
        }
	}
}
