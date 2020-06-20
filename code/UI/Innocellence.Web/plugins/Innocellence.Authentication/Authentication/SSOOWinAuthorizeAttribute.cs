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
    public class SSOOWinAuthorizeAttribute : OWinAuthorizeAttribute
    {

         public SSOOWinAuthorizeAttribute(bool dontValidate)
            : base(dontValidate)
        {

        }

         public SSOOWinAuthorizeAttribute(string provider)
             : this(false)
         {
             _provider = provider;
         }
         public SSOOWinAuthorizeAttribute(string provider, bool dontValidate)
             : this(dontValidate)
         {
             _provider = provider;
         }
         protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
       {
           if (_bolAutoBack)
           {
               var url = filterContext.RequestContext.HttpContext.Request.Url;
               var strUrl = string.Format("{0}://{1}/sso/ssoresult?RelayState=", url.Scheme,url.Host);
               filterContext.Result = new ChallengeResult(_provider, strUrl + filterContext.RequestContext.HttpContext.Request.Url.ToString().Replace(":" + url.Port, ""));
           }
           else
           {
               filterContext.Result = new ChallengeResult(_provider, _redirectUri);
           }

       }
      
    }

  
}
