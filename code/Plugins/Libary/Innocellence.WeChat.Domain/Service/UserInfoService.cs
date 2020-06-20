using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Service
{
    public class UserInfoService : BaseService<UserInfo>, IUserInfoService
    {
        public UserInfoService()
           : base("CAAdmin")
        {


        }

       
        public UserInfo GetByWeChatUserID(string WeChatUserID)
        {

            Expression<Func<UserInfo, bool>> predicate = n => n.WeChatUserID == WeChatUserID;
            var t = Repository.Entities.Where(predicate).ToList().FirstOrDefault();
           
            return t;
        }


    }
}
