using Infrastructure.Core.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using Infrastructure.Core.Logging;
using Infrastructure.Core.Caching;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Core.Infrastructure;
//using Innocellence.WeChatMain.Entity;
using Autofac;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.CommonAPIs;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
//using Innocellence.WeChatMain.Entity;
using Innocellence.Weixin.QY.AdvancedAPIs;
//using Innocellence.WeChatMain.Entity;
using Innocellence.Weixin.QY.AdvancedAPIs.App;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System.Text;
using System.Web;





namespace Innocellence.WeChat.Domain.Common
{
    public partial class WeChatCommonService : ICommonService
    {
        // public ILogger Logger { get; set; }

        private static object objLock = new object();

        private static object objLockSys = new object();

        private static object objLockWeChat = new object();
        private static ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(WeChatCommonService)));

        private static BaseService<AccountManage> _accountManageService = new BaseService<AccountManage>();

        //将所有成员列表加入到缓存中去
        public static List<GetMemberResult> lstUser(int AccountManageId)
        {

            var lst = cacheManager.Get<List<GetMemberResult>>("UserItem" + AccountManageId, () =>
              {
                  var Config = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.AccountManageId == AccountManageId);
                  string accessToken = AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);
                  var userlist = MailListApi.GetDepartmentMemberInfo(accessToken, 1, 1, 0).userlist;
                  return userlist;
              });
            return lst;

        }

        public static void ClearMemberLst(int AccountManageId)
        {
            cacheManager.Remove("MemberItem" + AccountManageId);
        }

        public static List<TagItem> lstTag(int AccountManageId)
        {

            var lst = cacheManager.Get<List<TagItem>>("TagItem", () =>
            {
                var Config = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.AccountManageId == AccountManageId);
                string accessToken = AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);
                var tagList = MailListApi.GetTagList(accessToken).taglist;

                return tagList;
            });

            return lst;

        }

        public static Dictionary<int, List<Tuple<string, string>>> lstDepartmentTagMap(int AccountManageId)
        {
            var lst = cacheManager.Get<Dictionary<int, List<Tuple<string, string>>>>("TagDepartmentMap" + AccountManageId, () =>
            {
                var Config = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.AccountManageId == AccountManageId);
                string accessToken = AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);
                var tagList = MailListApi.GetTagList(accessToken).taglist;
                var departmentList = lstDepartment(AccountManageId);
                var result = new Dictionary<int, List<Tuple<string, string>>>();
                foreach (var tag in tagList)
                {
                    string token = AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);
                    var ret = GetTagById(AccountManageId, int.Parse(tag.tagid));// MailListApi.GetTagMember(token, int.Parse(tag.tagid));
                    if (ret.partylist != null)
                    {
                        foreach (var pId in ret.partylist)
                        {
                            var department = departmentList.Where(p => p.id == pId).FirstOrDefault();
                            if (department != null)
                            {
                                var ids = GetAllSubDepartmentIds(pId, departmentList);
                                ids.Add(pId);
                                ids.ForEach(i =>
                                {
                                    if (result.Keys.Contains(i))
                                    {
                                        result[i].Add(new Tuple<string, string>(tag.tagid, tag.tagname));
                                    }
                                    else
                                    {
                                        result.Add(i, new List<Tuple<string, string>>() { new Tuple<string, string>(tag.tagid, tag.tagname) });
                                    }
                                });
                            }
                        }
                    }
                }
                return result;
            });
            return lst;
        }

        private static List<int> GetAllSubDepartmentIds(int parentId, List<DepartmentList> departments)
        {
            List<int> result = new List<int>();
            var currentSubList = departments.Where(d => d.parentid == parentId).ToList();
            if (currentSubList.Count > 0)
            {
                result.AddRange(currentSubList.Select(s => s.id));
                foreach (var item in currentSubList)
                {
                    var nextSubList = GetAllSubDepartmentIds(item.id, departments);
                    result.AddRange(nextSubList);
                }
            }
            return result;
        }

        public static List<EmployeeInfoWithDept> lstUserWithDeptTag(int AccountManageId)
        {

            var lst = cacheManager.Get<List<EmployeeInfoWithDept>>("UserWithDeptTagItem", () =>
            {
                var Config = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.AccountManageId == AccountManageId);
                string accessToken = AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);
                var userlist = MailListApi.GetDepartmentMemberInfo(accessToken, 1, 1, 0).userlist;
                var userWithDeptTagList = new List<EmployeeInfoWithDept>();

                var tags = lstTag(AccountManageId);

                // 取人
                foreach (var getMemberResult in userlist)
                {
                    userWithDeptTagList.Add(new EmployeeInfoWithDept()
                    {
                        avatar = getMemberResult.avatar,
                        department = getMemberResult.department.ToList(),
                        deptLvs = getMemberResult.deptLvs,
                        email = getMemberResult.email,
                        gender = getMemberResult.gender.ToString(),
                        mobile = getMemberResult.mobile,
                        name = getMemberResult.name,
                        position = getMemberResult.position,
                        status = getMemberResult.status,
                        userid = getMemberResult.userid,
                        weixinid = getMemberResult.weixinid
                    });
                }

                // 更新人的标签，默认为N
                foreach (var us in userWithDeptTagList)
                {
                    us.tags = new Dictionary<string, string>();
                    foreach (var t in tags)
                    {
                        us.tags[t.tagid] = "N";
                    }
                }

                // 获取每个标签里的人，给人打标签
                foreach (var tagitem in tags)
                {
                    var tagUsers = GetTagMembers(int.Parse(tagitem.tagid), 0).userlist;
                    foreach (var u in tagUsers)
                    {
                        var employee = userWithDeptTagList.FirstOrDefault(a => a.userid.ToUpper() == u.userid.ToUpper());
                        if (employee != null)
                        {
                            employee.tags[tagitem.tagid] = "Y";
                        }
                    }

                }

                // 获取部门列表
                var departments = lstDepartment(AccountManageId);
                // 处理部门关系，暂定只有5层
                foreach (var dept in departments)
                {
                    int level = 1;
                    int parentId = dept.parentid;
                    while (parentId != 0)
                    {
                        var parentDept = departments.FirstOrDefault(t => t.id == parentId);
                        if (parentDept != null)
                        {
                            level++;
                            parentId = parentDept.parentid;
                        }
                    }
                    dept.level = level;
                }

                // 更新员工信息，把部门的名称填入
                foreach (var emp in userWithDeptTagList)
                {
                    // 如果有多个部门的话，只列出其中一个部门。
                    if (emp.department.Count() >= 1)
                    {
                        var dept = departments.FirstOrDefault(t => t.id == emp.department[0]);
                        if (dept != null)
                        {
                            int level = dept.level;
                            while (level > 1)
                            {
                                emp.deptLvs[level] = dept.name;
                                dept = departments.FirstOrDefault(t => t.id == dept.parentid);
                                level--;
                            }
                        }
                        if (emp.department.Count() > 1)
                        {
                            emp.deptLvs[2] = "*" + emp.deptLvs[2];
                        }
                    }
                }


                return userWithDeptTagList;

            });
            return lst;

        }

        public static List<WeChatEmotion> lstWechatEmotion
        {
            get
            {
                var lst = cacheManager.Get<List<WeChatEmotion>>("WechatEmotionItem", () =>
                {
                    var str = CommonService.GetSysConfig("WechatEmotion", "[]");
                    try
                    {
                        var emotionList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeChatEmotion>>(str);
                        return emotionList;
                    }
                    catch (Exception ex)
                    {
                        return new List<WeChatEmotion>();
                    }
                });
                return lst;
            }
        }


        public static void ClearCache(int iType)
        {
            if (iType == 1)
            {
                //CacheManager.GetCacher("Default").Remove("Category");
            }
            else if (iType == 2)
            {
                //CacheManager.GetCacher("Default").Remove("SysConfig");
            }
            // add by liuwei
            else if (iType == 3)
            {
                cacheManager.Remove("SysWeChatConfig");
            }

        }

        public static void ClearDepartmentTagMapCache(int accountManageId)
        {
            cacheManager.Remove("TagDepartmentMap" + accountManageId);
        }

        /// <summary>
        /// 建议走缓存
        /// </summary>
        /// <param name="iAppID"></param>
        /// <returns></returns>
        public static GetAppInfoResult GetAppInfo(int iWeChatID)
        {
            var objConfig = GetWeChatConfigByID(iWeChatID);

            string strToken = AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);

            return AppApi.GetAppInfo(strToken, int.Parse(objConfig.WeixinAppId));
            // Ret.allow_userinfos
        }
        public static List<SysWechatConfig> GetAppList(string corpId)
        {
            //var corpId = GetSysConfig("WeixinCorpId", string.Empty);  //取出corpid
            var lst = lstSysWeChatConfig.Where(a => a.WeixinCorpId == corpId).ToList();//根据corpid将Applist拿出来

            return lst;
        }

        public static List<SysWechatConfig> GetAppList()
        {
            //var corpId = GetSysConfig("WeixinCorpId", string.Empty);  //取出corpid
            var lst = lstSysWeChatConfig.Where(a => a.IsCorp == true).ToList();//根据corpid将Applist拿出来

            return lst;
        }

        public static List<TagItem> GetTagListByAccountManageID(int accountManageId)
        {
            var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.AccountManageId == accountManageId);
            var token = AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
            var tagList = MailListApi.GetTagList(token).taglist;
            return tagList;
        }


        public static List<DepartmentList> lstDepartment(int AccountManageId)
        {

            // Logger.Debug("Getting lstDepartment");
            var lst = cacheManager.Get<List<DepartmentList>>("DepartmentList", () =>
            {
                Logger.Debug("cache is empty, creating cache....");
                var Config = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.AccountManageId == AccountManageId);

                var strToken = AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);
                var result = MailListApi.GetDepartmentList(strToken);
                var departments = result.department;
                //var departmentID = int.Parse(CommonService.GetSysConfig("DimissionDepartment", ""));
                //var dismissionDepartment = departments.Where(d => d.id == departmentID).FirstOrDefault();
                //if (dismissionDepartment != null)
                //{
                //    departments.Remove(dismissionDepartment);
                //}
                return departments;
            });
            //  Logger.Debug("returning lstDepartment");
            return lst;

        }

        public static GetTagMemberResult GetTagById(int AccountManageId, int tagId)
        {
            var lst = cacheManager.Get<GetTagMemberResult>("Tag_" + tagId, () =>
            {
                Logger.Debug("cache is empty, creating cache....");
                var Config = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.AccountManageId == AccountManageId);
                var strToken = AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);
                var result = MailListApi.GetTagMember(strToken, tagId);
                return result;
            });
            return lst;
        }

        public static void ClearTagCache(int tagId)
        {
            cacheManager.Remove("Tag_" + tagId);
        }

        private static void GetSubDepartments(int parentDepartmentId, IEnumerable<DepartmentList> allDepartments, List<DepartmentList> subDepartments, int AccountManageID)
        {
            var departments = allDepartments.Where(x => x.parentid == parentDepartmentId || x.id == parentDepartmentId).ToList();
            subDepartments.AddRange(departments);

            if (!departments.Any() || departments.Count == 1) return;

            //Parallel.ForEach(departments.AsParallel().Where(x => x.id != parentDepartmentId), department => GetSubDepartments(department.id, lstDepartment, subDepartments));
            foreach (var department in departments.AsParallel().Where(x => x.id != parentDepartmentId).ToList())
            {
                GetSubDepartments(department.id, lstDepartment(AccountManageID), subDepartments, AccountManageID);
            }
        }

        public static IEnumerable<DepartmentList> GetSubDepartments(IList<int> parentDepartmentIds, int AccountManageID)
        {
            var departments = new List<DepartmentList>();
            foreach (var id in parentDepartmentIds)
            {
                GetSubDepartments(id, lstDepartment(AccountManageID), departments, AccountManageID);
            }
            //Parallel.ForEach(parentDepartmentIds, id => GetSubDepartments(id, lstDepartment, departments));

            return departments.Distinct(new DepartmentCompare());
        }

        public static IEnumerable<DepartmentList> GetSubDepartments(int parentDepartmentId, int AccountManageID)
        {
            var departments = new List<DepartmentList>();
            GetSubDepartments(parentDepartmentId, lstDepartment(AccountManageID), departments, AccountManageID);

            return departments.Distinct(new DepartmentCompare());
        }

        public static GetTagMemberResult GetTagMembers(int tagId, int appId)
        {
            return MailListApi.GetTagMember(WeChatCommonService.GetWeiXinToken(appId), tagId);
        }


        //public  SysWechatConfig GetAppSysWechatConfig(string appId) {
        //     return lstSysWeChatConfig.Find(a => a.WeixinAppId == appId);

        //}

        public List<PersonGroup> GetAllPersonAndGroup(string appId, int accountManageId)
        {
            var id = int.Parse(appId);
            SysWechatConfig appInfo = null;
            if (id == 0)
            {
                appInfo = lstSysWeChatConfig.FirstOrDefault(a => a.AccountManageId == accountManageId);
            }
            else
            {
                appInfo = lstSysWeChatConfig.SingleOrDefault(a => a.Id == id && a.AccountManageId == accountManageId);
            }
            string token = (appInfo.IsCorp != null && !appInfo.IsCorp.Value) ? Innocellence.Weixin.MP.CommonAPIs.AccessTokenContainer.GetToken(appInfo.WeixinCorpId, appInfo.WeixinCorpSecret) : Innocellence.Weixin.QY.CommonAPIs.AccessTokenContainer.TryGetToken(appInfo.WeixinCorpId, appInfo.WeixinCorpSecret);
            var results = AppApi.GetAppInfo(token, int.Parse(appInfo.WeixinAppId));

            List<PersonGroup> list = new List<PersonGroup>();

            if (results.allow_userinfos != null)
            {
                foreach (var person in results.allow_userinfos.user)
                {
                    PersonGroup personGroup = new PersonGroup();

                    personGroup.Type = "Person";

                    var p = MailListApi.GetMember(token, person.userid);
                    personGroup.WeixinName = p.name;
                    personGroup.WeixinId = p.userid;
                    personGroup.Avatar = p.avatar;
                    list.Add(personGroup);

                }
            }

            if (results.allow_partys != null)
            {
                foreach (var groupId in results.allow_partys.partyid)
                {
                    PersonGroup personGroup = new PersonGroup();

                    personGroup.Type = "Group";

                    var d = MailListApi.GetDepartmentList(token, groupId).department.FirstOrDefault();
                    if (d != null)
                    {
                        personGroup.WeixinName = d.name;
                        personGroup.WeixinId = d.id.ToString();
                        list.Add(personGroup);
                    }
                }
            }

            if (results.allow_tags != null)
            {
                var allTagList = MailListApi.GetTagList(token).taglist;
                foreach (var groupId in results.allow_tags.tagid)
                //foreach (var tag in allTagList)
                {
                    PersonGroup personGroup = new PersonGroup();

                    personGroup.Type = "Tag";

                    var tag = allTagList.Find(a => a.tagid == groupId.ToString());
                    if (tag != null)
                    {
                        personGroup.WeixinName = tag.tagname;
                        personGroup.WeixinId = tag.tagid;
                        list.Add(personGroup);
                    }
                }
            }
            return list;
        }

        public List<PersonGroup> GetAllVisibleByGroup(string appId, string groupId, int accountManageId)
        {
            var id = int.Parse(appId);
            SysWechatConfig appInfo = null;
            if (id == 0)
            {
                appInfo = lstSysWeChatConfig.FirstOrDefault(a => a.AccountManageId == accountManageId);
            }
            else
            {
                appInfo = lstSysWeChatConfig.SingleOrDefault(a => a.Id == id && a.AccountManageId == accountManageId);
            }

            string token = (appInfo.IsCorp != null && !appInfo.IsCorp.Value) ? Innocellence.Weixin.MP.CommonAPIs.AccessTokenContainer.GetToken(appInfo.WeixinCorpId, appInfo.WeixinCorpSecret) : Innocellence.Weixin.QY.CommonAPIs.AccessTokenContainer.TryGetToken(appInfo.WeixinCorpId, appInfo.WeixinCorpSecret);
            var allDeptList = MailListApi.GetDepartmentList(token).department;

            var subDepts = allDeptList.Where(a => a.parentid == Int32.Parse(groupId));
            List<PersonGroup> list = new List<PersonGroup>();

            foreach (var dept in subDepts)
            {
                PersonGroup person = new PersonGroup();
                person.WeixinId = dept.id.ToString();
                person.WeixinName = dept.name;
                person.Type = "Group";
                list.Add(person);
            }


            var members = MailListApi.GetDepartmentMemberInfo(token, Int32.Parse(groupId), 0, 1);

            foreach (var member in members.userlist)
            {
                PersonGroup person = new PersonGroup();
                person.WeixinId = member.userid;
                person.WeixinName = member.name;
                person.Avatar = member.avatar;
                person.Type = "Person";
                list.Add(person);
            }
            return list;
        }

        public List<GetMemberResult> GetAllMembers(string appId, int AccountManageID)
        {
            var appInfo = lstSysWeChatConfig.SingleOrDefault(a => a.AccountManageId == AccountManageID);

            var token = CommonApi.GetToken(appInfo.WeixinCorpId, appInfo.WeixinCorpSecret);
            var members = MailListApi.GetDepartmentMemberInfo(token.access_token, 7, 1, 0);

            return members.userlist;
        }

        public string GetDisplayName(string appId, int AccountManageID, string userIds = "", string groupIds = "")
        {
            var appInfo = lstSysWeChatConfig.SingleOrDefault(a => a.AccountManageId == AccountManageID);
            var token = CommonApi.GetToken(appInfo.WeixinCorpId, appInfo.WeixinCorpSecret);
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(userIds))
            {
                var users = userIds.Split(',');
                foreach (var user in users)
                {
                    sb.Append(MailListApi.GetMember(token.access_token, user).name).Append(",");
                }
            }
            if (!string.IsNullOrEmpty(groupIds))
            {
                sb.Append("|");
                var groups = groupIds.Split(',');
                foreach (var group in groups)
                {
                    sb.Append(MailListApi.GetDepartmentList(token.access_token, Int32.Parse(group)).department[0].name).Append(",");
                }
            }
            return sb.ToString().TrimEnd(',');
        }

        ///// <summary>
        ///// 判断用户是否在APP的可见范围内
        ///// </summary>
        ///// <param name="appId"></param>
        ///// <param name="currentUserId"></param>
        ///// <param name="AccountManageID"></param>
        ///// <returns></returns>
        //public bool IsValidateUser(string appId, string currentUserId, int AccountManageID)
        //{


        //    if (HttpRuntime.Cache.Get("validateUsers") != null)
        //    {

        //        var obj = HttpRuntime.Cache.Get("validateUsers");
        //        List<String> allVisiblePerson = (List<String>)obj;
        //        return allVisiblePerson.Contains(currentUserId);

        //    }
        //    else
        //    {
        //        var appInfo = lstSysWeChatConfig.SingleOrDefault(a => a.AccountManageId == AccountManageID);

        //        var token = CommonApi.GetToken(appInfo.WeixinCorpId, appInfo.WeixinCorpSecret);
        //        var results = AppApi.GetAppInfo(token.access_token, Int32.Parse(appId));
        //        List<string> allVisiblePerson = new List<string>();
        //        foreach (var user in results.allow_userinfos.user)
        //        {
        //            allVisiblePerson.Add(user.userid);
        //        }
        //        foreach (var groupId in results.allow_partys.partyid)
        //        {
        //            var members = MailListApi.GetDepartmentMemberInfo(token.access_token, groupId, 1, 1);

        //            foreach (var user in members.userlist)
        //            {
        //                allVisiblePerson.Add(user.userid);
        //            }
        //        }

        //        HttpRuntime.Cache.Insert("validateUsers", allVisiblePerson, null, DateTime.Now.AddHours(2), TimeSpan.Zero);

        //        return allVisiblePerson.Contains(currentUserId);
        //    }
        //}

        /// <summary>
        ///  判断用户是否在APP的可见范围内
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="objUser"></param>
        /// <param name="AccountManageID"></param>
        /// <returns></returns>
        public static bool IsValidateUser(int appId, SysAddressBookMember objUser, int AccountManageID)
        {
            var AppInfo = cacheManager.Get<GetAppInfoResult>(string.Format("APP_{0}_{1}", AccountManageID, appId), () =>
            {
                Logger.Debug("cache is empty, creating cache....");
                var appInfo = GetWeChatConfigByID(appId);

                var token = CommonApi.GetToken(appInfo.WeixinCorpId, appInfo.WeixinCorpSecret);
                var results = AppApi.GetAppInfo(token.access_token, int.Parse(appInfo.WeixinAppId));

                return results;

            });
            //部门
            if (!string.IsNullOrWhiteSpace(objUser.Department))
            {
                string strParty = objUser.Department.Insert(objUser.Department.Length - 1, ",").Insert(1, ",");
                if (AppInfo.allow_partys.partyid.Count(a => strParty.IndexOf("," + a.ToString() + ",") >= 0) > 0)
                {
                    return true;
                }
                var departments = lstDepartment(AccountManageID);
                List<int> allDepartments = new List<int>();
                FindAllParentDepartments(strParty, departments, allDepartments);
                if (AppInfo.allow_partys.partyid.Any(a => allDepartments.Contains(a)))
                {
                    return true;
                }
            }
            //tag
            if (!string.IsNullOrWhiteSpace(objUser.TagList))
            {
                string strTag = objUser.TagList.Insert(objUser.TagList.Length - 1, ",").Insert(1, ",");
                if (AppInfo.allow_tags.tagid.Count(a => strTag.IndexOf("," + a.ToString() + ",") >= 0) > 0)
                {
                    return true;
                }
            }
            //user
            if (AppInfo.allow_userinfos.user.Exists(a => objUser.UserId == a.userid))
            {
                return true;
            }

            return false;
        }

        private static void FindAllParentDepartments(string currentDepartments, List<DepartmentList> departments, List<int> allDepartments)
        {
            if (null != departments && !string.IsNullOrWhiteSpace(currentDepartments))
            {
                var lstCurrent = currentDepartments.Split(',').ToList();
                var currentNodes = departments.FindAll(c => lstCurrent.Contains(c.id.ToString())).ToList();
                foreach (var item in currentNodes)
                {
                    var parentId = item.parentid;
                    if (!allDepartments.Contains(parentId))
                    {
                        allDepartments.Add(parentId);
                        FindAllParentDepartments(parentId.ToString(), departments, allDepartments);
                    }
                }
            }
        }
    }



    public class DepartmentCompare : IEqualityComparer<DepartmentList>
    {
        public bool Equals(DepartmentList x, DepartmentList y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(DepartmentList obj)
        {
            return obj.id;
        }
    }
}
