using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using System.Web.Mvc;
using Infrastructure.Core;
using Innocellence.Web.Service;
using Innocellence.Web.Extensions;
using System.Web;

using Infrastructure.Utility;
using Infrastructure.Web.Domain.Model;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Contracts;

namespace Innocellence.Web.Controllers
{
    public class SysRoleMenuController : BaseController<SysRoleMenuModel, SysRoleMenuView>
    {

        public SysRoleMenuController(ISysRoleMenuService newsService)
            : base(newsService)
        {

        }


        public override ActionResult Index()
        {

            ViewBag.CateType = Request["CateType"];
            return base.Index();
        }

        //初始化list页面
        public override List<SysRoleMenuView> GetListEx(Expression<Func<SysRoleMenuModel, bool>> predicate, PageCondition ConPage)
        {
            string strModeId = Request["SearchLoadModeId"];
            string strGroup = Request["SearchGroup"];

            // ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

             var q = _objService.GetList<SysRoleMenuView>(predicate, ConPage);

           // var pluginDescriptors = _pluginFinder.GetRoleMenuDescriptors(loadMode, 0, strGroup).ToList();

             return q.ToList();
        }

    }
}
