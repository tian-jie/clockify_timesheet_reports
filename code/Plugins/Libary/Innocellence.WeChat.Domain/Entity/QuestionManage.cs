using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using System.ComponentModel.DataAnnotations;

namespace Innocellence.WeChat.Domain.Entity
{
    //[Table("Category")]
    /// <summary>

    /// </summary>
    public partial class QuestionManage : EntityBase<int>
    {
        public override Int32 Id { get; set; }
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
    }
}
