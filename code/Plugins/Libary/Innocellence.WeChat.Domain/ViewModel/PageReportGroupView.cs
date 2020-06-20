using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
namespace Innocellence.WeChat.Domain.Entity
{
    public partial class PageReportGroupView : IViewModel
    {
        public int Id { get; set; }
        //分组名称
        public string GroupName { get; set; }
        //分组编码
        public string GroupCode { get; set; }
        public string PageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Int32? AppId { get; set; }
        public String AppName { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (PageReportGroup)obj;
            Id = entity.Id;
            GroupName = entity.GroupName;
            GroupCode = entity.GroupCode;
            PageUrl = entity.PageUrl;
            CreatedDate = entity.CreatedDate;
            UpdatedDate = entity.UpdatedDate;
            AppId = entity.AppId;

            return this;
        }
    }
}
