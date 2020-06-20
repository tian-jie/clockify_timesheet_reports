using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
	//[Table("Survey")]
	public partial class SurveyView :IViewModel
	{
	
		public Int32 Id { get;set; }
        public Int32? IdEN { get; set; }

		public  String SurveyTitle { get;set; }
        public  String SurveyTitleEN { get; set; }
        public String LanguageCode { get; set; }

        public  Guid? SurveyCode { get; set; }
		public  Int32? RefenrenceID { get;set; }
		public  String SurveyURL { get;set; }

		public  Int32? SurveyCate { get;set; }
        public  Int32? SurveyCateEN { get; set; }

		public  Int32? DeviceType { get;set; }
		public  Boolean? IsDeleted { get;set; }
		public  DateTime? CreatedDate { get;set; }
		public  String CreatedUserID { get;set; }
		public  DateTime? UpdatedDate { get;set; }
		public  String UpdatedUserID { get;set; }

        public String SurveyCateName { get; set; }
 
        public IViewModel ConvertAPIModel(object obj){
		var entity= (Survey)obj;
	    Id =entity.Id;
	    SurveyTitle =entity.SurveyTitle;
        LanguageCode = entity.LanguageCode;
        SurveyCode = entity.SurveyCode;
	    RefenrenceID =entity.RefenrenceID;
	    SurveyURL =entity.SurveyURL;
	    SurveyCate =entity.SurveyCate;
	    DeviceType =entity.DeviceType;
	    IsDeleted =entity.IsDeleted;
	    CreatedDate =entity.CreatedDate;
	    CreatedUserID =entity.CreatedUserID;
	    UpdatedDate =entity.UpdatedDate;
	    UpdatedUserID =entity.UpdatedUserID;
 
        return this;
        }
	}
}
