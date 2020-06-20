using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.CommonAPIs;
using System.Threading.Tasks;
using Innocellence.Weixin.QY.AdvancedAPIs.App;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain;


namespace Innocellence.WeChatMain.Controllers
{
    public class ReportController : BaseController<UserBehavior, UserBehaviorView>
    {
        private IUserBehaviorService _userBehaviorService;

        public ReportController(IUserBehaviorService userBehaviorService, IReportService objService)
            : base(objService)
        {
            _userBehaviorService = userBehaviorService;
        }


        public async Task<ActionResult> Department(string date)
        {
               if (string.IsNullOrEmpty(date))
                {

                }
                else
                {
                    // 获取token
                    var objConfig = WeChatCommonService.GetWeChatConfigByID(AccountManageID);
                    var token = await getToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);

                    // 使用获取员工详情的API
                    var empDetails = await getEmployeesDetail(token);

                    // 获取部门列表
                    var departments = await getDepartments(token);
                    // 处理部门关系，暂定只有5层
                    foreach (var dept in departments.department)
                    {
                        int level = 1;
                        int parentId = dept.parentid;
                        while (parentId != 0)
                        {
                            var parentDept = departments.department.Where(t => t.id == parentId).FirstOrDefault();
                            if (parentDept != null)
                            {
                                level++;
                                parentId = parentDept.parentid;
                            }
                        }
                        dept.level = level;
                    }
                    //找出部门关系是3的
                    //定义柱状图标的list
                    List<string> xAxisList = new List<string>();

                    //考虑到图表的SereisData数据为一个列表
                    List<SereisData> seriesList = new List<SereisData>();
                    List<SereisData> unseriesList = new List<SereisData>();
                    int subscrib = 0;
                    foreach (var dept in departments.department)
                    {
                        if (dept.level == 4)
                        {
                            if (xAxisList.Contains(dept.name))
                            {

                            }
                            else
                            {
                                xAxisList.Add(dept.name);
                            }

                        }
                    }
                    //根据xAxisList建立对应的seriesList，unseriesList
                    foreach (var t in xAxisList)
                    {
                        SereisData sda = new SereisData();
                        sda.Sereisname = t;
                        sda.Sereisvalue = 0;
                        seriesList.Add(sda);
                        SereisData unsda = new SereisData();
                        unsda.Sereisname = t;
                        unsda.Sereisvalue = 0;
                        unseriesList.Add(unsda);

                    }
                    // 更新员工信息，把部门的名称填入
                    foreach (var emp in empDetails.userlist)
                    {
                        if (emp.department.Count() == 1)
                        {
                            var dept = departments.department.Where(t => t.id == emp.department[0]).FirstOrDefault();
                            if (dept != null)
                            {
                                int level = dept.level;
                                while (level > 1)
                                {
                                    emp.deptLvs[level] = dept.name;
                                    dept = departments.department.Where(t => t.id == dept.parentid).FirstOrDefault();
                                    level--;
                                }
                            }
                        }
                    }

                    //找出部门对应员工的关注数
                    foreach (var emp in empDetails.userlist)
                    {
                        if (emp.department.Count() != 1)
                        {
                            continue;
                        }
                        foreach (var xAxis in xAxisList)
                        {
                            if (emp.deptLvs[4] == xAxis)
                            {
                                if (emp.status == 1)
                                {
                                    foreach (var t in seriesList)
                                    {
                                        if (t.Sereisname == xAxis)
                                        {
                                            t.Sereisvalue += 1;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var t in unseriesList)
                                    {
                                        if (t.Sereisname == xAxis)
                                        {
                                            t.Sereisvalue += 1;
                                            break;
                                        }
                                    }
                                }
                                break;
                            }

                        }
                    }
                    var newObj = new
                    {
                        xAxis = xAxisList,
                        Data = seriesList,
                        UnData = unseriesList
                    };
                    return Json(newObj, JsonRequestBehavior.AllowGet);
                }

                return View();
           
        }

        /// <summary>
        /// 企业号百分比统计
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Enterprise(string state, string date)
        {
            
                if (string.IsNullOrEmpty(date))
                {

                }
                else
                {
                    //定义饼柱状图标的list
                    List<string> xAxisList = new List<string>();
                    DateTime dt = DateTime.Parse(date);

                    // 获取token
                    var objConfig = WeChatCommonService.GetWeChatConfigByID(AccountManageID);
                    var token = await getToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);

                    // 使用获取app列表的API
                    var empagent = await getagentlist(token);
                    foreach (var emp in empagent.agentlist)
                    {
                        xAxisList.Add(emp.name);
                    }
                    var newObj = new
                    {
                        xAxis = xAxisList,
                        //Data = seriesList,

                    };
                }
                return null;
            
        }

        /// <summary>
        /// 应用统计
        /// </summary>
        /// <returns></returns>
        public
            async Task<ActionResult> Statistics(string state, string begindate, string enddate)
        {

            if (string.IsNullOrEmpty(begindate) || string.IsNullOrEmpty(enddate))
            {

            }
            else
            {
                DateTime dt = DateTime.Parse(begindate);
                DateTime dt1 = DateTime.Parse(enddate).AddDays(1);

                // 获取token
                var objConfig = WeChatCommonService.GetWeChatConfigByID(AccountManageID);
                var token = await getToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);
                // 使用获取app列表的API
                var empagent = await getagentlist(token);

                //定义饼状图标的list
                List<string> legendList = new List<string>();

                List<int> agentList = new List<int>();
                agentList = _userBehaviorService.GetAgentList(dt, dt1);
                Dictionary<int, string> middletrans = new Dictionary<int, string>();
                foreach (var emp in empagent.agentlist)
                {
                    foreach (var agent in agentList)
                    {
                        if (emp.agentid == agent.ToString())
                        {
                            legendList.Add(emp.name);
                            middletrans.Add(agent, emp.name);
                            break;
                        }
                    }

                }


                var tabledata = _userBehaviorService.GetByList(dt, dt1);

                //考虑到图表的SereisData数据为一个列表
                List<SereisData> seriesList = new List<SereisData>();
                string text = string.Empty;
                foreach (var kvp in tabledata)
                {
                    SereisData seriesObj = new SereisData();
                    foreach (KeyValuePair<int, string> mtrans in middletrans)
                    {
                        if (kvp.AppId == mtrans.Key)
                        {
                            text = mtrans.Value;
                            if (!string.IsNullOrEmpty(text))
                            {
                                seriesObj.Sereisname = text;
                                seriesObj.Sereisvalue = kvp.Count;
                                seriesList.Add(seriesObj);
                            }
                            break;
                        }
                    }

                }
                var newObj = new
                {
                    legend = legendList,
                    Data = seriesList,

                };

                if (state == "2")
                {
                    return Json(newObj, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return View();
                }
            }
            return View();
        }

        private async Task<string> getToken(string corpID, string corpSecrect)
        {
            return AccessTokenContainer.TryGetToken(corpID, corpSecrect, false);
        }

        private async Task<GetDepartmentMemberInfoResult> getEmployeesDetail(string token)
        {

            return MailListApi.GetDepartmentMemberInfo(token, 1, 1, 0);

        }

        private async Task<GetDepartmentListResult> getDepartments(string token)
        {
            return MailListApi.GetDepartmentList(token);
        }

        private async Task<GetAppListResult> getagentlist(string token)
        {

            return AppApi.GetAppList(token);

        }

    }
}
