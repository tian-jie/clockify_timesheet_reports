
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data;

using System.Linq.Expressions;
using System.Web.Configuration;

using Innocellence.Weixin.QY.CommonAPIs;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Controllers;

namespace Innocellence.WeChat.Controllers
{
    /// <summary>
    /// 基类BaseController，过滤器
    /// </summary>
    // [HandleError]
    //[FilterError]
    //[CustomAuthorize]
    //[Themed]
    public class WinxinBaseController : Controller
    {
        //public static readonly string Token = SysCommon.Token;// WebConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        //public static readonly string EncodingAESKey = SysCommon.EncodingAESKey;//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        //public static readonly string AppId = SysCommon.AppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。
        //public static readonly string CorpId = SysCommon.CorpId;//与微信企业账号后台的CorpId设置保持一致，区分大小写。
        //public static readonly string CorpSecret = SysCommon.CorpSecret;

       

        public WinxinBaseController()
        {
           
        }


        public string GetToken()
        {
            return GetToken(int.Parse(WebConfigurationManager.AppSettings["WeixinAppId"]));
            // return resultToken.access_token;
        }

        public string GetToken(int iWeChatID)
        {
            var Config = WeChatCommonService.GetWeChatConfigByID(iWeChatID);

            return AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);

           // return resultToken.access_token;
        }

        ////统一Index action处理
        //public override ActionResult Index()
        //{
        //    return View();
        //}

        /// <summary>
        /// 当前登录的用户属性
        /// </summary>
        // public BASE_USERINFO CurrentUserInfo { get; set; }


        /// <summary>
        /// 重新基类在Action执行之前的事情
        /// </summary>
        /// <param name="filterContext">重写方法的参数</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //////得到用户登录的信息
            //objLoginInfo = Session["UserInfo"] as LoginInfo;


            //////判断用户是否为空
            //if (objLoginInfo == null)
            //{
            //    //
            //    // if (filterContext.)
            //    filterContext.Result = new RedirectResult("/Login/Index");
            //    return;
            //}

           // System.Threading.Thread.Sleep(5000);

            //Logger log = Logger.GetLogger(filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.FullName, CurrentUserInfo.USERREALNAME);
            //log.Debug(WEBUI.Common.LogHelper.GetActionInfo(filterContext));

            //lstMenus=Session["UserMenus"] as List<BASE_SYSMENU>;

            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            //错误记录
            //WHC.Framework.Commons.LogTextHelper.Error(filterContext.Exception);

            // 当自定义显示错误 mode = On，显示友好错误页面
            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                filterContext.ExceptionHandled = true;
                // this.View("Error").ExecuteResult(this.ControllerContext);

                filterContext.Result = new ContentResult() { Content = "-1000 " + filterContext.Exception.Message };
            }
        }


      


    }

}
