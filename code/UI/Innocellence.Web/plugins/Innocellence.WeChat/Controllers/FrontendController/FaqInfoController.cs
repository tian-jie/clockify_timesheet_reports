using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Innocellence.WeChat.Domain.ModelsView;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.QY.Entities;
using Infrastructure.Web.ImageTools;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChat.Controllers
{
    public partial class FaqInfoController : WeChatBaseController<FaqInfo, FaqInfoView>
    {
        public IFaqInfoService _objService;
        public ISearchKeywordService _searchKeywordService;
        public FaqInfoController(IFaqInfoService objService, ISearchKeywordService searchKeywordService)
            : base(objService)
        {
            _objService = objService;
            _searchKeywordService = searchKeywordService;
            AppId = (int)Infrastructure.Web.Domain.Service.CategoryType.Undefined;
        }

        
        public ActionResult CommonIndex(int appId, string menuCode)
        {
#if !DEBUG
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }
#endif
            AppId = appId;
            var hotSearch = _searchKeywordService.GetSearchKeywordsByCategory(appId, 14);
            ViewBag.hotsearch = hotSearch;
            ViewBag.appid = appId;
            ViewBag.menucode = menuCode;
            //记录用户行为
            ExecuteBehavior(appId, 6, menuCode);

            return View();
        }

        public ActionResult CommonSearchResult(string keyword, int appid, string menuCode)
        {
#if !DEBUG
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }
#endif

            var hotSearch = _searchKeywordService.GetSearchKeywordsByCategory(appid, 14);
            ViewBag.hotsearch = hotSearch;
            ViewBag.appid = appid;
            ViewBag.menucode = menuCode;
            FaqInfoSearchResultView faqSearchResultViewModel = new FaqInfoSearchResultView()
            {
                Keyword = keyword
            };

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var faqList = SearchPrivate(AppId, keyword);
                faqSearchResultViewModel.List = faqList;
            }
            else
            {
                faqSearchResultViewModel.List = new List<FaqInfoView>();
            }

            return View("../FaqInfo/CommonSearchResult", faqSearchResultViewModel);
        }
        

        public override ActionResult Index()
        {
#if !DEBUG
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }
#endif
            
            var hotSearch = _searchKeywordService.GetSearchKeywordsByCategory(AppId, 14);
            ViewBag.hotsearch = hotSearch;
            //记录用户行为
            ExecuteBehavior(AppId, 6, "FAQ");

            return View("../FaqInfo/index");
        }

        public ActionResult SearchResult(string keyword)
        {
#if !DEBUG
            if (string.IsNullOrEmpty(ViewBag.WeChatUserID))
            {
                return Redirect("/notauthed.html");
            }
#endif

            var hotSearch = _searchKeywordService.GetSearchKeywordsByCategory(AppId, 14);
            ViewBag.hotsearch = hotSearch;

            FaqInfoSearchResultView faqSearchResultViewModel = new FaqInfoSearchResultView()
            {
                Keyword = keyword
            };

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var faqList = SearchPrivate(AppId, keyword);
                faqSearchResultViewModel.List = faqList;
            }
            else
            {
                faqSearchResultViewModel.List = new List<FaqInfoView>();
            }

            return View("../FaqInfo/SearchResult", faqSearchResultViewModel);
        }
       
        private List<FaqInfoView> SearchPrivate(int appId, string keyword)
        {
            // 将搜索条件保存起来，并将搜索次数累加
            // 将字符串截断保存
            foreach (var key_word in keyword.Split(' '))
            {
                //避免截断串后有空字符
                if (!string.IsNullOrEmpty(key_word.Trim()))
                {
                    var searchKeyword = _searchKeywordService.Repository.Entities.Where(a => a.AppId == appId && a.Keyword == key_word).FirstOrDefault();
                    if (searchKeyword == null)
                    {
                        searchKeyword = new SearchKeyword()
                        {
                            AppId = appId,
                            SearchCount = 1,
                            Keyword = key_word
                        };
                        _searchKeywordService.Repository.Insert(searchKeyword);
                    }
                    else
                    {
                        searchKeyword.SearchCount++;
                        _searchKeywordService.Repository.Update(searchKeyword);

                    }
                }
            }
            var article = _objService.GetListBySearchKey(appId, keyword);
            return article;
        }

        [HttpPost]
        public JsonResult ReadCountAdd(string Id)
        {
            int faqId = 0;

            if (!string.IsNullOrEmpty(Id) && int.TryParse(Id, out faqId))
            {
                var faqInfo = _objService.Repository.GetByKey(faqId);

                if (faqInfo != null && faqInfo.ReadCount.HasValue)
                {
                    faqInfo.ReadCount += 1;

                    _objService.Repository.Update(faqInfo);
                    //记录用户行为
                    ExecuteBehavior(faqInfo.AppId, 11, "",Id);
                }
            }

            return Json(doJson(null), JsonRequestBehavior.AllowGet);        
        }

    }
}
