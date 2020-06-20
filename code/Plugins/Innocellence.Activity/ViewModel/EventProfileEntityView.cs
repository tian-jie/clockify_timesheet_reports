
using Infrastructure.Core;
using Innocellence.Activity.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Innocellence.Activity.ModelsView
{
    public partial class EventProfileEntityView : IViewModel
    {
        public int Id { get; set; }
        public string TypeCode { get; set; }
        [DescriptionAttribute("礼来Id")]
        public string UserId { get; set; }

        [DescriptionAttribute("访问时间")]
        public DateTime OperatedDateTime { get; set; }
        public int EventId { get; set; }

        /// <summary>
        /// avatar.
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [DescriptionAttribute("姓名")]
        public string UserName { get; set; }
        public List<string> deptLvs { get; set; }

        [DescriptionAttribute("一级部门")]
        public string deptLv1
        {
            get
            {
                if (deptLvs != null && deptLvs.Count() > 0)
                {
                    return deptLvs[2];
                }
                else
                {
                    return string.Empty;
                }
            }
            set { }
        }

        [DescriptionAttribute("二级部门")]
        public string deptLv2
        {
            get
            {
                if (deptLvs != null && deptLvs.Count() > 1)
                {
                    return deptLvs[3];
                }
                else
                {
                    return string.Empty;
                }
            }
            set { }
        }

        [DescriptionAttribute("三级部门")]
        public string deptLv3
        {
            get
            {
                if (deptLvs != null && deptLvs.Count() > 2)
                {
                    return deptLvs[4];
                }
                else
                {
                    return string.Empty;
                }
            }
            set { }
        }
        
        [DescriptionAttribute("邮箱")]
        public string Email { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsDisplay1 { get; set; }
        public bool? IsDisplay2 { get; set; }
        public bool? IsDisplay3 { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (EventProfileEntity)obj;
            Id = entity.Id;
            TypeCode = entity.TypeCode;
            UserId = entity.UserId;
            OperatedDateTime = entity.OperatedDateTime;
            EventId = entity.EventId;
            ImgUrl = entity.ImgUrl;
            UserName = entity.UserName;
            IsDeleted = entity.IsDeleted;
            IsDisplay1 = entity.IsDisplay1;
            IsDisplay2 = entity.IsDisplay2;
            IsDisplay3 = entity.IsDisplay3;

            return this;
        }
    }
}
