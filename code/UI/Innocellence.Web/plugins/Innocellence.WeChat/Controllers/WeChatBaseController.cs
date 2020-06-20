
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data;
using Infrastructure.Utility.Extensions;
using Infrastructure.Web.UI;
using Infrastructure.Web;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;

using System.Linq.Expressions;
using System.Net;
using System.Security.Principal;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Service;
using System.Configuration;
using System.Web.Configuration;
using Innocellence.Weixin.HttpUtility;
using Innocellence.Weixin.QY.AdvancedAPIs.OAuth2;
using Infrastructure.Core.Logging;
using Infrastructure.Web.ImageTools;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.WeChat.Domain.Services;
//using Innocellence.WeChatMain.Service.Common;


namespace Innocellence.WeChat.Controllers
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
    public class WeChatBaseController<T, T1> : ParentController<T, T1>
        where T : EntityBase<int>, new()
        where T1 : IViewModel, new()
    {
        //private static string _spSsoUrl = WebConfigurationManager.AppSettings["SpSsoUrl"];

        //public static readonly string Token = WebConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        //public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        ////public static readonly string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
        //public static readonly string CorpId = WebConfigurationManager.AppSettings["WeixinCorpId"];//与微信企业账号后台的CorpId设置保持一致，区分大小写。
        //public static readonly string CorpSecret = WebConfigurationManager.AppSettings["WeixinCorpSecret"];
        //public static readonly string SSOUrl = WebConfigurationManager.AppSettings["SSOUrl"];

        /// <summary>
        /// 当前的DBService
        /// </summary>
        public IBaseService<T> _newsService;

        private WeChatAppUserService _weChatAppUserService = new WeChatAppUserService();

        //全局用户对象，当前的登录用户
        public SysUser objLoginInfo;

        public int AppId;

        ILogger log = LogManager.GetLogger("WeChat");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsService"></param>
        public WeChatBaseController(IBaseService<T> newsService)
            : base(newsService)
        {
            //var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //json.SerializerSettings.DateFormatHandling
            //= Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;

            _newsService = newsService;
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

            // 想办法获取用户的XXX id（或者用户id）
            ViewBag.WeChatUserID = "";

#if DEBUG
            string WeChatUserID = "C217353";
#else

            string WeChatUserID = GetWeChatUserID(filterContext);
#endif
            log.Debug("GetWeChatUserID() : " + WeChatUserID);
            if (!string.IsNullOrEmpty(WeChatUserID))
            {
                ViewBag.WeChatUserID = WeChatUserID;

                // 如果取到了，还要到我们的数据库里走一遭
                //var existingUser = _weChatAppUserService.Entities.Where(a => a.WeChatUserID == WeChatUserID).FirstOrDefault();

                // check if water mark file exists
                //string watermarkFilename = Server.MapPath("/Content/marks/" + WeChatUserID + ".png");
                //if (!System.IO.File.Exists(watermarkFilename))
                //{
                //    System.Drawing.Image img = ImageUtility.CreateWaterMarkImage(WeChatUserID);
                //    img.Save(watermarkFilename, System.Drawing.Imaging.ImageFormat.Png);
                //}
            }

            base.OnActionExecuting(filterContext);
        }
        public void ExecuteBehavior(int appid, int type, string typecontent,string content=null)
        {
            //记录用户行为
            WebRequestPost wr = new WebRequestPost();
            string url = Request.RawUrl.ToUpper();//带参数
            //将id作为内容插入到数据库中

            string functionid = Request.FilePath.ToUpper();//不带参数（不带？后面的参数，但如果是/123则带）
           
            if (url.Contains("?"))
            {
                
            }
            else
            {
                if (isNumberic1(content))
                {
                    functionid = functionid.Substring(0,functionid.Length - functionid.Split('/')[functionid.Split('/').Length - 1].Length - 1);
                }
                
            }
           
            if (!string.IsNullOrEmpty(typecontent))
            {
                wr.CallUserBehavior(functionid, ViewBag.WeChatUserID, appid.ToString(), typecontent, url, type);
            }
            else
            {
                wr.CallUserBehavior(functionid, ViewBag.WeChatUserID, appid.ToString(), content, url, type);
            }

        }
        //判断是否是数字
        public static bool isNumberic1(string _string)
        {
            if (string.IsNullOrEmpty(_string))
                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
        private string GetWeChatUserID(ActionExecutingContext filterContext)
        {


            string strwechatid = Request["wechatid"];

            if (string.IsNullOrEmpty(strwechatid) && RouteData.Values.ContainsKey("appid"))
            {
                strwechatid = RouteData.Values["appid"].ToString();
            }
            else if (string.IsNullOrEmpty(strwechatid))
            {
                log.Error("wechatid not found!"+Request.Url);
                filterContext.Result = new ContentResult() { Content = "wechatid not found!" };
                return null;
            }

            AppId =int.Parse(strwechatid);
            //var objLoginInfo = Session["UserInfo"] as WechatUser;

            //LogManager.GetLogger(this.GetType()).Debug("objLoginInfo : " + (objLoginInfo == null?"NULL":objLoginInfo.WeChatUserID));
            ////判断用户是否为空
            //if (objLoginInfo == null)
            {
                //LogManager.GetLogger(this.GetType()).Debug("objLoginInfo is null");
                if (HttpContext.Request.IsAuthenticated)
                {
                    log.Debug("HttpContext.Request.IsAuthenticated");
                    if (Request.UserAgent.IndexOf("MicroMessenger") >= 0)
                    {
                        log.Debug("WeChat Browser");
                        var windowsIdentity = User.Identity;
                        if (windowsIdentity != null)
                        {
                            return windowsIdentity.Name;
                        }
                        else
                        {
                            return string.Empty;
                        }
                        //LogManager.GetLogger(this.GetType()).Debug("User.Identity" + User.Identity.Name);
                        //SysUserService objServ = new SysUserService();
                        //var objUser = objServ.AutoLogin(windowsIdentity);
                        //if (objUser != null)
                        //{
                        //    objLoginInfo = new WechatUser() { WeChatUserID = objUser.UserName, WechatID = objUser.UserName };
                        //    Session["UserInfo"] = objLoginInfo;
                        //    return objUser.UserName;
                        //}
                        //else
                        //{
                        //    LogManager.GetLogger(this.GetType()).Debug("objUser is still NULL");
                        //}
                    }
                }
                else
                {
                    log.Debug("SessionId:" + Session.SessionID);
                    Session["ReturnUrl"] = Request.RawUrl;// Request.Url.ToString();


                    string strRet = WebConfigurationManager.AppSettings["UserBackUrl"];
                    string webUrl = CommonService.GetSysConfig("WeChatUrl", "");

                    

                    string strBackUrl = string.Format("{0}{1}?wechatid={2}",webUrl , strRet , strwechatid);

                    log.Debug("UrlStart :" + strBackUrl);

                    var weChatConfig=  WeChatCommonService.GetWeChatConfigByID(int.Parse(strwechatid));

                    string strUrl = OAuth2Api.GetCode(weChatConfig.WeixinCorpId, strBackUrl, Server.UrlEncode(Request.RawUrl));
                    log.Debug(strUrl);

                    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    {
                        AjaxResult<int> result = new AjaxResult<int>();
                        result.Message = new JsonMessage((int)HttpStatusCode.Unauthorized, strUrl);
                        filterContext.Result = Json(result, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        log.Debug("filterContext.Result = new RedirectResult(strUrl) : " + strUrl);
                        filterContext.Result = new RedirectResult(strUrl);
                    }
                    return string.Empty;
                }


            }
            return string.Empty;
        }


    }
}
