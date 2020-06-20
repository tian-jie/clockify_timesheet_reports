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
    public partial class PollingController : WeChatBaseController<PollingEntity, PollingView>
    {
        private IPollingService _objService;
        private IPollingResultService _objResultService;
        private IPollingResultTempService _objResultTempService;
        public PollingController(IPollingService objService,
            IPollingResultService objResultService, IPollingResultTempService objResultTempService)
            : base(objService)
        {
            _objService = objService;
            _objResultService = objResultService;
            _objResultTempService = objResultTempService;
        }


        public ActionResult WxVoteMainView(int id)
        {
            var eventId = Request["eventId"];
            var lillyid = ViewBag.LillyId;
            if (string.IsNullOrEmpty(lillyid))
            {
                return Redirect("/notauthed.html");
            }

            if (id != 0)
            {
                var pol = _objService.Repository.Entities.AsNoTracking().Where(a => a.Id == id).FirstOrDefault();
                if (pol == null || pol.IsDeleted == true)
                {
                    return View("../Polling/WxDeletedView"); ;
                }
                var poll = (PollingView)new PollingView().ConvertAPIModel(pol);
                int userCount = _objResultService.GetUserCount(id, lillyid);
                if (!string.IsNullOrEmpty(eventId))
                {
                    poll.EventId = eventId;
                }

                if (poll.Type == 1 && userCount > 0)
                {
                    return View("../Polling/WxQADetailView", poll);
                }
                else if ((poll.Type == 2 || poll.Type == 3) && userCount > 0)
                {
                    poll = _objService.GetPollingVote(id, lillyid);
                    if (!string.IsNullOrEmpty(eventId))
                    {
                        poll.EventId = eventId;
                    }
                    return View("../Polling/WxVoteDetailView", poll);
                }
                
                else
                {
                    //poll = _objService.GetPollingDetailView(query, lillyid);
                    poll = _objService.GetPollingDetailView(id);
                    if (!string.IsNullOrEmpty(eventId))
                    {
                        poll.EventId = eventId;
                    }
                    return View("../Polling/WxVoteView", poll);
                }
            }
            return View();
        }
        public ActionResult ResultTemp(int id)
        {
            var lillyid = ViewBag.LillyId;
            List<PollingResultTempView> temp= _objService.GetPollingResultTempByLillyId(id, lillyid);
            if (temp.Count() > 0)
            {
                return Json(new { Status = 200, Data = temp }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = 999}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WxVoteMain(int id)
        {
            var eventId = Request["eventId"];
            var lillyid = ViewBag.LillyId;
            if (string.IsNullOrEmpty(lillyid))
            {
                return Redirect("/notauthed.html");
            }

            if (id != 0)
            {
                var pol = _objService.Repository.Entities.AsNoTracking().Where(a => a.Id == id).FirstOrDefault();
                if (pol == null || pol.IsDeleted == true)
                {
                    return View("../Polling/WxDeletedView"); ;
                }
                var poll = (PollingView)new PollingView().ConvertAPIModel(pol);

                int userCount = _objResultService.GetUserCount(id, lillyid);

                if (!string.IsNullOrEmpty(eventId))
                {
                    poll.EventId = eventId;
                }

                if (poll.Type == 1 && userCount > 0)
                {
                    return View("../Polling/WxQADetail", poll);
                }
                else if ((poll.Type == 2 || poll.Type == 3) && userCount > 0)
                {
                    poll = _objService.GetPollingVote(id, lillyid);
                    if (!string.IsNullOrEmpty(eventId))
                    {
                        poll.EventId = eventId;
                    }
                    return View("../Polling/WxVoteDetail", poll);
                }
                else
                {
                    poll = _objService.GetPollingDetailView(id);
                    if (!string.IsNullOrEmpty(eventId))
                    {
                        poll.EventId = eventId;
                    }
                    return View("../Polling/WxVote", poll);
                }
            }
            return View();
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


}
