using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
namespace Innocellence.WeChat.Domain.Entity
{
    public partial class PageReport : EntityBase<int>
    {
        public override int Id { get; set; }
        public int Appid { get; set; }
        public string AppName { get; set; }
        public string PageUrl { get; set; }
        //访问时间
        public DateTime? AccessDate { get; set; }
        //创建时间
        public DateTime? CreatedDate { get; set; }
        //访问人数
        public int? VisitorCount { get; set; }
        //访问次数
        public int? VisitTimes { get; set; }
    }
}
