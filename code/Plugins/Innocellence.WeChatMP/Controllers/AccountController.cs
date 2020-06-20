using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.AdvancedAPIs.OAuth;
using Innocellence.Weixin.MP.AdvancedAPIs.User;
using Innocellence.Weixin.MP.CommonAPIs;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

//using Innocellence.Web.ModelsView;

namespace Innocellence.WeChatMP.Controllers
{
    //[Authorize]
    public class AccountController : ParentController<SysUser, SysUserView>
    {
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
            int wechatid = int.MinValue;
            if (int.TryParse(Request["wechatid"], out wechatid))
            {
                var config = WeChatCommonService.GetWeChatConfigByID(wechatid);
                string returnUrl = Session["ReturnUrlMP"] as string;
                try
                {
                    LogManager.GetLogger(this.GetType()).Error("Entering BackLogin, returnUrl=" + returnUrl);
                    LogManager.GetLogger(this.GetType()).Debug("getting config: {0}", config.WeixinCorpId);
                    //ViewBag.ReturnUrl = returnUrl;
                    if (Request["code"] != null)
                    {
                        string code = Request["code"].ToString();
                        LogManager.GetLogger(this.GetType()).Debug("code:" + code);

                        var Token = OAuthApi.GetAccessToken(config.WeixinCorpId, config.WeixinCorpSecret, code);
                        LogManager.GetLogger(this.GetType()).Error("Token:" + Token.access_token);

                        //var baseToken = AccessTokenContainer.GetToken(config.WeixinCorpId, config.WeixinCorpSecret);
                        var userinfo = UserApi.Info(config.WeixinCorpId, config.WeixinCorpSecret, Token.openid);

                        ////////Session["Username"] = code1.UserId;
                        //////LogManager.GetLogger(this.GetType()).Debug("code1.UserId:" + code1.UserId);
                        //////BaseService<WechatUser> ser = new BaseService<WechatUser>();
                        //////var objUser = ser.Entities.Where(a => a.WechatID == code1.UserId).FirstOrDefault();

                        //////LogManager.GetLogger(this.GetType()).Debug("objUser:" + (objUser == null ? "N" : "U"));
                        //////if (objUser == null)
                        //////{
                        //////    objUser = new WechatUser() { UserID = code1.UserId, WechatID = code1.UserId, Id = 0, LanguageCode = ConstData.LAN_EN };
                        //////}

                        //var user = UserManager.Entities.FirstOrDefault(a => a.UserName == code1.UserId);
                        var user = new SysUser()
                        {
                            UserTrueName = userinfo.nickname,
                            UserName = userinfo.openid,
                            Id = 0
                        };
                        await _authenticationService.SignInNoDB(user, true);
                        return Redirect(returnUrl);

                        ////////登录日志
                        //////BaseService<Logs> objServLogs = new BaseService<Logs>();
                        //////objServLogs.Insert(new Logs() { LogCate = "WechatLogin", LogContent = "登录成功", CreatedUserID = objUser.UserID, CreatedUserName = objUser.UserID });

                    }

                    //LogManager.GetLogger(this.GetType()).Error("strUrl:" + strUrl);

                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger(this.GetType()).Error(ex,ex.Message);
                    return Redirect("/notauthed.html");
                }
            }
            //  View();
            return Redirect("/notauthed.html");
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