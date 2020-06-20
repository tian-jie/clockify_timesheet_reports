using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EntityFramework.BulkInsert.Extensions;
using System.Data.Entity;
using System.Transactions;
using Infrastructure.Core;
using Innocellence.WeChat.WebService;
using Innocellence.WeChat.Domain.ViewModel;
using Infrastructure.Core.Logging;
using System.IO;
using System.Reflection;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.CommonAPIs;
using Infrastructure.Utility.Extensions;
using Innocellence.Weixin.QY;

namespace Innocellence.WeChat.Domain.Service
{
    public class AddressBookService : BaseService<SysAddressBookMember>, IAddressBookService
    {
        IRepository<SysAddressBookMemberTemp, int> _tempRepository;
        IBatchJobLogService _batchJobLogService;
        private ILogger logger = LogManager.GetLogger(typeof(AddressBookService));
        public AddressBookService(IRepository<SysAddressBookMemberTemp, int> tempRepository, IBatchJobLogService batchJobLogService)
        {
            _tempRepository = tempRepository;
            _batchJobLogService = batchJobLogService;
        }

        public AddressBookService()
        {
        }

        public StringBuilder InsertOrUpdateAddressBookMember(List<AddressBookMemberTemplateModel> models, HttpRequestBase request, List<string> updateColumns, bool canInsertNew)
        {
            StringBuilder report = new StringBuilder();
            List<SysAddressBookMember> insertTeam = new List<SysAddressBookMember>();
            List<SysAddressBookMember> updateTeam = new List<SysAddressBookMember>();
            //var oldMembers = this.Repository.Entities.Where(a => a.DeleteFlag != 1).ToList();
            foreach (var model in models)
            {
                var item = new AddressBookMemberView() { ConvertTemplateModel = model }.ConvertEntity;
                var oldMember = this.Repository.Entities.FirstOrDefault(a => a.DeleteFlag != 1 && item.EmployeeNo.Equals(a.EmployeeNo, StringComparison.CurrentCultureIgnoreCase));
                if (oldMember == null)
                {
                    if (canInsertNew)
                    {
                        item.UserId = GenerateUserId(item.EmployeeNo);
                        model.UserId = item.UserId;
                        item.CreateTime = DateTime.Now;
                        item.AccountManageId = int.Parse(request.Cookies["AccountManageId"].Value);
                        insertTeam.Add(item);
                    }
                    else
                    {
                        report.Append(string.Format("模板中缺少必要属性列，无法新增人员条目{0}。", model.EmployeeNo));
                        return report;
                    }
                }
                else
                {
                    //在进行判断时使用model中的属性进行,避免因Convert中进行的转换导致结果不正确
                    model.UserId = oldMember.UserId;
                    if (updateColumns.Contains("UserName") && !string.IsNullOrEmpty(model.UserName))
                    {
                        oldMember.UserName = item.UserName;
                    }
                    if (updateColumns.Contains("Position") && !string.IsNullOrEmpty(model.Position))
                    {
                        oldMember.Position = item.Position;
                    }
                    if (updateColumns.Contains("Gender") && !string.IsNullOrEmpty(model.Gender))
                    {
                        oldMember.Gender = item.Gender;
                    }
                    if (updateColumns.Contains("Email") && !string.IsNullOrEmpty(model.Email))
                    {
                        oldMember.Email = item.Email;
                    }
                    if (updateColumns.Contains("Department") && !string.IsNullOrEmpty(model.Department))
                    {
                        oldMember.Department = item.Department;
                        var departmentList = JsonConvert.DeserializeObject<List<int>>(item.Department);
                        var dxyDepartments = departmentList.Where(a => a < 10000).ToList();
                        var fishflowDepartments = departmentList.Where(a => a >= 10000).ToList();
                        oldMember.DXYDepartment = JsonConvert.SerializeObject(dxyDepartments);
                        oldMember.FishflowDepartment = JsonConvert.SerializeObject(fishflowDepartments);
                    }
                    if (updateColumns.Contains("Mobile") && !string.IsNullOrEmpty(model.Mobile))
                    {
                        oldMember.Mobile = item.Mobile;
                    }
                    //oldMember.WeiXinId = item.WeiXinId;
                    if (updateColumns.Contains("City") && !string.IsNullOrEmpty(model.City))
                    {
                        oldMember.City = item.City;
                    }
                    if (updateColumns.Contains("CompanyID") && !string.IsNullOrEmpty(model.CompanyID))
                    {
                        oldMember.CompanyID = item.CompanyID;
                    }
                    if (updateColumns.Contains("DirectManagerID") && !string.IsNullOrEmpty(model.DirectManagerID))
                    {
                        oldMember.DirectManagerID = item.DirectManagerID;
                    }
                    if (updateColumns.Contains("GradeCode") && !string.IsNullOrEmpty(model.GradeCode))
                    {
                        oldMember.GradeCode = item.GradeCode;
                    }
                    if (updateColumns.Contains("EnName") && !string.IsNullOrEmpty(model.EnName))
                    {
                        oldMember.EnName = item.EnName;
                    }
                    if (updateColumns.Contains("Birthday") && !string.IsNullOrEmpty(model.Birthday))
                    {
                        oldMember.Birthday = item.Birthday;
                    }
                    if (updateColumns.Contains("LabourUnionStatus") && !string.IsNullOrEmpty(model.LabourUnion))
                    {
                        oldMember.LabourUnionStatus = item.LabourUnionStatus;
                    }
                    if (updateColumns.Contains("EmployeeStatus") && !string.IsNullOrEmpty(model.EmployeeStatus))
                    {
                        oldMember.EmployeeStatus = string.IsNullOrEmpty(item.EmployeeStatus) ? "U" : item.EmployeeStatus;
                    }
                    model.Status = oldMember.Status;
                    //var updateColumns = new List<string>() { "UserName", "Position", "Gender", "Email", "Department", "DXYDepartment", "FishflowDepartment", "Mobile", "WeiXinId", "City", "CompanyID", "DirectManagerID", "EnName", "Birthday", "LabourUnionStatus", "EmployeeStatus", "GradeCode" };
                    //this.Repository.Update(oldMember, updateColumns);
                    updateTeam.Add(oldMember);
                }
            }
            updateTeam.ForEach(a => this.Repository.Update(a, updateColumns));
            this.Repository.Insert((IEnumerable<SysAddressBookMember>)insertTeam);
            return report;
        }

        public void InsertAddressBookMember(SysAddressBookMember item)
        {
            item.CreateTime = DateTime.Now;
            if (item.DeleteFlag == null)
            {
                item.DeleteFlag = 0;
            }
            this.Repository.Insert(item);
        }

        public List<SysAddressBookMember> GetAllAddressBookMember(int accountManageId)
        {
            return this.Repository.Entities.Where(a => a.DeleteFlag != 1 && a.AccountManageId == accountManageId).ToList();
        }


        public SysAddressBookMember GetMemberByUserId(string userId, bool ignoreDeleted = false)
        {
            if (ignoreDeleted)
            {
                return this.Repository.Entities.Where(a => a.UserId.Equals(userId, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            }
            else
            {
                return this.Repository.Entities.Where(a => a.UserId.Equals(userId, StringComparison.CurrentCultureIgnoreCase) && a.DeleteFlag != 1).FirstOrDefault();
            }
        }

        public SysAddressBookMember GetMemberById(int id)
        {
            return this.Repository.Entities.Where(a => a.Id == id && a.DeleteFlag != 1).FirstOrDefault();
        }

        public void UpdateMember(SysAddressBookMember member)
        {
            var oldMember = this.Repository.Entities.Where(a => a.Id == member.Id).FirstOrDefault();

            if (oldMember != null)
            {
                oldMember.UserName = member.UserName;
                oldMember.Position = member.Position;
                oldMember.Gender = member.Gender;
                oldMember.GradeCode = member.GradeCode;
                oldMember.Email = member.Email;
                oldMember.City = member.City;
                oldMember.Birthday = member.Birthday;
                oldMember.Status = member.Status;
                oldMember.CompanyID = member.CompanyID;
                oldMember.DirectManagerID = member.DirectManagerID;
                oldMember.EmployeeNo = member.EmployeeNo;
                oldMember.EnName = member.EnName;
                oldMember.Department = member.Department;
                var departmentList = JsonConvert.DeserializeObject<List<int>>(member.Department);
                var dxyDepartments = departmentList.Where(a => a < 10000).ToList();
                var fishflowDepartments = departmentList.Where(a => a >= 10000).ToList();
                oldMember.DXYDepartment = JsonConvert.SerializeObject(dxyDepartments);
                oldMember.FishflowDepartment = JsonConvert.SerializeObject(fishflowDepartments);
                oldMember.EmployeeStatus = member.EmployeeStatus;
                oldMember.Mobile = member.Mobile;
                oldMember.Avatar = member.Avatar;
                oldMember.TagList = member.TagList;
                oldMember.LabourUnionStatus = member.LabourUnionStatus;
                oldMember.SubscribeTime = member.SubscribeTime;
                var columnList = new List<string>() { "UserName", "Position", "Gender", "GradeCode", "Email", "City", "Birthday", "Status", "CompanyID",
                "DirectManagerID", "EmployeeNo", "EnName", "Department","DXYDepartment","FishflowDepartment", "EmployeeStatus", "Mobile", "Avatar", "TagList", "LabourUnionStatus","SubscribeTime"};
                this.Repository.Update(oldMember, columnList);
            }
        }

        public void DeleteMember(string userId)
        {
            var entity = this.Repository.Entities.Where(a => a.UserId.Equals(userId, StringComparison.CurrentCultureIgnoreCase) && a.DeleteFlag != 1).FirstOrDefault();
            if (entity != null)
            {
                entity.DeleteFlag = 1;
                var columnList = new List<string>() { "DeleteFlag" };
                this.Repository.Update(entity, columnList);
            }
        }

        /// <summary>
        /// 生成用户EmployeeNo算法
        /// </summary>
        /// <param name="employeeNo"></param>
        /// <returns></returns>
        public string GenerateUserId(string employeeNo)
        {
            return string.Format("NO{0}{1}", Math.Abs(employeeNo.GetHashCode()), Math.Abs(DateTime.Now.GetHashCode()));
        }

        /// <summary>
        /// 同步成员功能，与fishflow通信
        /// </summary>
        /// <param name="accountManageId"></param>
        public void SyncMember(int accountManageId)
        {
            var url = CommonService.GetSysConfig("FishflowUrl", "");
            var userName = CommonService.GetSysConfig("FishflowUserName", "");
            var password = CommonService.GetSysConfig("FishflowPassword", "");
            var whiteCompany = CommonService.GetSysConfig("FishflowWhiteCompany", "");
            var dDepartmentID = int.Parse(CommonService.GetSysConfig("DimissionDepartment", ""));
            EmployeeInfo client = new EmployeeInfo()
            {
                Url = url,
                GCSoapHeaderValue = new GCSoapHeader()
                { UserName = userName, Password = password }
            };
            int total = 0;
            int index = 1;
            //每次通信得到50条记录
            int pageCount = 50;
            HashSet<string> employeeNoBag = new HashSet<string>();
            do
            {
                //fishflow通信
                var result = client.GetEmployeeInfo2(index, pageCount);
                result = EncryptionHelper.DecryptDES(result);
                //得到返回结果反序列化成我们的对象
                var view = JsonConvert.DeserializeObject<EmployeeInfoView>(result);
                var currentEmployeeList = new List<EmployeeData>();
                if (index == 1)
                {
                    total = view.Data.TotalQty - pageCount;
                }
                else
                {
                    total -= pageCount;
                }
                index++;
                if (view.IsSuccess)
                {
                    foreach (var e in view.Data.EmployeeInfo)
                    {
                        //fishflow的部门在我们这里部门ID要加10000，以示与丁香园区分
                        if (e.BUID != null)
                        {
                            e.BUID += 10000;
                        }
                        if (e.SBUID != null)
                        {
                            e.SBUID += 10000;
                        }
                        if (e.GroupID != null)
                        {
                            e.GroupID += 10000;
                        }
                        //经确认，只有部门子公司的员工需要同步
                        if (!string.IsNullOrEmpty(whiteCompany))
                        {
                            var companies = whiteCompany.Split(",".ToArray()).Select(a => a.Trim()).ToList();
                            if (e.CompanyID == null || !companies.Contains(e.CompanyID.ToString()))
                            {
                                continue;
                            }
                        }
                        //经确认，符合7开头长度是9位的员工号员工不需要同步
                        if (!string.IsNullOrEmpty(e.EmployeeNo) && e.EmployeeNo.StartsWith("7") && e.EmployeeNo.Length == 9)
                        {
                            continue;
                        }
                        //经确认，fishflow中重复的用户只添加第一个
                        if (employeeNoBag.Contains(e.EmployeeNo))
                        {
                            continue;
                        }
                        else
                        {
                            employeeNoBag.Add(e.EmployeeNo);
                            currentEmployeeList.Add(e);
                        }
                    }
                    //同步成员信息到我们数据库，先将数据存入SysAddressBookMemberTemp缓存表，方便对比变更的用户信息
                    var ctx = (DbContext)_tempRepository.UnitOfWork;
                    var entities = currentEmployeeList.Select(e =>
                    {
                        var department = JsonConvert.SerializeObject(GetDepartmentId(e, accountManageId, dDepartmentID));
                        return new SysAddressBookMemberTemp()
                        {
                            EmployeeNo = e.EmployeeNo,
                            Status = 4, //未关注
                            EmployeeStatus = e.EmployeeStatus,
                            UserName = e.CName,
                            EnName = e.EName,
                            AccountManageId = accountManageId,
                            //Avatar = 
                            //Birthday =
                            Department = department,
                            FishflowDepartment = department,
                            UserId = GenerateUserId(e.EmployeeNo),
                            Gender = GetGenderNumber(e.Gender),
                            DirectManagerID = e.DirectManagerID,
                            CompanyID = e.CompanyID != null ? e.CompanyID.ToString() : string.Empty,
                            GradeCode = e.GradeCode,
                            City = e.City,
                            Mobile = GetMobileString(e.Mobile),
                            Position = GetPositionString(e.Position),
                            Email = e.Email,
                            DeleteFlag = 0,
                            CreateTime = DateTime.Now,
                            Extend1 = e.ADAccount,
                        };
                    }).Where(e => !e.Department.Equals("[]", StringComparison.CurrentCultureIgnoreCase));
                    using (var transactionScope = new TransactionScope())
                    {
                        #region SQL update
                        var options = new BulkInsertOptions
                        {
                            EnableStreaming = true,
                        };
                        ctx.BulkInsert(entities, options);
                        ctx.SaveChanges();
                        //经确认，已关注用户只更新姓名，性别，部门，在职离职状态变化的；
                        //未关注用户只更新姓名，性别，手机，邮箱，部门，在职离职状态变化的；
                        string findChangeSubscribeUserSQL = @"select t.id from SysAddressBookMember m join SysAddressBookMemberTemp t on m.EmployeeNo = t.EmployeeNo where m.[Status] = 1 and (m.DeleteFlag != 1 or m.DeleteFlag is null) and
                        (m.[UserName] != t.[UserName] or m.[Gender] != t.[Gender] or m.[FishflowDepartment] != t.[FishflowDepartment] or m.[EmployeeStatus] != t.[EmployeeStatus]) ";
                        string findChangeUnsubscribeUserSQL = @"select t.id from SysAddressBookMember m join SysAddressBookMemberTemp t on m.EmployeeNo = t.EmployeeNo where m.[Status] != 1 and (m.DeleteFlag != 1 or m.DeleteFlag is null) and
                        (m.[UserName] != t.[UserName] or m.[Gender] != t.[Gender] or m.[Mobile] != t.[Mobile] or m.[Email] != t.[Email] or m.[FishflowDepartment] != t.[FishflowDepartment] or m.[EmployeeStatus] != t.[EmployeeStatus]) ";
                        string findChangeUserSQL = @"select 
                        tt.[Id], tt.[Gender],tt.[UserName],tt.[EnName],tt.[Mobile],tt.[Position],tt.[Email],tt.[City],tt.[FishflowDepartment],tt.[Extend1],
                        CASE ISNULL(mm.[DXYDepartment], '') WHEN '' Then tt.[Department]
                        ELSE REPLACE(tt.[Department], ']',',')+REPLACE(mm.[DXYDepartment], '[','') End as Department,
                        mm.[WeiXinId],mm.[Status],mm.[Avatar],mm.[GradeCode],mm.[EmployeeNo],
                        mm.[EmployeeStatus],mm.[Birthday],mm.[CompanyID],mm.[DirectManagerID],
                        mm.[DeleteFlag],mm.[UserId] ,mm.[CreateTime],mm.[AccountManageId],mm.[TagList],mm.[LabourUnionStatus] 
                        from SysAddressBookMemberTemp tt join SysAddressBookMember mm on tt.EmployeeNo = mm.EmployeeNo where mm.AccountManageId = tt.AccountManageId and tt.id in (({0}) union ({1}))";
                        string findNewUserSQL = @"select * from SysAddressBookMemberTemp where EmployeeStatus = 'A' and EmployeeNo in (select EmployeeNo from SysAddressBookMemberTemp except (select EmployeeNo from SysAddressBookMember where (DeleteFlag!=1 or DeleteFlag is null) and AccountManageId = {0}))";
                        string updateOldUserSQL = @"update SysAddressBookMember 
                                                                    set [UserName] = SysAddressBookMemberTemp.UserName,             
                                                                    [EmployeeStatus] = SysAddressBookMemberTemp.EmployeeStatus,
                                                                    [EnName] = SysAddressBookMemberTemp.EnName,
                                                                    [FishflowDepartment] = SysAddressBookMemberTemp.Department,
                                                                    [Department] = CASE ISNULL([DXYDepartment], '') WHEN '' Then SysAddressBookMemberTemp.Department
                                                                                               ELSE REPLACE(SysAddressBookMemberTemp.Department, ']',',')+REPLACE([DXYDepartment], '[','')
                                                                                               End,
                                                                    [Gender] = SysAddressBookMemberTemp.Gender,
                                                                    [City] = SysAddressBookMemberTemp.City,
                                                                    [Mobile] = SysAddressBookMemberTemp.Mobile,
                                                                    [Position] = SysAddressBookMemberTemp.Position,
                                                                    [Email] = SysAddressBookMemberTemp.Email,
                                                                    [DirectManagerID] = SysAddressBookMemberTemp.DirectManagerID,
                                                                    [CompanyID] = SysAddressBookMemberTemp.CompanyID,
                                                                    [GradeCode] = SysAddressBookMemberTemp.GradeCode,
                                                                    [Extend1] = SysAddressBookMemberTemp.Extend1
                                                                    from SysAddressBookMemberTemp
                                                                    where SysAddressBookMember.EmployeeNo = SysAddressBookMemberTemp.EmployeeNo
                                                                    and SysAddressBookMember.AccountManageId = SysAddressBookMemberTemp.AccountManageId
                                                                    and (SysAddressBookMember.DeleteFlag != 1 or SysAddressBookMember.DeleteFlag is null)";

                        StringBuilder builder = new StringBuilder("insert into SysAddressBookMember (");
                        string tableValues = @"[WeiXinId],[Status],[Gender],[UserName],[EnName],[Mobile],[Avatar],[Position],[Email],[Department],[FishflowDepartment],
                                                               [GradeCode],[EmployeeNo],[EmployeeStatus],[City],[Birthday],[CompanyID],[DirectManagerID],
                                                               [DeleteFlag],[UserId] ,[CreateTime],[AccountManageId],[TagList],[Extend1]";
                        builder.Append(tableValues);
                        builder.AppendFormat(") select {0} ", tableValues);
                        builder.Append(@"from SysAddressBookMemberTemp where EmployeeStatus = 'A' and EmployeeNo in (select EmployeeNo from SysAddressBookMemberTemp except (select EmployeeNo from SysAddressBookMember where (DeleteFlag!=1 or DeleteFlag is null)  and AccountManageId = {0}))");
                        string insertNewUserSQL = builder.ToString();
                        var changeObjects = _tempRepository.SqlQuery(string.Format(findChangeUserSQL, findChangeSubscribeUserSQL, findChangeUnsubscribeUserSQL)).ToList();
                        var newObjects = _tempRepository.SqlQuery(string.Format(findNewUserSQL, accountManageId)).ToList();
                        changeObjects.AddRange(newObjects);

                        logger.Debug("Start changeObjects! Count:{0} ", changeObjects.Count);

                        changeObjects.ForEach(a =>
                        {
                            if (!string.IsNullOrEmpty(a.EmployeeStatus) && a.EmployeeStatus.Equals("D", StringComparison.CurrentCultureIgnoreCase))
                            {
                                a.Department = JsonConvert.SerializeObject(new int[] { dDepartmentID });
                            }
                        });
                        var apiModels = changeObjects.Select(e => e.ConvertToMember()).Select(m => (new AddressBookMemberView() { ConvertEntity = m }).ConvertApiModel).ToList();
                        apiModels.ForEach(a =>
                        {
                            if (a.status == 1 || a.department.Contains(dDepartmentID))
                            {
                                //裁减掉不需要同步到腾讯后台的用户信息
                                CutPropertyBefaultSync2TX(a);
                            }
                        });
                        var templateModels = apiModels.Select(a => (new AddressBookMemberView() { ConvertApiModel = a }).ConvertTemplateModel).ToList();
                        this.Repository.SqlExcute(updateOldUserSQL);
                        this.Repository.SqlExcute(insertNewUserSQL, accountManageId);
                        //this.Repository.SqlExcute(updateTempUserIdSQL, accountManageId);
                        #endregion

                        //同步成员信息到腾讯后台
                        #region Batch API update
                        if (templateModels.Count > 0)
                        {
                            Dictionary<string, Stream> dic = new Dictionary<string, Stream>();
                            //var templateModels = _tempRepository.Entities.ToList().Select(e => e.ConvertToMember()).Select(m => (new AddressBookMemberView() { ConvertEntity = m }).ConvertTemplateModel).ToList();
                            templateModels.ForEach(t =>
                            {
                                if (!string.IsNullOrEmpty(t.Position) && t.Position.IndexOf(",") > 0)
                                {
                                    t.Position = "\"" + t.Position + "\"";
                                }
                                if (!string.IsNullOrEmpty(t.UserName) && t.UserName.IndexOf(",") > 0)
                                {
                                    t.UserName = "\"" + t.UserName + "\"";
                                }
                            });
                            var stream = GetUploadCsv(templateModels);
                            dic.Add("上传模板.csv", stream);

                            logger.Debug("Start upload User to WeChat Server! Count:{0} ", templateModels.Count);

                            var ret = MediaApi.Upload(GetToken(accountManageId), UploadMediaFileType.file, dic, "");
                            System.Threading.Thread.Sleep(200);
                            if (ret.errcode == Weixin.ReturnCode_QY.请求成功)
                            {
                                var objConfig = WeChatCommonService.lstSysWeChatConfig.Find(a => a.AccountManageId == accountManageId);
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
                                try
                                {

                                    var objRet = MailListApi.BatchUpdateUser(GetToken(accountManageId), objUser);
                                    if (objRet.errcode == Weixin.ReturnCode.请求成功)
                                    {
                                        var jobLog = new BatchJobLog()
                                        {
                                            JobID = objRet.jobid,
                                            Status = 0,
                                            Type = "sync_user",
                                            CreatedDate = DateTime.Now
                                        };

                                        _batchJobLogService.Repository.Insert(jobLog);//插入表中

                                        //清除临时缓存数据
                                        _tempRepository.Delete(a => true);

                                        transactionScope.Complete();
                                    }
                                    else
                                    {
                                        logger.Error("An error occurred while uploading page: {0} msg:{1}", index, objRet.errmsg);
                                    }
                                }
                                catch (Exception e)
                                {
                                    logger.Error("An error occurred while uploading page {0}", index);
                                    logger.Error("CallbackURL: {0}", objUser.callback.url);
                                    logger.Error(e.ToString());
                                }
                            }
                        }
                        else
                        {
                            //清除临时缓存数据
                            _tempRepository.Delete(a => true);
                            transactionScope.Complete();
                        }

                        #endregion
                    }
                }
                else
                {
                    logger.Error("Sync Member Page Error: {0}, Index is {1}.", view.Message, index);
                }
            }
            while (total > 0);
        }

        /// <summary>
        /// 处理性别信息
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        private int GetGenderNumber(string gender)
        {
            int result = 0;
            if (string.IsNullOrWhiteSpace(gender))
            {
                result = 0;
            }
            else if (gender.Trim().Equals("男", StringComparison.InvariantCultureIgnoreCase)
                || gender.Trim().Equals("Male", StringComparison.InvariantCultureIgnoreCase))
            {
                result = 1;
            }
            else if (gender.Trim().Equals("女", StringComparison.InvariantCultureIgnoreCase)
                || gender.Trim().Equals("Female", StringComparison.InvariantCultureIgnoreCase))
            {
                result = 2;
            }
            return result;
        }

        /// <summary>
        /// 处理手机信息
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        private string GetMobileString(string mobile)
        {
            if (!string.IsNullOrEmpty(mobile))
            {
                mobile = mobile.Replace("(86)", string.Empty);
            }
            return mobile;
        }

        /// <summary>
        /// 处理职称信息
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private string GetPositionString(string position)
        {
            if (string.IsNullOrEmpty(position))
            {
                return null;
            }
            else
            {
                if (position.Length > 64)
                {
                    position = position.Substring(0, 64);
                }
                return position;
            }
        }

        /// <summary>
        /// 处理部门Id信息
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="accountManageId"></param>
        /// <param name="dismissDepartmentId"></param>
        /// <returns></returns>
        private List<int> GetDepartmentId(EmployeeData employee, int accountManageId, int dismissDepartmentId)
        {
            string GroupName = employee.GroupName;
            string BUName = employee.BUName;
            string SBUName = employee.SBUName;
            List<int> result = new List<int>();
            var departments = WeChatCommonService.lstDepartment(accountManageId);
            if (!string.IsNullOrEmpty(employee.EmployeeStatus) && employee.EmployeeStatus.Equals("D", StringComparison.CurrentCultureIgnoreCase))
            {
                result.Add(dismissDepartmentId);
                return result;
            }
            int targetId = 0;
            if (!string.IsNullOrEmpty(GroupName))
            {
                int GroupId = employee.GroupID.Value;
                GroupName = GroupName.Trim();
                targetId = GetTargetDepartmentId(accountManageId, GroupName, GroupId, departments, 1);
            }
            if (!string.IsNullOrEmpty(BUName))
            {
                int BUId = employee.BUID.Value;
                BUName = BUName.Trim();
                targetId = GetTargetDepartmentId(accountManageId, BUName, BUId, departments, targetId);
            }
            if (!string.IsNullOrEmpty(SBUName))
            {
                int SBUId = employee.SBUID.Value;
                SBUName = SBUName.Trim();
                targetId = GetTargetDepartmentId(accountManageId, SBUName, SBUId, departments, targetId);
            }
            if (targetId != 0)
            {
                result.Add(targetId);
            }
            return result;
        }

        private int GetTargetDepartmentId(int accountManageId, string groupName, int groupId, List<DepartmentList> departments, int parentId)
        {
            int targetId = 0;
            //groupName = (groupId - 10000)+ "_" + groupName;
            if (groupName.Length > 32)
            {
                groupName = groupName.Substring(0, 32);
            }
            //var groupDepartment = departments.Find(d => d.name.Trim().Equals(groupName, StringComparison.CurrentCultureIgnoreCase));
            var groupDepartment = departments.Find(d => d.id == groupId);
            var groupDepartmentSameName = departments.Find(d => d.name.Trim().Equals(groupName, StringComparison.CurrentCultureIgnoreCase) && d.parentid == parentId);
            if (groupDepartment == null)
            {
                if (groupDepartmentSameName != null)
                {
                    targetId = groupDepartmentSameName.id;
                }
                else
                {
                    var ret = MailListApi.CreateDepartment(GetToken(accountManageId), groupName, parentId, 1, groupId);
                    if (ret.errcode == Weixin.ReturnCode_QY.请求成功)
                    {
                        targetId = ret.id;
                        departments.Add(new DepartmentList() { id = ret.id, name = groupName, parentid = parentId, level = 0, order = 1 });
                    }
                }
            }
            else
            {
                targetId = groupDepartment.id;
                if (!groupDepartment.name.Equals(groupName, StringComparison.CurrentCultureIgnoreCase)
                    || groupDepartment.parentid != parentId)
                {
                    var ret = MailListApi.UpdateDepartment(GetToken(accountManageId), groupId.ToString(), groupName, parentId);
                    if (ret.errcode == Weixin.ReturnCode_QY.请求成功)
                    {
                        groupDepartment.name = groupName;
                        groupDepartment.parentid = parentId;
                    }
                }
            }
            return targetId;
        }

        private string GetToken(int accountManageId)
        {
            var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.AccountManageId == accountManageId);
            return AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
        }

        /// <summary>
        /// 处理批量上传成员到腾讯后台模板文件
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="needUploadColumns"></param>
        /// <returns></returns>
        public Stream GetUploadCsv(List<AddressBookMemberTemplateModel> modelList, List<string> needUploadColumns = null)
        {
            var props = typeof(AddressBookMemberTemplateModel).GetProperties();
            var names = new List<string>();
            var namePropMap = new Dictionary<string, PropertyInfo>();
            bool hasGenderValue = modelList.Any(m => !string.IsNullOrWhiteSpace(m.Gender));
            if (hasGenderValue)
            {
                foreach (var model in modelList)
                {
                    if ("Male".Equals(model.Gender, StringComparison.OrdinalIgnoreCase))
                    {
                        model.Gender = "男";
                    }
                    else if ("Female".Equals(model.Gender, StringComparison.OrdinalIgnoreCase))
                    {
                        model.Gender = "女";
                    }
                }
            }

            foreach (var item in props)
            {
                var columnAttr = item.GetAttribute<UploadColumnAttribute>();
                if (columnAttr != null)// && ignore == null)
                {
                    bool canAdd = false;
                    //fishflow
                    if (needUploadColumns == null)
                    {
                        canAdd = true;
                        //names.Add(columnAttr.ColumnName);
                        //namePropMap.Add(columnAttr.ColumnName, item);
                    }
                    //通过通信录手动批量上传
                    else if (needUploadColumns.Contains(columnAttr.ColumnName) || (hasGenderValue && "性别".Equals(columnAttr.ColumnName, StringComparison.OrdinalIgnoreCase)))
                    {
                        canAdd = true;
                        //names.Add(columnAttr.ColumnName);
                        //namePropMap.Add(columnAttr.ColumnName, item);
                    }
                    else if (columnAttr.ColumnName.Equals("帐号"))
                    {
                        canAdd = true;
                        //names.Add(columnAttr.ColumnName);
                        //namePropMap.Add(columnAttr.ColumnName, item);
                    }
                    if (canAdd && !namePropMap.ContainsKey(columnAttr.ColumnName))
                    {
                        names.Add(columnAttr.ColumnName);
                        namePropMap.Add(columnAttr.ColumnName, item);
                    }
                }
            }
            StringBuilder builder = new StringBuilder();
            foreach (var name in namePropMap.Keys)
            {
                builder.Append(name);
                builder.Append(",");
            }
            builder.Append("\r\n");
            foreach (var model in modelList)
            {
                //if (model.Status == 1 && model.EmployeeStatus != null && !model.EmployeeStatus.Equals("D", StringComparison.CurrentCultureIgnoreCase)) //已关注 非离职
                //{
                //    continue;
                //}
                foreach (var prop in namePropMap.Values)
                {
                    builder.Append(prop.GetValue(model));
                    builder.Append(",");
                }
                builder.Append("\r\n");
            }
            //var csv = new CsvSerializer<AddressBookMemberTemplateModel>();
            //var result = csv.SerializeStream(modelList, names.ToArray());
            logger.Debug("Begin log batch user csv.");
            logger.Debug(builder.ToString());
            logger.Debug("Finish log batch user csv.");
            var bytes = Encoding.Default.GetBytes(builder.ToString());
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// 经确认，敏感信息需要裁剪调
        /// </summary>
        /// <param name="objModal"></param>
        public void CutPropertyBefaultSync2TX(GetMemberResult objModal)
        {
            var cutConfig = CommonService.GetSysConfig("AddressBookCutInfo", string.Empty);
            if (!string.IsNullOrEmpty(cutConfig))
            {
                var configDic = JsonConvert.DeserializeObject<Dictionary<string, bool>>(cutConfig);
                foreach (var pair in configDic)
                {
                    if (pair.Value == true)
                    {
                        var propertyInfo = typeof(GetMemberResult).GetProperty(pair.Key);
                        if (propertyInfo != null)
                        {
                            propertyInfo.SetValue(objModal, propertyInfo.PropertyType.IsValueType ? (object)0 : (object)string.Empty);
                        }
                    }
                }
            }
        }

        public void addMemberTag(string userId, int tagId)
        {
            var user = this.Repository.Entities.Where(u => u.UserId.Equals(userId, StringComparison.CurrentCultureIgnoreCase) && u.DeleteFlag != 1).FirstOrDefault();
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.TagList))
                {
                    var tags = JsonConvert.DeserializeObject<List<int>>(user.TagList);
                    if (!tags.Contains(tagId))
                    {
                        tags.Add(tagId);
                        user.TagList = JsonConvert.SerializeObject(tags);
                    }
                }
                else
                {
                    user.TagList = JsonConvert.SerializeObject(new int[] { tagId });
                }
                this.Repository.Update(user, new List<string> { "TagList" });
            }
        }

        public void delMemberTag(string userId, int tagId)
        {
            var user = this.Repository.Entities.Where(u => u.UserId.Equals(userId, StringComparison.CurrentCultureIgnoreCase) && u.DeleteFlag != 1).FirstOrDefault();
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.TagList))
                {
                    var tags = JsonConvert.DeserializeObject<List<int>>(user.TagList);
                    if (tags.Contains(tagId))
                    {
                        tags.Remove(tagId);
                        user.TagList = JsonConvert.SerializeObject(tags);
                        this.Repository.Update(user, new List<string> { "TagList" });
                    }
                }
            }
        }

        public void InitTagListTemp()
        {
            this.Repository.SqlExcute("update SysAddressBookMember set TagListTemp = ''");
        }

        public void UpdateTagListTemp(List<string> userList, int tagId)
        {
            if (userList != null && userList.Count > 0)
            {
                logger.Debug("tag :{0} has {1} users.", tagId, userList.Count);
                StringBuilder sb = new StringBuilder();
                userList.ForEach(userId => sb.AppendFormat("'{0}',", userId));
                string sql = string.Format("update SysAddressBookMember set TagListTemp = TagListTemp + ',{0}' where UserID in ({1})", tagId, sb.ToString().Trim(','));
                var result = this.Repository.SqlExcute(sql);
                logger.Debug("update TagListTemp success :{0}", result);
            }
        }

        public void UpdateTagListByTagListTemp()
        {
            var result = this.Repository.SqlExcute("update SysAddressBookMember set TagListTemp = STUFF(TagListTemp, 1, 1, '') where LEFT(TagListTemp, 1) = ','");
            logger.Debug("execute stuff success :{0}", result);
            result = this.Repository.SqlExcute("update SysAddressBookMember set TagList = '[' + TagListTemp + ']'");
            logger.Debug("update TagList success :{0}", result);
        }
    }
}
