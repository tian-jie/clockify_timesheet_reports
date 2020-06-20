using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;

namespace Innocellence.Authentication.Controllers
{
    public class OWinLoginController : Controller
    {
    
              
        ISysUserService _userManager;
        private IAuthenticationService _authService;
        ISysUserService _sysUserService;
        ILogger log;

        public OWinLoginController(ISysUserService userManager, IAuthenticationService authService,ISysUserService sysUserService)
        {
            _userManager = userManager;
            _authService = authService;
            _sysUserService = sysUserService;
            log = LogManager.GetLogger(typeof(OWinLoginController));
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> SsoResult(string samlResponse, string RelayState)
        {
            LogManager.GetLogger(this.GetType()).Debug("Entering SsoResult... RelayState=" + RelayState);

            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            var user = _sysUserService.AutoLogin(loginInfo.Login.ProviderKey);

            if (user == null)
            {
                string strMsg = string.Format("login OK,but user [{0}] not found!", loginInfo.Login.ProviderKey);
                log.Error(strMsg);
                return View("Error", new HandleErrorInfo(new Exception(strMsg), "OWinLogin", "SsoResult"));
            }
            

           await _authService.SignInAsync(user,true);



            //// Sign in the user with this external login provider if the user already has a login
            //var user = await UserManager.UserContext.FindAsync(loginInfo.Login);
            //if (user != null)
            //{
            //    await SignInAsync(user, isPersistent: false);
            //    return RedirectToLocal(returnUrl);
            //}
            //else
            //{
            //    // If the user does not have an account, then prompt the user to create an account
            //    ViewBag.ReturnUrl = returnUrl;
            //    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            //    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            //}
            // 转回原网站
           if (string.IsNullOrEmpty(RelayState))
           {
               return Redirect("/");
           }
           else
           {
              return Redirect(RelayState);
           }
           
        }


     

    }
}
