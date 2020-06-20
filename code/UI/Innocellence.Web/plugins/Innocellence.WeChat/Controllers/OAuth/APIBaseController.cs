
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

using System.Configuration;

using System.Web.Configuration;
using Infrastructure.Core.Logging;
using Infrastructure.Web.ImageTools;
using Infrastructure.Web.Domain.Entity;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Entity;
using System.Collections.Specialized;


namespace Innocellence.WeChat.Controllers
{
    /// <summary>
    /// 基类BaseController，过滤器
    /// </summary>
    // [HandleError]

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public class APIBaseController<T, T1> : ParentController<T, T1>
        where T : EntityBase<int>, new()
        where T1 : IViewModel, new()
    {

       protected ILogger log;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsService"></param>
        public APIBaseController(IBaseService<T> newsService)
            : base(newsService)
        {
            log = LogManager.GetLogger("WeChatAPI");
        }

        /// <summary>
        /// 重新基类在Action执行之前的事情
        /// </summary>
        /// <param name="filterContext">重写方法的参数</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            // TODO: 想办法获取用户的User id（或者用户id）
            ViewBag.OpenId = "";


         //   log.Debug("API sign Start  appId:{0} Uri:{1} Request URL:{2}", appId, url, Request.Url);
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Request Action: Controller:{0} Action:{1}\r\n", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);
            sb.AppendFormat("Request URL:{0}\r\n", filterContext.HttpContext.Request.Url);

            foreach (var a in filterContext.ActionParameters)
            {
                sb.AppendFormat("Key0:{0} Value0:{1}\r\n", a.Key, a.Value);
            }

            foreach (var a in filterContext.HttpContext.Request.Params.AllKeys)
            {
                sb.AppendFormat("Key:{0} Value:{1}\r\n", a, filterContext.HttpContext.Request.Params[a]);
            }

            sb.AppendLine("Request End---------------------------------------------------------------------");

            log.Debug(sb.ToString());

            base.OnActionExecuting(filterContext);
        }


        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            // TODO: 想办法获取用户的User id（或者用户id）
            ViewBag.OpenId = "";
            //   log.Debug("API sign Start  appId:{0} Uri:{1} Request URL:{2}", appId, url, Request.Url);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Response URL:{0}\r\n", Request.Url);
            sb.AppendFormat("Response Result:{0}\r\n", filterContext.Result);

            //foreach (KeyValuePair<string, string> a in filterContext.HttpContext.)
            //{
            //    sb.AppendFormat("Key:{0} Value:{1}\r\n", a.Key, a.Value);
            //}

            sb.AppendLine("Response End---------------------------------------------------------------------");
            log.Debug(sb.ToString());

            base.OnActionExecuted(filterContext);
        }


        protected bool VerifyParam(string param)
        {
            if (Request["sign"] != WeChatCommonService.GetSignature(param, Request.Params))
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.Result = Json(new
            {
                message = filterContext.Exception.Message,
                success = false
            }, JsonRequestBehavior.AllowGet);
        }



        protected SysWechatConfig GetWechatConfig()
        {
            string appid = Request["appid"];
            return WeChatCommonService.GetWeChatConfigByID(int.Parse(appid));
        }

        protected ActionResult ErrMsg(string strMsg = "参数错误！")
        {
            return Json(new { message = strMsg, success = false }, JsonRequestBehavior.AllowGet);
        }


    }
}