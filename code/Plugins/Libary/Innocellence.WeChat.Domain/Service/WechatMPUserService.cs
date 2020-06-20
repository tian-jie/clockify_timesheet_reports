using Innocellence.WeChat.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Entity;
using System.Linq.Expressions;
using Infrastructure.Core.Data;
using Innocellence.Weixin.MP.AdvancedAPIs.User;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.WeChat.Domain.ViewModelFront;
using Infrastructure.Core.Logging;
using Infrastructure.Web.UI;

namespace Innocellence.WeChat.Domain.Service
{
    public class WechatMPUserService : BaseService<WechatMPUser>, IWechatMPUserService
    {
        ILogger log = LogManager.GetLogger("WeChat");

        public int Create(WechatMPUser user)
        {
            user.IsCanceled = false;
            var result = Repository.Insert(user);
            return result;
        }
        public int CreatUsers(List<WechatMPUser> user)
        {
            Repository.SqlQuery("");
            return 1;
        }
        public int CleanTable()
        {
            Repository.SqlExcute("Delete from WechatMPUser");
            return 1;
        }
        public int Delete(int userId)
        {
            var result = Repository.Delete(userId);
            return result;
        }

        public int Update(WechatMPUser user)
        {
            var record = Repository.Entities.FirstOrDefault(u => user.OpenId == u.OpenId);
            if (record != null)
            {
                record.GroupId = user.GroupId;
                record.HeadImgUrl = user.HeadImgUrl;
                record.AccountManageId = user.AccountManageId;
                record.City = user.City;
                record.Country = user.Country;
                record.IsCanceled = user.IsCanceled;
                record.Language = user.Language;
                record.NickName = user.NickName;
                record.OpenId = user.OpenId;
                record.Province = user.Province;
                record.Remark = user.Remark;
                record.Sex = user.Sex;
                record.SubScribe = user.SubScribe;
                record.SubScribeTime = user.SubScribeTime;
                record.TagIdList = user.TagIdList;
                record.Timestamp = user.Timestamp;
                var result = Repository.Update(record);
                if (result > 0)
                {
                    return record.Id;
                }
            }
            return -1;
        }
        public void InsertUsers(List<WechatMPUserView> usersList)
        {
            if (usersList != null && usersList.Count > 0)
            {
                foreach (var user in usersList)
                {
                    try
                    {
                        var model = ConvertToWechatMPUser(user);
                        model.IsCanceled = false;
                        Repository.Insert(model);
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
        }
        private WechatMPUser ConvertToWechatMPUser(WechatMPUserView user)
        {
            return new WechatMPUser
            {
                Id = user.Id,
                City = user.City,
                Province = user.Province,
                Country = user.Country,
                GroupId = user.GroupId,
                HeadImgUrl = user.HeadImgUrl,
                IsCanceled = user.IsCanceled,
                Language = user.Language,
                NickName = user.NickName,
                OpenId = user.OpenId,
                Remark = user.Remark,
                Sex = user.Sex,
                SubScribe = user.SubScribe,
                SubScribeTime = user.SubScribeTime,
                TagIdList = user.TagIdList,
                UnionId = user.UnionId,
                AccountManageId = user.AccountManageId,
            };
        }
        public List<WechatMPUserView> GetUserByTagId(int groupId)
        {
            if (groupId < 0)
            {
                var ungroupUser = Repository.Entities.Where(a => !a.IsCanceled && (a.TagIdList == null || string.IsNullOrEmpty(a.TagIdList.Trim()))).ToList()
                                                     .Select(a => (WechatMPUserView)new WechatMPUserView().ConvertAPIModel(a)).ToList();
                return ungroupUser;
            }
            //replace split(',')
            string selectedGroupIdStr = groupId.ToString();
            string startStr = string.Format("{0},", selectedGroupIdStr);
            string endStr = string.Format(",{0}", selectedGroupIdStr);
            string containsStr = string.Format(",{0},", selectedGroupIdStr);
            var users = Repository.Entities.Where(u => !u.IsCanceled
                                                        && (u.TagIdList == null || string.IsNullOrEmpty(u.TagIdList.Trim()))
                                                        && (u.TagIdList.Equals(selectedGroupIdStr)
                                                                || u.TagIdList.StartsWith(startStr)
                                                                || u.TagIdList.EndsWith(endStr)
                                                                || u.TagIdList.Contains(containsStr)
                                                            )
                                                    ).ToList()
                                                    .Select(a => (WechatMPUserView)new WechatMPUserView().ConvertAPIModel(a)).ToList();
            return users;
        }
        //private WechatMPUserView ConvertToUserView(WechatMPUser user)
        //{
        //    return new WechatMPUserView
        //    {
        //        Id = user.Id,
        //        City = user.City,
        //        Province = user.Province,
        //        Country = user.Country,
        //        GroupId = user.GroupId,
        //        HeadImgUrl = user.HeadImgUrl,
        //        Language = user.Language,
        //        NickName = user.NickName,
        //        OpenId = user.OpenId,
        //        Remark = user.Remark,
        //        Sex = user.Sex,
        //        SubScribe = user.SubScribe,
        //        SubScribeTime = user.SubScribeTime,
        //        TagIdList = user.TagIdList,
        //        UnionId = user.UnionId,
        //        AccountManageId = user.AccountManageId,
        //    };
        //}

        public bool ChangeGroup(int groupId, string userOpenId)
        {
            var users = Repository.Entities.Where(u => !u.IsCanceled && u.OpenId != null && u.OpenId == userOpenId).ToList();
            foreach (var user in users)
            {
                if (groupId > 0)
                {
                    user.TagIdList = groupId.ToString();
                }
                else
                {
                    user.TagIdList = null;
                }
                Repository.Update(user, new List<string> { "TagIdList" });
            }
            if (users.Count > 0)
            {
                return true;
            }
            return false;
        }

        public bool UpdateRemark(string openId, string remark)
        {
            var users = Repository.Entities.Where(u => !u.IsCanceled && u.OpenId != null && u.OpenId == openId).ToList();
            foreach (var user in users)
            {
                user.Remark = remark;
                Repository.Update(user);
            }
            if (users.Count > 0)
            {
                return true;
            }
            return false;
        }

        public List<WechatMPUserView> GetUnGroupUserList(int AccountManageID)
        {
            var ungroupUser = Repository.Entities.Where(a => !a.IsCanceled && a.AccountManageId == AccountManageID && (a.TagIdList == null || string.IsNullOrEmpty(a.TagIdList.Trim()))).ToList()
                                                 .Select(a => (WechatMPUserView)new WechatMPUserView().ConvertAPIModel(a)).ToList();
            return ungroupUser;
        }

        public bool CancelRegist(string weixinOpenId)
        {
            var users = Repository.Entities.Where(u => !u.IsCanceled && u.OpenId != null && u.OpenId == weixinOpenId).ToList();
            foreach (var user in users)
            {
                log.Debug("change user to canceled:{0}, {1}", user.Id, user.NickName);
                user.IsCanceled = true;
                user.SubScribe = 0;
                user.UnSubScribeTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                Repository.Update(user);
            }
            if (users.Count > 0)
            {
                return true;
            }
            return false;
        }
        public bool BatchUntagging(List<string> openid)
        {
            StringBuilder sql = new StringBuilder(@"update WechatMPUser set [TagIdList]='' where [OpenId] in");
            sql.Append("('");
            sql.Append(string.Join("','", openid));
            sql.Append("')");
            Repository.SqlExcute(sql.ToString());
            return true;
        }

        public bool BatchUntagging(int groupId)
        {
            string selectedGroupIdStr = groupId.ToString();
            string startStr = selectedGroupIdStr + ",";
            string endStr = "," + selectedGroupIdStr;
            string containsStr = "," + selectedGroupIdStr + ",";
            var entities = Repository.Entities.Where(u => (u.TagIdList != null && !string.IsNullOrEmpty(u.TagIdList.Trim()))
                                                    && (u.TagIdList.Equals(selectedGroupIdStr)
                                                        || u.TagIdList.StartsWith(startStr)
                                                        || u.TagIdList.EndsWith(endStr)
                                                        || u.TagIdList.Contains(containsStr)
                                                    )).ToList();
            if (entities.Count>0)
            {
                entities.ForEach(a=>
                {
                    var tags = a.TagIdList.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    tags.Remove(groupId.ToString());
                    a.TagIdList = string.Join(",", tags);
                    Repository.Update(a, new List<string> { "TagIdList" });
                });
            }
            return true;
        }

        public List<WechatMPUserView> AllUsers(bool? isBooked)
        {
            var users = Repository.Entities.Where(u => isBooked == null ? true : u.IsCanceled != isBooked).ToList().Select(a => (WechatMPUserView)new WechatMPUserView().ConvertAPIModel(a)).ToList();
            return users;
        }
        public WechatMPUserView GetUserByOpenId(string openId)
        {
            var user = Repository.Entities.Where(u => openId == u.OpenId).OrderByDescending(u => u.Id).FirstOrDefault();
            return user == null ? null : (WechatMPUserView)new WechatMPUserView().ConvertAPIModel(user);
        }

        public List<WechatMPUserView> GetUserBySearchCondition(SearchUserMPView searchCondition, int AccountManageId)
        {
            if (searchCondition == null)
            {
                return new List<WechatMPUserView> { };
            }
            Expression<Func<WechatMPUser, bool>> predicate = GetExpressionBySearchUserMPView(searchCondition, AccountManageId);
            var users = Repository.Entities.Where(predicate).ToList().Select(a => (WechatMPUserView)new WechatMPUserView().ConvertAPIModel(a)).ToList();
            //bool cityNotSearch = string.IsNullOrWhiteSpace(searchCondition.City) || (!string.IsNullOrWhiteSpace(searchCondition.City) && searchCondition.City == "-1");
            //bool provinceNotSearch = string.IsNullOrWhiteSpace(searchCondition.Province) || (!string.IsNullOrWhiteSpace(searchCondition.Province) && searchCondition.Province == "-1");
            //var users = Repository.Entities.Where(u => (!u.IsCanceled)
            //                                        && (u.AccountManageId == AccountManageId)
            //                                        && ((searchCondition.Group != null && searchCondition.Group.Value > 0) ? u.TagIdList == searchCondition.Group.ToString() : true)
            //                                        && ((searchCondition.Sex != null && (searchCondition.Sex.Value == 1 || searchCondition.Sex.Value == 2 || searchCondition.Sex.Value == 0)) ? u.Sex == searchCondition.Sex.Value : true)
            //                                        && (cityNotSearch ? true : (u.City.Equals(searchCondition.City) || searchCondition.City.Contains(u.City)))
            //                                        && (provinceNotSearch ? true : (u.Province.Equals(searchCondition.Province) || searchCondition.Province.Contains(u.Province)))
            //                                        && (searchCondition.Group != null && searchCondition.Group.Value == -1) ? (u.TagIdList == null || string.IsNullOrEmpty(u.TagIdList)) : true)
            //                                    .ToList().Select(a => (WechatMPUserView)new WechatMPUserView().ConvertAPIModel(a)).ToList();

            return users;
        }

        private Expression<Func<WechatMPUser, bool>> GetExpressionBySearchUserMPView(SearchUserMPView searchCondition, int accountManageId)
        {
            Expression<Func<WechatMPUser, bool>> predicate = p => !p.IsCanceled && p.AccountManageId == accountManageId;
            bool cityNotSearch = string.IsNullOrWhiteSpace(searchCondition.City) || (!string.IsNullOrWhiteSpace(searchCondition.City) && searchCondition.City == "-1");
            bool provinceNotSearch = string.IsNullOrWhiteSpace(searchCondition.Province) || (!string.IsNullOrWhiteSpace(searchCondition.Province) && searchCondition.Province == "-1");
            //选择分组
            if (null != searchCondition.Group)
            {
                //-1:未分组, -2:所有用户
                if (searchCondition.Group.Value < 0)
                {
                    //未分组
                    if (searchCondition.Group.Value == -1)
                    {
                        predicate = predicate.AndAlso(u => u.TagIdList == null || string.IsNullOrEmpty(u.TagIdList.Trim()));
                    }
                }
                //选择指定分组
                else if (searchCondition.Group.Value > 0)
                {
                    predicate = predicate.AndAlso(user => user.TagIdList.Equals(searchCondition.Group.Value.ToString()));
                }
            }
            //选择城市
            if (!cityNotSearch)
            {
                predicate = predicate.AndAlso(user => searchCondition.City.Contains(user.City));
            }
            //选择省
            if (!provinceNotSearch)
            {
                predicate = predicate.AndAlso(user => searchCondition.Province.Contains(user.Province));
            }
            //-1:不限制性别, 1:男, 2:女, 0:未知性别
            if (null != searchCondition.Sex && searchCondition.Sex.Value > -1)
            {
                predicate = predicate.AndAlso(user => user.Sex == searchCondition.Sex.Value);
            }
            return predicate;
        }

        public List<WechatMPUserView> GetListByOpenIds(List<string> openIds)
        {
            var users = Repository.Entities.Where(u => !u.IsCanceled && openIds.Contains(u.OpenId)).ToList();
            var result = users.Select(a => (WechatMPUserView)new WechatMPUserView().ConvertAPIModel(a)).ToList();
            return result;
        }

        public void RegistToWeiXin(WechatMPUser userInfo)
        {
            if (userInfo != null)
            {
                log.Debug("RegistToWeiXin:{0}, {1}, {2}", userInfo.Id, userInfo.NickName, userInfo.IsCanceled ? "Cancel" : "Focus");
                int result = 0;
                if (userInfo.Id > 0)
                {
                    result = Repository.Update(userInfo);
                }
                else
                {
                    result = Repository.Insert(userInfo);
                }
                log.Debug("execute :{0}", result);
            }
        }
    }
}
