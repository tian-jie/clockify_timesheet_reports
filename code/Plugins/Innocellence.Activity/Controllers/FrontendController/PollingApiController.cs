using Infrastructure.Core.Logging;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.Services;
using Innocellence.WeChat.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace Innocellence.Activity.Controllers
{
    public partial class PollingApiController : WeChatBaseController<PollingEntity, PollingView>
    {
        private IPollingService _objService;
        private IPollingResultService _objResultService;
        private IPollingResultTempService _objResultTempService;

        public PollingApiController(IPollingService objService,
            IPollingResultService objResultService, IPollingResultTempService objResultTempService)
            : base(objService)
        {
            _objService = objService;
            _objResultService = objResultService;
            _objResultTempService = objResultTempService;
        }

        /// <summary>
        /// 获取某个appid下的polling list
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPollingList()
        {
            var appidString = Request["AppId"];
            var appid = int.Parse(appidString);
            var pollings = _objService.Repository.Entities.Where(a => a.IsDeleted != true && a.AppId == appidString).ToList()
                .Select(a => (PollingView)(new PollingView().ConvertAPIModel(a))).ToList();

            return Json(new { Status = WxVoteType.NotVotedQA, Message = "polling列表", data = pollings }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 根据某个polling guid获取这个polling的所有内容（验证guid和appid之间的关系）
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult GetPolling(string guid)
        {
            var appidString = Request["AppId"];
            var appid = int.Parse(appidString);
            var polling = _objService.GetPollingView(guid);

            return Json(new { Status = WxVoteType.NotVotedQA, Message = "polling", data = polling }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ResultTemp(int id)
        {
            var lillyid = ViewBag.LillyId;
            List<PollingResultTempView> temp = _objService.GetPollingResultTempByLillyId(id, lillyid);
            if (temp.Count() > 0)
            {
                return Json(new { Status = 200, Data = temp }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = 999 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WxVoteMain(int id)
        {
            var eventId = Request["eventId"];
            var lillyid = User.Identity.Name;
            if (string.IsNullOrEmpty(lillyid))
            {
                return Json(new { Status = WxVoteType.NotAuthed, Message = "未授权" }, JsonRequestBehavior.AllowGet);
            }

            if (id != 0)
            {
                var pol = _objService.Repository.Entities.AsNoTracking().Where(a => a.Id == id).FirstOrDefault();
                if (pol == null || pol.IsDeleted == true)
                {
                    return Json(new { Status=WxVoteType.Deleted, Message="已删除"}, JsonRequestBehavior.AllowGet);
                }
                var poll = (PollingView)new PollingView().ConvertAPIModel(pol);

                int userCount = _objResultService.GetUserCount(id, lillyid);

                if (!string.IsNullOrEmpty(eventId))
                {
                    poll.EventId = eventId;
                }

                if (poll.Type == 1 && userCount > 0)
                {
                    return Json(new { Status = WxVoteType.NotVotedQA, Message = "有奖问答", data = poll }, JsonRequestBehavior.AllowGet);
                }
                else if ((poll.Type == 2 || poll.Type == 3) && userCount > 0)
                {
                    poll = _objService.GetPollingVote(id, lillyid);
                    if (!string.IsNullOrEmpty(eventId))
                    {
                        poll.EventId = eventId;
                    }
                    return Json(new { Status = WxVoteType.NotVotedQA, Message = "投票", data = poll }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    poll = _objService.GetPollingDetailView(id);
                    if (!string.IsNullOrEmpty(eventId))
                    {
                        poll.EventId = eventId;
                    }
                    return Json(new { Status = WxVoteType.NotVotedQA, Message = "投票", data = poll }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Status = WxVoteType.Deleted, Message = "已删除/不存在" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WxVotePreview(int id)
        {

            var lillyid = ViewBag.LillyId;
            if (string.IsNullOrEmpty(lillyid))
            {
                return Redirect("/notauthed.html");
            }

            if (!string.IsNullOrEmpty(id.ToString()) && id != 0)
            {

                var poll = _objService.GetPollingDetailView(id);

                if (poll == null || poll.IsDeleted.Value)
                {
                    return View("../Polling/WxDeleted");
                }

                return View(poll);
            }
            return View();
        }



        protected void InsertOrUpdate(PollingResultView objModal, string Id)
        {
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                objModal.UserId = ViewBag.lillyid;

                _objResultService.InsertView(objModal);
            }
            else
            {
                _objResultService.UpdateView(objModal);
            }
        }

        protected void InsertOrUpdateTemp(PollingResultTempView objModal, string Id)
        {
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                objModal.UserId = ViewBag.lillyid;

                _objResultTempService.InsertView(objModal);
            }
            else
            {
                _objResultTempService.UpdateView(objModal);
            }
        }
        public ActionResult InputDataTemp(PollingResultTempView objModal, string Id)
        {
            objModal.UserId = ViewBag.LillyId;
            int userCount = _objResultTempService.GetTempCountByLillyID(objModal.PollingId, ViewBag.LillyId);
            //已有数据，需要删除
            if (userCount > 0)
            {
                _objResultTempService.Delete(objModal.PollingId, ViewBag.LillyId);
            }
            InsertOrUpdateTemp(objModal, Id);
            //_objService.InsertResultTemp(objModal);
            return Json(new { Status = 200, Data = objModal.PollingId }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult InputData(PollingResultView objModal, string Id)
        {
            //如果未关注企业号
            if (string.IsNullOrEmpty(ViewBag.LillyId))
            {
                return Redirect("/notauthed.html");
            }
            int userCount = _objResultService.GetUserCount(objModal.PollingId, ViewBag.LillyId);
            if (userCount > 0)
            {
                return Json(new { Status = 100, Message = "不能重复投票" }, JsonRequestBehavior.AllowGet);
            }
            var currentDate = DateTime.Now;
            var pollingNumber = _objService.Repository.Entities.Count(x => x.Id == objModal.PollingId && x.EndDateTime < currentDate);
            if (pollingNumber > 0)
            {
                return Json(new { Status = 99, Message = "投票通道已关闭" }, JsonRequestBehavior.AllowGet);
            }

            objModal.UserId = ViewBag.LillyId;
            //InsertOrUpdate(objModal, Id);
            var score = _objService.InsertResult(objModal);
            var id = objModal.PollingId;
            if (id != 0)
            {
                //把分数发给用户
                var poll = _objService.Repository.Entities.Where(a => a.Id == id).Select(x => new
                {
                    x.Type,
                    x.AppId,
                    x.ReplyMessage
                }).ToList().Select(x => new PollingEntity { Id = id, Type = x.Type, ReplyMessage = x.ReplyMessage, AppId = x.AppId }).First();

                if (poll.Type == 1)
                {
                    Sendmessage(id, poll, score);
                }
            }

            return Json(new { Status = 200, Data = objModal.PollingId }, JsonRequestBehavior.AllowGet);
            //Json(new { rtnId = objModal.PollingId, str = "Insert Success." }, JsonRequestBehavior.AllowGet);
        }

        private void Sendmessage(int id, PollingEntity poll, decimal score)
        {
            var lillyid = ViewBag.LillyId;
            if (!string.IsNullOrEmpty(id.ToString()) && id != 0)
            {
                //var poll = _objService.Repository.Entities.FirstOrDefault(a => a.Id == id&& a.IsDeleted!=true);
                var reply = string.Empty;
                if (!string.IsNullOrEmpty(poll.ReplyMessage))
                {
                    reply = string.Format(poll.ReplyMessage, score);
                }

                //如果分数符合要求则发消息
                if (!string.IsNullOrEmpty(poll.AppId) && !string.IsNullOrEmpty(poll.ReplyMessage))
                {
                    LogManager.GetLogger(this.GetType()).Debug("reply=" + reply);
                    LogManager.GetLogger(this.GetType()).Debug("lillyid=" + lillyid);
                    //WechatCommon.SendMsg(int.Parse(poll.AppId), "text", lillyid, "", "", reply, null);
                }
            }
        }

    }

    public enum WxVoteType
    {
        NotVotedVote    = 0x1001,
        NotVotedQA      = 0x1002,
        VotedVote       = 0x2001,
        VotedQA         = 0x2002,
        Deleted         = 0x3001,
        NotAuthed       = 0x9999
    }


}
