using Infrastructure.Core.Data;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
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

namespace Innocellence.WeChat.Controllers
{

    //[Authorize]
    public class AccountController : ParentController<SysUser, SysUserView>
    {
        private static readonly Object StaticLockObj = new object();
        private static readonly Object StaticLockObjMP = new object();

        IAuthenticationService _authenticationService;

        ILogger log = LogManager.GetLogger("AccountController");

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
                log.Error("Entering BackLogin, returnUrl=" + returnUrl);
                //ViewBag.ReturnUrl = returnUrl;
                if (Request["code"] != null)
                {
                    string code = Request["code"].ToString();
                    string wechatid = Request["wechatid"].ToString();

                    log.Debug("code:" + code + " wechatid:" + wechatid);

                    SysUser user = null;

                    var weChatConfig = WeChatCommonService.GetWeChatConfigByID(int.Parse(wechatid));

                    if (weChatConfig.IsCorp.HasValue && !weChatConfig.IsCorp.Value)
                    {


                        var code1 = Innocellence.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(weChatConfig.WeixinCorpId, weChatConfig.WeixinCorpSecret, code);

                        IWechatMPUserService ser = EngineContext.Current.Resolve<IWechatMPUserService>();

                        WechatMPUser objUser = null;

                        objUser = ser.Repository.Entities.Where(a => a.OpenId == code1.openid).FirstOrDefault();

                        log.Debug("MP objUser:" + (objUser == null ? "N" : "U") + " OpenID:" + code1.openid);

                        //如果用户不存在就添加用户

                        if (objUser == null)
                        {
                            // var userinfo = Innocellence.Weixin.MP.AdvancedAPIs.UserApi.Info(weChatConfig.WeixinCorpId, weChatConfig.WeixinCorpSecret, code1.openid);
                            // objUser = WechatMPUserView.ConvertWeChatUserToMpUser(userinfo, weChatConfig.AccountManageId.Value, weChatConfig.Id);
                            // ser.Repository.Insert(objUser);
                            lock (StaticLockObj)//防止插入多条数据
                            {
                                objUser = ser.Repository.Entities.Where(a => a.OpenId == code1.openid).FirstOrDefault();  //为什么要重新查一次？？因为有lock
                                if (objUser == null)
                                {
                                    WeChatCommonService.SycUserFromWeixinMP(code1.openid, ref objUser, weChatConfig);
                                }
                            }
                        }


                        if (objUser == null || objUser.IsCanceled == true || objUser.SubScribe == 0) //未关注
                        {

                            log.Debug("未关注 OpenID:{0}", code1.openid);
                            user = new SysUser()
                            {
                                UserName = code1.openid,
                                WeChatUserID = "",
                                Id = 0,
                                Apps = new System.Collections.Generic.List<int>() { int.Parse(wechatid) }
                            };

                        }
                        else //已关注
                        {
                            user = new SysUser()
                            {
                                UserName = objUser.NickName,
                                WeChatUserID = objUser.OpenId,
                                Id = objUser.Id,
                                Apps = new System.Collections.Generic.List<int>() { int.Parse(wechatid) }
                            };
                        }


                        //var user = UserManager.Entities.FirstOrDefault(a => a.UserName == code1.UserId);


                    }
                    else
                    {
                        var Token = AccessTokenContainer.TryGetToken(weChatConfig.WeixinCorpId, weChatConfig.WeixinCorpSecret);
                        log.Error("Token:" + Token);
                        var code1 = OAuth2Api.GetUserId(Token, code);

                        // BaseService<SysAddressBookMember> ser = new BaseService<SysAddressBookMember>();
                        IAddressBookService ser = EngineContext.Current.Resolve<IAddressBookService>();

                        if (string.IsNullOrEmpty(code1.UserId)) //没关注
                        {
                            user = new SysUser()
                            {
                                UserName = code1.OpenId,
                                WeChatUserID = "",
                                Id = 0,
                                Apps = new System.Collections.Generic.List<int>() { int.Parse(wechatid) }
                            };

                        }
                        else //已经关注
                        {

                            var objUser = ser.Repository.Entities.Where(a => a.UserId == code1.UserId && a.DeleteFlag != 1).FirstOrDefault();

                            log.Debug("objUser:{1} UserID:{0} Status:{2}", code1.UserId, (objUser == null ? "N" : "U"), objUser == null ? "" : objUser.EmployeeStatus);

                            if (objUser != null && (objUser.EmployeeStatus == "D" || objUser.EmployeeStatus == "U")) //离职或状态不明
                            {
                                user = new SysUser()
                                {
                                    UserName = objUser.UserId,
                                    WeChatUserID = "",
                                    Id = 0,
                                    Apps = new System.Collections.Generic.List<int>() { int.Parse(wechatid) }
                                };

                            }
                            else  //已经关注
                            {
                                //objUser = new SysAddressBookMember() { UserId = code1.UserId, Id = 0 };
                                if (objUser == null)
                                {
                                    lock (StaticLockObjMP) //防止插入多条数据
                                    {
                                        if (objUser == null)  //lock后，重新查一次
                                        {
                                            objUser = ser.Repository.Entities.Where(a => a.UserId == code1.UserId).FirstOrDefault();
                                        }


                                        WeChatCommonService.SyncUserFromWechat(code1.UserId, ref objUser, weChatConfig);
                                    }
                                }
                                user = new SysUser()
                                {
                                    UserName = objUser.UserName,
                                    WeChatUserID = objUser.UserId,
                                    Id = objUser.Id,
                                    Apps = new System.Collections.Generic.List<int>() { int.Parse(wechatid) }
                                };
                            }
                        }





                        //var user = UserManager.Entities.FirstOrDefault(a => a.UserName == code1.UserId);


                    }

                    log.Debug("SignInNoDB UserID:{0} User:{1}", user.Id, user.UserName);


                    await _authenticationService.SignInNoDB(user, true);
                    //return Redirect(returnUrl);

                    ////////登录日志
                    //////BaseService<Logs> objServLogs = new BaseService<Logs>();
                    //////objServLogs.Insert(new Logs() { LogCate = "WechatLogin", LogContent = "登录成功", CreatedUserID = objUser.WeChatUserID, CreatedUserName = objUser.WeChatUserID });

                }

                //LogManager.GetLogger(this.GetType()).Error("strUrl:" + strUrl);

                var Ret = returnUrl + (returnUrl.IndexOf("?") > 0 ? "&_Callback=1" : "?_Callback=1");
                log.Debug("Ret URL:{0}", Ret);

                return Redirect(Ret);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(this.GetType()).Error(ex, ex.Message);
                return Redirect("/noCropPermission.html");
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