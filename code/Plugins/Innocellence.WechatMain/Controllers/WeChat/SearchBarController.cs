using System.Linq;
using System.Web.Mvc;

using Infrastructure.Web.UI;
using Infrastructure.Web.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts;

namespace Innocellence.WeChatMain.Controllers
{
    public class SearchBarController : WinxinBaseController
    {
        private IAppUserService _appUserService;

        public SearchBarController(ICategoryService newsService, IAppUserService appUserService)
            : base(newsService)
        {
            _appUserService = appUserService;
        }


        public override ActionResult Index()
        {
            //if (string.IsNullOrEmpty(id))
            //{
            //    //ViewBag.id = id;
            //    GridRequest req = new GridRequest(Request);
            //    //Expression<Func<T0, bool>> predicate = FilterHelper.GetExpression<T0>(req.FilterGroup);
            //    int iCount = req.PageCondition.RowCount;
            //    var list = _appUserService.QueryUser(id, req.PageCondition);
            //    ViewBag.list = list;
            //}
            //else
            //{
            //    ViewBag.id = null;
            //    ViewBag.list = null;

            //}
            return View();
        }

        [ActionName("search")]
        public ActionResult Index(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                ViewBag.id = key;
                GridRequest req = new GridRequest(Request);
                //Expression<Func<T0, bool>> predicate = FilterHelper.GetExpression<T0>(req.FilterGroup);
                int iCount = req.PageCondition.RowCount;
                var list = _appUserService.QueryUser(key,AccountManageID, req.PageCondition);
                ViewBag.list = list;
            }
            else
            {
                ViewBag.id = null;
                ViewBag.list = null;

            }
            return View();
        }

        //public override List<AppUserView> GetListEx(Expression<Func<A, bool>> predicate, PageCondition ConPage)
        //{
        //    string strKeyWord = Request["txtKeyWord"];

        //    var q = GetListPrivate(ref predicate, ConPage, strKeyWord);

        //    return q;
        //}

        public override ActionResult GetList() 
        {
            GridRequest req = new GridRequest(Request);
            
            string strKeyWord = Request["txtKeyWord"];

            var q = _appUserService.QueryUser(strKeyWord, AccountManageID,req.PageCondition);
            
            return Json(new
            {
                sEcho = Request["draw"],
                iTotalRecords = req.PageCondition.RowCount,
                iTotalDisplayRecords = req.PageCondition.RowCount,
                aaData = q.ToList()
            }, JsonRequestBehavior.AllowGet);
        }
        //private List<AppUserView> GetListPrivate(PageCondition ConPage, string strKeyWord)
        //{
        //    var q = _appUserService.QueryUser(strKeyWord, ConPage);
        //    return q.ToList();
        //}

        //// GET: SearchBar
        //public ActionResult SeachKeyWordList(string keyword)
        //{
        //    GridRequest req = new GridRequest(Request);
        //    //Expression<Func<T0, bool>> predicate = FilterHelper.GetExpression<T0>(req.FilterGroup);
        //    int iCount = req.PageCondition.RowCount;

        //    var list = _appUserService.QueryUser(keyword, req.PageCondition);
        //    ////List<Model> list = new List<Model>();
            
        //    return Json(new { list = list });
        //}
    }
}