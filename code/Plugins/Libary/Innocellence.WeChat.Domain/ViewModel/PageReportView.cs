using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
	//[Table("CourseAttention")]
    public partial class PageReportView : IViewModel
	{
	
		public Int32 Id { get;set; }

        public string GroupName { get; set; }
        public string AppName { get; set; }
        public string PageUrl { get; set; }
        public DateTime? AccessDate { get; set; }
        //访问人数
        public int? VisitorCount { get; set; }
        //访问次数
        public int? VisitTimes { get; set; }

        //访问总人数
        public int? TotalVisitorCount { get; set; }
        //访问总次数
        public int? TotalVisitTimes { get; set; }
	//	public  TrainingCourseView TrainingCourse { get;set; }
 
 
        public IViewModel ConvertAPIModel(object obj){
            var entity = (PageReport)obj;
	    Id =entity.Id;
        PageUrl = entity.PageUrl;
        AccessDate = entity.AccessDate;
        VisitorCount = entity.VisitorCount;
        VisitTimes = entity.VisitTimes;
        AppName = entity.AppName;
        return this;
        }

        public IViewModel ConvertAPIModel(object obj,string groupName,string appName)
        {
            var entity = (PageReport)obj;
            Id = entity.Id;
            PageUrl = entity.PageUrl;
            AccessDate = entity.AccessDate;
            VisitorCount = entity.VisitorCount;
            VisitTimes = entity.VisitTimes;
            GroupName = groupName;
            AppName = appName;

            return this;
        }
	}
}
