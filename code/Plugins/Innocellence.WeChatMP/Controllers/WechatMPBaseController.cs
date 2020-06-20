
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
using Innocellence.Weixin.MP.AdvancedAPIs.OAuth;
using Infrastructure.Core.Logging;
using Infrastructure.Web.ImageTools;
using Infrastructure.Web.Domain.Entity;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Entity;
using Infrastructure.Web.Domain.Service;


namespace Innocellence.WeChatMP.Controllers
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
    public class WechatMPBaseController<T, T1> : ParentController<T, T1>
        where T : EntityBase<int>, new()
        where T1 : IViewModel, new()
    {
        /// <summary>
        /// 当前的DBService
        /// </summary>
        public IBaseService<T> _newsService;

        //全局用户对象，当前的登录用户
        public SysUser objLoginInfo;

        public int AppId;

        ILogger log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsService"></param>
        public WechatMPBaseController(IBaseService<T> newsService)
            : base(newsService)
        {
            _newsService = newsService;

            log = LogManager.GetLogger("WeChat");

        }

        /// <summary>
        /// 重新基类在Action执行之前的事情
        /// </summary>
        /// <param name="filterContext">重写方法的参数</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // TODO: 想办法获取用户的User id（或者用户id）
            ViewBag.OpenId = "";
            string WeChatUserID = GetOpenId(filterContext);
            log.Debug("GetwechatUserID() : " + WeChatUserID);
            if (!string.IsNullOrEmpty(WeChatUserID))
            {
                ViewBag.wechatUserID = WeChatUserID;
               

                // check if water mark file exists
                //string watermarkFilename = Server.MapPath("/Content/marks/" + WeChatUserID + ".png");
                //if (!System.IO.File.Exists(watermarkFilename))
                //{
                //    try
                //    {
                //        System.Drawing.Image img = ImageUtility.CreateWaterMarkImage(WeChatUserID);
                //        img.Save(watermarkFilename, System.Drawing.Imaging.ImageFormat.Png);
                //    }
                //    catch (Exception e)
                //    {
                //        LogManager.GetLogger(this.GetType()).Error(e);
                //    }
                //}
            }

            base.OnActionExecuting(filterContext);
        }

        private string GetOpenId(ActionExecutingContext filterContext)
        {


            SysWechatConfig weChatConfig;
            string strwechatid = Request["wechatid"];
            if (string.IsNullOrEmpty(strwechatid) && RouteData.Values.ContainsKey("appid"))
            {
                strwechatid = RouteData.Values["appid"].ToString();
            }
            else if (string.IsNullOrEmpty(strwechatid))
            {
                log.Error("wechatid not found!" + Request.Url);
                throw new Exception("wechatid not found!");
            }

            int.TryParse(strwechatid, out AppId);

            //var objLoginInfo = Session["UserInfo"] as WechatUser;

            //LogManager.GetLogger(this.GetType()).Debug("objLoginInfo : " + (objLoginInfo == null?"NULL":objLoginInfo.wechatUserID));
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
                    }
                }
                else
                {
                   log.Debug("SessionId:" + Session.SessionID);
                    Session["ReturnUrlMP"] = Request.Url.ToString();

                    string strRet = WebConfigurationManager.AppSettings["UserBackUrlMP"];

                    string webUrl = CommonService.GetSysConfig("WeChatUrl", "");
                    // string AppId = Request["AppId"];


              

                  



                   


                    string strBackUrl = string.Format("{0}{1}?wechatid={2}",webUrl, strRet, strwechatid);

                    log.Debug("UrlStart:" + strBackUrl);

                    weChatConfig = WeChatCommonService.GetWeChatConfigByID(int.Parse(strwechatid));
                    if (weChatConfig == null)
                    {
                        filterContext.Result = new ContentResult(){ Content= string.Format("appid:{0} 不存在", strwechatid)};

                        log.Error(string.Format("appid:{0} 不存在", strwechatid));
                        return null;
                    }

                    log.Debug("UrlStart");
                    string strUrl = OAuthApi.GetAuthorizeUrl(weChatConfig.WeixinAppId, strBackUrl, "About", Weixin.MP.OAuthScope.snsapi_base);
                    log.Debug(strUrl);

                    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    {
                        AjaxResult<int> result = new AjaxResult<int>();
                        result.Message = new JsonMessage((int)HttpStatusCode.Unauthorized, strUrl);
                        filterContext.Result = Json(result, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        //log.Debug("filterContext.Result: "+strUrl);
                        filterContext.Result = new RedirectResult(strUrl);
                    }
                    return string.Empty;
                }


            }
            return string.Empty;
        }

        public void ExecuteBehavior(int appid, int type, string typecontent, string content = null)
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
                    functionid = functionid.Substring(0, functionid.Length - functionid.Split('/')[functionid.Split('/').Length - 1].Length - 1);
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


    }
}