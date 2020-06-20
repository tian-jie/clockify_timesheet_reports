using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Reflection;
using Autofac;
using Autofac.Integration.Mvc;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Core.Logging;
using Infrastructure.Web.Logging;
using System.IO;

using System.Threading;
using System.Web.Configuration;

using Infrastructure.Web.MVC.Handlers;
using System.Security.Principal;
using Infrastructure.Core.Infrastructure;
//using Infrastructure.Core.Mvc;
using Infrastructure.Web.Mvc.Routing;
using Infrastructure.Web.Mvc;
using Innocellence.Web.Extensions;
using Innocellence.WeChatMain.Controllers;
using Infrastructure.Web.Tasks;
//using Innocellence.Weixin.Entity;



namespace Innocellence.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //register custom routes (plugins, etc)
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);

         var route=   routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "Innocellence.Web.Controllers" }
            );
         route.DataTokens["area"] = "admin";

        }

        protected void Application_BeginRequest()
        {

        }

        protected void Application_Start()
        {
            // TODO: 防止出现sysUser not in context的错误
          //  new Infrastructure.Web.Domain.Entity.SysUser();

            //initialize engine context
           // EngineContext.Initialize(false);

            bool databaseInstalled = true;// DataSettingsHelper.DatabaseIsInstalled();
            if (databaseInstalled)
            {
                //remove all view engines
                ViewEngines.Engines.Clear();
                //except the themeable razor view engine we use
                // ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
                ViewEngines.Engines.Add(new PluginRazorViewEngine());

            }

            //Add some functionality on top of the default ModelMetadataProvider
            // ModelMetadataProviders.Current = new NopMetadataProvider();

            //Registering some regular mvc stuff
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            MappingExtensions.MapperRegister();
            Infrastructure.Web.Domain.Entity.ModelMap.MapperRegister();
            //fluent validation
            //  DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            //  ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new NopValidatorFactory()));

            //start scheduled tasks
            if (databaseInstalled)
            {
                TaskManager.Instance.Initialize();
                TaskManager.Instance.Start();
            }

            //miniprofiler
            //if (databaseInstalled)
            //{
            //    if (EngineContext.Current.Resolve<StoreInformationSettings>().DisplayMiniProfilerInPublicStore)
            //    {
            //        GlobalFilters.Filters.Add(new ProfilingActionFilter());
            //    }
            //}

            //log application start
            //if (databaseInstalled)
            //{
            //    try
            //    {
            //        //log
            //        var logger = EngineContext.Current.Resolve<ILogger>();
            //        logger.Info("Application started", null, null);
            //    }
            //    catch (Exception)
            //    {
            //        //don't throw new exception if occurs
            //    }
            //}



        }



        protected void Application_End()
        {
            //LogManager.GetLogger(this.GetType()).Warn("Application_End");
            //string url = "http://localhost:8088";

            //System.Net.HttpWebRequest myHttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

            //System.Net.HttpWebResponse myHttpWebResponse = (System.Net.HttpWebResponse)myHttpWebRequest.GetResponse();

            //System.IO.Stream receiveStream = myHttpWebResponse.GetResponseStream();//
            //LogManager.GetLogger(this.GetType()).Warn("Application_End1");

         //   TaskManager.Instance.Start();

        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception lastError = Server.GetLastError();
           // Response.TrySkipIisCustomErrors = true;
            if (lastError != null)
            {
                while (lastError != null)
                {
                    var log = LogManager.GetLogger("Application_Error");
                    log.Error(lastError);
                    log.Error(Request.Url);
                    lastError = lastError.InnerException;
                }

                lastError = Server.GetLastError();

                if (lastError is HttpException)
                {
                    if (((HttpException)lastError).GetHttpCode() == 404)
                    {
                        Response.StatusCode = 404;
                        Server.ClearError();
                        return;
                    }
                }
              //  CNBlogs.Infrastructure.Logging.Logger.Default.Error("Application_Error", lastError);
                Response.StatusCode = 500;
                Server.ClearError();
            }
        }


        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    var ex = Server.GetLastError();
        //    //Log.Error(ex);  //记录日志信息   
        //    var httpStatusCode = (ex is HttpException) ? (ex as HttpException).GetHttpCode() : 500;  //这里仅仅区分两种错误   
        //    var httpContext = ((MvcApplication)sender).Context;
        //    httpContext.ClearError();
        //    httpContext.Response.Clear();
        //    httpContext.Response.StatusCode = httpStatusCode;
        //    httpContext.Response.TrySkipIisCustomErrors = true;
        //    var shouldHandleException = true;
        //    HandleErrorInfo errorModel;

        //    var routeData = new RouteData();
        //    routeData.Values["controller"] = "Error";

        //    switch (httpStatusCode)
        //    {
        //        case 404:
        //            routeData.Values["action"] = "NotFound";
        //            errorModel = new HandleErrorInfo(new Exception(string.Format("No page Found", httpContext.Request.UrlReferrer), ex), "Error", "NotFound");
        //            break;

        //        default:
        //            routeData.Values["action"] = "HttpError";
                    
        //            shouldHandleException = true;// ExceptionPolicy.HandleException(ex,  "LogAndReplace" ,  out  exceptionToReplace);
        //            errorModel = new HandleErrorInfo(ex, "Error", "NotFound");
        //            break;
        //    }

        //    if (shouldHandleException)
        //    {
        //        var controller = new ErrorController();
        //        controller.ViewData.Model = errorModel;  //通过代码路由到指定的路径   
        //        ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        //    }

        //}


    }


}
