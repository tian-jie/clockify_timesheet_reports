
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Text;
//using System.Data;
//using Infrastructure.Web.UI;
//using Infrastructure.Web;
//using Infrastructure.Core;
//using Infrastructure.Core.Data;


//using System.Linq.Expressions;
//using System.Net;
//using System.Security.Principal;

//using Innocellence.WeChat.Domain.Service;
//using System.Configuration;

//using System.Web.Configuration;
//using Innocellence.Weixin.QY.AdvancedAPIs.OAuth2;
//using Infrastructure.Core.Logging;
//using Infrastructure.Web.ImageTools;
//using Infrastructure.Web.Domain.Entity;
//using Infrastructure.Web.Domain.Service;
//using Innocellence.Weixin.QY.AdvancedAPIs;
//using Innocellence.WeChatMain.Services;
//using Innocellence.WeChat.Domain.Services;


//namespace Innocellence.WeChatMain.Controllers
//{
//    /// <summary>
//    /// 基类BaseController，过滤器
//    /// </summary>
//    // [HandleError]

//    //[FilterError]
//    //[CustomAuthorize]

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <typeparam name="T1"></typeparam>
//    public class WeChatBaseController<T, T1> : ParentController<T, T1>
//        where T : EntityBase<int>, new()
//        where T1 : IViewModel, new()
//    {
//        private static string _spSsoUrl = WebConfigurationManager.AppSettings["SpSsoUrl"];

//        public static readonly string Token = WebConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。
//        public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
//        //public static readonly string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
//        public static readonly string CorpId = WebConfigurationManager.AppSettings["WeixinCorpId"];//与微信企业账号后台的CorpId设置保持一致，区分大小写。
//        public static readonly string CorpSecret = WebConfigurationManager.AppSettings["WeixinCorpSecret"];
//        public static readonly string SSOUrl = WebConfigurationManager.AppSettings["SSOUrl"];

//        /// <summary>
//        /// 当前的DBService
//        /// </summary>
//        public IBaseService<T> _newsService;

//        private WeChatAppUserService _weChatAppUserService = new WeChatAppUserService();

//        //全局用户对象，当前的登录用户
//        public SysUser objLoginInfo;

//        public int AppId;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="newsService"></param>
//        public WeChatBaseController(IBaseService<T> newsService)
//            : base(newsService)
//        {
//            //var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
//            //json.SerializerSettings.DateFormatHandling
//            //= Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;

//            _newsService = newsService;
//            //if (objLoginInfo != null)
//            //{
//            //    _newsService.LoginUsrID = objLoginInfo.UserID;
//            //}


//        }

//        /// <summary>
//        /// 重新基类在Action执行之前的事情
//        /// </summary>
//        /// <param name="filterContext">重写方法的参数</param>
//        protected override void OnActionExecuting(ActionExecutingContext filterContext)
//        {

//            // 想办法获取用户的wechatUser id（或者用户id）
//            ViewBag.WeChatUserID = "";

//            string WeChatUserID = GetWeChatUserID(filterContext);
//            LogManager.GetLogger(this.GetType()).Debug("GetWeChatUserID() : " + WeChatUserID);
//            if (!string.IsNullOrEmpty(WeChatUserID))
//            {
//                ViewBag.WeChatUserID = WeChatUserID;

//                // 如果取到了，还要到我们的数据库里走一遭
//                //var existingUser = _weChatAppUserService.Entities.Where(a => a.WeChatUserID == WeChatUserID).FirstOrDefault();

//                // check if water mark file exists
//                string watermarkFilename = Server.MapPath("/marks/" + WeChatUserID + ".png");
//                if (!System.IO.File.Exists(watermarkFilename))
//                {
//                    System.Drawing.Image img = ImageUtility.CreateWaterMarkImage(WeChatUserID);
//                    img.Save(watermarkFilename, System.Drawing.Imaging.ImageFormat.Png);
//                }
//            }


//            //////// if (string.IsNullOrEmpty(Request["_AppID"])) { Session["AppID"] = Request["_AppID"]; }

//            //////// var objAppID = Session["AppID"];
//            //////// AppId = 0;
//            //////// if (objAppID != null)
//            //////// {
//            ////////     AppId = int.Parse(objAppID.ToString());
//            //////// }
//            //////// ViewBag.AppList = CommonService.lstWeChatConfig;
//            //////// ViewBag.CurAppID = AppId;



//            //////// ////得到用户登录的信息
//            //////// objLoginInfo = Session["UserInfo"] as SysUser;



//            //////// ////判断用户是否为空   应该使用 AuthorizeAttribute,临时解决一下
//            ////////if (objLoginInfo == null && (filterContext.ActionDescriptor.ActionName.ToLower() != "login" ||
//            ////////     filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower()!="account"))
//            //////// {

//            ////////     //windows登录
//            ////////     if (objLoginInfo == null && (Request.IsAuthenticated && (User.Identity is WindowsIdentity)))
//            ////////     {
//            ////////         var windowsIdentity = (WindowsIdentity)User.Identity;
//            ////////         //获取用户
//            ////////         var strUser = SysCommon.GetUserName(windowsIdentity);
//            ////////         //数据库获取设置信息
//            ////////         BaseService<SysUser> objServ = new BaseService<SysUser>();
//            ////////         var obj = objServ.Entities.Where(a => a.WeChatUserID == strUser).FirstOrDefault();
//            ////////         if (obj != null)
//            ////////         {
//            ////////             objLoginInfo = obj;
//            ////////             Session["UserInfo"] = objLoginInfo;

//            ////////             //登录日志
//            ////////             BaseService<Logs> objServLogs = new BaseService<Logs>();
//            ////////             objServLogs.Insert(new Logs()
//            ////////             {
//            ////////                 LogCate = "AdminLogin",
//            ////////                 LogContent = "登录成功",
//            ////////                 CreatedUserID = objLoginInfo.Id.ToString(),
//            ////////                 CreatedUserName = objLoginInfo.WeChatUserID
//            ////////             });
//            ////////         }
//            ////////     }
//            ////////     //
//            ////////     // if (filterContext.

//            ////////     if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
//            ////////     {
//            ////////         AjaxResult<int> result = new AjaxResult<int>();
//            ////////         result.Message = new JsonMessage((int)HttpStatusCode.Unauthorized, "Please login");
//            ////////         filterContext.Result = Json(result, JsonRequestBehavior.AllowGet);

//            ////////     }
//            ////////     else
//            ////////     {
//            ////////         //Redirect()
//            ////////         filterContext.Result =  Redirect("~/Account/Login");
//            ////////     }
//            ////////    // return;
//            //////// }

//            ////////if (objLoginInfo != null)
//            ////////{
//            ////////    _BaseService.LoginUserID = objLoginInfo.Id;
//            ////////    _BaseService.LoginUserName = objLoginInfo.WeChatUserID;
//            ////////    SetLanguage("EN");
//            ////////}

//            ////////// System.Threading.Thread.Sleep(5000);

//            //////// //Logger log = Logger.GetLogger(filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.FullName, CurrentUserInfo.USERREALNAME);
//            //////// //log.Debug(WEBUI.Common.LogHelper.GetActionInfo(filterContext));

//            //////// //lstMenus=Session["UserMenus"] as List<BASE_SYSMENU>;

//            base.OnActionExecuting(filterContext);
//        }

//        private string GetWeChatUserID(ActionExecutingContext filterContext)
//        {
//            //var objLoginInfo = Session["UserInfo"] as WechatUser;

//            //LogManager.GetLogger(this.GetType()).Debug("objLoginInfo : " + (objLoginInfo == null?"NULL":objLoginInfo.WeChatUserID));
//            ////判断用户是否为空
//            //if (objLoginInfo == null)
//            {
//                //LogManager.GetLogger(this.GetType()).Debug("objLoginInfo is null");
//                if (HttpContext.Request.IsAuthenticated)
//                {
//                    LogManager.GetLogger(this.GetType()).Debug("HttpContext.Request.IsAuthenticated");
//                    if (Request.UserAgent.IndexOf("MicroMessenger") >= 0)
//                    {
//                        LogManager.GetLogger(this.GetType()).Debug("WeChat Browser");
//                        var windowsIdentity = User.Identity;
//                        if (windowsIdentity != null)
//                        {
//                            return windowsIdentity.Name;
//                        }
//                        else
//                        {
//                            return string.Empty;
//                        }
//                        //LogManager.GetLogger(this.GetType()).Debug("User.Identity" + User.Identity.Name);
//                        //SysUserService objServ = new SysUserService();
//                        //var objUser = objServ.AutoLogin(windowsIdentity);
//                        //if (objUser != null)
//                        //{
//                        //    objLoginInfo = new WechatUser() { WeChatUserID = objUser.UserName, WechatID = objUser.UserName };
//                        //    Session["UserInfo"] = objLoginInfo;
//                        //    return objUser.UserName;
//                        //}
//                        //else
//                        //{
//                        //    LogManager.GetLogger(this.GetType()).Debug("objUser is still NULL");
//                        //}
//                    }
//                }
//                else
//                {
//                    LogManager.GetLogger(this.GetType()).Debug("SessionId:" + Session.SessionID);
//                    Session["ReturnUrl"] = Request.RawUrl;// Request.Url.ToString();
                    

//                    string strRet = WebConfigurationManager.AppSettings["UserBackUrl"];
//                    string webUrl = CommonService.GetSysConfig("WebUrl", "");

//                    LogManager.GetLogger(this.GetType()).Debug("UrlStart");
//                    string strUrl = OAuth2Api.GetCode(CorpId, Server.UrlEncode(webUrl + strRet), Server.UrlEncode(Request.RawUrl));
//                    LogManager.GetLogger(this.GetType()).Debug(strUrl);

//                    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
//                    {
//                        AjaxResult<int> result = new AjaxResult<int>();
//                        result.Message = new JsonMessage((int)HttpStatusCode.Unauthorized, strUrl);
//                        filterContext.Result = Json(result, JsonRequestBehavior.AllowGet);

//                    }
//                    else
//                    {
//                        LogManager.GetLogger(this.GetType()).Debug("filterContext.Result = new RedirectResult(strUrl) : " + strUrl); 
//                        filterContext.Result = new RedirectResult(strUrl);
//                    }
//                    return string.Empty;
//                }


//            }
//            return string.Empty;
//        }


//    }
//}
