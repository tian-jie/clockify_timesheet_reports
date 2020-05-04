using System;
using System.Linq;
using Infrastructure.Core.Plugins;
using Infrastructure.Core;
using Infrastructure.Core.Events;

namespace Innocellence.Lccp.Backend
{
    public partial class Starter : BasePlugin, IPlugin
    {


        public override void Initialize()
        {
            GlobalApplicationObject.Current.EventsManager.OnApplication_InitializeComplete += EventsManager_OnApplication_InitializeComplete;
           // GlobalConfigurationManager.ODataBuilder.EntitySet<SiteAdminNavigation>("SiteAdminNavigations");
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