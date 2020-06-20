using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.Weixin.MP.AdvancedAPIs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innocellence.WeChat.Domain.ViewModelFront;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IWechatMPUserService : IDependency, IBaseService<WechatMPUser>
    {
        int Create(WechatMPUser user);
        int Delete(int userId);
        int Update(WechatMPUser user);
        int CleanTable();
        void InsertUsers(List<WechatMPUserView> usersList);
        List<WechatMPUserView> GetUserByTagId(int groupId);
        bool ChangeGroup(int groupId, string userOpenId);
        bool UpdateRemark(string openId, string remark);
        List<WechatMPUserView> GetUnGroupUserList(int AccountManageID);
        bool CancelRegist(string weixinOpenId);
        void RegistToWeiXin(WechatMPUser userInfo);
        List<WechatMPUserView> AllUsers(bool? isBooked);
        bool BatchUntagging(List<string> openid);
        bool BatchUntagging(int groupId);
        List<WechatMPUserView> GetUserBySearchCondition(SearchUserMPView searchCondition, int AccountManageId);
        List<WechatMPUserView> GetListByOpenIds(List<string> result);
        WechatMPUserView GetUserByOpenId(string openId);
    }
}
