using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.UI;
using Infrastructure.Utility.IO;
using System.Linq.Expressions;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChatMain.Controllers
{
    public class ArticleReportController : BaseController<ArticleReport, ArticleReportView>
    {
        private IUserBehaviorService _userBehaviorService;
        private readonly IArticleReportService _objService;

        public ArticleReportController(IUserBehaviorService userBehaviorService,
            IArticleReportService objService)
            : base(objService)
        {
            _userBehaviorService = userBehaviorService;
            _objService = objService;
        }

        // GET: ArticleReport
        public override ActionResult Index()
        {
            string appid = Request["appId"];

            if (!string.IsNullOrEmpty(appid))
            {
                AppId = int.Parse(appid);
                @ViewBag.AppId = appid;
            }
            else
            {
                AppId = (int)CategoryType.Undefined;
                @ViewBag.AppId = AppId;
            }

            ViewBag.Apps = WeChatCommonService.GetAppList();

            return View();
        }

        public ActionResult AppIndex()
        {
            string strId = Request["Id"];

            if (string.IsNullOrEmpty(strId) ||
                !int.TryParse(strId, out AppId))
            {
                throw new Exception("你必须接受错误，这是请求方式的苦难！");
            }

            @ViewBag.AppId = strId;

            return View();
        }


        public ActionResult LedIndex()
        {
            string MenuKey = Request["MenuKey"];

            if (string.IsNullOrEmpty(MenuKey))
            {
                throw new Exception("缺少参数menuKey");
            }

            @ViewBag.MenuKey = MenuKey;

            return View();
        }

        public ActionResult LedReport()
        {


            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest req = new GridRequest(Request);

            string strStartTime = Request["startDate"];
            string strEndTime = Request["endDate"];
            string errorMessage = string.Empty;
            string appCate = Request["appCate"];
            string MenuKey = Request["MenuKey"];

            if (!CheckDate(false, strStartTime, strEndTime, out errorMessage))
            {
                return ErrorNotification(errorMessage);
            }

            if (string.IsNullOrEmpty(strStartTime))
            {
                strStartTime = DateTime.Now.ToString("yyyy-MM-dd");
            }

            if (string.IsNullOrEmpty(strEndTime))
            {
                strEndTime = DateTime.Now.ToString("yyyy-MM-dd");
            }

            strStartTime = strStartTime + " 00:00:00";
            strEndTime = strEndTime + " 23:59:59";
            DateTime startTime = DateTime.Parse(strStartTime);
            DateTime endTime = DateTime.Parse(strEndTime);

            Expression<Func<ArticleReport, bool>> predicate = a => a.AccessDate >= startTime && a.AccessDate <= endTime;

            if (!string.IsNullOrEmpty(appCate))
            {
                var appId = int.Parse(appCate);
                predicate = predicate.AndAlso(a => a.AppId == appId); //实现按AppId过滤
            }

            if (!string.IsNullOrEmpty(MenuKey))
            {
                predicate = predicate.AndAlso(a => a.MenuKey.Equals(MenuKey)); //实现按MenuKey过滤
            }
            var q = _objService.GetList<ArticleReportView>(predicate, req.PageCondition);

            return Json(new
            {
                sEcho = Request["draw"],
                iTotalRecords = req.PageCondition.RowCount,
                iTotalDisplayRecords = req.PageCondition.RowCount,
                aaData = q.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckDate(bool emptyCheck, string startTime, string endTime, out string errorMessage)
        {
            bool result = true;
            errorMessage = string.Empty;

            if (emptyCheck)
            {
                if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                {
                    result = false;
                    errorMessage = "请输入搜索内容的时间段";
                }
            }

            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                DateTime dtstartTime = Convert.ToDateTime(startTime);
                DateTime dtendTime = Convert.ToDateTime(endTime);
                if (dtstartTime > dtendTime)
                {
                    result = false;
                    errorMessage = "起始日期不能大于终止日期";
                }
            }
            return result;
        }

        public override List<ArticleReportView> BeforeExport()
        {
            string startTime = Request["startDate"];
            string endTime = Request["endDate"];
            string appCate = Request["appCate"];
            return BeforeExportArticle(startTime, endTime, appCate);
        }

        public override ActionResult Export()
        {

            var lst = BeforeExport();
            CsvSerializer<ArticleReportView> csv = new CsvSerializer<ArticleReportView>();
            csv.UseLineNumbers = false;
            var ms = csv.SerializeStream(lst, (string[])null);

            return File(ms, "text/plain", string.Format("{0}.csv", "ArticleReport_" + DateTime.Now.ToString("yyyMMddHHmmss")));
        }


        private List<ArticleReportView> BeforeExportArticle(string strStartTime, string strEndTime, string appCate)
        {
            Expression<Func<ArticleReport, bool>> predicate = x => true;
            string MenuKey = Request["LEDMenuKey"];
            if (string.IsNullOrEmpty(strStartTime))
            {
                strStartTime = DateTime.Now.ToString("yyyy-MM-dd");
            }

            if (string.IsNullOrEmpty(strEndTime))
            {
                strEndTime = DateTime.Now.ToString("yyyy-MM-dd");
            }

            strStartTime = strStartTime + " 00:00:00";
            strEndTime = strEndTime + " 23:59:59";

            DateTime startTime = DateTime.Parse(strStartTime);
            DateTime endTime = DateTime.Parse(strEndTime);

            predicate = predicate.AndAlso(x => x.AccessDate >= startTime);
            predicate = predicate.AndAlso(x => x.AccessDate <= endTime);

            if (!string.IsNullOrEmpty(appCate))
            {
                var appId = int.Parse(appCate);
                predicate = predicate.AndAlso(a => a.AppId == appId);//实现按AppId过滤
            }
            if (!string.IsNullOrEmpty(MenuKey))
            {
                predicate = predicate.AndAlso(a => a.MenuKey.Equals(MenuKey)); //实现按MenuKey过滤
            }
            var q = _objService.GetListByDate<ArticleReportView>(predicate);

            return q;
        }
    }
}
