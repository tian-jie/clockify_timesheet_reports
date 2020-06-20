using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.AdvancedAPIs.OAuth2;
using Innocellence.Weixin.QY.CommonAPIs;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

//using Innocellence.Web.ModelsView;

namespace Innocellence.WeChatMeeting.Controllers
{
    //[Authorize]
    public class AccountController : ParentController<SysUser, SysUserView>
    {
        //public static readonly string Token = WebConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        //public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        ////public static readonly string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
        //public static readonly string CorpId = WebConfigurationManager.AppSettings["WeixinCorpId"];//与微信企业账号后台的CorpId设置保持一致，区分大小写。
        //public static readonly string CorpSecret = WebConfigurationManager.AppSettings["WeixinCorpSecret"];
        //public static readonly string SSOUrl = WebConfigurationManager.AppSettings["SSOUrl"];

        IAuthenticationService _authenticationService;

        public AccountController(ISysUserService userManager, IAuthenticationService authenticationService)
            : base(userManager)
        {
            UserManager = userManager;
            _authenticationService = authenticationService;
        }

        public ISysUserService UserManager { get; private set; }



        [AllowAnonymous]
        public async Task<ActionResult> BackLogin()
        {
            string returnUrl = Session["ReturnUrl"] as string;
            try
            {
                LogManager.GetLogger(this.GetType()).Error("Entering BackLogin, returnUrl=" + returnUrl);
                //ViewBag.ReturnUrl = returnUrl;
                if (Request["code"] != null)
                {
                    string code = Request["code"].ToString();
                    string wechatid = Request["wechatid"].ToString();

                    LogManager.GetLogger(this.GetType()).Debug("code:" + code + " wechatid:" + wechatid);



                    var weChatConfig = WeChatCommonService.GetWeChatConfigByID(int.Parse(wechatid));

               var Token = AccessTokenContainer.TryGetToken(weChatConfig.WeixinCorpId, weChatConfig.WeixinCorpSecret);
                    LogManager.GetLogger(this.GetType()).Error("Token:" + Token);
                    var code1 = OAuth2Api.GetUserId(Token, code);

                    ////////Session["Username"] = code1.UserId;
                    //////LogManager.GetLogger(this.GetType()).Debug("code1.UserId:" + code1.UserId);
                    //////BaseService<WechatUser> ser = new BaseService<WechatUser>();
                    //////var objUser = ser.Entities.Where(a => a.WechatID == code1.UserId).FirstOrDefault();

                    //////LogManager.GetLogger(this.GetType()).Debug("objUser:" + (objUser == null ? "N" : "U"));
                    //////if (objUser == null)
                    //////{
                    //////    objUser = new WechatUser() { WeChatUserID = code1.UserId, WechatID = code1.UserId, Id = 0, LanguageCode = ConstData.LAN_EN };
                    //////}

                    //var user = UserManager.Entities.FirstOrDefault(a => a.UserName == code1.UserId);
                    var user = new SysUser()
                    {
                         UserName = code1.UserId,
                         Id=0
                    };
                    await _authenticationService.SignInNoDB(user, true);
                    return Redirect(returnUrl);

                    ////////登录日志
                    //////BaseService<Logs> objServLogs = new BaseService<Logs>();
                    //////objServLogs.Insert(new Logs() { LogCate = "WechatLogin", LogContent = "登录成功", CreatedUserID = objUser.WeChatUserID, CreatedUserName = objUser.WeChatUserID });

                }

                //LogManager.GetLogger(this.GetType()).Error("strUrl:" + strUrl);

                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(this.GetType()).Error(ex.Message, ex);
                return Redirect("/notauthed.html");
            }

            //  View();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

    }
}