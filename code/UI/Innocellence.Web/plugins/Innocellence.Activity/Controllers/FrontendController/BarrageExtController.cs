using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.Entity;
using Innocellence.Activity.Services;
using Innocellence.WeChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;



namespace Innocellence.Activity.Controllers
{
    public partial class BarrageExtController : BaseController<Barrage, BarrageView>
    {
        private readonly IBarrageService _barrageService;
        private readonly IBarrageExtService _barrageExtService;
        private readonly IBarrageSummaryService _barrageSummaryService;

        public BarrageExtController(IBarrageService barrageService,
            IBarrageExtService barrageExtService,
            IBarrageSummaryService barrageSummaryService)
            : base(barrageService)
        {
            _barrageSummaryService = barrageSummaryService;
            _barrageExtService = barrageExtService;
            _barrageService = barrageService;
        }

        /// <summary>
        /// 提问大屏幕
        /// </summary>
        /// <returns></returns>
        public ActionResult RaiseQuestion()
        {
            string keyWord = Request["SummaryId"];
            string appid = Request["appId"];
            string type = Request["Type"];//0匿名 1实名

            ViewBag.keyWord = keyWord;
            ViewBag.appId = appid;
            ViewBag.type = type;

            Expression<Func<BarrageSummary, bool>> predicate = x => x.IsDeleted == false;

            if (!string.IsNullOrEmpty(appid))
            {
                predicate = predicate.AndAlso(x => x.AppId.Equals(appid));
            }

            if (!string.IsNullOrEmpty(keyWord))
            {
                var sId = int.Parse(keyWord);
                predicate = predicate.AndAlso(x => x.Id == sId);
            }

            var item = _barrageSummaryService.Repository.Entities.Where(predicate).FirstOrDefault();
            ViewBag.title = item != null ? item.Title : string.Empty;

            return View();
        }

        /// <summary>
        /// 微信弹幕
        /// </summary>
        /// <returns></returns>
        public ActionResult DisplayQuestion()
        {
            string keyWord = Request["SummaryId"];
            string appid = Request["appId"];

            ViewBag.keyWord = keyWord;
            ViewBag.appId = appid;

            Expression<Func<BarrageSummary, bool>> predicate = x => x.IsDeleted == false;

            if (!string.IsNullOrEmpty(appid))
            {
                predicate = predicate.AndAlso(x => x.AppId.Equals(appid));
            }

            if (!string.IsNullOrEmpty(keyWord))
            {
                var sId = int.Parse(keyWord);
                predicate = predicate.AndAlso(x => x.Id == sId);
            }

            var item = _barrageSummaryService.Repository.Entities.Where(predicate).FirstOrDefault();
            ViewBag.title = item != null ? item.Title : string.Empty;

            return View();
        }

        [HttpGet]
        public ActionResult GetInitialQuestiones(string appId, string keyWord)
        {
            Expression<Func<Barrage, bool>> predicate = x => x.Status == 1 && x.IsDisplay != false;
            var maxnum = Request["maxNum"] == null || Request["maxNum"] == "" ? 30 : int.Parse(Request["maxNum"]);

            if (!string.IsNullOrEmpty(appId))
            {
                predicate = predicate.AndAlso(x => x.AppId.Equals(appId));
            }

            if (!string.IsNullOrEmpty(keyWord))
            {
                var sId = int.Parse(keyWord);
                predicate = predicate.AndAlso(x => x.SummaryId == sId);
            }

            var sortCondition = new List<SortCondition>
            {
                new SortCondition("ApprovedDate", System.ComponentModel.ListSortDirection.Descending)
            };

            var lst = _barrageService.GetList<BarrageView>(maxnum, predicate, sortCondition).OrderBy(x => x.ApprovedDate).ToList();

            lst.ForEach(t => {t.FeedBackContent = QyMessageHandlers.QyCustomMessageHandler.ConvertEmotion(t.FeedBackContent); });

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetQuestiones(string appId, string keyWord)
        {
            Expression<Func<Barrage, bool>> predicate = x => x.Status == 1 && x.IsDisplay == false;

            if (!string.IsNullOrEmpty(appId))
            {
                predicate = predicate.AndAlso(x => x.AppId.Equals(appId));
            }

            if (!string.IsNullOrEmpty(keyWord))
            {
                var sId = int.Parse(keyWord);
                predicate = predicate.AndAlso(x => x.SummaryId == sId);
            }

            var sortCondition = new List<SortCondition>
            {
                new SortCondition("ApprovedDate", System.ComponentModel.ListSortDirection.Ascending)
            };

            var lst = _barrageService.GetList<BarrageView>(5, predicate, sortCondition);//每次取5条
            //在后台取出来后就将字段isDisplay更新为true.
            lst.ForEach(x =>
            {
                x.IsDisplay = true;
                _barrageService.UpdateView(x, new List<string>() { "IsDisplay" });
            });

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCancelQuestiones(string appId, string keyWord)
        {
            Expression<Func<Barrage, bool>> predicate = x => x.Status != 1 && x.IsDisplay != false;

            if (!string.IsNullOrEmpty(appId))
            {
                predicate = predicate.AndAlso(x => x.AppId.Equals(appId));
            }

            if (!string.IsNullOrEmpty(keyWord))
            {
                var sId = int.Parse(keyWord);
                predicate = predicate.AndAlso(x => x.SummaryId == sId);
            }

            var lst = _barrageService.GetQList<BarrageView>(predicate);
            //在后台取出来后就将字段isDisplay更新为false.
            lst.ForEach(x =>
            {
                x.IsDisplay = false;
                _barrageService.UpdateView(x, new List<string>() { "IsDisplay" });
            });

            return Json(lst, JsonRequestBehavior.AllowGet);
        }
    }
}