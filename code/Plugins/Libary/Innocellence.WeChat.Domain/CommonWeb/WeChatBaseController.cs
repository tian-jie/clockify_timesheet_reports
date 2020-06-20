
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
using System.Security.Claims;
using Innocellence.WeChat.Domain.ModelsView;
using Infrastructure.Core.Infrastructure;
using Newtonsoft.Json;
using Innocellence.WeChat.Domain.ViewModel;
using System.Threading.Tasks;
//using Innocellence.WeChatMain.Service.Common;


namespace Innocellence.WeChat.Domain
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


        /// <summary>
        /// 当前的DBService
        /// </summary>
        public IBaseService<T> _newsService;

        private WeChatAppUserService _weChatAppUserService = new WeChatAppUserService();
        private IAddressBookService _addressBookService = EngineContext.Current.Resolve<IAddressBookService>();
        private IWechatMPUserService _wechatMPUserService = EngineContext.Current.Resolve<IWechatMPUserService>();

        //全局用户对象，当前的登录用户
        public SysUser objLoginInfo;

        public int AppId;


        private string backRedirectUrl = string.Empty;

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

            objLoginInfo = new SysUser();

        }

        protected void OnBaseActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        public string GetFirstLoginUrl()
        {
            return this.backRedirectUrl;
        }

        /// <summary>
        /// 重新基类在Action执行之前的事情
        /// </summary>
        /// <param name="filterContext">重写方法的参数</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region Get AppId
            string strwechatid = Request["wechatid"];

            if (string.IsNullOrEmpty(strwechatid) && RouteData.Values.ContainsKey("appid"))
            {
                strwechatid = RouteData.Values["appid"].ToString();
            }
            else if (string.IsNullOrEmpty(strwechatid))
            {
                log.Error("wechatid not found!" + Request.Url);
                filterContext.Result = new ContentResult() { Content = "wechatid not found!" };
            }
            AppId = int.Parse(strwechatid);
            #endregion


            log.Debug("OnActionExecuting Not WxDetail:{0}", filterContext.RequestContext.HttpContext.Request.Url);

            backRedirectUrl = string.Empty;
            // 想办法获取用户的XXX id（或者用户id）
            ViewBag.WeChatUserID = "";
            string WeChatUserID = GetWeChatUserID(filterContext);
            log.Debug("GetWeChatUserID() : " + objLoginInfo.WeChatUserID);
            log.Debug("GetWeChatUserName() : " + objLoginInfo.UserName);
            ViewBag.WeChatUserID = objLoginInfo.WeChatUserID;
            ViewBag.WeChatUserName = objLoginInfo.UserName;

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 记录用户行为日志
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="type"></param>
        /// <param name="typecontent"></param>
        /// <param name="content"></param>
        public void ExecuteBehavior(int appid, int type, string typecontent, string content = null)
        {
            ////记录用户行为
            //WebRequestPost wr = new WebRequestPost();
            string url = Request.RawUrl.ToUpper();//带参数
            ////将id作为内容插入到数据库中

            string functionid = Request.FilePath.ToUpper();//不带参数（不带？后面的参数，但如果是/123则带）

            //if (url.Contains("?"))
            //{

            //}
            //else
            //{
            //    if (isNumberic1(content))
            //    {
            //        functionid = functionid.Substring(0, functionid.Length - functionid.Split('/')[functionid.Split('/').Length - 1].Length - 1);
            //    }

            //}

            //if (!string.IsNullOrEmpty(typecontent))
            //{
            //    wr.CallUserBehavior(functionid, ViewBag.WeChatUserID, appid.ToString(), typecontent, url, type);
            //}
            //else
            //{
            //    wr.CallUserBehavior(functionid, ViewBag.WeChatUserID, appid.ToString(), content, url, type);
            //}

            Task.Run(() =>
            {
                IUserBehaviorService _objService = new UserBehaviorService();
                _objService.Repository.Insert(new UserBehavior()
                {
                    UserId = ViewBag.WeChatUserID,
                    FunctionId = functionid,
                    AppId = appid,
                    Content = (string.IsNullOrEmpty(typecontent) ? content : typecontent),
                    Url = url,
                    ContentType = type,
                    Device = Request.UserAgent,
                    ClientIp = Request.UserHostAddress,
                    CreatedTime = DateTime.Now
                });
            });

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

        /// <summary>
        /// 获取已经登陆用户的用户信息
        /// </summary>
        /// <returns></returns>
        private string GetLoginUserInfo()
        {

            log.Debug("GetLoginUserInfo IsAuthenticated:" + HttpContext.Request.IsAuthenticated);

            if (HttpContext.Request.IsAuthenticated)
            {
                bool IsAuthenticated = true;
                backRedirectUrl = string.Empty;
                log.Debug("HttpContext.Request.IsAuthenticated");
                if (Request.UserAgent.IndexOf("MicroMessenger") >= 0)
                {
                    log.Debug("WeChat Browser");
                    var windowsIdentity = User.Identity as ClaimsIdentity;
                    if (windowsIdentity != null)
                    {
                        foreach (var a in windowsIdentity.Claims)
                        {
                            log.Debug("windowsIdentity：type:{0} value:{1} String:{2}", a.Type, a.Value, a.ToString());

                            if (a.Type == ClaimTypes.Name)
                            {
                                objLoginInfo.UserName = a.Value;
                            }
                            else if (a.Type == ClaimTypes.NameIdentifier)
                            {
                                int Id;
                                if (int.TryParse(a.Value, out Id))
                                {
                                    objLoginInfo.Id = Id;
                                }
                            }
                            else if (a.Type == "wechatuserid")
                            {
                                objLoginInfo.WeChatUserID = a.Value;
                            }
                            else if (a.Type == "wechatid") //解决不同的服务号和企业号共享cookie导致认证乱串的问题
                            {
                                var wechatidNew = int.Parse(a.Value);
                                if (wechatidNew != AppId) //不同的id，需要判断是否是同一个accountid
                                {
                                    var confignew = WeChatCommonService.GetWeChatConfigByID(wechatidNew);
                                    var config = WeChatCommonService.GetWeChatConfigByID(AppId);
                                    if (config.AccountManageId != confignew.AccountManageId) //不同的企业号，需要重新认证
                                    {
                                        IsAuthenticated = false;
                                        break;
                                    }

                                }

                                objLoginInfo.Apps = new List<int>() { wechatidNew };
                            }
                        }
                        log.Debug("windowsIdentity end!{0}", IsAuthenticated);
                        if (IsAuthenticated)
                        {
                            ViewBag.WeChatUserID = objLoginInfo.WeChatUserID;
                            ViewBag.WeChatUserName = objLoginInfo.UserName;

                            return windowsIdentity.Name;
                        }

                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            return string.Empty;
        }

        private string GetWeChatUserID(ActionExecutingContext filterContext)
        {
            //var objLoginInfo = Session["UserInfo"] as WechatUser;

            //LogManager.GetLogger(this.GetType()).Debug("objLoginInfo : " + (objLoginInfo == null?"NULL":objLoginInfo.WeChatUserID));
            ////判断用户是否为空
            //if (objLoginInfo == null)

            //LogManager.GetLogger(this.GetType()).Debug("objLoginInfo is null");

            //判断是否已经登陆
            var User = GetLoginUserInfo();
            if (!string.IsNullOrEmpty(User) && (objLoginInfo.Id > 0 || Request["_Callback"] == "1" || Request.Url.AbsoluteUri.Contains("_Callback=1")) /*防止死循环*/)
            {
                return User;
            }


            string strToUrl = Request.RawUrl.Replace(":5001", ""); //处理反向代理

            Session["ReturnUrl"] = strToUrl;// Request.Url.ToString();
            var weChatConfig = WeChatCommonService.GetWeChatConfigByID(AppId);
            string strUrl;

            string strRet = CommonService.GetSysConfig("UserBackUrl", "");
            string webUrl = CommonService.GetSysConfig("WeChatUrl", "");
            string strBackUrl = string.Format("{0}{1}?wechatid={2}", webUrl, strRet.Trim('/'), AppId);
            log.Debug("UrlStart :" + strBackUrl);




            //服务号
            if (weChatConfig.IsCorp.HasValue && !weChatConfig.IsCorp.Value)
            {
                strUrl = Innocellence.Weixin.MP.AdvancedAPIs.OAuthApi.GetAuthorizeUrl(weChatConfig.WeixinCorpId, strBackUrl, Guid.NewGuid().ToString(), Weixin.MP.OAuthScope.snsapi_base);
            }
            else //企业号
            {
                //string strRet = WebConfigurationManager.AppSettings["UserBackUrl"];
                //string webUrl = CommonService.GetSysConfig("WeChatUrl", "");

                // string strBackUrl = string.Format("{0}{1}?wechatid={2}", webUrl, strRet, AppId);

                //log.Debug("UrlStart :" + strBackUrl);
                //strUrl = OAuth2Api.GetCode(weChatConfig.WeixinCorpId, strBackUrl, Server.UrlEncode(strToUrl));
                strUrl = OAuth2Api.GetCode(weChatConfig.WeixinCorpId, strBackUrl, Guid.NewGuid().ToString());
            }

            log.Debug(strUrl);

            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                AjaxResult<int> result = new AjaxResult<int>();
                result.Message = new JsonMessage((int)HttpStatusCode.Unauthorized, strUrl);
                filterContext.Result = Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                log.Debug("filterContext.Result : " + strUrl);
                filterContext.Result = new RedirectResult(strUrl);
            }

            //if (null != filterContext.ActionDescriptor && "WxDetail".Equals(filterContext.ActionDescriptor.ActionName, StringComparison.OrdinalIgnoreCase))
            //{
            //    backRedirectUrl = strUrl;
            //    log.Debug("backRedirectUrl : " + backRedirectUrl);
            //}
            return string.Empty;


        }

        /// <summary>
        /// 验证可见范围
        /// </summary>
        /// <param name="news"></param>
        /// <param name="isCrop"></param>
        /// <returns></returns>
        public string ValidateNewsVisibleScope(ArticleInfo news, bool isCrop)
        {
            if (null != news)
            {
                //只验证发消息推送的图文
                //if (news.ArticleType == 1)
                //{
                return ValidateNewsIsVisibleToCurrentUser(news.SecurityLevel, news, isCrop);
                //}
                //return string.Empty;
            }
            return "/cropNoPermission.html";
        }

        /// <summary>
        /// 第一次执行时, 获取backRedirectUrl
        /// 第二次执行时, 返回WeChatUserID, 并进行可见范围判断
        /// </summary>
        /// <param name="news"></param>
        /// <param name="isCrop"></param>
        /// <param name="isSecond"></param>
        /// <returns></returns>
        public string ExecuteAuthorityFilterForNews(ArticleInfo news, bool isCrop, bool isSecond = false)
        {
            log.Debug("begin to validte news {0}.", news.SecurityLevel == null ? "null" : news.SecurityLevel.ToString());
            log.Debug("objLoginInfo.Id :{0}", objLoginInfo == null ? "null" : objLoginInfo.Id.ToString());
            //不是所有人可见，但是还没有关注的情况
            if (null != news && news.SecurityLevel != (int)SecurityLevel.AllPeople && objLoginInfo.Id == 0)
            {
                log.Debug("no permission");
                return "/noCropPermission.html";
            }
            else
            {
                log.Debug("begin to check :{0}", isCrop);
                return ValidateNewsVisibleScope(news, isCrop);
            }
        }

        private string ValidateNewsIsVisibleToCurrentUser(int? securityLevel, ArticleInfo news, bool isCrop)
        {
            try
            {
                log.Debug("begin to validate news visible {0}, {1}", news.ArticleTitle, news.SecurityLevel == null ? "null" : news.SecurityLevel.ToString());
                switch (securityLevel)
                {
                    //对不起，您不具备查看该信息的权限。
                    case (int)SecurityLevel.JustReceivedUser:
                        return ValidateJustReceivedUser(news.ToDepartment, news.ToTag, news.ToUser, isCrop) ? string.Empty : "/noNewsPermission.html";
                    //服务号没有该级别
                    case (int)SecurityLevel.AllUserInApp:
                        return ValidateUserInApp(news.AppId) ? string.Empty : "/noNewsPermission.html";
                    //对不起，您尚未关注企业号，目前尚不具备查看该信息的权限。请先长按扫描上面的二维码图片关注并认证。
                    case (int)SecurityLevel.AllUserInAccoumentManagement:
                        return !string.IsNullOrEmpty(ViewBag.WeChatUserID) ? string.Empty : "/noCropPermission.html";
                    case (int)SecurityLevel.AllPeople:
                    default:
                        return string.Empty;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return "/noCropPermission.html";
        }

        private bool ValidateJustReceivedUser(string toDepartment, string toTag, string toUser, bool isCrop)
        {
            bool isVisible = false;
            try
            {
                if (isCrop)
                {
                    #region 企业号
                    SysAddressBookMember user = GetCropUser();
                    if (null != user)
                    {
                        if (!string.IsNullOrWhiteSpace(toDepartment))
                        {
                            if (!string.IsNullOrWhiteSpace(user.Department))
                            {
                                var userDepartment = JsonConvert.DeserializeObject<List<int>>(user.Department);
                                var newsDepartment = toDepartment.Split(',').ToList();
                                isVisible = newsDepartment.Any(x => userDepartment.Contains(int.Parse(x)));
                                log.Debug("user is {0} in department", isVisible ? string.Empty : "not");
                            }
                        }
                        if (!isVisible && !string.IsNullOrWhiteSpace(toTag))
                        {
                            if (!string.IsNullOrWhiteSpace(user.TagList))
                            {
                                var userTag = JsonConvert.DeserializeObject<List<int>>(user.TagList);
                                var newsTag = toTag.Split(',').ToList();
                                isVisible = newsTag.Any(x => userTag.Contains(int.Parse(x)));
                                log.Debug("user is {0} in tag", isVisible ? string.Empty : "not");
                            }
                        }
                        if (!isVisible && !string.IsNullOrWhiteSpace(toUser))
                        {
                            if (!string.IsNullOrWhiteSpace(user.UserId))
                            {
                                var newsDepartment = toUser.Split(',').ToList();
                                isVisible = newsDepartment.Contains(user.UserId);
                                log.Debug("user is {0} in sended user", isVisible ? string.Empty : "not");
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 服务号
                    var mpUser = GetMPUser();
                    if (null != mpUser)
                    {
                        if (!isVisible && !string.IsNullOrWhiteSpace(toTag))
                        {
                            if (!string.IsNullOrWhiteSpace(mpUser.TagIdList))
                            {
                                var userTag = mpUser.TagIdList.Split(',').ToList();
                                var newsTag = toTag.Split(',').ToList();
                                isVisible = newsTag.Any(x => userTag.Contains(x));
                                log.Debug("mpUser is {0} in tag", isVisible ? string.Empty : "not");
                            }
                        }
                        if (!isVisible && !string.IsNullOrWhiteSpace(toUser))
                        {
                            if (!string.IsNullOrWhiteSpace(mpUser.OpenId))
                            {
                                var newsDepartment = toUser.Split(',').ToList();
                                isVisible = newsDepartment.Contains(mpUser.OpenId);
                                log.Debug("mpUser is {0} in sended user", isVisible ? string.Empty : "not");
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return isVisible;
        }

        /// <summary>
        /// 通过API获取APP的可见范围, 判断该User是否在其中
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        private bool ValidateUserInApp(int? appId)
        {
            if (null != appId)
            {
                SysAddressBookMember user = GetCropUser();
                if (null != user && null != user.AccountManageId)
                {
                    return WeChatCommonService.IsValidateUser((int)appId, user, (int)user.AccountManageId);
                }
            }
            return false;
        }

        private SysAddressBookMember GetCropUser()
        {
            string wechatUserId = ViewBag.WeChatUserID;
            log.Debug("begin to get {0}", string.IsNullOrEmpty(wechatUserId) ? "null" : wechatUserId);
            if (!string.IsNullOrWhiteSpace(wechatUserId))
            {
                var user = _addressBookService.GetMemberByUserId(wechatUserId);
                log.Debug("user is {0}", user == null ? "null" : user.UserName);
                return user;
            }
            return null;
        }

        private WechatMPUserView GetMPUser()
        {
            string wechatUserId = ViewBag.WeChatUserID;
            log.Debug("begin to get MP {0}", string.IsNullOrEmpty(wechatUserId) ? "null" : wechatUserId);
            if (!string.IsNullOrWhiteSpace(wechatUserId))
            {
                var user = _wechatMPUserService.GetUserByOpenId(wechatUserId);
                log.Debug("MPuser is {0}", user == null ? "null" : user.NickName);
                return user;
            }
            return null;
        }
    }
}
