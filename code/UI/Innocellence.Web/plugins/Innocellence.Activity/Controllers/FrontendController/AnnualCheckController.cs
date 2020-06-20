using Innocellence.Activity.Entity;
using Innocellence.Activity.Services;
using Innocellence.Activity.ViewModel;
using Innocellence.WeChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Innocellence.Activity.Controllers
{
    public class AnnualCheckController : WeChatBaseController<AnnualCheckinEntity, AnnualCheckinView>
    {
        private readonly IAnnualCheckinService _objService;

        public AnnualCheckController(IAnnualCheckinService objService)
            : base(objService)
        {
            _objService = objService;
        }

        private enum UserStauts
        {
            NoChecked,
            Checked
        }

        public override ActionResult Index()
        {
            //未关注企业号
#if DEBUG
            ViewBag.LillyId = "N000002";
#endif
            //如果未关注企业号
            if (string.IsNullOrEmpty(ViewBag.LillyId))
            {
                return Redirect("/notauthed.html");
            }
            string userId = ViewBag.LillyId;
            string eventNo = Request["EventNo"];
            AnnualCheckinView objView = null;
            //进来确认用户是否报名
            var obj = _objService.Repository.Entities.FirstOrDefault(x => x.LillyId.Equals(userId, StringComparison.CurrentCultureIgnoreCase) && x.EventNo.Equals(eventNo, StringComparison.CurrentCultureIgnoreCase));
            if (obj != null)
            {
                objView = (AnnualCheckinView)new AnnualCheckinView().ConvertAPIModel(obj);
            }
            else
            {
                objView = new AnnualCheckinView();
            }

            return View(objView);
        }

        [HttpPost]
        public ActionResult Check()
        {
            //未关注企业号
#if DEBUG
            ViewBag.LillyId = "N000002";
#endif
            //如果未关注企业号
            if (string.IsNullOrEmpty(ViewBag.LillyId))
            {
                return Redirect("/notauthed.html");
            }
            string userId = ViewBag.LillyId;
            string eventNo = Request["EventNo"];
            //修改表中的状态
            var obj = _objService.Repository.Entities.FirstOrDefault(x => x.LillyId.Equals(userId, StringComparison.CurrentCultureIgnoreCase) && x.EventNo.Equals(eventNo, StringComparison.CurrentCultureIgnoreCase));
            if (obj != null)
            {
                var objView = (AnnualCheckinView)new AnnualCheckinView().ConvertAPIModel(obj);
                if (obj.Status.Equals(UserStauts.NoChecked.ToString()))
                    obj.Status = UserStauts.Checked.ToString();
                obj.UpdatedUserId = userId;
                obj.UpdatedDate = DateTime.Now;
                _objService.Repository.Update(obj, new List<string> { "Status", "UpdatedUserId", "UpdatedDate" });
                //向前台推送消息
                //var reply = string.Format("您已确认签到：\n礼来ID：{0} \n姓名：{1} \n入住酒店：{2} \n资料袋号码：{3}", userId,
                //    objView.Name, objView.CheckHotel, objView.MaterialNum);
                //WechatCommon.SendMsg(14, "text", userId, "", "", reply, null);
            }
            else
            {
                return ErrorNotification("Faild");
            }

            return SuccessNotification("Success");
        }

    }
}