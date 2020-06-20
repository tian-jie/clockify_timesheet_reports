//using Infrastructure.Utility.Data;
//using Infrastructure.Web.UI;
//using Innocellence.Authentication.Authentication;
//using Innocellence.WeChat.Controllers;
//using Innocellence.WeChat.Domain.Contracts;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Web.Mvc;

//namespace Innocellence.Activity.Controllers.BackendController
//{
//    public class EventMessageController : MessageController
//    {
//        private readonly IEventProfileService _eventProfileService;
//        public EventMessageController(IMessageService objService, int appId, IEventProfileService eventProfileService)
//            : base(objService, appId)
//        {
//            _eventProfileService = eventProfileService;
//        }

//        public EventMessageController(IMessageService objService, IEventProfileService eventProfileService)
//            : base(objService)
//        {
//            _eventProfileService = eventProfileService;
//        }

//        [HttpGet, DataSecurityFilter, ActionName("EventMessageIndex")]
//        public ActionResult Index(int eventId, int appId)
//        {
//            if (eventId == 0 || appId == 0)
//            {
//                return ErrorNotification("活动id不能为空!");
//            }

//            ViewBag.eventId = eventId;
//            ViewBag.AppId = appId;

//            return View("index");
//        }

//        public override List<MessageView> GetListEx(Expression<Func<Message, bool>> predicate, PageCondition conPage)
//        {
//            var strTitle = Request["txtArticleTitle"];
//            var txtDate = Request["txtDate"];

//            predicate = predicate.AndAlso(x => x.MessageType == MessageType.EventMessage.ToString());

//            var q = GetListPrivate(ref predicate, conPage, strTitle, txtDate);
//            return q.ToList();
//        }

//        [HttpGet, DataSecurityFilter, ActionName("EventMessageEdit")]
//        public ActionResult Edit(string id, int appId, int eventId)
//        {
//            if (appId == 0 || eventId == 0)
//            {
//                return ErrorNotification("活动id或者appid 不能为空!");
//            }
//            ViewBag.AppId = appId;
//            ViewBag.EventId = eventId;

//            AppId = appId;

//            PrepareEditData();
//            var obj = GetObject(id);

//            if (!string.IsNullOrEmpty(id))
//            {

//                base.AppDataPermissionCheck(Permission, appId, obj.AppId.GetValueOrDefault());
//            }

//            return View("edit", obj);
//        }

//        public override bool BeforeAddOrUpdate(MessageView objModal, string id)
//        {
//            objModal.EventPersonCategory = string.IsNullOrEmpty(objModal.EventPersonCategory) ? null :
//            JsonHelper.ToJson(objModal.EventPersonCategory.Split(','));
//            objModal.MessageType = MessageType.EventMessage.ToString();
//            return base.BeforeAddOrUpdate(objModal, id);
//        }

//        [HttpPost]
//        public JsonResult CheckSendObjects(int appId, string partyids, string tagids, string userids, int contentId, string personType, int eventId)
//        {
//            if (!string.IsNullOrEmpty(personType) && eventId != 0)
//            {
//                userids = GetAllUsers(personType.Split(','), eventId, userids);
//            }

//            if (string.IsNullOrEmpty(partyids) && string.IsNullOrEmpty(tagids) && string.IsNullOrEmpty(userids))
//            {
//                return ErrorNotification("消息接收对象不能为空!");
//            }

//            return base.CheckSendObjects(appId, partyids, tagids, userids, contentId);
//        }

//        private string GetAllUsers(IList<string> types, int eventId, string currentUser)
//        {
//            var list = _eventProfileService.GetUserUnderEvent(eventId, types).Select(x => x.UserId).ToList();
//            var usersUnderEvent = string.Join("|", list);

//            if (string.IsNullOrEmpty(currentUser))
//            {
//                return usersUnderEvent;
//            }

//            if (string.IsNullOrEmpty(usersUnderEvent))
//            {
//                return currentUser;
//            }


//            return string.Format("{0}|{1}", currentUser, usersUnderEvent);

//        }

//        [HttpGet]
//        public override JsonResult ChangeStatus(string id, int appid, bool ispush)
//        {
//            var objModel = _BaseService.Repository.GetByKey(int.Parse(id));
            
//            if (objModel == null)
//            {
//                return ErrorNotification("消息 id 不存在!");
//            }

//            if (objModel.RefId == 0)
//            {
//                return ErrorNotification("该信息不是活动消息!");
//            }
            
//            if (objModel.RefId.GetValueOrDefault() != 0 && !string.IsNullOrEmpty(objModel.RefEntity))
//            {
//                var userType = JsonHelper.FromJson<List<string>>(objModel.RefEntity);
//                objModel.toUser = GetAllUsers(userType, objModel.RefId.GetValueOrDefault(), objModel.toUser);
//            }

//            if (string.IsNullOrEmpty(objModel.toDepartment) && string.IsNullOrEmpty(objModel.toTag) && string.IsNullOrEmpty(objModel.toUser))
//            {
//                return ErrorNotification("消息接收对象不能为空!");                
//            }

//            return ChangeStatus(objModel, appid, ispush);
//        }

//        public override ActionResult Export()
//        {
//            string strTitle = Request["txtArticleTitle"];
//            string txtDate = Request["txtDate"];

//            Expression<Func<Message, bool>> predicate = x => x.MessageType == MessageType.EventMessage.ToString() && x.IsDeleted == false;

//            int appid,eventid;

//            if (int.TryParse(Request["appId"], out appid))
//            {
//                AppId = appid;
//                predicate = predicate.AndAlso(x => x.AppId == AppId);
//            }

//            if (int.TryParse(Request["eventId"], out eventid))
//            {
//                predicate = predicate.AndAlso(x => x.RefId == eventid);
//            }
            
//            return ExportCSV(strTitle, txtDate, predicate);
//        }
//    }
//}