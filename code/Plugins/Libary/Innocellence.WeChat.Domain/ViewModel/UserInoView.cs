using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Contracts.ViewModel
{
    public class UserInfoView : IViewModel
    {
        public Int32 Id { get; set; }
        //public string OId { get; set; }
        //public string ActionId { get; set; }
        //public DateTime Time { get; set; }

        public string WeChatUserID { get; set; }
        public string Tel { get; set; }
       
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (UserInfo)obj;
            Id = entity.Id;
            WeChatUserID = entity.WeChatUserID;
            Tel = entity.Tel;
            return this;
        }
    }
}
