
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Logging;
using Infrastructure.Web.Identity;
using Infrastructure.Web.Identity.Permissions;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Services;
using Infrastructure.Web.Localization;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;


namespace Infrastructure.Web.Service.Common
{
    //[UsedImplicitly]
    public class RolesBasedAuthorizationService : IAuthorizationService
    {
        private readonly ISysRoleService _roleService;
        //    private readonly IWorkContextAccessor _workContextAccessor;
        //    private readonly IAuthorizationServiceEventHandler _authorizationServiceEventHandler;
        private static readonly string[] AnonymousRole = new[] { "Anonymous" };
        private static readonly string[] AuthenticatedRole = new[] { "Authenticated" };

        public RolesBasedAuthorizationService(ISysRoleService roleService)
        {
            _roleService = roleService;
            //  _workContextAccessor = workContextAccessor;
            //  _authorizationServiceEventHandler = authorizationServiceEventHandler;

            T = NullLocalizer.Instance;
            Logger = LogManager.GetLogger(typeof(RolesBasedAuthorizationService));
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }


        public void CheckAccess(Permission permission, IUser<int> user)
        {
            if (!TryCheckAccess(permission, user))
            {
                throw new Exception(T("A security exception occurred in the content management system.").Text)
                {

                };
            }
        }


        public bool TryCheckAccess(AuthorizationContext filterContext, IUser<int> user)
        {
            // var strUrl = string.Format("/{0}/{1}",
            // filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,filterContext.ActionDescriptor.ActionName);

            //超级管理员
            if (!String.IsNullOrEmpty(EngineContext.Current.WebConfig.SupperUser) &&
                string.Equals(EngineContext.Current.WebConfig.SupperUser, user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }


            //判断url，多个url用“,”号分割
            var strUrl = filterContext.HttpContext.Request.Url.AbsolutePath;

            return (user as SysUser).Menus.Exists(a =>
            {
                if (a.NavigateUrl.IndexOf(',') >= 0)
                {
                    var str = a.NavigateUrl.Split(',');
                    foreach (var b in str)
                    {
                        if (UrlCompare(filterContext.HttpContext.Request.Url, b))
                        {
                            return true;
                        }
                        //if (strUrl.Equals(b, StringComparison.OrdinalIgnoreCase))
                        //{
                        //    return true;
                        //}
                    }
                    return false;
                }
                else
                {
                    return UrlCompare(filterContext.HttpContext.Request.Url, a.NavigateUrl);// strUrl.Equals(a.NavigateUrl, StringComparison.OrdinalIgnoreCase);

                }
            });
            // return true;
        }

        /// <summary>
        /// Url 比较
        /// </summary>
        /// <param name="uRequestUrl"></param>
        /// <param name="strUrlTo"></param>
        /// <returns></returns>
        private bool UrlCompare(Uri uRequestUrl,string strUrlTo)
        {
            //直接相等
            if (uRequestUrl.AbsolutePath.Equals(strUrlTo, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            //带查询条件的相等
            if (string.IsNullOrEmpty(uRequestUrl.Query))
            {
                if ((uRequestUrl.AbsolutePath + uRequestUrl.Query).Equals(strUrlTo, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            //带通配符号的相等
            if (strUrlTo.EndsWith("*"))
            {
                if (uRequestUrl.AbsolutePath.IndexOf(strUrlTo.TrimEnd('*'), StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryCheckAccess(Permission permission, AuthorizationContext filterContext, IUser<int> user)
        {
            if (permission != null)
            {
                return TryCheckAccess(permission, user);
            }
            else
            {
                return TryCheckAccess(filterContext, user);
            }
        }

        public bool TryCheckAccess(Permission permission, IUser<int> user)
        {
            return true;

            //  var context = new CheckAccessContext { Permission = permission, User = user, Content = content };
            ////  _authorizationServiceEventHandler.Checking(context);

            //  for (var adjustmentLimiter = 0; adjustmentLimiter != 3; ++adjustmentLimiter) {
            //      if (!context.Granted && context.User != null) {
            //          if (!String.IsNullOrEmpty(_workContextAccessor.GetContext().CurrentSite.SuperUser) &&
            //                 String.Equals(context.User.UserName, _workContextAccessor.GetContext().CurrentSite.SuperUser, StringComparison.Ordinal)) {
            //              context.Granted = true;
            //          }
            //      }

            //      if (!context.Granted) {

            //          // determine which set of permissions would satisfy the access check
            //          var grantingNames = PermissionNames(context.Permission, Enumerable.Empty<string>()).Distinct().ToArray();

            //          // determine what set of roles should be examined by the access check
            //          IEnumerable<string> rolesToExamine;
            //          if (context.User == null) {
            //              rolesToExamine = AnonymousRole;
            //          }
            //          else if (context.User.Has<IUserRoles>()) {
            //              // the current user is not null, so get his roles and add "Authenticated" to it
            //              rolesToExamine = context.User.As<IUserRoles>().Roles;

            //              // when it is a simulated anonymous user in the admin
            //              if (!rolesToExamine.Contains(AnonymousRole[0])) {
            //                  rolesToExamine = rolesToExamine.Concat(AuthenticatedRole);   
            //              }
            //          }
            //          else {
            //              // the user is not null and has no specific role, then it's just "Authenticated"
            //              rolesToExamine = AuthenticatedRole;
            //          }

            //          foreach (var role in rolesToExamine) {
            //              foreach (var permissionName in _roleService.GetPermissionsForRoleByName(role)) {
            //                  string possessedName = permissionName;
            //                  if (grantingNames.Any(grantingName => String.Equals(possessedName, grantingName, StringComparison.OrdinalIgnoreCase))) {
            //                      context.Granted = true;
            //                  }

            //                  if (context.Granted)
            //                      break;
            //              }

            //              if (context.Granted)
            //                  break;
            //          }
            //      }

            //      //context.Adjusted = false;
            //      //_authorizationServiceEventHandler.Adjust(context);
            //      //if (!context.Adjusted)
            //      //    break;
            //  }

            // // _authorizationServiceEventHandler.Complete(context);

            //  return context.Granted;
        }

        public IUser<int> GetUser(IIdentity objWI)
        {
            SysUserService objServ = new SysUserService();
            return objServ.AutoLogin(objWI);
        }

        private static IEnumerable<string> PermissionNames(Permission permission, IEnumerable<string> stack)
        {
            // the given name is tested
            yield return permission.Name;

            // iterate implied permissions to grant, it present
            if (permission.ImpliedBy != null && permission.ImpliedBy.Any())
            {
                foreach (var impliedBy in permission.ImpliedBy)
                {
                    // avoid potential recursion
                    if (stack.Contains(impliedBy.Name))
                        continue;

                    // otherwise accumulate the implied permission names recursively
                    foreach (var impliedName in PermissionNames(impliedBy, stack.Concat(new[] { permission.Name })))
                    {
                        yield return impliedName;
                    }
                }
            }

            yield return StandardPermissions.SiteOwner.Name;
        }

    }
}
