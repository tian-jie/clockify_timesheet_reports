using Infrastructure.Core;
using Infrastructure.Core.Events;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web;
using Infrastructure.Web.MVC;
using Microsoft.Owin;
using Owin;
using System;
using System.Web;

[assembly: OwinStartupAttribute(typeof(Innocellence.Web.Startup))]
[assembly: PreApplicationStartMethod(typeof(Innocellence.Web.Startup), "Start")]
namespace Innocellence.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

          //  app.MapSignalR();
            var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IOWinAuthorizeProvider>();
            foreach (var startUpTaskType in startUpTaskTypes)
                ((IOWinAuthorizeProvider)Activator.CreateInstance(startUpTaskType)).ConfigureAuth(app);

        }

        /// <summary>
        /// 初始化插件
        /// </summary>
        public static void Start()
        {
            //设置全局Context对象
            GlobalApplicationObject.Current.ApplicationContext = new ApplicationContext();

            GlobalApplicationObject.Current.ApplicationContext.PreApplicationStartInitialize();
            //执行预初始化事件
            GlobalApplicationObject.Current.EventsManager.InitApplicationEvents(null, ApplicationEvents.OnApplication_PreInitialize);
        }
    }
}
