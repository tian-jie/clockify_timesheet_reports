using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.MVC.Attribute;
using Innocellence.WeChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Innocellence.WeChatMain.Controllers
{
    
    public class PlateformManageController : BaseController<SysRole, SysRoleView>
    {
        ISysRoleService _roleService;
        ISysRoleMenuService ServiceRoleMenu;

        public PlateformManageController(ISysRoleService roleService, ISysRoleMenuService serviceRoleMenu) : base(roleService)
        {
            _roleService = roleService;
            ServiceRoleMenu = serviceRoleMenu;
        }

        // GET: PlateformManage
        public override ActionResult Index()
        {
            var user = Session["UserInfo"] as SysUser;
            //user 包含System Admin
            if (user != null && user.Apps.Contains(-1))
            {
                ViewBag.NeedLoadRoels = true;
                ViewBag.Roles = _roleService.Repository.Entities.Where(a => !a.IsDeleted.Value & a.Name != "Super Admin").ToList();
            }
            ViewBag.IsRole = false;
            ViewBag.IsUser = false;
            if (user != null && user.Menus != null)
            {
                if (user.Menus.Where(p => p.MenuName == "权限管理" & p.MenuGroup == "System Admin").Count() > 0)
                {
                    ViewBag.IsRole = true;
                }
                if (user.Menus.Where(p => p.MenuName == "用户管理" & p.MenuGroup == "System Admin").Count() > 0)
                {
                    ViewBag.IsUser = true;
                }

            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult PasswordChange()
        {           
            return View();
        }

        public ActionResult SetMenu(string Menus, int RoleID)
        {
            //if (string.IsNullOrEmpty(Menus))
            //{
            //    return ErrorNotification("please select a menu!");
            //}
            ServiceRoleMenu.SetRoleMenu(RoleID, Menus);
            Session.Clear();
            return SuccessNotification("OK");
        }
    }
}