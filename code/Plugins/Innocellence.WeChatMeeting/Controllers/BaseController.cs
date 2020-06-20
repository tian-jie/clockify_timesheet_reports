using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.MVC.Attribute;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Innocellence.WeChatMeeting.Controllers
{
    /// <summary>
    /// 基类BaseController，过滤器
    /// </summary>
    // [HandleError]

    //[FilterError]
    //[CustomAuthorize]
    [CustomAuthorize]
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public class BaseController<T, T1> : ParentController<T, T1>
        where T : EntityBase<int>, new()
        where T1 : IViewModel, new()
    {
        /// <summary>
        /// 当前的DBService
        /// </summary>
        public IBaseService<T> _objService;

        //全局用户对象，当前的登录用户
        public SysUser objLoginInfo;

        public int AppId;

        public int AccountManageID;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsService"></param>
        public BaseController(IBaseService<T> newsService)
            : base(newsService)
        {
            _objService = newsService;
           
        }

        /// <summary>
        /// 重新基类在Action执行之前的事情
        /// </summary>
        /// <param name="filterContext">重写方法的参数</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.Cookies.AllKeys.Contains("AccountManageId"))
            {
                var AM = Request.Cookies["AccountManageId"];
                AccountManageID = int.Parse(AM.Value);
            }

            if (!string.IsNullOrEmpty(Request["AppId"]))
            {
                AppId = int.Parse(Request["AppId"]);
                //AccountManageID = AM.Value);
            }
         
            base.OnActionExecuting(filterContext);
        }

        public string AccessToken
        {
            get { return WeChatCommonService.GetWeiXinToken(AppId); }
        }

        protected virtual void AppDataPermissionCheck(IDataPermissionCheck service, int targetAppId, int currentAppId)
        {
            if (service.AppDataCheck(targetAppId, currentAppId)) return;
            //log.Warn("user({0}) want to access {1} app that have not been authorization!", sysUser.UserName, targetAppId);
            if (HttpContext.Request.IsAjaxRequest())
            {
                throw new HttpException(403, "You are not authorized to access this page.");
            }
            HttpContext.Response.Redirect("/403.html", true);
        }

    }
}