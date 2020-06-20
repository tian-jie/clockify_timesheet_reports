using System;
using System.Linq;
using Infrastructure.Core.Plugins;
using Infrastructure.Core;
using Infrastructure.Core.Events;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.Weixin.Cache;
using Infrastructure.Core.Data;
using Innocellence.Weixin.Entities;

namespace Innocellence.WeChatMain
{
    public partial class Starter : BasePlugin, IPlugin
    {


        public override void Initialize()
        {
            ModelMappers.MapperRegister();
            GlobalApplicationObject.Current.EventsManager.OnApplication_InitializeComplete += EventsManager_OnApplication_InitializeComplete;
           // GlobalConfigurationManager.ODataBuilder.EntitySet<SiteAdminNavigation>("SiteAdminNavigations");


            var Token = System.Configuration.ConfigurationManager.AppSettings["TokenStoreType"];  //Database Server Local

         if (Token == "Database")
         {
             //微信token 数据库缓存
             CacheStrategyFactory.RegisterContainerCacheStrategy(() =>
             {
                 var obj = DBContainerCacheStrategy.Instance;
                 ((DBContainerCacheStrategy)obj)._unitOfWork = new CodeFirstDbContext();
                 return obj;
             });
         }

         //var Ticket = System.Configuration.ConfigurationManager.AppSettings["TicketStoreType"];  //Database Server Local

         //if (Ticket == "Database")
         //{
         //    //微信token 数据库缓存
         //    CacheStrategyFactory<JsApiTicketBag>.RegisterContainerCacheStrategy(() =>
         //    {
         //        var obj = DBContainerCacheStrategy.Instance;
         //        ((DBContainerCacheStrategy)obj)._unitOfWork = new CodeFirstDbContext();
         //        return obj;
         //    });
         //}
           


        }
        void EventsManager_OnApplication_InitializeComplete(object sender, ApplicationArgs e)
        {
           // BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public override void Install()
        {
            //locales
           // this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRole", "Required customer role");
           // this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRole.Hint", "Discount will be applied if customer is in the selected customer role.");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
          //  this.DeletePluginLocaleResource("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRole");
          //  this.DeletePluginLocaleResource("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRole.Hint");
            base.Uninstall();
        }
    }
}