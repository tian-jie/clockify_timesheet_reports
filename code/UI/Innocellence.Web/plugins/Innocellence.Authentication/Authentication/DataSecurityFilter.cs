using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Entity;
using Microsoft.AspNet.Identity;

namespace Innocellence.Authentication.Authentication
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class DataSecurityFilter : ActionFilterAttribute
    {
        //private readonly bool checkData;
        //private string KeyParameterName = "id";
        //private Type _service;


        //public DataSecurityFilter()
        //{
        //    //_controllerType = controllerType;
        //    //this.checkData = checkData;
        //    //if (!string.IsNullOrEmpty(keyParameterName))
        //    //{
        //    //    KeyParameterName = keyParameterName;
        //    //}

        //    //if (service != null)
        //    //{
        //    //    _service = service;
        //    //}
        //}

        private static readonly ILogger log = LogManager.GetLogger(typeof(DataSecurityFilter).Name);

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            var val = filterContext.ActionParameters.FirstOrDefault(x => string.Equals(x.Key, "AppId", StringComparison.CurrentCultureIgnoreCase));

            if (val.Key == null || val.Value == null)
            {
                log.Debug("the {0} action dones not exist appid parameter!", filterContext.ActionDescriptor.ActionName);
                throw new ArgumentException(@"Appid is required!");
            }

            var targetAppId = Convert.ToInt32(val.Value);

            // ReSharper disable once PossibleNullReferenceException
            var user = session["UserInfo"] as IUser<int>;

            var sysUser = user as SysUser;

            // ReSharper disable once PossibleNullReferenceException
            if (sysUser.Menus.Any(x =>
            {
                var navigates = x.NavigateUrl.Split(',');
                if (string.IsNullOrEmpty(x.NavigateUrl) || navigates.Length == 1)
                {
                    if (x.AppId == targetAppId)
                    {
                        return true;
                    }
                }

                var list = navigates.ToList();
                list.RemoveAt(0);
                return x.AppId == targetAppId && list.Any(y =>
                {
                    if (y.Contains("*"))
                    {
                        y = y.Replace("*", string.Empty);
                    }
                    return filterContext.HttpContext.Request.CurrentExecutionFilePath.IndexOf(y, StringComparison.Ordinal) >= 0;
                });
            }))
            { }
            else
            {
                UnauthRedirect(filterContext, sysUser.UserName, targetAppId);
            }


            base.OnActionExecuting(filterContext);
        }

        private static void UnauthRedirect(ActionExecutingContext filterContext, string userName, int targetAppId)
        {
            log.Warn("user({0}) want to access {1} app that have not been authorization!", userName, targetAppId);
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                throw new HttpException(403, "You are not authorized to access this page.");
            }
            filterContext.HttpContext.Response.Redirect("/403.html", true);
        }
    }

}