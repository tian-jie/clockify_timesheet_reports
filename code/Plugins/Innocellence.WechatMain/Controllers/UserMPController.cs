using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Core.Data;
using Infrastructure.Core.Caching;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Services;
using Infrastructure.Web.Domain.Services;
using Infrastructure.Core.Infrastructure;
using Innocellence.WeChat.Domain.Service;
using Autofac;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.MP.AdvancedAPIs.User;
using Innocellence.Weixin.MP.AdvancedAPIs.UserTag;
using Innocellence.Weixin;
using Infrastructure.Utility.Data;
using System.Linq.Expressions;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.ViewModelFront;
using Innocellence.WeChat.Domain;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Infrastructure.Web.Domain.Service;
using Newtonsoft.Json;

namespace Innocellence.WeChatMain.Controllers
{
    public class UserMPController : BaseController<WechatMPUser, WechatMPUserView>
    {
        private ISystemUserTagService _SystemUserTagService;
        private ISysWechatConfigService _SysWechatConfigService;
        private ISystemUserTagMappingService _SystemUserTagMappingService;
        private IWechatMPUserService _WechatMPUserService;
        private IQrCodeService _QrCodeService;
        private IFocusHistoryService _FocusHistoryService;
        public UserMPController(IWechatMPUserService newsService,
            ISysWechatConfigService sysWechatConfigService,
            ISystemUserTagMappingService SystemUserTagMappingService,
            ISystemUserTagService SystemUserTagService,
                  ISysWechatConfigService SysWechatConfigService,
                  IWechatMPUserService WechatMPUserService,
                  IFocusHistoryService FocusHistoryService,
                  IQrCodeService QrCodeService)
            : base(newsService)
        {
            _SystemUserTagMappingService = SystemUserTagMappingService;
            _WechatMPUserService = newsService;
            _SysWechatConfigService = SysWechatConfigService;
            _SystemUserTagService = SystemUserTagService;
            _WechatMPUserService = WechatMPUserService;
            _QrCodeService = QrCodeService;
            _FocusHistoryService = FocusHistoryService;
        }
        #region  main page actions
        /// <summary>
        /// MainView loaded
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="tabName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GroupList(string searchString, int? selectedGroupId, int? Type, PageCondition ConPage)
        {
            return View();
        }
        /// <summary>
        /// main function to get user info 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetListOld()
        {
            PageParameter param = new PageParameter();
            TryUpdateModel(param);
            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            param.length = param.length == 0 ? 10 : param.length;
            int iTotal = param.iRecordsTotal;

            var list = GetListData((int)Math.Floor(param.start / param.length * 1.0d) + 1, param.length, ref iTotal);// _newsService.GetList(null, null, 10, 10);
            if (list == null)
            {
                list = new List<WechatMPUserView>();
            }
            var groups = GetAllGroups();
            list.ForEach(g =>
            {
                g.GroupList = groups;
            });
            return Json(new
            {
                sEcho = param.draw,
                iTotalRecords = iTotal,
                iTotalDisplayRecords = iTotal,
                aaData = list
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// get user info with page info
        /// </summary>
        /// <param name="iPage"></param>
        /// <param name="iCount"></param>
        /// <param name="iTotal"></param>
        /// <returns></returns>
        public List<WechatMPUserView> GetListData(int iPage, int iCount, ref int iTotal)
        {
            int selectedGroupId = int.Parse(Request["selectedGroupId"]);
            string selectedType = Request["selectedType"];
            var test = Request.Params.Keys;

            var list = new List<WechatMPUserView> { };
            PageCondition page = new PageCondition { PageIndex = iPage, PageSize = iCount };
            if (!string.IsNullOrEmpty(selectedType) && string.Equals(selectedType, "all"))
            {
                list = _WechatMPUserService.AllUsers(true);
            }
            else if (!string.IsNullOrEmpty(selectedType) && string.Equals(selectedType, "cancled"))
            {
                list = _WechatMPUserService.AllUsers(false);
            }
            else if (!string.IsNullOrEmpty(selectedType) && string.Equals(selectedType, "grouped"))
            {
                list = _WechatMPUserService.GetUserByTagId(selectedGroupId);
            }
            list = list.Where(a => AccountManageID == a.AccountManageId).ToList();
            string searchUserName = Request["SearchUserName"];
            if (!string.IsNullOrEmpty(searchUserName))
            {
                list = list.Where(a => a.NickName.Contains(searchUserName)).ToList();
            }
            List<int> tagList = new List<int> { };
            foreach (var key in Request.Params.Keys)
            {
                if (key.ToString().StartsWith("SelectedUserTag", StringComparison.OrdinalIgnoreCase))
                {
                    int i = 0;
                    string tagid = Request[key.ToString()];
                    int.TryParse(tagid, out i);
                    tagList.Add(i);
                }
            }
            if (tagList.Count > 0)
            {
                var userIds = _SystemUserTagMappingService.GetMappingByTagIds(tagList).Select(a => a.UserOpenid).Distinct().ToList();
                list = list.Where(a => userIds.Contains(a.OpenId)).ToList();
            }
            iTotal = list.Count();
            return list.Skip((iPage - 1) * iCount).Take(iCount).ToList();
        }

        public override List<WechatMPUserView> GetListEx(Expression<Func<WechatMPUser, bool>> predicate, PageCondition con)
        {
            int selectedGroupId = int.Parse(Request["selectedGroupId"]);
            string selectedType = Request["selectedType"];
            predicate = predicate.AndAlso(u => u.AccountManageId == AccountManageID);
            if (!string.IsNullOrEmpty(selectedType) && string.Equals(selectedType, "all"))
            {
                predicate = predicate.AndAlso(u => u.IsCanceled == false);
            }
            else if (!string.IsNullOrEmpty(selectedType) && string.Equals(selectedType, "cancled"))
            {
                predicate = predicate.AndAlso(u => u.IsCanceled == true);
            }
            else if (!string.IsNullOrEmpty(selectedType) && string.Equals(selectedType, "grouped"))
            {
                predicate = predicate.AndAlso(u => u.IsCanceled == false);
                if (selectedGroupId < 0)
                {
                    predicate = predicate.AndAlso(u => u.TagIdList == null || string.IsNullOrEmpty(u.TagIdList.Trim()));
                }
                else
                {
                    string selectedGroupIdStr = Request["selectedGroupId"];
                    string startStr = string.Format("{0},", selectedGroupIdStr);
                    string endStr = string.Format(",{0}", selectedGroupIdStr);
                    string containsStr = string.Format(",{0},", selectedGroupIdStr);
                    predicate = predicate.AndAlso(u => (u.TagIdList != null && !string.IsNullOrEmpty(u.TagIdList.Trim()))
                                                        && (u.TagIdList.Equals(selectedGroupIdStr)
                                                            || u.TagIdList.StartsWith(startStr)
                                                            || u.TagIdList.EndsWith(endStr)
                                                            || u.TagIdList.Contains(containsStr)
                                                        )
                                                 );
                    //predicate = predicate.AndAlso(u => string.Equals(selectedGroupId, u.TagIdList));
                }
            }
            string searchUserName = Request["SearchUserName"];
            if (!string.IsNullOrEmpty(searchUserName))
            {
                predicate = predicate.AndAlso(u => u.NickName.Contains(searchUserName));
            }
            List<int> tagList = new List<int> { };
            foreach (var key in Request.Params.Keys)
            {
                if (key.ToString().StartsWith("SelectedUserTag", StringComparison.OrdinalIgnoreCase))
                {
                    int i = 0;
                    string tagid = Request[key.ToString()];
                    int.TryParse(tagid, out i);
                    tagList.Add(i);
                }
            }
            if (tagList.Count > 0)
            {
                var userIds = _SystemUserTagMappingService.GetMappingByTagIds(tagList).Select(a => a.UserOpenid).Distinct().ToList();
                predicate = predicate.AndAlso(u => userIds.Contains(u.OpenId));
            }
            var list = _WechatMPUserService.GetList<WechatMPUserView>(predicate, con);
            if (null != list && list.Count > 0)
            {
                var groups = GetAllGroups();
                list.ForEach(g =>
                {
                    g.GroupList = groups;
                    g.TagIdList = string.IsNullOrEmpty(g.TagIdList) ? string.Empty : g.TagIdList.Split(',')[0];
                });
            }
            return list;
        }

        /// <summary>
        /// return values to pages to insert group
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllGroupList()
        {
            var groups = GetAllGroups();
            int allFollowedUserCount = _BaseService.Repository.Entities.Count(user => user.AccountManageId == this.AccountManageID && user.IsCanceled == false);
            int allCanceledUserCount = _BaseService.Repository.Entities.Count(user => user.AccountManageId == this.AccountManageID && user.IsCanceled == true);
            return Json(new { data = groups, allFollowedUserCount = allFollowedUserCount, allCanceledUserCount = allCanceledUserCount }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Change user Group to another group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userOpenId"></param>
        /// <param name="regionGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ChangeGroup(int groupId, string userOpenId)
        {
            var chat = GetWechatConfig();
            int regionGroupId = 0;
            if (chat == null)
            {
                return GetResultJason(208, null, "Not service apllication");
            }
            var user = _WechatMPUserService.GetListByOpenIds(new List<string> { userOpenId }).FirstOrDefault();
            int.TryParse(user.TagIdList, out regionGroupId);
            if (regionGroupId > 0)
            {
                var untagResult = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.BatchUntagging(chat.WeixinAppId, chat.WeixinCorpSecret, regionGroupId, new List<string> { userOpenId });
                if (untagResult != null && untagResult.errcode != ReturnCode.请求成功)
                {
                    return GetResultJason(201, null, untagResult.errmsg);
                }
            }
            if (groupId > 0)
            {
                var tagResult = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.BatchTagging(chat.WeixinAppId, chat.WeixinCorpSecret, groupId, new List<string> { userOpenId });
                if (tagResult != null && tagResult.errcode != ReturnCode.请求成功)
                {
                    return GetResultJason(201, null, tagResult.errmsg);
                }
            }
            bool result = _WechatMPUserService.ChangeGroup(groupId, userOpenId);
            if (!result)
            {
                return GetResultJason(202, null, "Error in changeGroup in database");
            }

            return GetResultJason(200, null, "sucessful");
        }
        #endregion

        #region admin
        /// <summary>
        /// clean groups: for test unexpected user belongs to different group
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public JsonResult CleanGroups(string password)
        {
            if (string.Equals(password, "yyyhaha"))
            {
                _WechatMPUserService.CleanTable();
                var chat = GetWechatConfig();
                if (chat == null)
                {
                    return GetResultJason(208, null, "Not service apllication");
                }
                var returnValue = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.Get(chat.WeixinAppId, chat.WeixinCorpSecret);
                if (returnValue != null && returnValue.tags != null)
                {
                    foreach (var group in returnValue.tags)
                    {
                        var returnValue1 = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.Get(chat.WeixinAppId, chat.WeixinCorpSecret, group.id);
                        if (returnValue1 != null && returnValue1.data.openid != null)
                        {
                            Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.BatchUntagging(chat.WeixinAppId, chat.WeixinCorpSecret, group.id, returnValue1.data.openid);
                        }
                    }
                }
            }

            WeChatCommonService.CleanGroupCache(this.AccountManageID);
            return Json(new { Status = 201 }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// function to clean and inport all users
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public JsonResult CleanTableAndExportAllUsers(string password)
        {
            if (string.Equals(password, "yyyhaha"))
            {
                _WechatMPUserService.CleanTable();
                var chat = GetWechatConfig();
                if (chat == null)
                {
                    return GetResultJason(208, null, "Not service apllication");
                }
                string nextOpenId = "";
                do
                {
                    var returnValue = Innocellence.Weixin.MP.AdvancedAPIs.UserApi.Get(chat.WeixinAppId, chat.WeixinCorpSecret, nextOpenId);
                    if (returnValue != null && returnValue.data != null && returnValue.data.openid != null)
                    {
                        var userInfoList = new List<Weixin.MP.AdvancedAPIs.User.BatchGetUserInfoData> { };
                        foreach (var openId in returnValue.data.openid)
                        {
                            if (!string.IsNullOrWhiteSpace(openId))
                            {
                                userInfoList.Add(new BatchGetUserInfoData { openid = openId, lang = Language.zh_CN.ToString() });
                            }
                        }
                        var result = Innocellence.Weixin.MP.AdvancedAPIs.UserApi.BatchGetUserInfo(chat.WeixinAppId, chat.WeixinCorpSecret, userInfoList);
                        if (result != null && result != null)
                        {
                            var resultList = result.user_info_list.Select(a => WechatMPUserView.ConvertWeChatUserToMpUser(a, chat.AccountManageId.Value, chat.Id)).ToList();
                            _WechatMPUserService.Repository.Insert(resultList);
                        }
                    }
                } while (!string.IsNullOrWhiteSpace(nextOpenId));
            }

            return Json(new { Status = 201 }, JsonRequestBehavior.AllowGet);
        }


        #endregion 

        #region Group Manage 
        /// <summary>
        /// add group to weixin api
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public JsonResult AddGroup(string groupName)
        {
            var chat = GetWechatConfig();
            if (chat == null)
            {
                return GetResultJason(208, null, "Not service apllication");
            }
            var group = WeChatCommonService.GetAllGroupFromServer(AccountManageID).Where(a => a.name == groupName).FirstOrDefault();
            if (group != null)
            {
                return GetResultJason(206, null, "组名称已存在");
            }
            try
            {
                var returnValue = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.Create(chat.WeixinAppId, chat.WeixinCorpSecret, groupName);
                if (returnValue != null && returnValue.errcode == ReturnCode.请求成功)
                {
                    WeChatCommonService.CleanGroupCache(this.AccountManageID);
                    return GetResultJason(200, null, returnValue.errmsg);
                }
                else if (returnValue != null)
                {
                    return GetResultJason(201, null, returnValue.errmsg);
                }
                return GetResultJason(202, null, "Unexcept error");
            }
            catch (Exception e)
            {
                return GetResultJason(205, null, "API 异常" + e);
            }

        }
        /// <summary>
        /// delete group in weixin api
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public JsonResult DeleteGroup(int groupId)
        {
            var chat = GetWechatConfig();
            if (chat == null)
            {
                return GetResultJason(208, null, "Not service apllication");
            }

            var getUsersInTag = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.Get(chat.WeixinAppId, chat.WeixinCorpSecret, groupId);
            if (getUsersInTag != null && getUsersInTag.data != null && getUsersInTag.data.openid != null)
            {
                Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.BatchUntagging(chat.WeixinAppId, chat.WeixinCorpSecret, groupId, getUsersInTag.data.openid);
                bool value = _WechatMPUserService.BatchUntagging(getUsersInTag.data.openid);
            }
            var returnValue = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.Delete(chat.WeixinAppId, chat.WeixinCorpSecret, groupId);
            if (returnValue != null && returnValue.errcode == ReturnCode.请求成功)
            {
                WeChatCommonService.CleanGroupCache(this.AccountManageID);
                return GetResultJason(200, null, returnValue.errmsg);
            }
            return GetResultJason(202, null, "Unexcept error");
        }

        /// <summary>
        /// Update group in weixin api
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public JsonResult UpdateGroupName(int groupId, string groupName)
        {
            var chat = GetWechatConfig();
            if (chat == null)
            {
                return GetResultJason(208, null, "Not service apllication");
            }
            var returnValue = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.Update(chat.WeixinAppId, chat.WeixinCorpSecret, groupId, groupName);
            if (returnValue != null && returnValue.errcode == ReturnCode.请求成功)
            {
                WeChatCommonService.CleanGroupCache(this.AccountManageID);
                return GetResultJason(200, null, returnValue.errmsg);
            }
            return GetResultJason(202, null, "Unexcept error");
        }

        /// <summary>
        /// get all groups include 未分组 id=-1
        /// </summary>
        /// <returns></returns>
        private List<GroupTagView> GetAllGroups()
        {
            var allGroup = WeChatCommonService.GetAllGroupFromServer(AccountManageID).Select(a => ConvertToGroupTagView(a)).ToList();
            List<GroupTagView> groups = new List<GroupTagView> { };
            var groupCountList = _WechatMPUserService.Repository.Entities.Where(a => a.IsCanceled != true && a.AccountManageId == AccountManageID).GroupBy(a => a.TagIdList).Select(g => new { Key = "," + g.Key + ",", Count = g.Count() }).ToList();
            foreach (var group in allGroup)
            {
                string selectedGroupIdStr = group.Id.ToString();
                //string startStr = selectedGroupIdStr + ",";
                //string endStr = "," + selectedGroupIdStr;
                string containsStr = "," + selectedGroupIdStr + ",";
                var groupCount = groupCountList.Where(u => (u.Key != null && !string.IsNullOrEmpty(u.Key.Trim()))
                                                        && (//u.Key.Equals(selectedGroupIdStr)
                                                            //|| u.Key.StartsWith(startStr)
                                                            //|| u.Key.EndsWith(endStr)
                                                            //|| 
                                                            u.Key.Contains(containsStr)
                                                        )).Select(a => a.Count).Sum();
                group.count = groupCount;
            }
            var ungroupUser = _WechatMPUserService.GetUnGroupUserList(AccountManageID);
            groups.Add(new GroupTagView { Id = -1, name = "未分组", count = ungroupUser.Count });
            groups.AddRange(allGroup);
            return groups;
        }
        //private WechatMPUserView ConvertToWechatMPUserView(UserInfoJson model)
        //{
        //    return new WechatMPUserView
        //    {
        //        City = model.city,
        //        Country = model.country,
        //        Province = model.province,
        //        GroupId = model.groupid,
        //        HeadImgUrl = model.headimgurl,
        //        Language = model.language,
        //        NickName = model.nickname,
        //        OpenId = model.openid,
        //        Remark = model.remark,
        //        Sex = model.sex,
        //        SubScribe = model.subscribe,
        //        SubScribeTime = model.subscribe_time,
        //        TagIdList = string.Join(",", model.tagid_list),
        //        UnionId = model.unionid,
        //        AccountManageId = AccountManageID,
        //    };
        //}

        /// <summary>
        /// an convert method
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private GroupTagView ConvertToGroupTagView(TagJson_Tag group)
        {
            return new GroupTagView
            {
                Id = group.id,
                name = group.name,
                count = group.count
            };
        }
        #endregion

        #region User Manage
        /// <summary>
        /// change user remark
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public JsonResult ChangeUserRemark(string openId, string remark)
        {
            var chat = GetWechatConfig();
            if (chat == null)
            {
                return GetResultJason(208, null, "Not service apllication");
            }
            var returnValue = Innocellence.Weixin.MP.AdvancedAPIs.UserApi.UpdateRemark(chat.WeixinAppId, chat.WeixinCorpSecret, openId, remark);
            if (returnValue != null && returnValue.errcode != ReturnCode.请求成功)
            {
                return GetResultJason(201, null, returnValue.errmsg);
            }
            var databaseresult = _WechatMPUserService.UpdateRemark(openId, remark);
            if (!databaseresult)
            {
                return GetResultJason(201, null, "failed remark in database");
            }
            return GetResultJason(200, null, "sucessfull");
        }
        /// <summary>
        /// Syc User from server to database
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public JsonResult SycUserToDB(string openId)
        {
            //添加IsCancel判断,对于取消关注的用户直接从DB 中读取信息.
            var userInDB = _WechatMPUserService.GetUserByOpenId(openId);
            if (null != userInDB) //修改业务逻辑,所有用户数据均从DB中读取
            {
                userInDB = AssembleMPUserView(userInDB, openId);
                return GetResultJason(200, userInDB, "sucessfull");
            }
            else
            {
                return GetResultJason(201, null, "用户不存在.");
            }
            var chat = GetWechatConfig();
            if (chat == null)
            {
                return GetResultJason(208, null, "Not service apllication");
            }
            BatchGetUserInfoData user = new BatchGetUserInfoData { openid = openId, lang = Language.zh_CN.ToString() };
            var returnValue = Innocellence.Weixin.MP.AdvancedAPIs.UserApi.BatchGetUserInfo(chat.WeixinAppId, chat.WeixinCorpSecret, new List<BatchGetUserInfoData> { user });
            if (returnValue != null && returnValue.errcode == ReturnCode.请求成功 && returnValue.user_info_list != null)
            {
                var userTobeSave = returnValue.user_info_list.Select(u => WechatMPUserView.ConvertWeChatUserToMpUser(u, chat.AccountManageId.Value, chat.Id)).ToList().FirstOrDefault();
                int userid = _WechatMPUserService.Update(userTobeSave);
                userTobeSave.Id = userid;
                var userview = (WechatMPUserView)new WechatMPUserView().ConvertAPIModel(userTobeSave); // ConvertWechatMPUserToWechatMPUserView(userTobeSave);
                userview = AssembleMPUserView(userview, openId);
                if (null != userInDB)
                {
                    userview.CustomerNO = userInDB.CustomerNO;
                    userview.UnSubScribeTime = userInDB.UnSubScribeTime;
                }
                return GetResultJason(200, userview, "sucessfull");
            }
            else
            {
                return GetResultJason(201, null, returnValue.errmsg);
            }
        }

        [AllowAnonymous]
        public ActionResult UpdateUserCustomerNO(string openId, string customerNO)
        {
            try
            {
                _Logger.Debug("openId:{0}, customerNO:{1}", openId, customerNO);
                var user = _WechatMPUserService.Repository.Entities.Where(u => u.OpenId.Equals(openId, StringComparison.CurrentCultureIgnoreCase) && u.IsCanceled == false).FirstOrDefault();
                if (user != null)
                {
                    user.CustomerNO = customerNO;
                    user.CustomerRegisteredTime = DateTime.Now;
                    int result = _WechatMPUserService.Repository.Update(user);
                    _Logger.Debug("user:{0}, updated: {1}", user.NickName, result);
                    if (result > 0)
                    {
                        if (!string.IsNullOrEmpty(user.UnionId))
                        {
                            WebServiceAgent agent = new WebServiceAgent(CommonService.GetSysConfig("WentangshequService", "http://120.92.191.39:9012/Service1.asmx"));
                            dynamic bindResult = agent.Invoke("MinaBind", openId, user.UnionId);
                            _Logger.Debug(bindResult);
                        }
                        else
                        {
                            _Logger.Error("unionId is null:{0}, {1}", user.NickName, user.Id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }
            return SuccessNotification("Success");
        }

        [AllowAnonymous]
        public JsonResult GetLastestTagIdByOpenId(string openId)
        {
            string tagId = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(openId))
                {
                    _Logger.Debug("openId:{0}", openId);
                    SystemUserTagMappingService systemUserTagMappingService = new SystemUserTagMappingService();
                    var tag = systemUserTagMappingService.GetUserTagByOpenId(openId).OrderByDescending(t => t.Id).FirstOrDefault();
                    if (null != tag)
                    {
                        List<dynamic> test = typeof(AvatorTagIdMapping).GetAllItems();
                        dynamic enumObj = test.FirstOrDefault(e => tag.TagId.ToString().Equals(e.Value.ToString()));
                        if (null != enumObj && !string.IsNullOrEmpty(enumObj.Text))
                        {
                            string enumName = enumObj.Text.ToString();
                            tagId = enumName.Split('_')[1];
                        }
                    }
                    _Logger.Debug("{0}'s lastest tag id is :{1}", openId, tagId);
                }
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }
            return Json(new { TagId = tagId }, JsonRequestBehavior.AllowGet);
        }





        [AllowAnonymous]
        public ActionResult GetSimUid(string openId)
        {
            var user = _WechatMPUserService.Repository.Entities.Where(u => u.OpenId.Equals(openId, StringComparison.CurrentCultureIgnoreCase) && u.IsCanceled == false).FirstOrDefault();
            if (user != null)
            {
                return Content(user.SimUID);
                _Logger.Debug("user:{0}, SimUID: {1}", user.OpenId, user.SimUID);
            }
            return Content(null);
        }

        private WechatMPUserView AssembleMPUserView(WechatMPUserView view, string openId)
        {
            var groupName = WeChatCommonService.GetAllGroupFromServer(AccountManageID).Where(a => a.id.ToString() == view.TagIdList).Select(a => a.name).ToList().FirstOrDefault();
            view.TagName = string.IsNullOrWhiteSpace(groupName) ? "" : groupName;
            var userTagIds = _SystemUserTagMappingService.GetUserTagByOpenId(openId).Select(a => a.TagId).ToList();
            var tagNames = _SystemUserTagService.GetAllTags().Where(t => userTagIds.Contains(t.Id)).Select(t => t.Name);
            var userTagStr = string.Join(",", tagNames);
            view.UserTagStr = string.IsNullOrWhiteSpace(userTagStr) ? "" : userTagStr;
            view.LocationDisplayStr = string.Join(", ", view.Province, view.City).Trim().Trim(',');
            return view;
        }
        //private WechatMPUserView ConvertWechatMPUserToWechatMPUserView(WechatMPUser model)
        //{
        //    return new WechatMPUserView
        //    {
        //        City = model.City,
        //        Country = model.Country,
        //        Province = model.Province,
        //        GroupId = model.GroupId,
        //        HeadImgUrl = model.HeadImgUrl,
        //        Language = model.Language,
        //        NickName = model.NickName,
        //        OpenId = model.OpenId,
        //        Remark = model.Remark,
        //        Sex = model.Sex,
        //        SubScribe = model.SubScribe,
        //        SubScribeTime = model.SubScribeTime,
        //        TagIdList = model.TagIdList,
        //        UnionId = model.UnionId,
        //        AccountManageId = model.AccountManageId,

        //    };
        //}

        //private WechatMPUser ConvertToWechatMPUser(UserInfoJson model)
        //{
        //    return new WechatMPUser
        //    {
        //        City = model.city,
        //        Country = model.country,
        //        Province = model.province,
        //        GroupId = model.groupid,
        //        HeadImgUrl = model.headimgurl,
        //        Language = model.language,
        //        NickName = model.nickname,
        //        OpenId = model.openid,
        //        Remark = model.remark,
        //        Sex = model.sex,
        //        SubScribe = model.subscribe,
        //        SubScribeTime = model.subscribe_time,
        //        TagIdList = string.Join(",", model.tagid_list),
        //        UnionId = model.unionid,
        //        AccountManageId = AccountManageID,
        //    };
        //}
        #endregion

        #region User Tag related 
        /// <summary>
        /// full page user tag
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUserTagList()
        {
            var userFirstLeveltag = _SystemUserTagService.GetFirstLevelTag().Where(t => t.AccountManageId == AccountManageID);
            var allTag = _SystemUserTagService.GetAllTags().Where(t => t.AccountManageId == AccountManageID);
            var firstleveKeys = userFirstLeveltag.Select(a => a.Id).ToList();
            Dictionary<int, List<SystemUserTag>> dic = new Dictionary<int, List<SystemUserTag>> { };
            List<KeyValuePair<SystemUserTag, List<SystemUserTag>>> userTagMapDic = new List<KeyValuePair<SystemUserTag, List<SystemUserTag>>> { };
            foreach (var f in userFirstLeveltag)
            {
                var secondLevelTag = new List<SystemUserTag> { };
                foreach (var s in allTag)
                {
                    if (s.ParentId == f.Id)

                        secondLevelTag.Add(s);
                }
                userTagMapDic.Add(new KeyValuePair<SystemUserTag, List<SystemUserTag>>(f, secondLevelTag));

            }
            return Json(new { data = userTagMapDic, }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// //create tag, tag group
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public JsonResult AddUserTag(string tagName, int? parentId)
        {
            var tag = _SystemUserTagService.Repository.Entities.Where(a => a.Name.Equals(tagName, StringComparison.CurrentCultureIgnoreCase)
                && a.ParentId == parentId && a.IsDeleted == false).FirstOrDefault();
            if (tag != null)
            {
                return GetResultJason(206, null, "标签已存在");
                //throw new Exception("标签已存在");
            }
            var result = _SystemUserTagService.Create(new SystemUserTag { Name = tagName, ParentId = parentId, AccountManageId = AccountManageID });

            return Json(new { Status = result > 0 ? 200 : 201, TagId = result }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// update tag ,tag group
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public JsonResult EditUserTagName(string tagName, int tagId)
        {
            var oldTag = _SystemUserTagService.Repository.Entities.Where(a => a.Id == tagId).FirstOrDefault();
            var tag = _SystemUserTagService.Repository.Entities.Where(a => a.Name.Equals(tagName, StringComparison.CurrentCultureIgnoreCase)
    && a.ParentId == oldTag.ParentId && a.IsDeleted == false && a.Id != oldTag.Id).FirstOrDefault();
            if (tag != null)
            {
                return GetResultJason(206, null, "标签已存在");
            }
            var result = _SystemUserTagService.EditTagName(tagName, tagId);
            return Json(new { Status = result > 0 ? 200 : 201, TagId = result }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// delete tag,tag group
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public JsonResult DeleteUserTag(int? tagId)
        {
            var tags = _SystemUserTagService.GetTagByParentId(tagId == null ? 0 : tagId.Value);
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    _SystemUserTagMappingService.DeleteByTag(tag.Id);
                    _SystemUserTagService.Delete(tag.Id);
                }
            }
            var result = _SystemUserTagMappingService.DeleteByTag(tagId.Value);
            var result1 = _SystemUserTagService.Delete(tagId.Value);
            return Json(new { Status = result1 > 0 ? 200 : 201, }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// add tags to user by openId
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="tagIdstr"></param>
        /// <returns></returns>
        public JsonResult AddUserTagToUser(string openId, string tagIdstr)
        {
            var tagIds = tagIdstr.Split(',').Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
            if (string.IsNullOrWhiteSpace(openId))
            {
                return Json(new { Status = 201, }, JsonRequestBehavior.AllowGet);
            }
            var result = _SystemUserTagMappingService.GetUserTagByOpenId(openId);
            var existingTagIds = result.Select(a => a.TagId.ToString()).ToList();
            foreach (var id in tagIds)
            {
                if (!existingTagIds.Contains(id))
                {
                    int tagId = 0;
                    int.TryParse(id, out tagId);
                    if (tagId > 0)
                    {
                        _SystemUserTagMappingService.Create(new SystemUserTagMapping { UserOpenid = openId, TagId = tagId });
                    }
                }
            }
            foreach (var exist in result)
            {
                if (!tagIds.Contains(exist.TagId.ToString()))
                {
                    _SystemUserTagMappingService.Delete(exist.Id);
                }
            }
            return Json(new { Status = 200 }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// get all tags for user add or delete selected tag, including user selected info
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public JsonResult GetUserSelectedTagList(string openId)
        {
            if (string.IsNullOrWhiteSpace(openId))
            {
                return Json(new { Status = 201, }, JsonRequestBehavior.AllowGet);
            }
            var result = _SystemUserTagMappingService.GetUserTagByOpenId(openId);
            var existingTagIds = result.Select(a => a.TagId).ToList();
            var userFirstLeveltag = _SystemUserTagService.GetFirstLevelTag().Where(t => t.AccountManageId == AccountManageID);
            var allTag = _SystemUserTagService.GetAllTags().Where(t => t.AccountManageId == AccountManageID);
            var firstleveKeys = userFirstLeveltag.Select(a => a.Id).ToList();
            Dictionary<int, List<SystemUserTag>> dic = new Dictionary<int, List<SystemUserTag>> { };
            List<KeyValuePair<SystemUserTag, List<SystemUserTagView>>> userTagMapDic = new List<KeyValuePair<SystemUserTag, List<SystemUserTagView>>> { };
            foreach (var f in userFirstLeveltag)
            {
                var secondLevelTag = new List<SystemUserTagView> { };
                foreach (var s in allTag)
                {
                    if (s.ParentId == f.Id)
                    {
                        SystemUserTagView view = ConvertToUserTagView(s);
                        if (existingTagIds.Contains(s.Id))
                        {
                            view.IsSelected = true;
                        }
                        secondLevelTag.Add(view);
                    }

                }
                userTagMapDic.Add(new KeyValuePair<SystemUserTag, List<SystemUserTagView>>(f, secondLevelTag));

            }
            return Json(new { data = userTagMapDic }, JsonRequestBehavior.AllowGet);
        }
        private SystemUserTagView ConvertToUserTagView(SystemUserTag s)
        {
            return new SystemUserTagView { Id = s.Id, ParentId = s.ParentId, IsDeleted = s.IsDeleted, Name = s.Name };
        }
        #endregion

        /// <summary>
        /// Search in db a function used by message 
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUserCountBySearchCondition(SearchUserMPView searchCondition)
        {
            if (searchCondition != null)
            {
                Expression<Func<WechatMPUser, bool>> predicate = GetExpressionBySearchUserMPView(searchCondition);
                var count = _WechatMPUserService.Repository.Entities.Count(predicate);
                return GetResultJason(200, count, "sucessfull");
            }
            _Logger.Error("search condition is null.");
            return GetResultJason(500, 0, "failed");
        }

        public JsonResult SearchUserByName(string namePattern)
        {
            if (!string.IsNullOrEmpty(namePattern))
            {
                var users = _WechatMPUserService.Repository.Entities.Where(u => u.NickName.ToUpper().Contains(namePattern.ToUpper()) && u.IsCanceled == false && u.AccountManageId == this.AccountManageID).OrderBy(u => u.NickName.Length).Take(10).ToList();
                return GetResultJason(200, users, null);
            }
            return GetResultJason(200, new List<WechatMPUser>(), null);
        }

        private Expression<Func<WechatMPUser, bool>> GetExpressionBySearchUserMPView(SearchUserMPView searchCondition)
        {
            Expression<Func<WechatMPUser, bool>> predicate = p => !p.IsCanceled && p.AccountManageId == AccountManageID;
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

        private SysWechatConfig GetWechatConfig()
        {
            var sysWechatConfig = WeChatCommonService.lstSysWeChatConfig.Find(p => p.AccountManageId == AccountManageID && !p.IsCorp.Value);

            if (sysWechatConfig != null)
            {
                return sysWechatConfig;
            }
            return null;
        }
        private JsonResult GetResultJason(int status, object data, string message)
        {
            return new JsonResult { Data = new { Status = status, Data = data, Message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult UpdateUserUnionId(string ticket)
        {
            if (!"1qaz2wsxEDemo12!@".Equals(ticket, StringComparison.OrdinalIgnoreCase))
            {
                return ErrorNotification("ticket is wrong");
            }
            List<string> openIds = this._BaseService.Repository.Entities.Where(s => s.IsCanceled == false && string.IsNullOrEmpty(s.UnionId) && s.AccountManageId.HasValue && this.AccountManageID == s.AccountManageId.Value).Select(s => s.OpenId).ToList();
            _Logger.Debug("begin to update unionId:{0}", openIds.Count);
            int successCount = 0;
            int executedCount = 0;
            int avratoFailedCount = 0;
            var config = GetWechatConfig();
            //WebServiceAgent agent = new WebServiceAgent(CommonService.GetSysConfig("WentangshequService", "http://120.92.191.39:9012/Service1.asmx"));
            foreach (var openId in openIds)
            {
                try
                {
                    executedCount++;
                    var user = UserApi.Info(config.WeixinAppId, config.WeixinCorpSecret, openId);
                    if (!string.IsNullOrEmpty(user.unionid))
                    {
                        this._BaseService.Repository.SqlExcute(string.Format("update WechatMPUser set unionid='{0}' where openid='{1}'", user.unionid, openId));
                        successCount++;
                        //dynamic result = agent.Invoke("MinaBind", openId, user.unionid);
                        //var obj = JsonConvert.DeserializeObject(result);
                        //int stauts = obj.Status;
                        //if (stauts == 1)
                        //{
                        //    successCount++;
                        //}
                        //else
                        //{
                        //    avratoFailedCount++;
                        //    _Logger.Error("aravto failed, openId:{0}, unionId:{1}", openId, user.unionid);
                        //}
                    }
                    if (executedCount % 100 == 0)
                    {
                        _Logger.Debug("{0} has executed, success:{1}, arvato failedCount:{2}", executedCount, successCount, avratoFailedCount);
                    }
                }
                catch (Exception ex)
                {
                    _Logger.Error("{0} is failed:{1}", openId, ex);
                }
            }
            _Logger.Debug("success:{0}, failed:{1}, arvato failedCount:{2}", successCount, executedCount - successCount, avratoFailedCount);
            return SuccessNotification(string.Format("success:{0}, failed:{1}", successCount, executedCount - successCount));
        }

        public JsonResult UpdateArvatoMapping(string ticket)
        {
            if (!"1qaz2wsxEDemo12!@".Equals(ticket, StringComparison.OrdinalIgnoreCase))
            {
                return ErrorNotification("ticket is wrong");
            }
            int successCount = 0;
            int failedCount = 0;
            List<string> openIds = this._BaseService.Repository.Entities
                .Where(s => s.IsCanceled == false && !string.IsNullOrEmpty(s.UnionId) && s.AccountManageId.HasValue && this.AccountManageID == s.AccountManageId.Value)
                .Select(s => s.OpenId).ToList();
            if (openIds != null && openIds.Count > 0)
            {
                WebServiceAgent agent = new WebServiceAgent(CommonService.GetSysConfig("WentangshequService", "http://120.92.191.39:9012/Service1.asmx"));
                _Logger.Debug("mapping count:{0}", openIds.Count);
                foreach (var openId in openIds)
                {
                    try
                    {
                        var user = this._BaseService.Repository.Entities.FirstOrDefault(s => openId.Equals(s.OpenId, StringComparison.OrdinalIgnoreCase));
                        if (user != null && !string.IsNullOrEmpty(user.UnionId))
                        {
                            string unionId = user.UnionId;
                            dynamic result = agent.Invoke("MinaBind", openId, unionId);
                            var obj = JsonConvert.DeserializeObject(result);
                            int stauts = obj.Status;
                            if (stauts == 1)
                            {
                                successCount++;
                            }
                            else
                            {
                                failedCount++;
                                _Logger.Error(result);
                            }
                        }
                        else
                        {
                            failedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _Logger.Error(ex);
                    }

                }
            }
            return Json(new
            {
                successCount = successCount,
                failedCount = failedCount
            }, JsonRequestBehavior.AllowGet);
        }
    }

    public enum AvatorTagIdMapping
    {
        TagId_10 = 6692,
        TagId_11 = 6693,
        TagId_12 = 6694,
        TagId_13 = 6695,
        TagId_14 = 6696,
        TagId_15 = 6697,
        TagId_16 = 6698,
        TagId_17 = 6699,
        TagId_18 = 6700,
        TagId_19 = 6701,
        TagId_20 = 6702,
        TagId_21 = 6703,
        TagId_22 = 6704,
        TagId_23 = 6705,
        TagId_24 = 6706,
        TagId_25 = 6707,
        TagId_26 = 6708,
        TagId_27 = 6709,
        TagId_28 = 6710,
        TagId_29 = 6711,
        TagId_30 = 6712,
        TagId_31 = 6713,
        TagId_32 = 6714,
        TagId_33 = 6715,
        TagId_34 = 6716,
        TagId_35 = 6717,
        TagId_36 = 6718,
        TagId_37 = 6719,
        TagId_38 = 6720,
        TagId_39 = 6721,
        TagId_40 = 6722,
        TagId_41 = 6723,
        TagId_42 = 6724,
        TagId_43 = 6725,
        TagId_44 = 6726,
        TagId_45 = 6727,
        TagId_46 = 6728,
        TagId_47 = 6729,
        TagId_48 = 6730,
        TagId_49 = 6731,
        TagId_50 = 6732,
        TagId_51 = 6733,
        TagId_52 = 6734,
        TagId_53 = 6735,
        TagId_54 = 6736,
        TagId_55 = 6737,
        TagId_56 = 6738,
        TagId_57 = 6739,
        TagId_58 = 6740,
        TagId_59 = 6741,
        TagId_60 = 6742,
        TagId_61 = 6743,
        TagId_62 = 6744,
        TagId_63 = 6745,
        TagId_64 = 6746,
        TagId_65 = 6747,
        TagId_66 = 6748,
        TagId_67 = 6749,
        TagId_68 = 6750,
        TagId_69 = 6751,
        TagId_70 = 6752,
        TagId_71 = 6753,
        TagId_72 = 6754,
        TagId_73 = 6755,
        TagId_74 = 6756,
        TagId_75 = 6757,
        TagId_76 = 6758,
        TagId_77 = 6759,
        TagId_78 = 6760,
        TagId_79 = 6761,
        TagId_80 = 6762,
        TagId_81 = 6763,
        TagId_82 = 6764,
        TagId_83 = 6765,
        TagId_84 = 6766,
        TagId_85 = 6767,
        TagId_86 = 6768,
        TagId_87 = 6769,
        TagId_88 = 6770,
        TagId_89 = 6771,
        TagId_90 = 6772,
        TagId_91 = 6773,
        TagId_92 = 6774,
        TagId_93 = 6775,
        TagId_94 = 6776,
        TagId_95 = 6777,
        TagId_96 = 6778,
        TagId_97 = 6779,
        TagId_98 = 6780,
        TagId_99 = 6781,
        TagId_100 = 6782,
        TagId_101 = 6783,
        TagId_102 = 6784,
        TagId_103 = 6785,
        TagId_104 = 6786,
        TagId_105 = 6787,
        TagId_106 = 6788,
        TagId_107 = 6789,
        TagId_108 = 6790,
        TagId_109 = 6791,
        TagId_110 = 6792,
        TagId_111 = 6793,
        TagId_112 = 6794,
        TagId_113 = 6795,
        TagId_114 = 6796,
        TagId_115 = 6797,
        TagId_116 = 6798,
        TagId_117 = 6799,
        TagId_118 = 6800,
        TagId_119 = 6801,
        TagId_120 = 6802,
        TagId_121 = 6803,
        TagId_122 = 6804,
        TagId_123 = 6805,
        TagId_124 = 6806,
        TagId_125 = 6807,
        TagId_126 = 6808,
        TagId_127 = 6809,
        TagId_128 = 6810,
        TagId_129 = 6811,
        TagId_130 = 6812,
        TagId_131 = 6813,
        TagId_132 = 6814,
        TagId_133 = 6815,
        TagId_134 = 6816,
        TagId_135 = 6817,
        TagId_136 = 6818,
        TagId_137 = 6819,
        TagId_138 = 6820,
        TagId_139 = 6821,
        TagId_140 = 6822,
        TagId_141 = 6823,
        TagId_142 = 6824,
        TagId_143 = 6825,
        TagId_144 = 6826,
        TagId_145 = 6827,
        TagId_146 = 6828,
        TagId_147 = 6829,
        TagId_148 = 6830,
        TagId_149 = 6831,
        TagId_150 = 6832,
        TagId_151 = 6833,
        TagId_152 = 6834,
        TagId_153 = 6835,
        TagId_154 = 6836,
        TagId_155 = 6837,
        TagId_156 = 6838,
        TagId_157 = 6839,
        TagId_158 = 6840,
        TagId_159 = 6841,
        TagId_160 = 6842,
        TagId_161 = 6843,
        TagId_162 = 6844,
        TagId_163 = 6845,
        TagId_164 = 6846,
        TagId_165 = 6847,
        TagId_166 = 6848,
        TagId_167 = 6849,
        TagId_168 = 6850,
        TagId_169 = 6851,
        TagId_170 = 6852,
        TagId_171 = 6853,
        TagId_172 = 6854,
        TagId_173 = 6855,
        TagId_174 = 6856,
        TagId_175 = 6857,
        TagId_176 = 6858,
        TagId_177 = 6859,
        TagId_178 = 6860,
        TagId_179 = 6861,
        TagId_180 = 6862,
        TagId_181 = 6863,
        TagId_182 = 6864,
        TagId_183 = 6865,
        TagId_184 = 6866,
        TagId_185 = 6867,
        TagId_186 = 6868,
        TagId_187 = 6869,
        TagId_188 = 6870,
        TagId_189 = 6871,
        TagId_190 = 6872,
        TagId_191 = 6873,
        TagId_192 = 6874,
        TagId_193 = 6875,
        TagId_194 = 6876,
        TagId_195 = 6877,
        TagId_196 = 6878,
        TagId_197 = 6879,
        TagId_198 = 6880,
        TagId_199 = 6881,
        TagId_200 = 6882,
        TagId_201 = 6883,
        TagId_202 = 6884,
        TagId_203 = 6885,
        TagId_204 = 6886,
        TagId_205 = 6887,
        TagId_206 = 6888,
        TagId_207 = 6889,
        TagId_208 = 6890,
        TagId_209 = 6891,
        TagId_210 = 6892,
        TagId_211 = 6893,
        TagId_212 = 6894,
        TagId_213 = 6895,
        TagId_214 = 6896,
        TagId_215 = 6897,
        TagId_216 = 6898,
        TagId_217 = 6899,
        TagId_218 = 6900,
        TagId_219 = 6901,
        TagId_220 = 6902,
        TagId_221 = 6903,
        TagId_222 = 6904,
        TagId_223 = 6905,
        TagId_224 = 6906,
        TagId_225 = 6907,
        TagId_226 = 6908,
        TagId_227 = 6909,
        TagId_228 = 6910,
        TagId_229 = 6911,
        TagId_230 = 6912,
        TagId_231 = 6913,
        TagId_232 = 6914,
        TagId_233 = 6915,
        TagId_234 = 6916,
        TagId_235 = 6917,
        TagId_236 = 6918,
        TagId_237 = 6919,
        TagId_238 = 6920,
        TagId_239 = 6921,
        TagId_240 = 6922,
        TagId_241 = 6923,
        TagId_242 = 6924,
        TagId_243 = 6925,
        TagId_244 = 6926,
        TagId_245 = 6927,
        TagId_246 = 6928,
        TagId_247 = 6929,
        TagId_248 = 6930,
        TagId_249 = 6931,
        TagId_250 = 6932,
        TagId_251 = 6933,
        TagId_252 = 6934,
        TagId_253 = 6935,
        TagId_254 = 6936,
        TagId_255 = 6937,
        TagId_256 = 6938,
        TagId_257 = 6939,
        TagId_258 = 6940,
        TagId_259 = 6941,
        TagId_260 = 6942,
        TagId_261 = 6943,
        TagId_262 = 6944,
        TagId_263 = 6945,
        TagId_264 = 6946,
        TagId_265 = 6947,
        TagId_266 = 6948,
        TagId_267 = 6949,
        TagId_268 = 6950,
        TagId_269 = 6951,
        TagId_270 = 6952,
        TagId_271 = 6953,
        TagId_272 = 6954,
        TagId_273 = 6955,
        TagId_274 = 6956,
        TagId_275 = 6957,
        TagId_276 = 6958,
        //2017.4.14新加19家医院
        TagId_277 = 6960,//河北省人民医院
        TagId_278 = 6961,
        TagId_279 = 6962,
        TagId_280 = 6963,
        TagId_281 = 6964,//沈阳市骨科医院
        TagId_282 = 6965,
        TagId_283 = 6966,
        TagId_284 = 6967,
        TagId_285 = 6968,
        TagId_286 = 6969,
        TagId_287 = 6970,
        TagId_288 = 6971,//昆明市第一人民医院
        TagId_289 = 6972,
        TagId_290 = 6973,
        TagId_291 = 6974,
        TagId_292 = 6975,
        TagId_293 = 6976,
        TagId_294 = 6977,
        TagId_295 = 6978,//复旦大学附属上海市第五人民医院
    }
}