using System.Security.Policy;
using System.Web.UI.WebControls;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ModelsView
{
    public class QuestionManageView : IViewModel
    {

        public Int32 Id { get; set; }
        //应用ID
        public Int32? AppId { get; set; }
        //类别
        public string Category { get; set; }

        //提问者id
        public string CreatedUserId { get; set; }
        //提问者name
        public string QUserName { get; set; }

        //回答者id
        public string UpdatedUserId { get; set; }
        //回答者name
        public string AUsername { get; set; }

        //问题描述
        public string Question { get; set; }
        //问题答复
        public string Answer { get; set; }
        //提问时间
        public DateTime? CreatedDate { get; set; }
        //修改时间
        public DateTime? UpdatedDate { get; set; }
        //问题状态
        public string Status { get; set; }
        //阅读次数
        public Int32? ReadCount { get; set; }
        //删除标识
        public Boolean? IsDeleted { get; set; }
        public Int32? Satisfaction { get; set; }
        public string Tel { get; set; }
        public string EMail { get; set; }
        //QuestionManageView list
        public List<QuestionManageView> List { get; set; }
        //Question image list
        public List<QuestionImagesView> QuestionImages { get; set; }
        public string ImageIdList { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (QuestionManage)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            Category = entity.Category;
            CreatedUserId = entity.CreatedUserId;
            QUserName = entity.QUserName;
            UpdatedUserId = entity.UpdatedUserId;
            AUsername = entity.AUsername;
            Question = entity.Question;
            Answer = entity.Answer;
            CreatedDate = entity.CreatedDate;
            UpdatedDate = entity.UpdatedDate;
            Status = entity.Status;
            ReadCount = entity.ReadCount;
            IsDeleted = entity.IsDeleted;
            Satisfaction = entity.Satisfaction;
            return this;
        }
    }
}
