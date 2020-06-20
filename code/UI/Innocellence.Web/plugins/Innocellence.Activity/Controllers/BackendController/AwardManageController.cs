using Infrastructure.Utility.Data;
using Infrastructure.Utility.IO;
using Innocellence.Activity.Entity;
using Innocellence.Activity.ModelsView;
using Innocellence.Activity.Services;
using Innocellence.Authentication.Authentication;
using Innocellence.WeChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace Innocellence.Activity.Admin.Controllers
{
    public partial class AwardManageController : BaseController<AwardEntity, AwardView>
    {
        public AwardManageController(IAwardService objService)
            : base(objService)
        {
            _objService = objService;
        }

        [DataSecurityFilter]
        public  override ActionResult Index()
        {
            string pollingId = Request["pollingId"];
            string type = Request["type"];
            if (string.IsNullOrEmpty(pollingId) || string.IsNullOrEmpty(type))
            {
                return ErrorNotification("pollingId或者type为空！");
            }
            return View();
        }

       
        public override List<AwardView> GetListEx(Expression<Func<AwardEntity, bool>> predicate, PageCondition con)
        {   
            string strStartTime = Request["StartTime"];
            string strEndTime = Request["EndTime"];

            int pollingId = Convert.ToInt32(Request["pollingId"]);
            string type = Request["type"];
            if (!string.IsNullOrEmpty(strStartTime) && !string.IsNullOrEmpty(strEndTime))
            {
                strStartTime = strStartTime + " 00:00:00";
                strEndTime = strEndTime + " 23:59:59";
                DateTime strtime = Convert.ToDateTime(strStartTime);
                DateTime endtime = Convert.ToDateTime(strEndTime);
                predicate = x => x.PollingId == pollingId && x.Type == type && x.AccessDate >= strtime && x.AccessDate <= endtime;
              
            }
            else
            {
                predicate = x => x.PollingId == pollingId && x.Type == type;
            }
            con.SortConditions.Add(new SortCondition("AccessDate", System.ComponentModel.ListSortDirection.Descending));

            var list = _BaseService.GetList<AwardView>(predicate, con);
            return list;
        }
        public override List<AwardView> BeforeExport()
        {
            string strStartTime = Request["StartTime"];
            string strEndTime = Request["EndTime"];

            int pollingId = Convert.ToInt32(Request["pollingId"]);
            string type = Request["type"];
            Expression<Func<AwardEntity, bool>> predicate;
            if (!string.IsNullOrEmpty(strStartTime) && !string.IsNullOrEmpty(strEndTime))
            {
                strStartTime = strStartTime + " 00:00:00";
                strEndTime = strEndTime + " 23:59:59";
                DateTime strtime = Convert.ToDateTime(strStartTime);
                DateTime endtime = Convert.ToDateTime(strEndTime);
                predicate = x => x.PollingId == pollingId && x.Type == type && x.AccessDate >= strtime && x.AccessDate <= endtime;

            }
            else
            {
                predicate = x => x.PollingId == pollingId && x.Type == type;
            }
            var list = (from item in _objService.Repository.Entities.Where(predicate).ToList()
                        orderby item.Id descending
                        select new AwardView()
                        {
                            Id = item.Id,
                            SecurityCode = item.SecurityCode,
                            Status = item.Status,
                            AccessDate = item.AccessDate,
                            Type = item.Type,
                        }).ToList();
            return list;
        }

        public override ActionResult Export()
        {
            string strStartTime = Request["StartTime"];
            string strEndTime = Request["EndTime"];
           
            if (!CheckDate(strStartTime, strEndTime))
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            var lst = BeforeExport();
            string[] headLine = { "Id", "SecurityCode", "Status", "AccessDate", "Type" };
            var csv = new CsvSerializer<AwardView> {UseLineNumbers = false};
            var ms = csv.SerializeStream(lst, headLine);

            return File(ms, "text/plain", string.Format("{0}.csv", "Award_" + DateTime.Now.ToString("yyyMMddHHmmss")));
        }
        /// <summary>
        /// 验证日期
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private bool CheckDate(string startTime, string endTime)
        {
            bool result = true;
            StringBuilder errMsg = new StringBuilder();
            if (string.IsNullOrEmpty(startTime))
            {
                result = false;
                errMsg.Append(T("起始日期不能为空!<br/>"));
            }

            if (string.IsNullOrEmpty(endTime))
            {
                result = false;

                errMsg.Append(T("截止日期不能为空!<br/>"));
            }


            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                DateTime dtstartTime = Convert.ToDateTime(startTime);
                DateTime dtendTime = Convert.ToDateTime(endTime);
                if (dtstartTime > dtendTime)
                {
                    result = false;
                    errMsg.Append(T("截止日期不能早于起始日期!<br/>"));
                }

                if (dtendTime.AddDays(-60) > dtstartTime)
                {
                    result = false;
                    errMsg.Append(T("起始日期与截止日期间隔不能超过60天!<br/>"));
                }
            }
            if (!result)
            {
                ModelState.AddModelError("不正确的输入", errMsg.ToString());
            }
            return result;
        }
    }
}
