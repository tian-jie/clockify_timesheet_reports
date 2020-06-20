using Infrastructure.Utility.Data;
using Infrastructure.Utility.Extensions;
using Infrastructure.Utility.Filter;
using Infrastructure.Utility.IO;
using Infrastructure.Web.UI;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.ModelsView;
using Innocellence.Activity.Services;
using Innocellence.Authentication.Authentication;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace Innocellence.Activity.Admin.Controllers
{
    public partial class ActivityManageController : BaseController<EventEntity, EventEntityView>
    {
        IDataPermissionCheck _permissionService;
        IEventProfileService _eventProfileService;
        IEventService _eventService;

        public ActivityManageController(IEventService eventService, IEventProfileService eventProfileService, IDataPermissionCheck permissionService)
            : base(eventService)
        {
            _eventService = eventService;
            _eventProfileService = eventProfileService;
            _permissionService = permissionService;
        }
        [HttpGet, DataSecurityFilter]
        public ActionResult Edit(int appId, string id)
        {
            ViewBag.appid = appId;
            var obj = GetObject(id);

            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                AppDataPermissionCheck(_permissionService, appId, obj.AppId);
            }


            return View(obj);
        }

        [HttpGet, DataSecurityFilter]
        public ActionResult Index(int appId)
        {
            ViewBag.appid = appId;

            return View();
        }
        // GET: AttendList
        public ActionResult EventProfileList()
        {
            ViewBag.EventId = Request["EventId"];
            ViewBag.TypeCode = Request["TypeCode"];
            return View();
        }

        public override ActionResult GetList()
        {

            var type = Request["Type"];

            var req = new GridRequest(Request);

            if (type == "Profile")
            {
                var profilePredicate = FilterHelper.GetExpression<EventProfileEntity>(req.FilterGroup);

                var profileList = GetListPrivate(ref profilePredicate, req.PageCondition);

                return GetPageResult(profileList, req);
            }

            var eventPredicate = FilterHelper.GetExpression<EventEntity>(req.FilterGroup);
            return GetPageResult(GetListPrivate(ref eventPredicate, req.PageCondition), req);
        }

        protected List<EventProfileEntityView> GetListPrivate(ref Expression<Func<EventProfileEntity, bool>> predicate,
            PageCondition ConPage)
        {
            int EventId = Convert.ToInt32(Request["EventId"]);
            string TypeCode = Request["TypeCode"];

            predicate = predicate.AndAlso(x => x.EventId == EventId && x.TypeCode.Equals(TypeCode) && x.IsDeleted != true);

            ConPage.SortConditions.Add(new SortCondition("OperatedDateTime", System.ComponentModel.ListSortDirection.Descending));

            //var q = _BaseService.GetList<EventProfileEntityView>(predicate, ConPage);

            var q = _eventProfileService.GetList<EventProfileEntityView>(predicate, ConPage);


            //_eventProfileService.GetUserList(q);  ?????
            //TODO:考虑真实情况一个人签到多次的情况不多见，暂时在内存做disstinct，如需要在移到数据库中
            return q;
        }

        protected List<EventEntityView> GetListPrivate(ref Expression<Func<EventEntity, bool>> predicate, PageCondition ConPage)
        {
            int strNewsCate = int.Parse(Request["appId"].ToString());
            //int strNewsCate = 20;
            string name = Request["Name"];
            predicate = predicate.AndAlso(a => a.IsDeleted == false);
            if (!string.IsNullOrEmpty(name))
            {
                predicate = predicate.AndAlso(a => a.Name.Contains(name));
            }
            if (strNewsCate != 0)
            {
                predicate = predicate.AndAlso(a => a.AppId == strNewsCate);
            }
            ConPage.SortConditions.Add(new SortCondition("Id", System.ComponentModel.ListSortDirection.Descending));

            var q = _BaseService.GetList<EventEntityView>(predicate, ConPage);
            return q;

        }

        private bool CheckDate(string startTime, string endTime, string name, out string errorMessage)
        {
            bool result = true;
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(startTime))
            {
                result = false;
                errorMessage = "开始时间必填。<br/>";
            }

            if (string.IsNullOrEmpty(endTime))
            {
                result = false;
                errorMessage = "结束时间必填。<br/>";
            }


            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                DateTime dtstartTime = Convert.ToDateTime(startTime);
                DateTime dtendTime = Convert.ToDateTime(endTime);
                if (dtstartTime > dtendTime)
                {
                    result = false;
                    errorMessage = name + " 结束时间不能早于开始时间。<br/>";
                }
            }
            return result;
        }

        //三个时间验证
        private bool DateCheck(DateTime? registerstart, DateTime? cheeckin, DateTime activitytime, out string errorMessage)
        {
            bool result = true;
            errorMessage = string.Empty;
            //如果签到时间和报名时间同时为空
            if ((cheeckin == null) && (registerstart == null))
            {
                errorMessage = ("签到时间和报名时间至少填一个。");
                return false;
            }
            else if (!(cheeckin == null) && !(registerstart == null))
            {
                if (registerstart > cheeckin)
                {
                    errorMessage = "签到开始时间不能早于报名开始时间。<br/>";
                    result = false;
                }
                if (cheeckin > activitytime)
                {
                    errorMessage = "活动开始时间不能早于签到开始时间。<br/>";
                    result = false;
                }
            }
            else if ((cheeckin == null) && !(registerstart == null))
            {
                if (registerstart > activitytime)
                {
                    errorMessage = "活动开始时间不能早于报名开始时间。<br/>";
                    result = false;
                }
            }
            else if (!(cheeckin == null) && (registerstart == null))
            {
                if (cheeckin > activitytime)
                {
                    errorMessage = "活动开始时间不能早于签到开始时间。<br/>";
                    result = false;
                }
            }

            //if (checkinTime!=null)
            //{
            //    if (startTime> checkinTime) {
            //        errorMessage="签到开始时间不能早于报名开始时间。<br/>";
            //        result = false;
            //    }
            //    if (checkinTime > activityTime)
            //    {
            //        errorMessage = "活动开始时间不能早于签到开始时间。<br/>";
            //        result = false;
            //    }
            //} else {
            //    if (startTime > activityTime)
            //    {
            //        errorMessage = "活动开始时间不能早于报名开始时间。<br/>";
            //        result = false;
            //    }
            //}
            return result;
        }
        // checkin 时间可以为空
        private bool Checkdateisnull(DateTime? startTime, DateTime? endTime, string name, out string errorMessage)
        {
            bool result = true;
            errorMessage = string.Empty;


            if (startTime != null && endTime != null)
            {

                if (startTime > endTime)
                {
                    result = false;
                    errorMessage = name + " 结束时间不能早于开始时间<br/>";
                }
                else if (startTime == null)
                {
                    result = false;
                    errorMessage = "开始时间必填。<br/>";
                }
                else if (endTime == null)
                {
                    result = false;
                    errorMessage = "结束时间必填。<br/>";
                }

            }
            return result;
        }
        /// <summary>
        /// 必填项验证
        /// </summary>
        /// <param name="name"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool CheckRequire(string name, string hint, out string errorMessage)
        {
            bool result = true;
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                result = false;
                errorMessage = hint;
            }
            return result;
        }
        //验证输入字符数长度
        public bool CheckLength(string name, string hint, int length, out string errorMessage)
        {
            bool result = true;
            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(name) && name.Length > length)
            {
                result = false;
                errorMessage = hint;
            }
            return result;
        }

        //BackEnd校验
        public override bool BeforeAddOrUpdate(EventEntityView objModal, string Id)
        {
            //后台校验 Go here..
            bool validate = true;
            StringBuilder errMsg = new StringBuilder();
            string departId = Request["departmentAll"];
            string errorMessage = string.Empty;
            if (!CheckDate(objModal.StartedDateTime.ToLongDateString(), objModal.EndedDateTime.ToLongDateString(), "活动", out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!(objModal.CheckinStartedDateTime == null && objModal.CheckinEndedDateTime == null))
            {
                if (!Checkdateisnull(objModal.CheckinStartedDateTime, objModal.CheckinEndedDateTime, "签到",
                      out errorMessage))
                {
                    validate = false;
                    errMsg.Append(T(errorMessage));
                }
            }
            if (!(objModal.RegisteredStartedDateTime == null && objModal.RegisteredEndedDateTime == null))
            {
                if (!Checkdateisnull(objModal.RegisteredStartedDateTime, objModal.RegisteredEndedDateTime, "报名",
                      out errorMessage))
                {
                    validate = false;
                    errMsg.Append(T(errorMessage));
                }
            }
            //if (!CheckDate(objModal.RegisteredStartedDateTime.GetValueOrDefault().ToLongDateString(), objModal.RegisteredEndedDateTime.GetValueOrDefault().ToLongDateString(), "报名",
            //     out errorMessage))
            //{
            //    validate = false;
            //    errMsg.Append(T(errorMessage));
            //}
            if (!DateCheck(objModal.RegisteredStartedDateTime.GetValueOrDefault(), objModal.CheckinStartedDateTime, objModal.StartedDateTime,
               out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckRequire(objModal.Name, "活动名称必填。<br/>", out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckRequire(objModal.Location, "活动地点必填。<br/>", out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckRequire(objModal.Desc, "活动描述必填。<br/>", out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckRequire(objModal.MaxUser.ToString(), "活动最大人数必填。<br/>", out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckLength(objModal.Name, "活动名称不能超过250个字符。<br/>", 250, out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckLength(objModal.Location, "活动地点不能超过250个字符<br/>", 250, out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckLength(objModal.Desc, "活动描述不能超过100个字符。<br/>", 1400, out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!validate)
            {
                ModelState.AddModelError("无效的输入", errMsg.ToString());
            }

            return validate;
        }

        [HttpPost]
        public ActionResult Create(EventEntityView objModal, string Id)
        {

            objModal.IsDeleted = true;

            var lst = new List<string>() {
                    "CanceledReason","IsDeleted" };
            _BaseService.UpdateView(objModal, lst);
#if !DEBUG
                //var BaseUrl = WebConfigurationManager.AppSettings["WebUrl"];
                //string reply = "";
                //reply = "This activity is canceled,please forgive me.!";
                //WechatCommon.SendMsg(objModal.AppId, "text", objModal.CreatedUserId, "", "", reply, null);
#endif
            return Json(new { rtnId = objModal.Id, str = "Canel Success." });

        }

        //Post方法
        [HttpPost]
        [ValidateInput(false)]
        public override JsonResult Post(EventEntityView objModal, string Id)
        {
            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            //objModal.CreatedUserId = User.Identity.GetUserName();
            //objModal.CreatedDate = DateTime.Now;
            objModal.AppId = int.Parse(Request["ActivityAppId"]);
            objModal.Desc = Request["Desc"].Trim();
            InsertOrUpdate(objModal, Id);


            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEventList(string appId)
        {

            Expression<Func<EventEntity, bool>> predicate = x => x.IsDeleted == false;
            var appid = int.Parse(appId);
            predicate = predicate.AndAlso(x => x.AppId == appid);

            var lst = _eventService.GetList<EventEntityView>(int.MaxValue, predicate);

            return Json(new { data = lst }, JsonRequestBehavior.AllowGet);
        }

        protected void InsertOrUpdate(EventEntityView objModal, string Id)
        {
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                _BaseService.InsertView(objModal);
            }
            else
            {
                var lst = new List<string>() {
                    "Name","PollingName","AppId","PollingId","StartedDateTime","EndedDateTime","CheckinStartedDateTime",
                    "CheckinEndedDateTime","MaxUser","Location","Desc","RegisteredStartedDateTime","RegisteredEndedDateTime","CanceledReason","IsNeedRegisterBeforeCheckin"
                   };
                _BaseService.UpdateView(objModal, lst);
            }
        }

        public override ActionResult Export()
        {
            string strEventId = Request["txtEventId"];
            string strTypeCode = Request["txtTypeCode"];

            return ExportCSV(strEventId, strTypeCode);
        }

        /// <summary>
        /// CSV文件出力
        /// </summary>
        /// <param name="title">新闻标题</param>
        /// <param name="pulishDate">发布时间</param>
        /// <param name="categoryId">标签ID</param>
        /// <returns></returns>
        public ActionResult ExportCSV(string eventId, string typeCode)
        {
            int eventMathId = -1;

            int.TryParse(eventId, out eventMathId);

            Expression<Func<EventProfileEntity, bool>> predicate = x => x.EventId == eventMathId && x.IsDeleted != true && x.TypeCode.ToLower().Equals(typeCode.ToLower());

            List<EventProfileEntityView> reportList = new List<EventProfileEntityView>();

            reportList = _eventProfileService.GetList<EventProfileEntityView>(predicate).OrderByDescending(x => x.OperatedDateTime).DistinctBy(x => x.UserId).ToList();

            // var userIds = reportList.Select(x => x.UserId);

            //var users = WeChatCommonService.lstUser.Where(x => userIds.Contains(x.userid)).Select(x => new { TrueName = x.name, UserId = x.userid }).ToList();

            //foreach (var item in reportList)
            //{
            //    var user = users.FirstOrDefault(x => x.UserId == item.UserId);
            //    item.UserName = user == null ? null : user.TrueName;
            //    item.deptLvs = emp.deptLvs;
            //}
            _eventProfileService.GetUserList(reportList);
            string fileHeadName = "activity_" + eventId + "_" + typeCode;

            return exportToCSV(reportList, fileHeadName);
        }

        private ActionResult exportToCSV(List<EventProfileEntityView> wechatFollowReportList, string fileHeadName)
        {
            string[] headLine = { "UserId", "UserName", "deptLv1", "deptLv2", "deptLv3", "Email", "OperatedDateTime" };
            var csv = new CsvSerializer<EventProfileEntityView> { UseLineNumbers = false };
            string sRet = csv.Serialize(wechatFollowReportList, headLine);
            var stream = new MemoryStream();
            byte[] bits = System.Text.Encoding.UTF8.GetBytes(sRet);
            byte[] bom = new[] { (byte)0xEF, (byte)0xBB, (byte)0xBF };
            stream.Write(bom, 0, bom.Length);
            stream.Write(bits, 0, bits.Length);
            string fileName = fileHeadName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            return File(stream.ToArray(), "text/comma-separated-values", fileName);
        }

        [HttpPost, Obsolete, NonAction]
        public JsonResult Events(int appId)
        {
            if (appId == 0)
            {
                return ErrorNotification("appid 不能为空!");
            }

            var list = _eventService.GetActivityEvents(appId).Select(x => new { x.Id, x.Name }).ToList();
            var result = new { Status = 200, Data = list };
            return Json(result);
        }

        [HttpPost, NonAction]
        public JsonResult EventUsers(int eventId, string typeCode)
        {
            if (eventId == 0)
            {
                return ErrorNotification("eventId 不能为空!");
            }

            var users = _eventProfileService.GetUserUnderEvent(eventId, string.IsNullOrEmpty(typeCode) ? new List<string>() : typeCode.Split(',').ToList()).Select(x => x.UserId).Distinct().ToList();
            var result = new { Status = 200, Data = users };
            return Json(result);
        }
    }
}
