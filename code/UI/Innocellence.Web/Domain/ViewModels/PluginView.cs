using Infrastructure.Core;
using System.Collections.Generic;
using System.Web.Mvc;
//using FluentValidation.Attributes;
//using Nop.Admin.Models.Stores;
//using Nop.Admin.Validators.Plugins;
//using Nop.Web.Framework;
//using Nop.Web.Framework.Localization;
//using Nop.Web.Framework.Mvc;

namespace Innocellence.Web.Models.Plugins
{
    //[Validator(typeof(PluginValidator))]
    public partial class PluginView : IViewModel //: BaseNopModel, ILocalizedModel<PluginLocalizedModel>
    {
        public PluginView()
        {
           // Locales = new List<PluginLocalizedModel>();
        }

        public int Id { get; set; }
       // [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Group")]
        [AllowHtml]
        public string Group { get; set; }

       // [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        //[NopResourceDisplayName("Admin.Configuration.Plugins.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

       // [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Version")]
        [AllowHtml]
        public string Version { get; set; }

       // [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Author")]
        [AllowHtml]
        public string Author { get; set; }

      //  [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

      //  [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Configure")]
        public string ConfigurationUrl { get; set; }

      //  [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Installed")]
        public bool Installed { get; set; }

        public bool CanChangeEnabled { get; set; }
      //  [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.IsEnabled")]
        public bool IsEnabled { get; set; }

     //   [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Logo")]
        public string LogoUrl { get; set; }

        public IList<PluginLocalizedModel> Locales { get; set; }


        //Store mapping
       // [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.LimitedToStores")]
     //   public bool LimitedToStores { get; set; }
      //  [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.AvailableStores")]
     //   public List<StoreModel> AvailableStores { get; set; }
     //   public int[] SelectedStoreIds { get; set; }


        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (PluginModel)obj;
           // Id = entity.Id;
            Group = entity.Group;
            FriendlyName = entity.FriendlyName;
            SystemName = entity.SystemName;
            Version = entity.Version;
            Author = entity.Author;
            DisplayOrder = entity.DisplayOrder;
            ConfigurationUrl = entity.ConfigurationUrl;
            Installed = entity.Installed;
            CanChangeEnabled = entity.CanChangeEnabled;
            IsEnabled = entity.IsEnabled;
            LogoUrl = entity.LogoUrl;

            return this;
        }
    }
  
}