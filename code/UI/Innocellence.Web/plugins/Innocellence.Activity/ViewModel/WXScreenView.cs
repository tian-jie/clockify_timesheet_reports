using Infrastructure.Core;
using Innocellence.Activity.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Innocellence.Activity.ViewModel
{
    //[Table("News")]
    public class WXScreenView : IViewModel
    {
        public Int32 Id { get; set; }
        public string LillyId { get; set; }
      
        public Int32 AppId { get; set; }

        public DateTime? OperatedTime { get; set; }
        public Int32 EventId { get; set; }

        public string UserName { get; set; }
        public List<string> deptLvs { get; set; }

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

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (WXScreen)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            LillyId = entity.LillyId;
            OperatedTime = entity.OperatedTime;
            EventId = entity.EventId;
            return this;
        }
    }
}
