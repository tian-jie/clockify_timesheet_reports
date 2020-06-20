using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.Weixin.QY.CommonAPIs;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using Infrastructure.Utility.IO;
using System.ComponentModel;

namespace Innocellence.WeChatMain.Controllers
{
    public partial class AppReportController : BaseController<PageReportGroup, PageReportGroupView>
    {
        private Infrastructure.Web.Domain.Contracts.ICommonService _commonService;
        private IAddressBookService _addressBookService;
        public AppReportController(IPageReportGroupService pageReportGroupService,
            Infrastructure.Web.Domain.Contracts.ICommonService commonService,
            IAddressBookService addressBookService)
            : base(pageReportGroupService)
        {
            _commonService = commonService;
            _addressBookService = addressBookService;
        }

        public ActionResult Report(string id)
        {
            if (string.IsNullOrEmpty(id) ||
                !int.TryParse(id, out AppId))
            {
                throw new Exception("你必须接受错误，这是请求方式的苦难！");
            }
            ViewBag.AppId = AppId;
            ViewBag.Title = ((CategoryType)AppId).ToString() + " Report";

            return View();
        }

        public JsonResult JsonForAppReport(string AppId, DateTime? BeginDate, DateTime? EndDate)
        {
            DateTime thisEnd = DateTime.Now;
            DateTime thisStart = DateTime.Now.AddDays(-30);
            DateTime thisMonthStart = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 1);
            List<MemberInCountWithSubscribe> list = new List<MemberInCountWithSubscribe>();

            try
            {

                if (BeginDate != null && EndDate != null)
                {
                    thisStart = (DateTime)BeginDate;
                    thisEnd = (DateTime)EndDate;
                }

                list = GetAllMemberCountVisible(AppId.ToString());
                if (list != null)
                {
                    var allCount = list.Count();
                    var subscribeCount = list.Where(x => x.IsSubscribe == (int)WinXinSubscribeEnum.Subscribe).Count();
                    var unSubscribeCount = list.Where(x => x.IsSubscribe == (int)WinXinSubscribeEnum.UnSubscribe).Count();
                    var subScribeCountThisMonth = list.Where(x => x.SubscribeTime >= thisMonthStart && x.SubscribeTime <= thisEnd).Count();

                    InteractAndRead tableOfmessage = new InteractAndRead();
                    tableOfmessage = GetSqlResultCount(AppId, thisMonthStart, thisEnd.AddDays(1));

                    List<DateData> listDateData = new List<DateData>();
                    while (thisStart <= thisEnd)
                    {
                        DateData dateData = new DateData();
                        dateData.dateTime = thisStart.ToString("yyyy-MM-dd");
                        dateData.SubNum = list.Where(x => System.Convert.ToDateTime(x.SubscribeTime).ToString("yyyy-MM-dd") == thisStart.ToString("yyyy-MM-dd")).Count();
                        listDateData.Add(dateData);
                        thisStart = thisStart.AddDays(1);
                    }
                    return Json(new
                    {
                        allcountnum = allCount,
                        subscribeCountnum = subscribeCount,
                        unSubscribeCountnum = unSubscribeCount,
                        subScribeCountnumThisMonth = subScribeCountThisMonth,
                        monthSubDetailList = listDateData,
                        interacAndReadCount = tableOfmessage,

                    }, "200", JsonRequestBehavior.AllowGet);
                }
                return Json(new { results = new { Data = 500 } });
            }
            catch (Exception e)
            {
                _Logger.Error(e, "An error occurred while sending news.");
                //throw;
                return Json(new { results = new { Data = 500 } });
            }

        }

        #region 通过api和数据库获取可见范围

        public List<MemberInCountWithSubscribe> GetAllMemberCountVisible(string AppId)
        {
            List<PersonGroup> list = new List<PersonGroup>();
            List<PersonGroup> MemberCountList = new List<PersonGroup>();
            list = ((WeChatCommonService)_commonService).GetAllPersonAndGroup(AppId, AccountManageID);
            var userInfos = _addressBookService.Repository.Entities.Where(address => address.EmployeeStatus == "A" && address.DeleteFlag != 1).ToList();
            foreach (var mem in list)
            {
                if (mem.Type.Equals("Tag"))
                {
                    List<PersonGroup> tagSub = new List<PersonGroup>();
                    tagSub = GetListDataBytag(mem.WeixinId, userInfos);
                    if (tagSub.Any())
                    {
                        foreach (var unitinTag in tagSub)
                        {
                            if (unitinTag.Type.Equals("Group"))
                            {
                                GetMemberInGroupList(AppId, unitinTag.WeixinId, ref MemberCountList, userInfos);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(unitinTag.WeixinId))
                                {
                                    MemberCountList.Add(unitinTag);
                                }
                            }
                        }
                    }
                }
                else if (mem.Type.Equals("Group"))
                {
                    GetMemberInGroupList(AppId, mem.WeixinId, ref MemberCountList, userInfos);
                }
                else
                {
                    if (!string.IsNullOrEmpty(mem.WeixinId))
                    {
                        MemberCountList.Add(mem);
                    }
                }
            }

            var newMemInCountList = MemberCountList.Distinct(new ModelComparer()).ToList();
            List<MemberInCountWithSubscribe> AddSubscribetoList = new List<MemberInCountWithSubscribe>();
            foreach (var memWithoutSrcibe in newMemInCountList)
            {
                var subscribeMem = userInfos.Where(me => me.UserId == memWithoutSrcibe.WeixinId);
                if (subscribeMem.Any())
                {
                    int subscribeValue = (int)subscribeMem.FirstOrDefault().Status;
                    var subscribeTime = subscribeMem.FirstOrDefault().SubscribeTime;
                    MemberInCountWithSubscribe newMemWithSub = new MemberInCountWithSubscribe();
                    newMemWithSub.Avatar = memWithoutSrcibe.Avatar;
                    newMemWithSub.Email = memWithoutSrcibe.Email;
                    newMemWithSub.Type = memWithoutSrcibe.Type;
                    newMemWithSub.WeixinId = memWithoutSrcibe.WeixinId;
                    newMemWithSub.WeixinName = memWithoutSrcibe.WeixinName;
                    newMemWithSub.IsSubscribe = subscribeValue;
                    //newMemWithSub.UserId = UserId;
                    newMemWithSub.SubscribeTime = subscribeTime;
                    AddSubscribetoList.Add(newMemWithSub);
                }

            }

            return AddSubscribetoList.ToList();
        }

        private List<PersonGroup> GetMemberInGroupList(string AppId, string groupId, ref List<PersonGroup> NewMemberList, List<SysAddressBookMember> userInfos)
        {
            List<PersonGroup> GroupList = new List<PersonGroup>();

            var list = GetListByGroupId(groupId, userInfos);

            foreach (var item in list)
            {
                if (item.Type.Equals("Group"))
                {
                    GetMemberInGroupList(AppId, item.WeixinId, ref NewMemberList, userInfos);
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.WeixinId))
                    {
                        NewMemberList.Add(item);
                    }
                }
            }
            return NewMemberList;
        }
        private List<PersonGroup> GetListDataBytag(string strID, List<SysAddressBookMember> userInfos)
        {
            var targetList = new List<PersonGroup>();
            if (!string.IsNullOrEmpty(strID))
            {
                var result = WeChatCommonService.GetTagById(this.AccountManageID, int.Parse(strID));// MailListApi.GetTagMember(GetToken(), int.Parse(strID));
                var userlst = result.userlist;
                var userIds = result.userlist.Select(u => u.userid).ToList();
                var allDeptList = WeChatCommonService.lstDepartment(this.AccountManageID);// MailListApi.GetDepartmentList(GetToken()).department;
                var userInfosNew = userInfos.Where(u => userIds.Contains(u.UserId) && u.DeleteFlag != 1);
                var partylst = new List<DepartmentList>();
                if (result.partylist != null)
                {
                    partylst = allDeptList.Where(a => result.partylist.Contains(a.id)).ToList();
                }

                //allDeptList

                targetList.AddRange(userInfosNew.Select(a => new PersonGroup { Type = "Person", WeixinId = a.UserId, WeixinName = a.UserName }));
                targetList.AddRange(partylst.Select(a => new PersonGroup { Type = "Group", WeixinId = a.id.ToString(), WeixinName = a.name }));
                return targetList;
            }
            return targetList;
        }

        private List<PersonGroup> GetListByGroupId(string groupId, List<SysAddressBookMember> userInfos)
        {
            var allDeptList = MailListApi.GetDepartmentList(GetToken()).department;
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
            var members = userInfos.Where(a => a.Department.Replace("[", ",").Replace("]", ",").Contains("," + groupId + ",")).ToList();

            foreach (var member in members)
            {
                PersonGroup person = new PersonGroup();
                person.WeixinId = member.UserId;
                person.WeixinName = member.UserName;
                person.Avatar = member.Avatar;
                person.Type = "Person";
                list.Add(person);
            }
            return list;
        }
        private string GetToken()
        {
            var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.AccountManageId == AccountManageID);
            return AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
        }
        #endregion

        #region 存储过程获取消息条目、阅读及互动情况
        private InteractAndRead GetSqlResultCount(string appid, DateTime start, DateTime end)
        {
            InteractAndRead totalCountTableData = new InteractAndRead();
            using (SqlConnection sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CAAdmin"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = sqlconn;
                cmd.CommandText = "CropAppReport";
                cmd.CommandType = CommandType.StoredProcedure;


                IDataParameter[] parameters =
                    {
                        new SqlParameter("@AppId", SqlDbType.Int) ,
                        new SqlParameter("@StartDate", SqlDbType.VarChar,100),
                        new SqlParameter("@EndDate", SqlDbType.VarChar,100),
                     };
                parameters[0].Value = System.Convert.ToInt32(appid);
                parameters[1].Value = start;
                parameters[2].Value = end;
                cmd.Parameters.Add(parameters[0]);
                cmd.Parameters.Add(parameters[1]);
                cmd.Parameters.Add(parameters[2]);

                using (DataSet ds = new DataSet())
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(ds);

                    //<MpFollowReport>
                    totalCountTableData = RowToModel(ds.Tables[0]);

                }

            }
            return totalCountTableData;
        }
        private InteractAndRead RowToModel(DataTable table)
        {
            InteractAndRead model = new InteractAndRead();
            model.MsgThisMonth = (int)table.Rows[0].ItemArray[0];
            model.TotalMsg = (int)table.Rows[0].ItemArray[1];
            model.InteractThisMonth = (int)table.Rows[0].ItemArray[2];
            model.TotalInteract = (int)table.Rows[0].ItemArray[3];
            model.ReadCountThisMonth = (int)table.Rows[0].ItemArray[4];
            model.ReadCountTotal = (int)table.Rows[0].ItemArray[5];
            return model;
        }

        #endregion

        /// <summary>
        /// 导出报表
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public ActionResult MpExportCSV(string appid, DateTime? BeginDate, DateTime? EndDate)
        {
            DateTime thisEnd = DateTime.Now;
            DateTime thisStart = DateTime.Now.AddDays(-30);
            List<MemberInCountWithSubscribe> list = new List<MemberInCountWithSubscribe>();
            try
            {
                if (BeginDate != null && EndDate != null)
                {
                    thisStart = (DateTime)BeginDate;
                    thisEnd = (DateTime)EndDate;
                }
                list = GetAllMemberCountVisible(AppId.ToString());
                List<DateData> listDateData = new List<DateData>();
                if (list != null)
                {

                    while (thisStart <= thisEnd)
                    {
                        DateData dateData = new DateData();
                        dateData.dateTime = thisStart.ToString("yyyy-MM-dd");
                        dateData.SubNum = list.Where(x => System.Convert.ToDateTime(x.SubscribeTime).ToString("yyyy-MM-dd") == thisStart.ToString("yyyy-MM-dd")).Count();
                        listDateData.Add(dateData);
                        thisStart = thisStart.AddDays(1);
                    }

                }


                return mpExportToCSV(listDateData);
            }
            catch (Exception e)
            {
                _Logger.Error(e, "An error occurred while sending news.");
                //throw;
                return Json(new { results = new { Data = 500 } });
            }



        }
        private ActionResult mpExportToCSV(List<DateData> ReportList)
        {
            string[] headLine = { "dateTime", "SubNum" };
            var lst = ReportList;
            CsvSerializer<DateData> csv = new CsvSerializer<DateData>();
            csv.UseLineNumbers = false;
            var ms = csv.SerializeStream(lst, headLine);

            return File(ms, "text/plain", string.Format("{0}.csv", "Report_" + DateTime.Now.ToString("yyyMMddHHmmss")));

        }


        /// <summary>
        /// 去重比较
        /// </summary>
        /// 
        public class ModelComparer : IEqualityComparer<PersonGroup>
        {
            public bool Equals(PersonGroup x, PersonGroup y)
            {
                return x.WeixinId.ToUpper() == y.WeixinId.ToUpper();
            }
            public int GetHashCode(PersonGroup obj)
            {
                return obj.WeixinId.ToUpper().GetHashCode();
            }
        }
        /// <summary>
        /// 为 model 添加字段
        /// </summary>
        public class MemberInCountWithSubscribe : PersonGroup
        {
            public int IsSubscribe { get; set; }
            public DateTime? SubscribeTime { get; set; }
            public string UserId { get; set; }
        }

        public enum WinXinSubscribeEnum
        {

            //MARKETING = 11,
            Subscribe = 1,
            UnSubscribe = 4,
        }

        public class DateData
        {
            [Description("日期")]
            public string dateTime { get; set; }
            [Description("关注人数")]
            public int SubNum { get; set; }
        }


        public class InteractAndRead
        {
            public int MsgThisMonth { get; set; }
            public int TotalMsg { get; set; }
            public int InteractThisMonth { get; set; }
            public int TotalInteract { get; set; }
            public int ReadCountThisMonth { get; set; }
            public int ReadCountTotal { get; set; }

        }

    }
}
