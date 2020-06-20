
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

using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Infrastructure.Utility.Secutiry;



namespace qyAPITest.Controllers
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
    public class WechatQYBaseController : ParentController<BlankEntity, BlankView>
    {


        //全局用户对象，当前的登录用户
        public SysUser objLoginInfo;

        public int AppId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsService"></param>
        public WechatQYBaseController()
            : base(null)
        {
           

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
            LogManager.GetLogger(this.GetType()).Debug("GetwechatUserID() : " + WeChatUserID);
            if (!string.IsNullOrEmpty(WeChatUserID))
            {
                ViewBag.wechatUserID = WeChatUserID;
                int.TryParse(ViewBag.wechatUserID, out AppId);

                // check if water mark file exists
                string watermarkFilename = Server.MapPath("/Content/marks/" + WeChatUserID + ".png");
                if (!System.IO.File.Exists(watermarkFilename))
                {
                    try
                    {
                        System.Drawing.Image img = ImageUtility.CreateWaterMarkImage(WeChatUserID);
                        img.Save(watermarkFilename, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch (Exception e)
                    {
                        LogManager.GetLogger(this.GetType()).Error(e);
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private string GetOpenId(ActionExecutingContext filterContext)
        {
            //var objLoginInfo = Session["UserInfo"] as WechatUser;

            //LogManager.GetLogger(this.GetType()).Debug("objLoginInfo : " + (objLoginInfo == null?"NULL":objLoginInfo.wechatUserID));
            ////判断用户是否为空
            //if (objLoginInfo == null)
            {
                //LogManager.GetLogger(this.GetType()).Debug("objLoginInfo is null");
                if (HttpContext.Request.IsAuthenticated)
                {
                    LogManager.GetLogger(this.GetType()).Debug("HttpContext.Request.IsAuthenticated");
                    if (Request.UserAgent.IndexOf("MicroMessenger") >= 0)
                    {
                        LogManager.GetLogger(this.GetType()).Debug("WeChat Browser");
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


                   if (!string.IsNullOrEmpty(Request["Ticket"]))
                    {

                       var ticket=Request["Ticket"];
                       Session["Ticket"] = ticket;



                        string enTicket = EncryptionHelper.DecodeFrom64(ticket);
                        enTicket = DesHelper.Decrypt(enTicket, WebConfigurationManager.AppSettings["EncryptKey"]);

                        var openid = enTicket.Split('|')[0];

                        //登录
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, openid, "http://www.w3.org/2001/XMLSchema#string"));
                        claimsIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, openid, "http://www.w3.org/2001/XMLSchema#string"));
                        claimsIdentity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"));

                        HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimsIdentity);

                        // 
                    } else  if (Session["Ticket"] == null)
                    {
                        var strUrl = WebConfigurationManager.AppSettings["OAuthUrl"];

                        filterContext.Result = new RedirectResult(strUrl +Server.UrlEncode( EncryptionHelper.ConvertBase64(Request.Url.ToString())));
                    }
                   


                    //    LogManager.GetLogger(this.GetType()).Debug("SessionId:" + Session.SessionID);
                    //    Session["ReturnUrlMP"] = Request.Url.ToString();

                    //    string strRet = WebConfigurationManager.AppSettings["UserBackUrlMP"];
                    //    // string AppId = Request["AppId"];


                    //    string strwechatid = Request["wechatid"];

                    //    string strBackUrl = string.Format("{0}?wechatid={1}", strRet, strwechatid);

                    //    LogManager.GetLogger(this.GetType()).Debug("UrlStart:" + strBackUrl);

                    //   // var weChatConfig = WeChatCommonService.GetWeChatConfigByID(int.Parse(strwechatid));


                    //    LogManager.GetLogger(this.GetType()).Debug("UrlStart");
                    //    string strUrl = OAuthApi.GetAuthorizeUrl("wx2a3f5167603c5caf", strBackUrl, "About", Innocellence.Weixin.MP.OAuthScope.snsapi_base);
                    //    LogManager.GetLogger(this.GetType()).Debug(strUrl);

                    //    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    //    {
                    //        AjaxResult<int> result = new AjaxResult<int>();
                    //        result.Message = new JsonMessage((int)HttpStatusCode.Unauthorized, strUrl);
                    //        filterContext.Result = Json(result, JsonRequestBehavior.AllowGet);

                    //    }
                    //    else
                    //    {
                    //        LogManager.GetLogger(this.GetType()).Debug("filterContext.Result = new RedirectResult(strUrl)");
                    //        filterContext.Result = new RedirectResult(strUrl);
                    //    }
                    //    return string.Empty;
                }


            }
            return string.Empty;
        }

        //public void ExecuteBehavior(int appid, int type, string typecontent, string content = null)
        //{
        //    //记录用户行为
        //    WebRequestPost wr = new WebRequestPost();
        //    string url = Request.RawUrl.ToUpper();//带参数
        //    //将id作为内容插入到数据库中

        //    string functionid = Request.FilePath.ToUpper();//不带参数（不带？后面的参数，但如果是/123则带）

        //    if (url.Contains("?"))
        //    {

        //    }
        //    else
        //    {
        //        if (isNumberic1(content))
        //        {
        //            functionid = functionid.Substring(0, functionid.Length - functionid.Split('/')[functionid.Split('/').Length - 1].Length - 1);
        //        }

        //    }

        //    if (!string.IsNullOrEmpty(typecontent))
        //    {
        //        wr.CallUserBehavior(functionid, ViewBag.WeChatUserID, appid.ToString(), typecontent, url, type);
        //    }
        //    else
        //    {
        //        wr.CallUserBehavior(functionid, ViewBag.WeChatUserID, appid.ToString(), content, url, type);
        //    }

        //}
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