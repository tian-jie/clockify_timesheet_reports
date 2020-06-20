/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：MenuController.cs
    文件功能描述：自定义菜单设置工具Controller
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using System.Text;
using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Logging;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Extensions;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.UI;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.QY;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using Infrastructure.Utility.Filter;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.Entities;
using Microsoft.SqlServer.Server;
using WebBackgrounder;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using System.Transactions;
using Newtonsoft.Json;
using System.Reflection;
using System.ComponentModel;
using Innocellence.WeChat.Domain.ModelsView;
using Infrastructure.Utility.IO;
using System.Text.RegularExpressions;
using Innocellence.Weixin.QY.CommonAPIs;
using System.Globalization;
using System.Collections.Concurrent;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.Weixin.Exceptions;
using Innocellence.WeChat.Domain;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace Innocellence.WeChatMain.Controllers
{
    public class UserWechatController : BaseController<SysAddressBookMember, AddressBookMemberView>
    {
        private static ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(WeChatCommonService)));

        public IBatchJobLogService _batchJobLogService;
        public IAddressBookService _addressBookService;
        //public IDepartmentTempService _tempService;
        public UserWechatController(ICategoryService newsService, IBatchJobLogService batchJobLogService,
            IAddressBookService addressBookService)
            : base(addressBookService)
        {
            // _newsService = newsService;
            _batchJobLogService = batchJobLogService;
            _addressBookService = addressBookService;
            //_tempService = tempService;
        }

        //
        // GET: /Menu/

        private List<SysAddressBookMember> MemberList
        {
            get
            {
                var lst = cacheManager.Get<List<SysAddressBookMember>>("MemberItem" + this.AccountManageID, () =>
                  {
                      return _addressBookService.GetAllAddressBookMember(this.AccountManageID);
                  });
                return lst;
            }
        }

        public override ActionResult Index()
        {
            //string departments = System.IO.File.ReadAllText("c:\\a.json");
            //var data = JsonConvert.DeserializeObject<departmentData>(departments);
            //foreach (var item in data.items)
            //{
            //    _tempService.Repository.Insert(item.ConvertEntity);
            //}
            //var ds = _tempService.Repository.Entities.OrderBy(a=>a.departId).ToList();
            //foreach (var item in ds)
            //{
            //    if (item.departId >= 191)
            //    {
            //        item.name = item.name.Replace("&amp;", "&");
            //        var r = MailListApi.CreateDepartment(GetToken(), item.name, item.parent, 1, item.departId);
            //        if (r.errcode != Weixin.ReturnCode_QY.请求成功)
            //        {

            //        }
            //    }
            //}

            ViewBag.TagList = WeChatCommonService.GetTagListByAccountManageID(this.AccountManageID);
            return View();
        }

        public ActionResult SyncMember()
        {
            _addressBookService.SyncMember(this.AccountManageID);
            WeChatCommonService.ClearMemberLst(this.AccountManageID);
            cacheManager.Remove("DepartmentList");
            return SuccessNotification("Success");
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

        public ActionResult JobList()
        {
            return View();
        }

        public ActionResult GetPageReportList()
        {
            GridRequest req = new GridRequest(Request);
            Expression<Func<BatchJobLog, bool>> predicate = FilterHelper.GetExpression<BatchJobLog>(req.FilterGroup);
            int iCount = req.PageCondition.RowCount;
            String accessDate = Request["AccessDate"];
            string strStartTime = accessDate + " 00:00:00";
            string strEndTime = accessDate + " 23:59:59";

            if (!string.IsNullOrEmpty(accessDate))
            {
                DateTime startDate = DateTime.Parse(strStartTime);
                DateTime endDate = DateTime.Parse(strEndTime);
                predicate = predicate.AndAlso(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);
            }

            var list = _batchJobLogService.GetList<BatchJobLogView>(predicate, req.PageCondition);
            return GetPageResult(list, req);
        }


        public override List<AddressBookMemberView> GetListEx(Expression<Func<SysAddressBookMember, bool>> predicate, PageCondition ConPage)
        {
            var list = GetListData(ConPage);// _newsService.GetList(null, null, 10, 10);
            if (list == null)
            {
                list = new List<AddressBookMemberView>();
            }
            var departmentLst = WeChatCommonService.lstDepartment(AccountManageID);

            list.ForEach(a =>
            {
                var names = new List<string>();
                foreach (var dId in a.DepartmentIds)
                {
                    var department = departmentLst.Where(d => d.id == dId).FirstOrDefault();
                    if (department != null)
                    {
                        names.Add(department.name);
                    }
                }
                a.DisplayDepartmentNames = string.Join(", ", names);
            });

            return list;
        }

        //public override ActionResult GetList()
        //{
        //    PageParameter param = new PageParameter();
        //    TryUpdateModel(param);

        //    GridRequest req = new GridRequest(Request);

        //    //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
        //    param.length = param.length == 0 ? 10 : param.length;
        //    int iTotal = param.iRecordsTotal;
        //    //Defect #14487 bug修正
        //    //string strWeChatUserID = Request["WeChatUserID"];

        //    var list = GetListData((int)Math.Floor(param.start / param.length * 1.0d) + 1, param.length, ref iTotal, req.PageCondition);// _newsService.GetList(null, null, 10, 10);
        //    if (list == null)
        //    {
        //        list = new List<AddressBookMemberView>();
        //    }
        //    var departmentLst = WeChatCommonService.lstDepartment(AccountManageID);
        //    list.ForEach(a =>
        //    {
        //        var names = new List<string>();
        //        foreach (var dId in a.DepartmentIds)
        //        {
        //            var department = departmentLst.Where(d => d.id == dId).FirstOrDefault();
        //            if (department != null)
        //            {
        //                names.Add(department.name);
        //            }
        //        }
        //        a.DisplayDepartmentNames = string.Join(", ", names);
        //    });

        //    return Json(new
        //    {
        //        sEcho = param.draw,
        //        iTotalRecords = iTotal,
        //        iTotalDisplayRecords = iTotal,
        //        aaData = list
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

        public List<AddressBookMemberView> GetListData(PageCondition con)
        {
            string strID = Request["DeptId"];
            //Defect #14487 bug修正
            string strSearchCondition = Request["SearchCondition"];

            var departmentLst = WeChatCommonService.lstDepartment(AccountManageID);
            var root = departmentLst.Where(a => a.parentid == 0).First();
            int rootId;
            if (string.IsNullOrEmpty(strID))
            {
                rootId = root.id;
            }
            else
            {
                rootId = int.Parse(strID);
            }

            var departmentIds = new List<int>() { rootId };
            var departmentIdsDB = new List<string>();
            //当页面左侧所选部门不为最上层根部门时，得到所选的部门及其下所有子部门
            if (rootId != root.id)
            {
                List<int> cursorIds = new List<int>() { rootId };
                var subs = new List<DepartmentList>();
                do
                {
                    subs = departmentLst.Where(a => cursorIds.Contains(a.parentid)).ToList();
                    departmentIds.AddRange(subs.Select(a => a.id));
                    cursorIds.Clear();
                    cursorIds.AddRange(subs.Select(a => a.id));
                } while (subs.Count > 0);


                departmentIdsDB = departmentIds.Select(a => "," + a.ToString() + ",").ToList();

                _Logger.Debug(departmentIdsDB);
            }

            //Expression<Func<SysAddressBookMember, bool>> predicate =
            //    a => a.DeleteFlag != 1 && a.AccountManageId == AccountManageID && (departmentIdsDB.Contains(a.Department) || departmentIdsDB.Contains(a.DXYDepartment) || departmentIdsDB.Contains(a.FishflowDepartment));

            Expression<Func<SysAddressBookMember, bool>> predicate = a => a.DeleteFlag != 1 && a.AccountManageId == AccountManageID;
            //按部门过滤用户记录，看用户是否在所选部门或其子部门内，当所选为根部门时不走下面的逻辑是为了提高一些效率
            if (departmentIdsDB.Count > 0)
            {
                Expression<Func<SysAddressBookMember, bool>> departmentPredicate = a => false;
                foreach (var item in departmentIdsDB)
                {
                    departmentPredicate = departmentPredicate.OrElse(a => a.Department.Replace("[", ",").Replace("]", ",").Contains(item));
                }
                predicate = predicate.AndAlso(departmentPredicate);
            }


            if (!string.IsNullOrEmpty(strSearchCondition))
            {
                // Defect#15596修正&性能改善
                strSearchCondition = strSearchCondition.Trim().ToLower();
                predicate = predicate.AndAlso(x => x.UserId.ToLower().Contains(strSearchCondition) ||
                    (x.Email != null && x.Email.ToLower().Contains(strSearchCondition)) ||
                    x.UserName.ToLower().Contains(strSearchCondition) ||
                    (x.EmployeeNo != null && x.EmployeeNo.ToLower().Contains(strSearchCondition)) ||
                    (x.Mobile != null && x.Mobile.Contains(strSearchCondition)));
            }

            return _addressBookService.GetList<AddressBookMemberView>(predicate, con);

            //var lst = this.MemberList
            //.Select(i => new AddressBookMemberView() { ConvertEntity = i })
            //.Where(predicate.Compile());

            //iTotal = lst.Count();
            //return lst.Skip((iPage - 1) * iCount).Take(iCount).ToList();
        }

        private string GetToken()
        {
            var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.AccountManageId == AccountManageID);
            return AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
        }

        /// <summary>
        /// 批量操作--导入成员
        /// </summary>
        /// <param name="OPType"></param>
        /// <returns></returns>
        public ActionResult batchuser(int? OPType)
        {
            string strRet = "[\"jsonrpc\" : \"2.0\", \"result\" : null, \"id\" : \"{0}\"]";

            string strErrRet = "[\"jsonrpc\" : \"2.0\", \"error\" : [\"code\": {0}, \"message\": \"{1}\"], \"id\" : \"id\"]";

            string operateType = "";

            try
            {
                if (Request.Files.Count == 0)
                {
                    strRet = string.Format(strErrRet, "100", "未发现上传文件！");
                    return Content(strRet);
                }
                BatchUserResult objRet = null;
                using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                   new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    Dictionary<string, Stream> dic = new Dictionary<string, Stream>();

                    for (int i = 0; i < Request.Files.Count; i++)
                    { DealUploadFile(dic, Request.Files[i]); }

                    if (dic.Count > 0)
                    {
                        #region 同步到微信
                        var ret = MediaApi.Upload(GetToken(), UploadMediaFileType.file, dic, "");

                        System.Threading.Thread.Sleep(1000);

                        //上传失败
                        if (!string.IsNullOrEmpty(ret.errmsg))
                        {
                            strRet = string.Format(strErrRet, ret.errcode, ret.errmsg);
                            return Content(strRet);
                        }

                        var objConfig = WeChatCommonService.lstSysWeChatConfig.Find(a => a.AccountManageId == AccountManageID);
                        string callbackUrl = CommonService.GetSysConfig(SysConfigCode.CallBackUrl, "");//从Sysconfig数据库中读url
                        BatchUser objUser = new BatchUser()
                        {
                            media_id = ret.media_id,
                            callback = new callback()
                            {
                                token = objConfig.WeixinToken,// Token,
                                encodingaeskey = objConfig.WeixinEncodingAESKey,// EncodingAESKey,
                                url = callbackUrl + string.Format("?appid={0}", objConfig.Id) //app.innocellence.com/wechatqy/led
                            }

                        };

                        if (OPType == 1)
                        {
                            objRet = MailListApi.BatchUpdateUser(GetToken(), objUser);
                            operateType = "sync_user";
                        }
                        else if (OPType == 2)
                        {
                            objRet = MailListApi.BatchReplaceUser(GetToken(), objUser);
                            operateType = "replace_user";
                        }
                        else if (OPType == 3)
                        {
                            objRet = MailListApi.BatchReplaceDept(GetToken(), objUser);
                            operateType = "replace_party";
                        }
                        else
                        {
                            objRet = new BatchUserResult()
                            {
                                errmsg = "参数错误！"
                            };
                        }
                        if (objRet.errcode == Weixin.ReturnCode.请求成功)
                        {
                            transactionScope.Complete();
                        }
                        #endregion
                    }
                    else
                    {
                        strRet = string.Format(strErrRet, "200", "批量更新成员成功, 但此次更新不会同步到微信 ");
                        transactionScope.Complete();
                    }
                }
                //当执行导入操作时将返回的jobId插入到表中去
                if (objRet != null)
                {
                    if (!string.IsNullOrEmpty(objRet.errmsg))
                    {
                        strRet = string.Format(strRet, objRet.jobid);
                        var jobLog = new BatchJobLog()
                        {
                            JobID = objRet.jobid,
                            Status = 0,
                            Type = operateType,
                            CreatedDate = DateTime.Now
                        };

                        _batchJobLogService.Repository.Insert(jobLog);//插入表中
                    }
                    else
                    {
                        strRet = string.Format(strErrRet, objRet.errcode, objRet.errmsg);
                    }
                }
                if (this.MemberList != null)
                {
                    cacheManager.Remove("MemberItem" + this.AccountManageID);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                strRet = string.Format(strErrRet, "102", ex.Message);
            }

            return Content(strRet.Replace("[", "{").Replace("]", "}"));
        }

        /// <summary>
        /// 得到上传的文件编码方式，避免乱码问题
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        private static Encoding GetFileEncodeType(Stream fs)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            Byte[] buffer = br.ReadBytes(2);
            Encoding result = null;
            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    result = System.Text.Encoding.UTF8;
                }
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    result = System.Text.Encoding.BigEndianUnicode;
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    result = System.Text.Encoding.Unicode;
                }
                else
                {
                    result = System.Text.Encoding.Default;
                }
            }
            else
            {
                result = System.Text.Encoding.Default;
            }
            fs.Position = 0;
            return result;
        }

        private void DealUploadFile(Dictionary<string, Stream> dic, HttpPostedFileBase objFile)
        {
            var allDeptList = MailListApi.GetDepartmentList(GetToken()).department;
            var dDepartmentId = int.Parse(CommonService.GetSysConfig("DimissionDepartment", ""));
            List<string> columnNames;
            bool canInsertNew;
            List<string> needUploadColumns;
            var modelList = GetTemplateModels(objFile, out columnNames, out canInsertNew, out needUploadColumns);
            if (modelList.Count > 0)
            {
                #region 更新DB
                if (columnNames.Contains("Department"))
                {
                    modelList.ForEach(m =>
                    {
                        if (string.IsNullOrEmpty(m.EmployeeStatus) || m.EmployeeStatus.Equals("D", StringComparison.CurrentCultureIgnoreCase))
                        {
                            //#871,上传/新建用户时若状态为离职,归类到离职人员部门。
                            m.DepartmentIds = new int[] { dDepartmentId };
                        }
                        else if (!string.IsNullOrEmpty(m.OrgFullName))
                        {
                            var department = GetSamePathDepartment(allDeptList, m.OrgFullName);
                            if (department == null)
                            {
                                var departments = m.OrgFullName.Split(">".ToCharArray()).ToList();
                                int parentId = 1;
                                string parentPath = null;
                                string path = null;
                                for (int j = 0; j < departments.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        path = departments[j];
                                    }
                                    else
                                    {
                                        parentPath = path;
                                        path = path + ">" + departments[j];
                                    }
                                    var org = GetSamePathDepartment(allDeptList, path);
                                    if (org != null)
                                    {
                                        parentId = org.id;
                                    }
                                    else
                                    {
                                        if (j > 0)
                                        {
                                            var parent = GetSamePathDepartment(allDeptList, parentPath);
                                            parentId = parent.id;
                                        }
                                        var result = MailListApi.CreateDepartment(GetToken(), departments[j], parentId);
                                        if (result.errcode == Weixin.ReturnCode_QY.请求成功)
                                        {
                                            if (j == (departments.Count - 1))
                                            {
                                                m.DepartmentIds = new int[] { result.id };
                                            }
                                            allDeptList.Add(new DepartmentList() { id = result.id, name = departments[j], level = 0, order = 1, parentid = parentId });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                m.DepartmentIds = new int[] { department.id };
                            }
                        }
                    });
                }
                var report = _addressBookService.InsertOrUpdateAddressBookMember(modelList, Request, columnNames, canInsertNew);
                var message = report.ToString();
                if (!string.IsNullOrEmpty(message))
                {
                    throw new Exception(message);
                }
                #endregion
            }
            cacheManager.Remove("DepartmentList");
            //如果有微信需要的全部列, 则更新到微信
            //姓名 帐号 微信号 手机号	邮箱 所在部门 职位
            //其中账号和微信号没有匹配的列,因此只要有其余5列,则认为符合微信官方模板的限制
            if (needUploadColumns.Count == 5)
            {
                #region 同步到微信时,组装上传的Dictionary
                var cutModelList = new List<AddressBookMemberTemplateModel>();
                modelList.ForEach(m =>
                {
                    if (string.IsNullOrEmpty(m.EmployeeStatus) || m.EmployeeStatus.Equals("D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        m.DepartmentIds = new int[] { dDepartmentId };
                        var apiModel = new AddressBookMemberView() { ConvertTemplateModel = m }.ConvertApiModel;
                        _addressBookService.CutPropertyBefaultSync2TX(apiModel);
                        m = new AddressBookMemberView() { ConvertApiModel = apiModel }.ConvertTemplateModel;
                        cutModelList.Add(m);
                    }
                    else
                    {
                        cutModelList.Add(m);
                    }
                });
                var stream = _addressBookService.GetUploadCsv(cutModelList, needUploadColumns);
                dic.Add(objFile.FileName, stream);
                #endregion
            }
        }

        /// <summary>
        /// 根据路径匹配路径完全相同的部门，而不是仅仅名字相同
        /// </summary>
        /// <param name="allDeptList"></param>
        /// <param name="departmentFullPath"></param>
        /// <returns></returns>
        private DepartmentList GetSamePathDepartment(List<DepartmentList> allDeptList, string departmentFullPath)
        {
            var departmentSplits = departmentFullPath.Split(">".ToCharArray()).ToList();
            var sameNameDepartment = allDeptList.Where(d => d.name.Equals(departmentSplits.Last().Trim(), StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (sameNameDepartment != null && sameNameDepartment.Count > 0)
            {
                foreach (var item in sameNameDepartment)
                {
                    StringBuilder pathBuilder = new StringBuilder();
                    pathBuilder.Append(item.name);
                    var cursor = item;
                    for (var parent = allDeptList.Where(d => d.id == cursor.parentid).FirstOrDefault();
                        cursor.parentid != 0 && parent != null;
                        cursor = parent, parent = allDeptList.Where(d => d.id == cursor.parentid).FirstOrDefault())
                    {
                        pathBuilder.Insert(0, parent.name + ">");
                    }
                    if (departmentFullPath.Equals(pathBuilder.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 从上传的文件数据中，转化出AddressBookMemberTemplateModel对象列表，方便后续操作
        /// </summary>
        /// <param name="objFile"></param>
        /// <param name="columnNames"></param>
        /// <param name="canInsertNew"></param>
        /// <param name="uploadColumnNames"></param>
        /// <returns></returns>
        private List<AddressBookMemberTemplateModel> GetTemplateModels(HttpPostedFileBase objFile, out List<string> columnNames, out bool canInsertNew, out List<string> uploadColumnNames)
        {
            uploadColumnNames = new List<string>();
            var modelList = new List<AddressBookMemberTemplateModel>();
            var encodeType = GetFileEncodeType(objFile.InputStream);
            IWorkbook workbook = null;
            // 2007版本  
            if (objFile.FileName.IndexOf(".xlsx") > 0)
            { workbook = new XSSFWorkbook(objFile.InputStream); }
            // 2003版本  
            else if (objFile.FileName.IndexOf(".xls") > 0)
            { workbook = new HSSFWorkbook(objFile.InputStream); }
            var sheet = workbook.GetSheetAt(0);
            var firstRow = sheet.GetRow(0);
            int cellCount = firstRow.LastCellNum;
            int rowCount = sheet.LastRowNum;
            //StreamReader reader = new StreamReader(objFile.InputStream, encodeType);
            Dictionary<int, PropertyInfo> columnInfo = new Dictionary<int, PropertyInfo>();
            //var headers = reader.ReadLine();
            var properties = typeof(AddressBookMemberTemplateModel).GetProperties();
            //var headerArray = headers.Split(",\t".ToCharArray());
            for (int j = firstRow.FirstCellNum; j < cellCount; j++)
            {
                ICell cell = firstRow.GetCell(j);
                var headerName = cell.StringCellValue;
                foreach (var prop in properties)
                {
                    var attr = prop.GetAttribute<DescriptionAttribute>();
                    if (attr != null && attr.Description.Equals(headerName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var uploadAttr = prop.GetAttribute<UploadColumnAttribute>();
                        if (uploadAttr != null && !"性别".Equals(uploadAttr.ColumnName, StringComparison.OrdinalIgnoreCase))
                        {
                            uploadColumnNames.Add(uploadAttr.ColumnName);
                        }
                        else if (attr.Description.Equals("OrgName", StringComparison.InvariantCultureIgnoreCase))
                        {
                            uploadColumnNames.Add("所在部门");
                        }
                        columnInfo.Add(j, prop);
                        break;
                    }
                }
            }
            var reportBuilder = ValidateColumn(columnInfo, out columnNames, out canInsertNew);
            if (reportBuilder != null)
            {
                string invalidReport = reportBuilder.ToString();
                throw new Exception(invalidReport);
            }
            //for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
            for (var i = 1; i <= rowCount; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                var templateModel = new AddressBookMemberTemplateModel();
                //var itemArray = line.Split(",\t".ToCharArray());
                for (int k = 0; k <= row.LastCellNum; k++)
                {
                    var cell = row.GetCell(k);
                    if (cell == null)
                    {
                        continue;
                    }
                    string iValue;
                    switch (cell.CellType)
                    {
                        case CellType.Boolean:
                            iValue = cell.BooleanCellValue.ToString();
                            break;
                        case CellType.Numeric:
                            iValue = cell.NumericCellValue.ToString();
                            break;
                        case CellType.String:
                            iValue = cell.StringCellValue;
                            break;
                        default:
                            iValue = cell.StringCellValue;
                            break;
                    }

                    if (string.IsNullOrEmpty(iValue) || iValue.Equals("NULL", StringComparison.CurrentCultureIgnoreCase))
                    {
                        iValue = null;
                    }
                    else if (columnInfo[k].Name.Equals("Mobile", StringComparison.CurrentCultureIgnoreCase))
                    {
                        iValue = Regex.Replace(iValue, @"[\(（]86[\)）]", string.Empty);
                        iValue = iValue.Replace("(", "+").Replace("（", "+").Replace(")", string.Empty).Replace("）", string.Empty);
                    }

                    columnInfo[k].SetValue(templateModel, iValue);
                }
                modelList.Add(templateModel);
            }
            StringBuilder invalidReportBuilder = ValidateCell(modelList, columnNames, uploadColumnNames.Count == 7);
            if (!string.IsNullOrEmpty(invalidReportBuilder.ToString()))
            {
                invalidReportBuilder.Insert(0, objFile.FileName + ":<br>");
                string invalidReport = invalidReportBuilder.ToString();
                throw new Exception(invalidReport);
            }
            return modelList;
        }

        /// <summary>
        /// 验证模板文件列结构是否合法
        /// </summary>
        /// <param name="columnInfo"></param>
        /// <param name="columnNames"></param>
        /// <param name="canInsertNew"></param>
        /// <returns></returns>
        private StringBuilder ValidateColumn(Dictionary<int, PropertyInfo> columnInfo, out List<string> columnNames, out bool canInsertNew)
        {
            StringBuilder report = new StringBuilder();
            columnNames = new List<string>();
            canInsertNew = false;
            if (columnInfo.Count < 2)
            {
                report.Append("模板中至少要有两列，且其中一列为EmployeeNo");
                return report;
            }
            var keyColumn = columnInfo.Values.Where(a => a.Name.Equals("EmployeeNo", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var nameColumn = columnInfo.Values.Where(a => a.Name.Equals("UserName", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var mobileColumn = columnInfo.Values.Where(a => a.Name.Equals("Mobile", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var emailColumn = columnInfo.Values.Where(a => a.Name.Equals("Email", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var orgNameColumn = columnInfo.Values.Where(a => a.Name.Equals("OrgName", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var orgFullNameColumn = columnInfo.Values.Where(a => a.Name.Equals("OrgFullName", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var genderColumn = columnInfo.Values.Where(a => a.Name.Equals("Gender", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var positionColumn = columnInfo.Values.Where(a => a.Name.Equals("Position", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (keyColumn == null)
            {
                report.Append("模板中至少要有两列，且其中一列为EmployeeNo");
                return report;
            }
            if ((orgNameColumn == null && orgFullNameColumn != null) || (orgFullNameColumn == null && orgNameColumn != null))
            {
                report.Append("OrgName与OrgFullName两列需要同时具备。");
                return report;
            }

            if (!((nameColumn == null && (mobileColumn == null && emailColumn == null) && orgNameColumn == null)
                || (nameColumn != null && !(mobileColumn == null && emailColumn == null) && orgNameColumn != null)))
            {
                report.Append("CName，Mobile，Email，OrgName，OrgFullName需要同时具备。");
                return report;
            }


            if (nameColumn == null || (mobileColumn == null && emailColumn == null) || (orgNameColumn == null && orgFullNameColumn == null))
            {
                canInsertNew = false;
            }
            else
            {
                canInsertNew = true;
            }

            foreach (var item in columnInfo)
            {
                if (item.Value.Name.Equals("EmployeeNo") || item.Value.Name.Equals("OrgFullName"))
                {
                    continue;
                }
                else if (item.Value.Name.Equals("OrgName"))
                {
                    columnNames.Add("Department");
                    columnNames.Add("DXYDepartment");
                    columnNames.Add("FishflowDepartment");
                }
                else if (item.Value.Name.Equals("LabourUnion"))
                {
                    columnNames.Add("LabourUnionStatus");
                }
                else
                {
                    columnNames.Add(item.Value.Name);
                }
            }
            return null;
        }

        /// <summary>
        /// 验证模板文件表格里数据是否合法
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="columnNames"></param>
        /// <param name="needUploadToWX"></param>
        /// <returns></returns>
        private StringBuilder ValidateCell(List<AddressBookMemberTemplateModel> modelList, List<string> columnNames, bool needUploadToWX)
        {
            var members = _addressBookService.GetAllAddressBookMember(this.AccountManageID);
            var phoneSet = new ConcurrentDictionary<string, string>();
            var emailSet = new ConcurrentDictionary<string, string>();
            var weiXinSet = new ConcurrentDictionary<string, string>();
            if (members != null && members.Count > 0)
            {
                foreach (var member in members)
                {
                    if (!string.IsNullOrEmpty(member.Mobile))
                    {
                        phoneSet[member.Mobile.ToUpper()] = member.EmployeeNo;
                    }
                    if (!string.IsNullOrEmpty(member.Email))
                    {
                        emailSet[member.Email.ToUpper()] = member.EmployeeNo;
                    }
                    if (!string.IsNullOrEmpty(member.WeiXinId))
                    {
                        weiXinSet[member.WeiXinId.ToUpper()] = member.EmployeeNo;
                    }
                }
            }
            int lineNumber = 2;
            StringBuilder invalidReportBuilder = new StringBuilder();
            foreach (var item in modelList)
            {
                ValidateModel(invalidReportBuilder, item, lineNumber, phoneSet, emailSet, weiXinSet, columnNames, needUploadToWX);
                lineNumber++;
            }
            return invalidReportBuilder;
        }

        /// <summary>
        /// 验证模板文件表格里数据是否合法的具体逻辑
        /// </summary>
        /// <param name="invalidReportBuilder"></param>
        /// <param name="model"></param>
        /// <param name="lineNumber"></param>
        /// <param name="phoneSet"></param>
        /// <param name="emailSet"></param>
        /// <param name="weiXinSet"></param>
        /// <param name="columnNames"></param>
        /// <param name="needUploadToWX"></param>
        private void ValidateModel(StringBuilder invalidReportBuilder, AddressBookMemberTemplateModel model, int lineNumber,
            IDictionary<string, string> phoneSet, IDictionary<string, string> emailSet, IDictionary<string, string> weiXinSet, List<string> columnNames, bool needUploadToWX)
        {
            bool hasEmployeeNoValue = !string.IsNullOrEmpty(model.EmployeeNo);
            if (string.IsNullOrEmpty(model.EmployeeNo))
            {
                invalidReportBuilder.AppendFormat("EmployeeNo 在第 {0} 行为空 <br>", lineNumber);
            }
            if (columnNames.Contains("Mobile") && !string.IsNullOrEmpty(model.Mobile) &&
                phoneSet.Keys.Contains<string>(model.Mobile, StringComparer.CurrentCultureIgnoreCase) &&
                !string.IsNullOrWhiteSpace(phoneSet[model.Mobile.ToUpper()]) &&
                !phoneSet[model.Mobile.ToUpper()].Equals(model.EmployeeNo, StringComparison.CurrentCultureIgnoreCase))
            {
                invalidReportBuilder.AppendFormat("Mobile 重复于第 {0} 行 <br>", lineNumber);
            }

            if (columnNames.Contains("Email") && !string.IsNullOrEmpty(model.Email) &&
                emailSet.Keys.Contains<string>(model.Email, StringComparer.CurrentCultureIgnoreCase) &&
                !string.IsNullOrWhiteSpace(emailSet[model.Email.ToUpper()]) &&
                !emailSet[model.Email.ToUpper()].Equals(model.EmployeeNo, StringComparison.CurrentCultureIgnoreCase))
            {
                invalidReportBuilder.AppendFormat("Email 重复于第 {0} 行 <br>", lineNumber);
            }

            if (columnNames.Contains("WeiXinId") && !string.IsNullOrEmpty(model.WeiXinId) &&
                weiXinSet.Keys.Contains<string>(model.WeiXinId, StringComparer.CurrentCultureIgnoreCase) &&
                !string.IsNullOrWhiteSpace(weiXinSet[model.WeiXinId.ToUpper()]) &&
                !weiXinSet[model.WeiXinId.ToUpper()].Equals(model.EmployeeNo, StringComparison.CurrentCultureIgnoreCase))
            {
                invalidReportBuilder.AppendFormat("WeiXinId 重复于第 {0} 行 <br>", lineNumber);
            }

            if (hasEmployeeNoValue && !needUploadToWX)
            {
                bool hasOneValue = !AllValueIsNull(model);
                if (!hasOneValue)
                {
                    invalidReportBuilder.AppendFormat("模板中第 {0} 行, 除EmployeeNo有值外, 至少要有一列有值 <br>", lineNumber);
                }
            }
            else
            {
                if ((columnNames.Contains("Email") || columnNames.Contains("Mobile") || columnNames.Contains("WeiXinId")) &&
                    string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.Mobile) && string.IsNullOrEmpty(model.WeiXinId))
                {
                    invalidReportBuilder.AppendFormat("Email 和 Phone 在第 {0} 行不可同时为空 <br>", lineNumber);
                }

                if (columnNames.Contains("UserName") && string.IsNullOrEmpty(model.UserName))
                {
                    invalidReportBuilder.AppendFormat("CName 在第 {0} 行为空 <br>", lineNumber);
                }

                if ((columnNames.Contains("OrgName") || columnNames.Contains("OrgFullName")) && string.IsNullOrEmpty(model.OrgName))
                {
                    invalidReportBuilder.AppendFormat("OrgName 在第 {0} 行为空 <br>", lineNumber);
                }

                if ((columnNames.Contains("OrgName") || columnNames.Contains("OrgFullName")) && string.IsNullOrEmpty(model.OrgFullName))
                {
                    invalidReportBuilder.AppendFormat("OrgFullName 在第 {0} 行为空 <br>", lineNumber);
                }
            }

            var departmentLst = WeChatCommonService.lstDepartment(AccountManageID);
            var root = departmentLst.Where(d => d.parentid == 0).First();
            if (!string.IsNullOrEmpty(model.OrgFullName) && !model.OrgFullName.StartsWith(root.name + ">", StringComparison.CurrentCultureIgnoreCase))
            {
                invalidReportBuilder.AppendFormat("第 {0} 行，OrgFullName 要以 \\\"{1}\\\" 开头 <br>", lineNumber, root.name);
            }

            if (!string.IsNullOrEmpty(model.OrgName) && !string.IsNullOrEmpty(model.OrgFullName)
                && !model.OrgFullName.EndsWith(">" + model.OrgName, true, CultureInfo.CurrentCulture))
            {
                invalidReportBuilder.AppendFormat("OrgFullName 在第 {0} 行未以 OrgName 结尾  <br>", lineNumber);
            }

            if (!string.IsNullOrEmpty(model.Position) && model.Position.Length > 32)
            {
                invalidReportBuilder.AppendFormat("Position 在第 {0} 行超过长度限制 <br>", lineNumber);
            }

            if (!string.IsNullOrEmpty(model.LabourUnion) && (!model.LabourUnion.Equals("Y", StringComparison.CurrentCultureIgnoreCase) && !model.LabourUnion.Equals("N", StringComparison.CurrentCultureIgnoreCase)))
            {
                invalidReportBuilder.AppendFormat("LabourUnion 在第 {0} 行不合法 <br>", lineNumber);
            }
        }

        private bool AllValueIsNull(AddressBookMemberTemplateModel model)
        {
            return string.IsNullOrEmpty(model.Birthday)
                && string.IsNullOrEmpty(model.City)
                && string.IsNullOrEmpty(model.CompanyID)
                && string.IsNullOrEmpty(model.Department)
                && string.IsNullOrEmpty(model.DirectManagerID)
                && string.IsNullOrEmpty(model.Email)
                && string.IsNullOrEmpty(model.EmployeeStatus)
                && string.IsNullOrEmpty(model.EnName)
                && string.IsNullOrEmpty(model.Gender)
                && string.IsNullOrEmpty(model.GradeCode)
                && string.IsNullOrEmpty(model.LabourUnion)
                && string.IsNullOrEmpty(model.Mobile)
                && string.IsNullOrEmpty(model.OrgFullName)
                && string.IsNullOrEmpty(model.OrgName)
                && string.IsNullOrEmpty(model.Position)
                && string.IsNullOrEmpty(model.UserId)
                && string.IsNullOrEmpty(model.UserName)
                && string.IsNullOrEmpty(model.WeiXinId)
                && model.Status == null;
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

        /// <summary>
        /// 获取异步任务结果
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetJobResult(string jobId, string type)
        {
            string errormsg = "";
            object ret = null;
            //根据jobId去取result
            var jobLog = _batchJobLogService.GetLogByJobId(jobId);

            if (string.IsNullOrEmpty(jobLog.Result))
            {
                errormsg = "获取异步任务结果失败，请过段时间后再尝试";
                return Json(new { msg = errormsg }, JsonRequestBehavior.AllowGet);
            }

            switch (type)
            {
                case "sync_user":
                case "replace_user":
                    var ret1 = JsonHelper.FromJson<List<JobResultObjectUser>>(jobLog.Result);
                    ret = ret1;
                    break;
                case "invite_user":
                    var ret2 = JsonHelper.FromJson<List<JobResultObjectInvite>>(jobLog.Result);
                    ret = ret2;
                    break;
                case "replace_party":
                    var ret3 = JsonHelper.FromJson<List<JobResultObjectParty>>(jobLog.Result);
                    ret = ret3;
                    break;
                default:
                    errormsg = "获取异步任务结果失败，请检查JobID和Type是否正确";
                    ret = null;
                    break;
            }

            return Json(new { lst = ret, msg = errormsg, batchType = type }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetBatchNotification(RequestMessageEvent_Batch_Job_Result jobResult)
        //{
        //    return Json(doJson(null), JsonRequestBehavior.AllowGet);
        //}

        public override ActionResult Get(string Id)
        {
            string strID = Request["Id"];
            //if (!string.IsNullOrEmpty(strID))
            //{

            //    return Json(MailListApi.GetMember(GetToken(), Id), JsonRequestBehavior.AllowGet);
            //}
            if (!string.IsNullOrEmpty(strID))
            {
                var entity = _addressBookService.GetMemberById(int.Parse(strID));
                var departmentTagMap = WeChatCommonService.lstDepartmentTagMap(this.AccountManageID);
                var view = new AddressBookMemberView { ConvertEntity = entity };
                view.DepartmentTagList = new ConcurrentDictionary<string, string>();
                var departmentLst = WeChatCommonService.lstDepartment(AccountManageID);
                var names = new List<string>();
                foreach (var dId in view.DepartmentIds)
                {
                    var department = departmentLst.Where(d => d.id == dId).FirstOrDefault();
                    if (department != null)
                    {
                        names.Add(department.name);
                    }

                    if (departmentTagMap.Keys.Contains(dId))
                    {
                        var tags = departmentTagMap[dId];
                        tags.ForEach(t => view.DepartmentTagList[t.Item1] = t.Item2);
                    }
                }
                view.DisplayDepartmentNames = string.Join(", ", names);
                return Json(view, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMember(string userId)
        {
            var entity = _addressBookService.GetMemberByUserId(userId);
            if (entity != null)
            {
                return Json(new AddressBookMemberView { ConvertEntity = entity }, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public bool BeforeAddOrUpdate(AddressBookMemberView objModal)
        {
            //后台校验 Go here..
            bool validate = true;
            StringBuilder errMsg = new StringBuilder();
            string departId = Request["departmentAll"];

            //if (string.IsNullOrEmpty(objModal.UserId))
            //{
            //    validate = false;
            //    errMsg.Append(T("Please input UserID.<br/>"));
            //}
            if (string.IsNullOrEmpty(objModal.EmployeeNo))
            {
                validate = false;
                errMsg.Append(T("请输入 EmployeeNo.<br/>"));
            }
            if (!string.IsNullOrEmpty(objModal.EmployeeNo))
            {
                var otherEmp = _addressBookService.Repository.Entities.Where(e => e.EmployeeNo.Equals(objModal.EmployeeNo) && e.Id != objModal.Id && e.DeleteFlag != 1 && e.AccountManageId == this.AccountManageID).FirstOrDefault();
                if (otherEmp != null)
                {
                    validate = false;
                    errMsg.Append(T("EmployeeNo与已有用户重复.<br/>"));
                }
            }

            if (string.IsNullOrEmpty(objModal.UserName))
            {
                validate = false;
                errMsg.Append(T("请输入姓名.<br/>"));
            }

            if (string.IsNullOrEmpty(departId))
            {
                validate = false;
                errMsg.Append(T("请选择部门.<br/>"));
            }
            string regex = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            //if (string.IsNullOrEmpty(objModal.Email))
            //{
            //    validate = false;
            //    errMsg.Append(T("请输入邮箱.<br/>"));
            //}三种信息不可同时为空
            if (string.IsNullOrEmpty(objModal.Email) && string.IsNullOrEmpty(objModal.Mobile) && string.IsNullOrEmpty(objModal.WeiXinId))
            {
                validate = false;
                errMsg.Append(T("邮箱、微信、手机三种信息不可同时为空.<br/>"));
            }
            if (!string.IsNullOrEmpty(objModal.Email) && !Regex.IsMatch(objModal.Email, regex))
            {
                validate = false;
                errMsg.Append(T("邮箱不合法.<br/>"));
            }

            if (!validate)
            {
                ModelState.AddModelError("输入不合法", errMsg.ToString());
            }

            return validate;
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult PostUser(AddressBookMemberView objModal)
        {
            //验证错误
            if (!BeforeAddOrUpdate(objModal) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            int[] strs = Request["departmentAll"].Split(',').Select(a => int.Parse(a)).ToArray();
            //int[] strs = JsonConvert.DeserializeObject<int[]>(Request["departmentAll"]);
            objModal.DepartmentIds = strs;
            //objModal.UserId = objModal.UserId.Trim();
            objModal.UserName = objModal.UserName.Trim();
            objModal.Position = objModal.Position != null ? objModal.Position.Trim() : "";
            objModal.WeiXinId = objModal.WeiXinId != null ? objModal.WeiXinId.Trim() : "";
            objModal.Mobile = objModal.Mobile != null ? objModal.Mobile.Trim() : "";
            objModal.Email = objModal.Email != null ? objModal.Email.Trim() : "";
            if (!string.IsNullOrEmpty(Request["UserTags"]))
            {
                objModal.TagList = Request["UserTags"].Split(',').Select(a => int.Parse(a)).OrderBy(a => a).Distinct().ToArray();
            }

            //_Logger.Debug("Mobile:{0}" ,objModal.Mobile);

            var dDepartmentID = int.Parse(CommonService.GetSysConfig("DimissionDepartment", ""));
            if (!string.IsNullOrEmpty(objModal.UserId))
            {
                var oldEntity = _addressBookService.GetMemberById(objModal.Id);
                int[] oldTags = null;
                if (!string.IsNullOrEmpty(oldEntity.TagList))
                {
                    oldTags = JsonConvert.DeserializeObject<int[]>(oldEntity.TagList);
                }
                using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                      new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    var entity = objModal.ConvertEntity;
                    var apiModel = objModal.ConvertApiModel;
                    if (objModal.EmployeeStatus == null || string.IsNullOrEmpty(entity.EmployeeStatus) || entity.EmployeeStatus.Equals("D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //离职用户部门修改为离职人员部门
                        apiModel.department = new int[] { dDepartmentID };
                        entity.Department = JsonConvert.SerializeObject(new int[] { dDepartmentID });
                        entity.DXYDepartment = null;
                        entity.FishflowDepartment = entity.Department;
                    }
                    //entity.AccountManageId = Request.Cookies["AccountManageId"].Value;
                    _addressBookService.UpdateMember(entity);


                    //_Logger.Debug("apiModel Mobile:{0}", apiModel.mobile);


                    if (apiModel.status == 1 || apiModel.department.Contains(dDepartmentID))
                    {
                        _addressBookService.CutPropertyBefaultSync2TX(apiModel);
                    }
                    var wxResult = MailListApi.UpdateMember(GetToken(), apiModel);
                    if (wxResult.errcode == Weixin.ReturnCode.请求成功)
                    {
                        //修改用户标签逻辑
                        List<int> addTagList = new List<int>();
                        List<int> removeTagList = new List<int>();
                        if (objModal.TagList != null && oldTags != null)
                        {
                            addTagList = objModal.TagList.Except(oldTags).ToList();
                            removeTagList = oldTags.Except(objModal.TagList).ToList();

                        }
                        else if (objModal.TagList != null && oldTags == null)
                        {
                            addTagList = objModal.TagList.ToList();
                        }
                        else if (objModal.TagList == null && oldTags != null)
                        {
                            removeTagList = oldTags.ToList();
                        }
                        foreach (var item in addTagList)
                        {
                            MailListApi.AddTagMember(GetToken(), item, new string[] { objModal.UserId });
                        }
                        foreach (var item in removeTagList)
                        {
                            MailListApi.DelTagMember(GetToken(), item, new string[] { objModal.UserId }, null);
                        }
                        transactionScope.Complete();
                    }
                    else
                    {
                        throw new Exception(wxResult.errmsg);
                    }
                }
            }
            else
            {
                objModal.UserId = _addressBookService.GenerateUserId(objModal.EmployeeNo);
                var entity = objModal.ConvertEntity;
                using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                   new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    entity.AccountManageId = this.AccountManageID;//int.Parse(Request.Cookies["AccountManageId"].Value);
                    //#871
                    if (objModal.EmployeeStatus == null || string.IsNullOrEmpty(entity.EmployeeStatus) || entity.EmployeeStatus.Equals("D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        entity.Department = JsonConvert.SerializeObject(new int[] { dDepartmentID });
                        entity.DXYDepartment = null;
                        entity.FishflowDepartment = entity.Department;
                    }
                    _addressBookService.InsertAddressBookMember(entity);
                    //CutPropertyBefaultSync2TX(objModal);
                    var apiModel = objModal.ConvertApiModel;
                    if (objModal.EmployeeStatus == null || string.IsNullOrEmpty(entity.EmployeeStatus) || entity.EmployeeStatus.Equals("D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var departmentID = int.Parse(CommonService.GetSysConfig("DimissionDepartment", ""));
                        apiModel.department = new int[] { departmentID };
                        _addressBookService.CutPropertyBefaultSync2TX(apiModel);
                    }
                    var wxResult = MailListApi.CreateMember(GetToken(), apiModel);
                    if (wxResult.errcode == Weixin.ReturnCode.请求成功)
                    {
                        if (objModal.TagList != null)
                        {
                            foreach (var tag in objModal.TagList)
                            {
                                MailListApi.AddTagMember(GetToken(), tag, new string[] { objModal.UserId });
                            }
                        }
                        transactionScope.Complete();
                    }
                }
            }
            //添加或更新后会清除掉缓存
            //if (WeChatCommonService.lstUser != null)
            //{
            //    cacheManager.Remove("UserItem");
            //}
            if (this.MemberList != null)
            {
                cacheManager.Remove("MemberItem" + this.AccountManageID);
            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public override JsonResult Delete(string sIds)
        {
            if (!string.IsNullOrEmpty(sIds))
            {
                ILogger log = LogManager.GetLogger("DeleteUser");
                log.Warn("{0} is deleting {1}", base.User.Identity.Name, sIds);
                string[] arrID = sIds.Split(',');
                foreach (string strID in arrID)
                {
                    if (string.IsNullOrEmpty(strID)) { continue; }
                    using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                    {
                        _addressBookService.DeleteMember(strID);
                        try
                        {
                            var wxResult = MailListApi.DeleteMember(GetToken(), strID);
                            if (wxResult.errcode == Weixin.ReturnCode_QY.请求成功)
                            {
                                transactionScope.Complete();
                            }
                        }
                        catch (ErrorJsonResultException e)
                        {
                            if ((int)e.JsonResult.errcode == 60111)
                            {
                                transactionScope.Complete();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
                //删除后会清除掉缓存
                //if (WeChatCommonService.lstUser != null)
                //{
                //    cacheManager.Remove("UserItem");
                //}
                //if (this.MemberList != null)
                //{
                cacheManager.Remove("MemberItem" + this.AccountManageID);
                //}
            }

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RefreshUserData()
        {
            foreach (var member in this.MemberList)
            {
                if (string.IsNullOrEmpty(member.Avatar))
                {
                    var apiResult = MailListApi.GetMember(GetToken(), member.UserId);
                    if (apiResult.errcode == Weixin.ReturnCode_QY.请求成功)
                    {
                        member.Avatar = apiResult.avatar;
                        if (string.IsNullOrEmpty(member.WeiXinId) && !string.IsNullOrEmpty(apiResult.weixinid))
                        {
                            member.WeiXinId = apiResult.weixinid;
                        }
                        if (string.IsNullOrEmpty(member.Email) && !string.IsNullOrEmpty(apiResult.email))
                        {
                            member.Email = apiResult.email;
                        }
                        if (string.IsNullOrEmpty(member.Mobile) && !string.IsNullOrEmpty(apiResult.mobile))
                        {
                            member.Mobile = apiResult.mobile;
                        }
                        _addressBookService.UpdateMember(member);
                        _Logger.Debug("Update member {0}", member.UserId);
                    }
                }
            }
            return Json(new { Data = 200 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult QueryUser(string queryStr)
        {
            var users = (from data in _addressBookService.Repository.Entities
                         where data.UserName.Contains(queryStr) && data.DeleteFlag != 1 && data.AccountManageId == this.AccountManageID
                         orderby data.UserName.Length
                         select data).Take(10);
            List<PersonGroup> result = new List<PersonGroup>();
            foreach (var user in users)
            {
                PersonGroup p = new PersonGroup()
                {
                    Type = "Person",
                    WeixinId = user.UserId,
                    WeixinName = user.UserName,
                    Avatar = user.Avatar,
                    Email = user.Email,
                };
                result.Add(p);
            }
            return Json(new
            {
                PersonGroup = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SyncUserTagIndex()
        {
            return View();
        }

        public JsonResult SyncUserTag()
        {
            try
            {
                var tagList = WeChatCommonService.GetTagListByAccountManageID(this.AccountManageID);
                _Logger.Debug("tag count:{0}", tagList.Count);
                Dictionary<string, List<int>> userTagMapping = new Dictionary<string, List<int>>();
                this._addressBookService.InitTagListTemp();
                foreach (var tag in tagList)
                {
                    int tagId = -1;
                    if (int.TryParse(tag.tagid, out tagId) && tagId > 0)
                    {
                        var tagContent = MailListApi.GetTagMember(GetToken(), tagId);
                        if (null != tagContent.userlist)
                        {
                            List<string> userList = new List<string>();
                            foreach (var user in tagContent.userlist)
                            {
                                userList.Add(user.userid);
                                if (userList.Count >= 500)
                                {
                                    DoUpdateTagListTemp(userList, tagId);
                                }
                            }
                            DoUpdateTagListTemp(userList, tagId);
                        }
                    }
                }
                this._addressBookService.UpdateTagListByTagListTemp();
                return SuccessNotification("同步成功.");
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                return ErrorNotification(ex);
            }
        }

        private void DoUpdateTagListTemp(List<string> userList, int tagId)
        {
            if (userList != null && userList.Count > 0)
            {
                this._addressBookService.UpdateTagListTemp(userList, tagId);
                userList.Clear();
            }
        }
    }

}
