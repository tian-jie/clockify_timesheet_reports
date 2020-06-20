using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using System.Web.Mvc;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Core.Data;
using Microsoft.AspNet.Identity;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.Domain.Service;

namespace Innocellence.Web.Controllers
{
    public class SysUserController : BaseController<SysUser, SysUserView>
    {
        ISysRoleService ServiceRole;
        private IAuthenticationService authService;
        public SysUserController(ISysUserService newsService, ISysRoleService _ServiceRole, IAuthenticationService _authService)
            : base(newsService)
        {
            ServiceRole = _ServiceRole;
            authService = _authService;
        }


        public override ActionResult Index()
        {
            ViewBag.Roles = ServiceRole.Repository.Entities.Where(a => !a.IsDeleted.Value & a.Name != "Super Admin").ToList();
            return base.Index();
        }


        //初始化list页面
        public override List<SysUserView> GetListEx(Expression<Func<SysUser, bool>> predicate, PageCondition ConPage)
        {
            string strModeId = Request["SearchLoadModeId"];
            string strGroup = Request["SearchGroup"];
            string strCondition = Request["search_condition"];

            if (!string.IsNullOrEmpty(strCondition))
            {
                strCondition = strCondition.Trim().ToLower();
                predicate = predicate.AndAlso(x => x.WeChatUserID.Contains(strCondition) ||
                        (x.Email != null && x.Email.ToLower().Contains(strCondition)) ||
                        x.UserName.ToLower().Contains(strCondition) ||
                        x.UserTrueName.ToLower().Contains(strCondition));
            }

            var q = _objService.GetList<SysUserView>(predicate.AndAlso(x => x.IsDeleted == false), ConPage);

            return q.ToList();
        }

        public override void BeforeGet(SysUserView obj, SysUser objSrc)
        {
            var UserRoles = new BaseService<SysUserRole>().Repository.Entities.Where(a => a.UserId == objSrc.Id).ToList();
            foreach (var a in UserRoles)
            {
                obj.strRoles += "," + a.RoleId.ToString();
            }
            if (!string.IsNullOrEmpty(obj.strRoles))
            {
                obj.strRoles = obj.strRoles.Substring(1);
            }
        }

        public override bool BeforeAddOrUpdate(SysUserView objModal, string Id)
        {
            if (objModal.Id == 0)
            {
                //create
                if (_objService.Repository.CheckExists(x => x.UserName == objModal.UserName && !x.IsDeleted.Value))
                {
                    ModelState.AddModelError("userName", @"该用户名已经被注册,请选择其它用户名进行注册!");
                    return false;
                }
                if (objModal.strRoles == null)
                {
                    ModelState.AddModelError("userName", @"请选择Role!");
                    return false;
                }
                objModal.PasswordHash = new PasswordHasher().HashPassword(objModal.PasswordHash);
                objModal.SecurityStamp = "Innocellence";
            }
            else
            {
                //update
                var userEntity = _objService.Repository.Entities.Where(x => x.Id == objModal.Id).AsNoTracking().FirstOrDefault();
                if (userEntity == null)
                {
                    ModelState.AddModelError("sysUser", @"您操作的用户不存在 ,操作失败!");
                    return false;
                }
                if (objModal.strRoles == null)
                {
                    ModelState.AddModelError("userName", @"请选择Role!");
                    return false;
                }
                if (userEntity.PasswordHash != objModal.PasswordHash)
                {
                    objModal.PasswordHash = new PasswordHasher().HashPassword(objModal.PasswordHash);
                }

            }

            //去掉密码外入力项的多余空格
            if (objModal != null)
            {
                objModal.WeChatUserID = objModal.WeChatUserID == null ? objModal.WeChatUserID : objModal.WeChatUserID.Trim();
                objModal.UserName = objModal.UserName == null ? objModal.UserName : objModal.UserName.Trim();
                objModal.UserTrueName = objModal.UserTrueName == null ? objModal.UserTrueName : objModal.UserTrueName.Trim();
                objModal.Email = objModal.Email == null ? objModal.Email : objModal.Email.Trim();
            }

            return true;
        }

        [AllowAnonymous]
        public JsonResult GetCurrentUser()
        {

            var userId = HttpContext.User.Identity.GetUserId<int>();
            Expression<Func<SysUser, bool>> predicate = FilterHelper.GetExpression<SysUser>();
            var userEntity = _objService.Repository.Entities.Where(x => x.Id == userId).AsNoTracking().FirstOrDefault();
            SysUserView data = new SysUserView { };
            data = ConvertToSysUserView(userEntity);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private SysUserView ConvertToSysUserView(SysUser userEntity)
        {
            if (userEntity != null)
            {
                var role = string.Empty;
                if (userEntity.Roles != null && userEntity.Roles.Count > 0)
                {
                    var rolesId = userEntity.Roles.Select(r => r.RoleId).ToList();
                    var roles = string.Join(",", rolesId);
                    role += roles;
                }
                return new SysUserView
                {
                    Id = userEntity.Id,
                    PasswordHash = userEntity.PasswordHash,
                    UserName = userEntity.UserName,
                    UserTrueName = userEntity.UserTrueName,
                    Email = userEntity.Email,
                    SecurityStamp = userEntity.SecurityStamp,
                    WeChatUserID = userEntity.WeChatUserID,
                    PhoneNumber = userEntity.PhoneNumber,
                    strRoles = role,
                    UpdatedDate = DateTime.UtcNow,
                };
            }
            return new SysUserView { };
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult ChangePassword(SysUserView objModal)
        {
            try
            {
                var userEntity = _objService.Repository.Entities.Where(x => x.Id == objModal.Id).AsNoTracking().FirstOrDefault();
                if (!string.IsNullOrEmpty(objModal.PasswordHash))
                {
                    if (new PasswordHasher().VerifyHashedPassword(userEntity.PasswordHash, objModal.PasswordHash) != PasswordVerificationResult.Failed)
                    {
                        var newPWD = Request["newPWD"];
                        userEntity.PasswordHash = new PasswordHasher().HashPassword(newPWD);
                        string sql = string.Format(@"Update SysUser set PasswordHash='{0}' where id={1}", userEntity.PasswordHash, userEntity.Id);
                        _objService.Repository.SqlExcute(sql);
                        authService.SignOut();
                        return Json(new { Status = 200, Message = "sucessfull" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { Status = 201, Message = "原始密码不正确, 请重新输入." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { Status = 201, Message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        //public override JsonResult Post(SysUserView objModal, string Id)
        //{

        //    //验证错误
        //    if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
        //    {
        //        return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
        //    }

        //    // T0 t = new T0();

        //    if (string.IsNullOrEmpty(Id) || Id == "0")
        //    {
        //        _BaseService.InsertView(objModal);
        //    }
        //    else
        //    {
        //        _BaseService.UpdateView(objModal);

        //    }
        //    return Json(doJson(null), JsonRequestBehavior.AllowGet);
        //}

    }
}
