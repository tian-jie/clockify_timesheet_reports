﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data;
using Infrastructure.Web.UI;
using Infrastructure.Web;
using Infrastructure.Core;
using Infrastructure.Core.Data;


using System.Linq.Expressions;
using System.Net;
using System.Security.Principal;




namespace Innocellence.Authentication.Controllers
{
    /// <summary>
    /// 基类BaseController，过滤器
    /// </summary>
    // [HandleError]

    //[FilterError]
    //[CustomAuthorize]

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public class BaseController<T, T1> : ParentController<T,T1>
        where T : EntityBase<int>,new()
        where T1 : IViewModel, new()
    {
        /// <summary>
        /// 当前的DBService
        /// </summary>
        public IBaseService<T> _objService;

        //全局用户对象，当前的登录用户
      //  public SysUser objLoginInfo;

        public int AppId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsService"></param>
        public BaseController(IBaseService<T> newsService)
            : base(newsService)
        {
            //var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //json.SerializerSettings.DateFormatHandling
            //= Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;

            _objService = newsService;
            //if (objLoginInfo != null)
            //{
            //    _newsService.LoginUsrID = objLoginInfo.UserID;
            //}

          
        }

        /// <summary>
        /// 重新基类在Action执行之前的事情
        /// </summary>
        /// <param name="filterContext">重写方法的参数</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {


           //// if (!string.IsNullOrEmpty(Request["_AppID"])) { Session["AppID"] = Request["_AppID"]; }

           // var objAppID = Session["AppID"];
           // AppId = 0;
           // if (objAppID != null)
           // {
           //     AppId = int.Parse(objAppID.ToString());
           // }
           // else if (CommonService.lstWeChatConfig.Count>0)
           // {
           //     AppId = int.Parse(CommonService.lstWeChatConfig[0].WeixinAppId);
           //     Session["AppID"] = AppId;
           // }
           //// ViewBag.AppList = CommonService.lstWeChatConfig;
           //// ViewBag.CurAppID = AppId;



           // ////得到用户登录的信息
           // objLoginInfo = Session["UserInfo"] as SysUser;

          

           // ////判断用户是否为空   应该使用 AuthorizeAttribute,临时解决一下
           // if ((objLoginInfo == null ||! Request.IsAuthenticated) && (filterContext.ActionDescriptor.ActionName.ToLower() != "login" ||
           //     filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower()!="account"))
           // {

           //     //windows登录,自动获取用户信息
           //     if (objLoginInfo == null && (Request.IsAuthenticated && (User.Identity is WindowsIdentity)))
           //     {
           //         var windowsIdentity = (WindowsIdentity)User.Identity;

           //         SysUserService objServ = new SysUserService();
           //        var objUser= objServ.AutoLogin(windowsIdentity);
           //        if (objUser != null)
           //        {
           //            objLoginInfo = objUser;
           //            Session["UserInfo"] = objLoginInfo;
           //        }
           //        else
           //        {

           //        }
                  
           //     }

           //     if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
           //     {
           //         AjaxResult<int> result = new AjaxResult<int>();
           //         result.Message = new JsonMessage((int)HttpStatusCode.Unauthorized, "Please login");
           //         filterContext.Result = Json(result, JsonRequestBehavior.AllowGet);

           //     }
           //     else
           //     {
           //         //Redirect()
           //         filterContext.Result =  Redirect("~/Account/Login");
           //     }
           //    // return;
           // }

           //if (objLoginInfo != null)
           //{
           //    _BaseService.LoginUserID = objLoginInfo.Id;
            //    _BaseService.LoginUserName = objLoginInfo.XXX;
           //    SetLanguage("EN");
           //}

           //// System.Threading.Thread.Sleep(5000);

           // //Logger log = Logger.GetLogger(filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.FullName, CurrentUserInfo.USERREALNAME);
           // //log.Debug(WEBUI.Common.LogHelper.GetActionInfo(filterContext));

           // //lstMenus=Session["UserMenus"] as List<BASE_SYSMENU>;

            base.OnActionExecuting(filterContext);
        }

        

    }
}
