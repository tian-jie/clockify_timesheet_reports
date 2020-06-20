using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using System.Web.Mvc;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Service.Common;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Autofac;
using Infrastructure.Web.Domain.Service;


namespace Innocellence.Web.Controllers
{
    public class SysConfigController : BaseController<SysConfigModel, SysConfigView>
    {
        private static ICacheManager _cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(CommonService)));

        public SysConfigController(ISysConfigService newsService)
            : base(newsService)
        {
           // _newsService = newsService;
        }


        //初始化list页面
        public override List<SysConfigView> GetListEx(Expression<Func<SysConfigModel, bool>> predicate, PageCondition ConPage)
        {
            string strCateType = Request["CateType"];

            if (!string.IsNullOrEmpty(strCateType))
            {
                int iCateType = int.Parse(strCateType);

               // predicate = predicate.AndAlso();
            }

           // ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

            var q = _BaseService.GetList<SysConfigView>(predicate, ConPage);

            return q.ToList();
        }

        public override bool BeforeAddOrUpdate(SysConfigView objModal, string Id)
        {
            // WeChatCommonService.ClearCache(1);
            return true;
        }

        public bool ClearAllCaches()
        {
            _cacheManager.Clear();
            return true;
        }
        public override bool AfterDelete(string sIds)
        {
           // CommonService.ClearCache(1);
            return true;
        }
          //Post方法
        [HttpPost]
        [ValidateInput(false)]
        public override JsonResult Post(SysConfigView objModal, string Id)
        {
            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            InsertOrUpdate(objModal, Id);
            return Json(doJson(null));
        }
        protected void InsertOrUpdate(SysConfigView objModal, string Id)
        {
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                _BaseService.InsertView(objModal);
            }
            else
            {
                _BaseService.UpdateView(objModal);
            }
        }
    }
}
