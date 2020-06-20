using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Innocellence.CA.ModelsView;
using Innocellence.CA.Contracts;
using Innocellence.CA.Entity;
using System.Linq.Expressions;
using System.Web.Mvc;
using Infrastructure.Web.UI;

namespace Innocellence.WeChatMain.Controllers
{
    public class UserController : BaseController<SysUser, SysUserView>
    {


        public ISysUserService UserManager { get; private set; }
        public ISysRoleService RoleManager { get; set; }

        public UserController(ISysUserService userManager,ISysRoleService roleManager)
            : base(userManager)
        {
           // _newsService = newsService;
             UserManager = userManager;
             RoleManager = roleManager;
        }

        public ActionResult UserIndex()
        {
            ViewBag.UserId = objLoginInfo.Id;
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult UserList()
        {
            GridRequest req = new GridRequest(Request);

           Expression<Func<SysUser, bool>> predicate =a => a.IsDeleted == false;
           string txtUserName = Request["txtUserName"];
           if (!string.IsNullOrEmpty(txtUserName))
           {
               predicate.AndAlso(a => a.UserName.Contains(txtUserName) ||
                   a.PhoneNumber.Contains(txtUserName) || a.UserTrueName.Contains(txtUserName));
           }

           var lst = UserManager.GetList<SysUserView>(predicate, req.PageCondition);

            return base.GetPageResult(lst.ToList(), req);

        }

        [HttpPost]
         public ActionResult UserEdit()
        {
             SysUserView objUser=new SysUserView ();
             TryUpdateModel(objUser);
            if(!string.IsNullOrEmpty(objUser.PasswordHash))
            {
               objUser.PasswordHash= UserManager.UserContext.PasswordHasher.HashPassword(objUser.PasswordHash);
            }
             return base.Post(objUser, objUser.Id.ToString());
        }


        public  ActionResult UserGet(string id)
        {
            return base.Get(id);
        }


         public ActionResult RoleIndex()
         {
             return View();
         }
         /// <summary>
         /// 
         /// </summary>
         /// <returns></returns>
         public ActionResult RoleList()
         {
             GridRequest req = new GridRequest(Request);
            
             Expression<Func<SysRole, bool>> predicate = a => a.IsDeleted == false;

             string txtRoleName = Request["txtRoleName"];
             if (!string.IsNullOrEmpty(txtRoleName))
             {
                 predicate.AndAlso(a => a.Name.Contains(txtRoleName));
             }


             var lst = RoleManager.GetList<SysRoleView>(predicate, req.PageCondition);

             return base.GetPageResult(lst.ToList(), req);

         }

         public ActionResult RoleGet(string id)
         {
             var obj = RoleManager.GetByKey(int.Parse(id));
             if (obj == null)
             {
                 return Json(null, JsonRequestBehavior.AllowGet);
             }
             else
             {
                 SysRoleView a = (SysRoleView)new SysRoleView().ConvertAPIModel(obj);
                
                 return Json(a, JsonRequestBehavior.AllowGet);
             }
         }

         [HttpPost]
         public ActionResult RoleEdit()
         {
             SysRoleView objModal = new SysRoleView();
             TryUpdateModel(objModal);
             string Id = objModal.Id.ToString();

             if ( !ModelState.IsValid)
             {
                 return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
             }

             // T0 t = new T0();

             if (string.IsNullOrEmpty(Id) || Id == "0")
             {
                 RoleManager.InsertView(objModal);
             }
             else
             {
                 RoleManager.UpdateView(objModal);

             }
             return Json(doJson(null), JsonRequestBehavior.AllowGet);
         }

         public override JsonResult Delete(string sIds)
         {
             if (!string.IsNullOrEmpty(sIds) && !sIds.Equals(objLoginInfo.Id.ToString()))
             {
                 int[] arrID = sIds.TrimEnd(',').Split(',').Select(a => int.Parse(a)).ToArray();

                 _BaseService.Delete(arrID);
                 //}
             }
             else
             {
                 ModelState.AddModelError("DelSelf", "You can't delelte yourself.");
             }
             return Json(doJson(null), JsonRequestBehavior.AllowGet);
         }
         
    }
}
