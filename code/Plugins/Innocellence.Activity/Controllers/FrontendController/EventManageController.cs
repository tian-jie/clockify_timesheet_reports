using Infrastructure.Web.UI;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.ModelsView;
using Innocellence.Activity.Services;
using Innocellence.WeChat.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Innocellence.Activity.Controllers
{
    public partial class EventManageController : WeChatBaseController<EventProfileEntity, EventProfileEntityView>
    {
        private readonly IEventProfileService _objService;
        private readonly IEventService _eventService;

        public EventManageController(IEventProfileService objService,
            IEventService eventService)
            : base(objService)
        {
            _objService = objService;
            _eventService = eventService;
        }

        private enum EventType
        {
            Registered,
            Checkin
        }

        public ActionResult EventDetail(int eventId)
        {
            //如果未关注企业号
            if (string.IsNullOrEmpty(ViewBag.LillyId))
            {
                return Redirect("/notauthed.html");
            }

            EventEntityView objEvent = null;
            //根据enventId将活动信息拿出来
            var eventInfo = _eventService.Repository.Entities.FirstOrDefault(x => x.Id == eventId && x.IsDeleted == false);

            if (eventInfo != null)
            {
                objEvent = (EventEntityView)new EventEntityView().ConvertAPIModel(eventInfo);

                string lillyId = ViewBag.LillyId;
                var dateTimeNow = DateTime.Now;
                var registers = _objService.Repository.Entities.Where(x => x.EventId == eventId && x.TypeCode == EventType.Registered.ToString()&&x.IsDeleted!=true).ToList();
                var checkResult = _objService.CheckinRegister(eventInfo, registers, lillyId, dateTimeNow);
                objEvent.Status = checkResult.Status.ToString();
            }
            else
            {
                objEvent = new EventEntityView();
                return Redirect("../EventManage/Invalid");
            }
            //记录用户行为
            ExecuteBehavior(eventInfo.AppId, 12, "", eventId.ToString());

            return View(objEvent);
        }

        public ActionResult EventCancel(int eventId, int? pollingId)
        {
            if (string.IsNullOrEmpty(ViewBag.LillyId))
            {
                return Redirect("/notauthed.html");
            }
            string lillyId = ViewBag.LillyId;

            //取消报名
            //var userInfo = _objService.Repository.Entities.FirstOrDefault(x => x.EventId == eventId && x.UserId == lillyId&&x.IsDeleted==false);
            //if (userInfo != null) {
            //    userInfo.IsDeleted = true;

            //    _objService.Repository.Update(userInfo, new System.Collections.Generic.List<string>() { "IsDeleted" });
            //}
            bool rtn=_objService.CancelEvent(eventId, lillyId, pollingId);
            //_objService.Repository.Entities.Where(x => x.EventId == eventId && x.UserId == lillyId&&x.IsDeleted!=true).Update()

            if (rtn == true)
            {
                return SuccessNotification("成功");
            }
            else
            {
                return SuccessNotification("活动已结束，不能取消报名");
            }
        }

        public ActionResult EventRegister(int eventId)
        {
            //如果未关注企业号
            if (string.IsNullOrEmpty(ViewBag.LillyId))
            {
                return Redirect("/notauthed.html");
            }

            //检查此人是否已注册
            string lillyId = ViewBag.LillyId;
            var result = _objService.RegisteredEvent(eventId, lillyId);
            string eventStatus = result.Status.ToString();
            string errmsg = "";
            switch (result.Status)
            {
                case EventStatus.NotStarted:
                    errmsg = "报名未开始";
                    break;
                case EventStatus.Finished:
                    errmsg = "报名已结束";
                    break;
                case EventStatus.OverMaxUser:
                    errmsg = "报名人数已达最大值";
                    break;
                case EventStatus.RepeatRegistered:
                    errmsg = "您已经报名，请注意活动开始时间准时参加活动哦！";
                    break;
                case EventStatus.CanceledEvent:
                    string reason = string.IsNullOrEmpty(((EventEntity)result.Entity).CanceledReason) ? "" : ((EventEntity)result.Entity).CanceledReason;
                    errmsg = "很抱歉，活动已取消!<br/>" + reason;
                    break;
            }

            if (!string.IsNullOrEmpty(errmsg))
            {
                return SuccessNotification(eventStatus + ";" + errmsg);//自定义错误 用成功打
            }
            else
            {
                return SuccessNotification(eventStatus + ";" + "报名成功,请留意活动开始时间！");
            }
        }

        public ActionResult Invalid()
        {
            return View();
        }

        public ActionResult EventCheckIn(string id)
        {
            string isNeedRegister = Request["isNeedRegister"];
            string lillyId = ViewBag.LillyId;

            //如果未关注企业号
            if (string.IsNullOrEmpty(lillyId))
            {
                return Redirect("/notauthed.html");
            }

            // 非法进入的场合
            if (string.IsNullOrEmpty(id))
            {
                return View("../EventManage/EventError");
            }

            int eventId = 0;

            // 非法ID的场合
            if (!int.TryParse(id, out eventId))
            {
                return View("../EventManage/EventError");
            }

            // 取得活动详情
            var eventInfo = _eventService.Repository.Entities.Where(x => x.Id == eventId && x.IsDeleted == false).Distinct().FirstOrDefault();

            // 活动不存在的场合
            if (eventInfo == null)
            {
                return View("../EventManage/EventError");
            }

            DateTime dtNow = DateTime.Now;
            // 活动过期的场合(主要针对已经是签到成功状态的情况)
            if (dtNow > eventInfo.EndedDateTime)
            {
                return View("../EventManage/EventError");
            }

            // 用户活动信息及状态
            //var eventResult = string.IsNullOrEmpty(isNeedRegister) ? _objService.CheckinEvent(eventId, lillyId)
            //     : _objService.CheckinAnnualEvent(eventId, lillyId);
            var eventResult = _objService.CheckinEvent(eventId, lillyId);
            // 签到失败的场合
            if (eventResult.Status != EventStatus.Success)
            {
                return View("../EventManage/EventError");
            }

            var checkInResult = new EventCheckInResultView()
            {
                EventName = eventInfo.Name,
                LillyId = lillyId,
                UserLogoUrl = ((EventProfileEntity)eventResult.Entity).ImgUrl,
                EventStartTime = eventInfo.StartedDateTime.ToString("yyyy-MM-dd HH:mm"),
                EventEndTime = eventInfo.EndedDateTime.ToString("yyyy-MM-dd HH:mm"),
                Address = eventInfo.Location,
                EventDescription = eventInfo.Desc
            };
            //记录用户行为
            ExecuteBehavior(eventInfo.AppId, 12, "", id);
            return View(checkInResult);
        }

        public ActionResult EventError()
        {
            return View();
        }

        /// <summary>
        /// Annual SignIn-Inifite load
        /// </summary>
        public ActionResult GetEmployeeForSignIn()
        {
            string eventId = Request["eventId"];
            string screenType = Request["ScreenType"];

            Expression<Func<EventProfileEntity, bool>> predicate = x => x.IsDeleted == false && x.TypeCode == EventType.Checkin.ToString();

            if (!string.IsNullOrEmpty(eventId))
            {
                var eventid = int.Parse(eventId);
                predicate = predicate.AndAlso(x => x.EventId == eventid);
            }

            switch (screenType)
            {
                case "L":
                    predicate = predicate.AndAlso(x => x.IsDisplay1 == false);
                    break;
                case "M":
                    predicate = predicate.AndAlso(x => x.IsDisplay2 == false);
                    break;
                case "S":
                    predicate = predicate.AndAlso(x => x.IsDisplay3 == false);
                    break;
                default:
                    predicate = predicate.AndAlso(x => x.IsDisplay1 == false);
                    break;
            }

            var eventProfile =
                _objService.Repository.Entities.Where(predicate).OrderBy(x => x.OperatedDateTime).FirstOrDefault();

            if (eventProfile != null)
            {
                switch (screenType)
                {
                    case "L":
                        eventProfile.IsDisplay1 = true;
                        break;
                    case "M":
                        eventProfile.IsDisplay2 = true;
                        break;
                    case "S":
                        eventProfile.IsDisplay3 = true;
                        break;
                    default:
                        eventProfile.IsDisplay1 = true;
                        break;
                }
                _objService.Repository.Update(eventProfile);
            }

            return Json(new { Employee = eventProfile }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Annual SignIn-Intial
        /// </summary>
        public ActionResult GetInitializedEmployeeList()
        {
            string eventId = Request["eventId"];
            string screenType = Request["ScreenType"];

            Expression<Func<EventProfileEntity, bool>> predicate = x => x.IsDeleted == false && x.TypeCode == EventType.Checkin.ToString();

            if (!string.IsNullOrEmpty(eventId))
            {
                var eventid = int.Parse(eventId);
                predicate = predicate.AndAlso(x => x.EventId == eventid);
            }

            switch (screenType)
            {
                case "L":
                    predicate = predicate.AndAlso(x => x.IsDisplay1 == true);
                    break;
                case "M":
                    predicate = predicate.AndAlso(x => x.IsDisplay2 == true);
                    break;
                case "S":
                    predicate = predicate.AndAlso(x => x.IsDisplay3 == true);
                    break;
                default:
                    predicate = predicate.AndAlso(x => x.IsDisplay1 == true);
                    break;
            }

            var eventProfiles =
                _objService.GetList<EventProfileEntityView>(predicate).OrderBy(x => x.OperatedDateTime).ToList();

            return Json(new { EmployeeList = eventProfiles }, JsonRequestBehavior.AllowGet);
        }
    }
}