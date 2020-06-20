using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Contracts.ViewModel
{
    public class MenuReportView : IViewModel
    {
        public Int32 Id { get; set; }
        public string UserId { get; set; }
        public string Menukey { get; set; }
        public DateTime AccessDate { get; set; }
        public int  AppID { get; set; }
        public string MenuName { get; set; }
        public string AppName { get; set; }
        public int VisitorCount { get; set;}
        public int VisitorTimes{ get;set;}
        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (Innocellence.WeChat.Domain.Entity.MenuReport)model;
            Id = entity.Id;
            UserId = entity.UserId;
            Menukey = entity.MenuKey;
            AccessDate = entity.AccessDate;
            AppID = entity.AppId;
            MenuName = entity.MenuName;
            AppName = entity.AppName;
            VisitorCount = entity.VisitorCount;
            VisitorTimes = entity.VisitTimes;
            return this;
        }
    }
}
