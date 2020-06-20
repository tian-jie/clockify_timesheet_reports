using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web.Identity;
using Microsoft.AspNet.Identity;
using Infrastructure.Web.UI;
using System.Net;
using Infrastructure.Web.MVC.Attribute;
using Microsoft.Owin.Security;
using System.Web;


namespace Innocellence.Authentication.Attribute
{
   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class OWinAuthorizeAttribute : CustomAuthorizeAttribute
    {
       protected string _provider;
        protected string _redirectUri = "~/Account/ExternalLoginCallback";
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "Innocellence";

        protected bool _bolAutoBack=true;

        public OWinAuthorizeAttribute()
            : this(false)
        {
        }

        /// <summary>
        /// dontValidate:true 不验证权限，只判断是否登录
        /// </summary>
        /// <param name="dontValidate"></param>
        public OWinAuthorizeAttribute(bool dontValidate)
            : base(dontValidate)
        {

        }

        public OWinAuthorizeAttribute(string provider)
            : this(false)
        {
            _provider = provider;
        }

        public OWinAuthorizeAttribute(string provider, string redirectUri)
            : this(false)
        {
            _provider = provider;
            _redirectUri = redirectUri;
        }

        public OWinAuthorizeAttribute(string provider, string redirectUri,bool bolAutoBack)
            : this(false)
        {
            _provider = provider;
            _redirectUri = redirectUri;
            _bolAutoBack = bolAutoBack;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (_bolAutoBack)
            {
                var url = _redirectUri + (_redirectUri.IndexOf('?') > 0 ? "&redirectUri=" : "?redirectUri=");
                filterContext.Result = new ChallengeResult(_provider, url + filterContext.RequestContext.HttpContext.Request.Url.ToString());
            }
            else
            {
                filterContext.Result = new ChallengeResult(_provider, _redirectUri);
            }
            
        }


        protected class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

      
    }

  
}
