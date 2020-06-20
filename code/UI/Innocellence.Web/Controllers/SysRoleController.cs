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
using Infrastructure.Web.Domain.Services;

namespace Innocellence.Web.Controllers
{
    public class SysRoleController : BaseController<SysRole, SysRoleView>
    {
        ISysRoleMenuService ServiceRoleMenu;
        ISysUserRoleService SysUserRoleService;
        ISysRoleService _newsService;
        public SysRoleController(ISysRoleService newsService, ISysRoleMenuService _ServiceRoleMenu, ISysUserRoleService _SysUserRoleService)
            : base(newsService)
        {
            ServiceRoleMenu = _ServiceRoleMenu;
            SysUserRoleService = _SysUserRoleService;
            _newsService = newsService;
        }


        public override ActionResult Index()
        {

            ViewBag.CateType = Request["CateType"];
            return base.Index();
        }


        public override JsonResult Post(SysRoleView objModal, string Id)
        {
            var allRole = _newsService.Repository.Entities.Where(x => x.IsDeleted == false).Select(u => u.Name).ToList();
            if (allRole.Contains(objModal.Name))
            {
                return Json(new { Data = 410, Message= "SameName" });
            }
            return base.Post(objModal,Id);
        }

        //初始化list页面
        public override List<SysRoleView> GetListEx(Expression<Func<SysRole, bool>> predicate, PageCondition ConPage)
        {
            string strModeId = Request["SearchLoadModeId"];
            string strGroup = Request["SearchGroup"];

            // ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

            var q = _objService.GetList<SysRoleView>(predicate.AndAlso(x => x.IsDeleted == false), ConPage);

           // var pluginDescriptors = _pluginFinder.GetRoleDescriptors(loadMode, 0, strGroup).ToList();

             return q.ToList();
        }


        public ActionResult SetMenu(string Menus,int RoleID)
        {
            //if (string.IsNullOrEmpty(Menus))
            //{
            //    return ErrorNotification("please select a menu!");
            //}
            ServiceRoleMenu.SetRoleMenu(RoleID, Menus);
            Session.Clear();
            return SuccessNotification("OK");
        }

        public override JsonResult Delete(string sIds)
        {
            if (!string.IsNullOrEmpty(sIds))
            {
                int[] arrID = sIds.TrimEnd(',').Split(',').Select(a => int.Parse(a)).ToArray();
                
                foreach (int roleId in arrID)
                {
                    if (SysUserRoleService.CheckExistUserRoleUsed(roleId))
                    {
                        return ErrorNotification("该role下还有关联用户，无法删除");
                    }
                }

                _BaseService.Repository.Delete(arrID);
                AfterDelete(sIds);                
            }

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

    }
}
