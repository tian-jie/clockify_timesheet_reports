/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：MenuController.cs
    文件功能描述：自定义菜单设置工具Controller
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using System.Text;
using Autofac;
using Infrastructure.Core;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.QY;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.AdvancedAPIs.Media;
using Innocellence.Weixin.QY.CommonAPIs;
using Innocellence.Weixin.QY.Entities;
using Innocellence.Weixin.Entities.Menu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.WeChat.Domain.ViewModel;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Innocellence.Weixin.Exceptions;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Contracts;
using System.Transactions;
using Newtonsoft.Json;

namespace Innocellence.WeChatMain.Controllers
{
    public class TagManageController : BaseController<Category, CategoryView>
    {
        private static ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(WeChatCommonService)));
        private Infrastructure.Web.Domain.Contracts.ICommonService _commonService;
        private IAddressBookService _addressBookService;
        public TagManageController(ICategoryService newsService, Infrastructure.Web.Domain.Contracts.ICommonService commonService,
            IAddressBookService addressBookService)
            : base(newsService)
        {
            _commonService = commonService;
            _addressBookService = addressBookService;
        }

        public override ActionResult Index()
        {


            //var tagList = MailListApi.GetTagList(GetToken()).taglist;
            var tagList = WeChatCommonService.GetTagListByAccountManageID(this.AccountManageID);
            ViewBag.taglist = tagList;



            return View();
        }

        //public  ActionResult GetDepartment()
        //{
        //    var strToken = GetToken();
        //    var result = MailListApi.GetDepartmentList(strToken);

        //    var q = from a in result.department
        //            select new { Id = a.id, UserName = a.name,expanded=true,
        //                loaded = true, parent = a.parentid, level = (a.parentid == 0 ? 0 : (a.parentid < 3 ? 1 : 2)), isLeaf = a.parentid<3?false:true };

        //    return Json(new
        //    {
        //        aaData = q
        //    }, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult GetListTree()
        {
            GetDepartmentMemberResult objReturn;

            string strID = Request["DeptId"];
            if (!string.IsNullOrEmpty(strID))
            {
                objReturn = MailListApi.GetDepartmentMember(GetToken(), int.Parse(strID), 0, 0);
                List<UserList_Simple> lst = objReturn.userlist;

                List<EasyUITreeData> lstRet = new List<EasyUITreeData>();
                lst.ForEach(a => { lstRet.Add(new EasyUITreeData() { id = a.userid, text = a.name }); });

                return Json(lstRet, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public List<GetMemberResult> GetListData(int iPage, int iCount, ref int iTotal)
        {
            string strID = Request["DeptId"];
            //Defect #14487 bug修正
            string strSearchCondition = Request["SearchCondition"];
            if (!string.IsNullOrEmpty(strID))
            {
                var lst = MailListApi.GetDepartmentMemberInfo(GetToken(), int.Parse(strID), 1, 0).userlist;//递归的去取子部门下面的成员
                if (!string.IsNullOrEmpty(strSearchCondition))
                {
                    // Defect#15596修正&性能改善
                    strSearchCondition = strSearchCondition.Trim().ToLower();

                    lst = lst.Where(x => x.userid.ToLower().Contains(strSearchCondition) ||
                        (x.email != null && x.email.ToLower().Contains(strSearchCondition)) ||
                        x.name.ToLower().Contains(strSearchCondition)).ToList();
                }

                iTotal = lst.Count();
                return lst.Skip((iPage - 1) * iCount).Take(iCount).ToList();
            }

            return null;
        }

        public List<TagView> GetListDataBytag(int iPage, int iCount, ref int iTotal)
        {

            string strID = Request["TagId"];
            string strSearchCondition = Request["SearchCondition"];

            if (!string.IsNullOrEmpty(strID))
            {
                var result = WeChatCommonService.GetTagById(AccountManageID, int.Parse(strID));// MailListApi.GetTagMember(GetToken(), int.Parse(strID));
                var userlst = result.userlist;
                var userIds = result.userlist.Select(u => u.userid).ToList();
                var allDeptList = WeChatCommonService.lstDepartment(AccountManageID); //MailListApi.GetDepartmentList(GetToken()).department;
                var userInfos = _addressBookService.Repository.Entities.Where(u => userIds.Contains(u.UserId) && u.DeleteFlag != 1).ToList();
                if (!string.IsNullOrEmpty(strSearchCondition))
                {
                    userInfos = userInfos.Where(x => x.UserId.IndexOf(strSearchCondition.Trim(), StringComparison.OrdinalIgnoreCase) >= 0 ||
                        x.UserName.IndexOf(strSearchCondition.Trim(), StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (!string.IsNullOrEmpty(x.Email) && x.Email.IndexOf(strSearchCondition.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(x.Mobile) && x.Mobile.IndexOf(strSearchCondition.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(x.EmployeeNo) && x.EmployeeNo.IndexOf(strSearchCondition.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
                }
                var partylst = new List<DepartmentList>();
                if (result.partylist != null)
                {
                    partylst = allDeptList.Where(a => result.partylist.Contains(a.id)).ToList();
                    if (!string.IsNullOrEmpty(strSearchCondition))
                    {
                        partylst = partylst.Where(x => x.id.ToString().IndexOf(strSearchCondition.Trim(), StringComparison.OrdinalIgnoreCase) >= 0 ||
                            x.name.IndexOf(strSearchCondition.Trim(), StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    }
                }
                //allDeptList
                var targetList = new List<TagView>();
                targetList.AddRange(userInfos.Select(a => new TagView { Type = "Person", WeixinId = a.UserId, WeixinName = a.UserName, Department = GetDepartmentName(allDeptList, a.Department) }));
                targetList.AddRange(partylst.Select(a => new TagView { Type = "Group", WeixinId = a.id.ToString(), WeixinName = a.name, Department = GetDepartmentPath(allDeptList, a.id) }));

                iTotal = targetList.Count();
                return targetList.Skip((iPage - 1) * iCount).Take(iCount).ToList();
            }

            return null;
        }
        /// <summary>
        /// 得到人员的部门名称
        /// </summary>
        /// <param name="allDepartment"></param>
        /// <param name="departmentIds"></param>
        /// <returns></returns>
        private string GetDepartmentName(List<DepartmentList> allDepartment, string departmentIds)
        {
            var builder = new StringBuilder();
            var ids = JsonConvert.DeserializeObject<int[]>(departmentIds);
            foreach (var department in allDepartment)
            {
                if (ids.Contains(department.id))
                {
                    builder.Append(department.name).Append(",");
                }
            }
            var rs = builder.ToString();
            if (rs.EndsWith(","))
            {
                rs = rs.Substring(0, rs.Length - 1);
            }
            return rs;
        }

        /// <summary>
        /// 得到部门的部门路径
        /// </summary>
        /// <param name="allDepartment"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        private string GetDepartmentPath(List<DepartmentList> allDepartment, int departmentId)
        {
            StringBuilder pathBuilder = new StringBuilder();
            var department = allDepartment.Where(d => d.id == departmentId).FirstOrDefault();
            if (department != null)
            {
                pathBuilder.Append(department.name);
                var cursor = department;
                for (var parent = allDepartment.Where(d => d.id == cursor.parentid).FirstOrDefault();
                    cursor.parentid != 0 && parent != null;
                    cursor = parent, parent = allDepartment.Where(d => d.id == cursor.parentid).FirstOrDefault())
                {
                    pathBuilder.Insert(0, parent.name + ">");
                }
            }
            return pathBuilder.ToString();
        }

        //public ActionResult GetDept(int? Id)
        //{
        //    string strID = Request["Id"];
        //    if (!string.IsNullOrEmpty(strID))
        //    {
        //        var strToken = GetToken();
        //        var result = MailListApi.GetDepartmentList(strToken);

        //        return Json(result.department.Find(a=>a.id==Id), JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetEasyUITreeData()
        //{
        //         var strToken = GetToken();
        //         var result = MailListApi.GetDepartmentList(strToken);

        //        var lst= result.department;



        //        return Json(EasyUITreeData.GetTreeData(lst, "id", "name", "parentid"), JsonRequestBehavior.AllowGet);


        //}

        /// <summary>
        /// 获取某标签下成员列表
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public ActionResult GetListByTag()
        {
            PageParameter param = new PageParameter();
            TryUpdateModel(param);
            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            param.length = param.length == 0 ? 10 : param.length;
            int iTotal = param.iRecordsTotal;
            string strWeChatUserID = Request["WeChatUserID"];

            var list = GetListDataBytag((int)Math.Floor(param.start / param.length * 1.0d) + 1, param.length, ref iTotal);// _newsService.GetList(null, null, 10, 10);
            if (list == null)
            {
                list = new List<TagView>();
            }
            else
            {
                if (!string.IsNullOrEmpty(strWeChatUserID))
                {
                    list = list.Where(x => x.WeixinId == strWeChatUserID).ToList();
                }
                list = list.OrderBy(a => a.WeixinName).OrderBy(a => a.Type).ToList();
            }

            return Json(new
            {
                sEcho = param.draw,
                iTotalRecords = iTotal,
                iTotalDisplayRecords = iTotal,
                aaData = list
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 过滤成员
        /// </summary>
        /// <returns></returns>
        public JsonResult GetListByCondation()
        {
            string condation = Request["txtSearch"].ToUpper();
            GetMemberResult member = new GetMemberResult();

            if (WeChatCommonService.lstUser(AccountManageID) != null)
            {
                member = WeChatCommonService.lstUser(AccountManageID).Find(a => a.userid.Equals(condation, StringComparison.InvariantCultureIgnoreCase));
            }

            return Json(new { data = member }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加标签成员
        /// </summary>
        /// <returns></returns>
        public JsonResult addTagMember(string[] userlist, int[] partylist, string tagId)
        {
            string accessToken = GetToken();
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                     new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                if (userlist != null)
                {
                    foreach (var userId in userlist)
                    {
                        _addressBookService.addMemberTag(userId, Int32.Parse(tagId));
                    }
                }
                var ret = MailListApi.AddTagMember(accessToken, Int32.Parse(tagId), userlist, partylist);
                if (ret.errcode == Weixin.ReturnCode_QY.请求成功)
                {
                    transactionScope.Complete();
                }
            }
            WeChatCommonService.ClearDepartmentTagMapCache(this.AccountManageID);
            WeChatCommonService.ClearTagCache(int.Parse(tagId));
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public JsonResult delTagmember(string userId, string tagId, string mType)
        {
            string accessToken = GetToken();
            string[] userlist = null;
            int[] partylist = null;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                if (mType.Equals("Person", StringComparison.CurrentCultureIgnoreCase))
                {
                    userlist = new string[] { userId };
                    _addressBookService.delMemberTag(userId, Int32.Parse(tagId));
                }
                else
                {
                    partylist = new int[] { Int32.Parse(userId) };
                }

                var ret = MailListApi.DelTagMember(accessToken, Int32.Parse(tagId), userlist, partylist);
                if (ret.errcode == Weixin.ReturnCode_QY.请求成功)
                {
                    transactionScope.Complete();
                }
            }
            WeChatCommonService.ClearDepartmentTagMapCache(this.AccountManageID);
            WeChatCommonService.ClearTagCache(int.Parse(tagId));
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <returns></returns>
        public JsonResult addTag(string tagName)
        {
            string accessToken = GetToken();
            MailListApi.CreateTag(accessToken, tagName);

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public JsonResult renameTag(string Id, string tagName)
        {
            string accessToken = GetToken();
            MailListApi.UpdateTag(accessToken, Int32.Parse(Id), tagName);
            WeChatCommonService.ClearDepartmentTagMapCache(this.AccountManageID);
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public JsonResult delTag(string Id)
        {
            try
            {
                string accessToken = GetToken();
                var memberResult = MailListApi.GetTagMember(accessToken, Int32.Parse(Id));
                string[] userList = null;
                if (memberResult.userlist != null && memberResult.userlist.Count > 0)
                {
                    userList = memberResult.userlist.Select(a => a.userid).ToArray();
                }
                if ((userList != null && userList.Count() > 0) || (memberResult.partylist != null && memberResult.partylist.Count() > 0))
                {
                    MailListApi.DelTagMember(accessToken, Int32.Parse(Id), userList, memberResult.partylist);
                    foreach (var userId in userList)
                    {
                        _addressBookService.delMemberTag(userId, Int32.Parse(Id));
                    }
                }
                MailListApi.DeleteTag(accessToken, Int32.Parse(Id));
            }
            catch (ErrorJsonResultException ex)
            {
                if ((int)ex.JsonResult.errcode == 60018)
                {
                    var e = new Exception("请在微信企业号管理平台中，取消各应用对此标签的可见范围，再进行删除操作。");
                    throw e;
                }

            }
            finally
            {
                WeChatCommonService.ClearDepartmentTagMapCache(this.AccountManageID);
            }

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateUserlist()
        {
            if (WeChatCommonService.lstUser(AccountManageID) != null)
            {
                cacheManager.Remove("UserItem" + AccountManageID);
            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 某标签下批量导入成员
        /// </summary>
        /// <param name="Optype"></param>
        /// <returns></returns>
        public ActionResult addusersByTag()
        {
            string strRet = "[\"jsonrpc\" : \"2.0\", \"result\" : \"{1}\", \"id\" : \"{0}\"]";
            string strErrRet = "[\"jsonrpc\" : \"2.0\", \"error\" : [\"code\": {0}, \"message\": \"{1}\"], \"id\" : \"id\"]";
            try
            {
                if (Request.Files.Count == 0)
                {
                    strRet = string.Format(strErrRet, "100", "未发现上传文件！");
                    return Content(strRet);
                }

                string invalidList = ImportToTag();
                strRet = string.Format(strRet, "", invalidList.Trim('|'));
            }
            catch (Exception ex)
            {
                strRet = string.Format(strErrRet, "102", ex.Message);
            }

            return Content(strRet.Replace("[", "{").Replace("]", "}"));

        }

        private string ImportToTag()
        {
            Dictionary<string, Stream> dic = new Dictionary<string, Stream>();
            Dictionary<string, TagImport> dics = new Dictionary<string, TagImport>();
            //获取accesstoken
            string accessToken = GetToken();
            string invalidList = string.Empty;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase objFile = Request.Files[i];
                var f = new byte[objFile.InputStream.Length];
                //将csv文件一次性读出来
                objFile.InputStream.Read(f, 0, (int)objFile.InputStream.Length);
                //定义数据格式UTF-8
                string str = System.Text.Encoding.GetEncoding("UTF-8").GetString(f);
                //获取第一行数据然后取两列数据WeChatUserID,TagID
                string[] sArray = str.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                string[] rowOne = sArray[0].Split(',');
                int TagidIndex = 0, WeChatUserIDIndex = 0;
                //循环判断返回数组对应列下标
                for (int s = 0; s < rowOne.Length; s++)
                {
                    if (string.Equals(rowOne[s], "TagID", StringComparison.OrdinalIgnoreCase))
                    {
                        TagidIndex = s;
                    }
                    else if (string.Equals(rowOne[s], "WeChatUserID", StringComparison.OrdinalIgnoreCase))
                    {
                        WeChatUserIDIndex = s;
                    }
                }

                //循环其他行去把对应下标的数据封装成TagImport对象
                for (int x = 1; x < sArray.Length; x++)
                {
                    string[] rowX = sArray[x].Split(',');
                    string tagid = rowX[TagidIndex];
                    string WeChatUserID = rowX[WeChatUserIDIndex];
                    TagImport tagimport = null;
                    //确认dics是否有tagid 有add,没有就新建对象
                    if (dics.ContainsKey(tagid))
                    {
                        tagimport = dics[tagid];
                        tagimport.userlist.Add(WeChatUserID);
                    }
                    else
                    {
                        tagimport = new TagImport();
                        tagimport.tagid = tagid;
                        tagimport.userlist = new List<string>();
                        tagimport.userlist.Add(WeChatUserID);
                        tagimport.partylist = new List<int>();
                        dics.Add(tagid, tagimport);
                    }

                }

                //返回invalidlist
                AddTagMemberResult addResult = new AddTagMemberResult();

                //先根据tagid去取当前tagid的userlist再删除掉成员
                foreach (var b in dics.Keys)
                {
                    var lstTagUser = MailListApi.GetTagMember(accessToken, int.Parse(b)).userlist;
                    if (lstTagUser == null || lstTagUser.Count <= 0)
                    {
                        break;
                    }

                    List<string> userList = new List<string>();
                    lstTagUser.ForEach(a =>
                    {
                        userList.Add(a.userid);
                    });
                    MailListApi.DelTagMember(accessToken, int.Parse(b), userList.ToArray(), null);
                }

                //封装完后循环dics,序列化后调用API的接口Post过去
                foreach (var a in dics)
                {
                    var strs = string.Join(",", a.Value.userlist.ToArray());
                    List<string> lst = new List<string>();
                    GetStr(strs, lst);

                    foreach (var b in lst)
                    {
                        addResult = MailListApi.AddTagMember(
                             accessToken,
                             int.Parse(a.Key),
                             b.Split(',')
                         );
                        if (!string.IsNullOrEmpty(addResult.invalidlist))
                        {
                            invalidList = invalidList + '|' + addResult.invalidlist;
                        }
                    }
                }
                dic.Add(objFile.FileName, objFile.InputStream);
            }

            return invalidList;
        }

        public void GetStr(string strSource, List<string> lst)
        {
            if (strSource.Length > 1000)
            {
                var str = strSource.Substring(0, 1000);
                var index = str.LastIndexOf(',');
                lst.Add(strSource.Substring(0, index));

                GetStr(strSource.Substring(index + 1), lst);
            }
            else
            {
                lst.Add(strSource);
            }
        }

        /// <summary>
        /// 获取批量操作的状态
        /// </summary>
        /// <param name="media_id"></param>
        /// <param name="OPType"></param>
        /// <returns></returns>
        public ActionResult GetStatus(string media_id, int? OPType)
        {
            var ret = MailListApi.BatchJobStatus<JobResultObjectUser>(GetToken(), media_id);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllVisiblePersonGroup(string appId, string groupId)
        {
            List<PersonGroup> list = new List<PersonGroup>();
            int accountManageId = int.Parse(Request.Cookies["AccountManageId"].Value);
            if (string.IsNullOrEmpty(groupId))
            {
                list = ((WeChatCommonService)_commonService).GetAllPersonAndGroup(appId, accountManageId);
            }
            else
            {
                list = ((WeChatCommonService)_commonService).GetAllVisibleByGroup(appId, groupId, accountManageId);
            }


            return Json(new
            {
                PersonGroup = list
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSubPersonGroup(string appId, string groupId)
        {
            int accountManageId = int.Parse(Request.Cookies["AccountManageId"].Value);
            var list = ((WeChatCommonService)_commonService).GetAllVisibleByGroup(appId, groupId, accountManageId);
            return Json(new
            {
                PersonGroup = list
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetToken()
        {
            var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.AccountManageId == AccountManageID);
            return AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
        }

    }

    public class TagImport
    {
        //Tag标签对应的ID
        public string tagid { get; set; }

        //对应Tag标签要导入的用户list
        public List<string> userlist { get; set; }

        //企业部门ID列表
        public List<int> partylist { get; set; }
    }


}
