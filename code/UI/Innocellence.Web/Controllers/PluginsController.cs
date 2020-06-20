using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using System.Web.Mvc;
using Infrastructure.Core.Plugins;
using Infrastructure.Core;
using Innocellence.Web.Models.Plugins;
using Innocellence.Web.Service;
using Innocellence.Web.Extensions;
using System.Web;
using Infrastructure.Utility;
using Infrastructure.Web.Domain.Model;
using System.IO;
using Infrastructure.Core.Logging;

namespace Innocellence.Web.Controllers
{
    public class PluginsController : BaseController<PluginModel, PluginView>
    {

        private readonly IWebHelper _webHelper;
        private readonly IPluginFinder _pluginFinder;
        private ILogger Logger;

        public PluginsController(IPluginService newsService, IWebHelper webHelper, IPluginFinder pluginFinder)
            : base(newsService)
        {
            // _newsService = newsService;
            this._pluginFinder = pluginFinder;
            this._webHelper = webHelper;
            Logger = LogManager.GetLogger("Main");
        }


        public override ActionResult Index()
        {

            ViewBag.CateType = Request["CateType"];
            return base.Index();
        }



        public override ActionResult Get(string id)
        {

            var plugTemp = ((List<PluginDescriptor>)PluginManager.AllPlugins)
                                       .Find(a => a.SystemName == id);
            if (plugTemp == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    PluginPath = plugTemp.PluginPath,
                    FriendlyName = plugTemp.FriendlyName,
                    SystemName = plugTemp.SystemName,
                    Version = plugTemp.Version,
                    Installed = plugTemp.IsEnabled,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 上传plugin zip文件
        /// </summary>
        /// <returns></returns>
        public ActionResult PostFile(string systemName)
        {
            string strErrorMsg = "";
            string strName = "";
            try
            {
                string strPath = "";
                if (Request.Files.Count > 0)
                {

                    if (!System.IO.Directory.Exists(string.Format("{0}\\temp", Request.PhysicalApplicationPath)))
                    {
                        System.IO.Directory.CreateDirectory(string.Format("{0}\\temp", Request.PhysicalApplicationPath));
                    }

                    if (!System.IO.Directory.Exists(string.Format("{0}\\BackUp", Request.PhysicalApplicationPath)))
                    {
                        System.IO.Directory.CreateDirectory(string.Format("{0}\\BackUp", Request.PhysicalApplicationPath));
                    }

                    HttpPostedFileBase objFile = Request.Files[0];
                    var pad = DateTime.Now.ToString("yyyyMMddHHmmss");
                    strPath = string.Format("{0}\\temp\\{1}{2}", Request.PhysicalApplicationPath,
                       pad, System.IO.Path.GetExtension(objFile.FileName));
                    //  dic.Add(objFile.FileName, objFile.InputStream);
                    objFile.SaveAs(strPath);


                    //System.IO.MemoryStream stream = new System.IO.MemoryStream(objZip.ZipContent);
                    SharpZip.UnpackFiles(strPath, string.Format("{0}temp\\{1}\\", Request.PhysicalApplicationPath, pad));

                    var strPathTemp = strPath.Replace(System.IO.Path.GetExtension(objFile.FileName), "\\");

                    var bolCheck = CheckModule(objFile.FileName, strPathTemp);
                    PluginDescriptor plugTemp = null;
                    if (bolCheck)
                    {
                        plugTemp = PluginFileParser.ParsePluginDescriptionFile(strPathTemp + "\\Description.txt");
                        if (!string.IsNullOrEmpty(systemName) && systemName != "0" && plugTemp.SystemName != systemName)
                        {
                            throw new Exception("System Name不对，插件不兼容！");
                        }
                        if (!string.IsNullOrEmpty(systemName) && systemName != "0")  //编辑时先删除以前的
                        {
                            var plugTemp1 = ((List<PluginDescriptor>)PluginManager.AllPlugins)
                                       .Find(a => a.SystemName == plugTemp.SystemName);

                            // 如果前后名字对不上，不能装啊
                            if (plugTemp1 == null)
                            {
                                // 对不上了，看systemName，如果空的话就是添加，否则不让操作
                                if (!string.IsNullOrEmpty(systemName))
                                {
                                    throw new Exception("System Name不对，插件不兼容！");
                                }
                            }
                        }
                        else
                        {
                            //是否存在相同的
                            // plugTemp = PluginManager.GetPluginDescriptor(new DirectoryInfo(System.IO.Path.GetDirectoryName(strPath)));
                            if (((List<PluginDescriptor>)PluginManager.AllPlugins)
                                .Exists(a => a.PluginFileName == plugTemp.PluginFileName))
                            {
                                strErrorMsg = "the Module has existed!";
                                bolCheck = false;
                            }
                            else
                            {

                            }
                        }

                        var pluginsVersions = plugTemp.Version.Split('.');
                        if (pluginsVersions.Length < 4)
                        {
                            strErrorMsg = "the Module's Version format is error! Correct Format is '40.1.20160911.01'";
                            bolCheck = false;
                        }


                    }
                    else
                    {
                        System.IO.Directory.Delete(strPathTemp, true);
                        strErrorMsg = "Zip File is not a Module!";
                    }


                    if (bolCheck)
                    {

                        string strPathTo = string.Format("{0}Plugins\\{1}", Request.PhysicalApplicationPath, plugTemp.SystemName);

                        var plug = plugTemp;
                        if (!Directory.Exists(strPathTo))
                        {
                            Directory.CreateDirectory(strPathTo);
                            System.IO.File.Copy(strPathTemp + "\\Description.txt", strPathTo + "\\Description.txt");

                        }
                        plug.InstallFrom = pad;
                        plug.NeedInstalled = false;
                        plug.Installed = false;
                        plug.PluginPath = strPathTo;

                        PluginFileParser.SavePluginDescriptionFile(plug);
                        PluginManager.SavePlugins(plug);
                    }
                }

                strName = System.IO.Path.GetFileName(strPath);

            }
            catch (Exception ex)
            {
                strErrorMsg = "Server Error:" + ex.Message;
            }

            if (strErrorMsg != "")
            {
                return Json(new UploadMessageError("1", strErrorMsg, Request["id"]), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new UploadMessageSuccess(new UploadMessageSuccessMsg(strName, "/temp/" + strName), Request["id"]), JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ZipFile(string systemName)
        {
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                //No plugin found with the specified id
                return ErrorNotification("plugin not found!");

            var strPath = string.Format("{0}\\Plugins\\{1}", Request.PhysicalApplicationPath, pluginDescriptor.SystemName);

            if (!Directory.Exists(Request.PhysicalApplicationPath + "Temp"))
            {
                Directory.CreateDirectory(Request.PhysicalApplicationPath + "Temp");
            }

            var strPathTemp = string.Format("{0}\\Temp\\{1}.zip", Request.PhysicalApplicationPath, Guid.NewGuid());
            // ZipOutputStream ms = new ZipOutputStream(a);
            SharpZip.PackFiles(strPathTemp, strPath, @"-\.cs$;-\.csproj$;-\.user$;-\\obj\\;-\\Properties\\;");



            // ms.Position = 0;
            // ms.Flush();
            return File(strPathTemp, "application/x-zip-compressed", pluginDescriptor.SystemName + ".zip");

        }


        private bool CheckModule(string strFileName, string strBasePath)
        {
            string strModuleName = strFileName.Replace(System.IO.Path.GetExtension(strFileName), "");
            string strPath = strBasePath;// +strModuleName;
            if (!System.IO.Directory.Exists(strPath))
            {
                return false;
            }


            if (!System.IO.File.Exists(strPath + "\\Description.txt"))
            {
                return false;
            }

            return true;

        }


        //初始化list页面
        public override List<PluginView> GetListEx(Expression<Func<PluginModel, bool>> predicate, PageCondition ConPage)
        {
            string strModeId = Request["SearchLoadModeId"];
            string strGroup = Request["SearchGroup"];

            LoadPluginsMode loadMode = LoadPluginsMode.All;

            if (!string.IsNullOrEmpty(strModeId))
            {
                loadMode = (LoadPluginsMode)int.Parse(strModeId);
            }


            // ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

            // var q = _objService.GetList<PluginView>(predicate, ConPage);

            var pluginDescriptors = _pluginFinder.GetPluginDescriptors(loadMode, 0, strGroup).ToList();
            //var pluginDescriptors = PluginManager.AllPlugins;
            ConPage.RowCount = pluginDescriptors.Count();


            return pluginDescriptors.Select(x => PreparePluginModel(x, false)).Select(x => (PluginView)new PluginView().ConvertAPIModel(x)).ToList();
        }



        //public ActionResult List()
        //{
        //    //if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
        //    //    return AccessDeniedView();

        //    var model = new PluginListModel();
        //    //load modes
        //    model.AvailableLoadModes = LoadPluginsMode.All.ToSelectList(false).ToList();
        //    //groups
        //    model.AvailableGroups.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "" });
        //    foreach (var g in _pluginFinder.GetPluginGroups())
        //        model.AvailableGroups.Add(new SelectListItem { Text = g, Value = g });
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult ListSelect(DataSourceRequest command, PluginListModel model)
        //{
        //    //if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
        //    //    return AccessDeniedView();

        //    var loadMode = (LoadPluginsMode) Request["SearchLoadModeId"];
        //    var pluginDescriptors = _pluginFinder.GetPluginDescriptors(loadMode, 0, model.SearchGroup).ToList();
        //    var gridModel = new DataSourceResult
        //    {
        //        Data = pluginDescriptors.Select(x => PreparePluginModel(x, false, false))
        //        .OrderBy(x => x.Group)
        //        .ToList(),
        //        Total = pluginDescriptors.Count()
        //    };
        //    return Json(gridModel);
        //}


        public override JsonResult Post(PluginView objModal, string Id)
        {

            //验证错误
            //if (!BeforeAddOrUpdate(objModal, Id) )
            //{
            //    return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            //}

            _webHelper.RestartAppDomain();
            // Install(objModal.SystemName);

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Install(string systemName)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
            //    return AccessDeniedView();


            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                //No plugin found with the specified id
                return ErrorNotification("plugin not found!"); ;

            //check whether plugin is not installed
            if (pluginDescriptor.IsEnabled)
                return ErrorNotification("plugin is Installed!");


            PluginManager.MarkPluginInstallEvent(pluginDescriptor);

            //install plugin
            // pluginDescriptor.Instance().Install();
            // SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.Installed"));

            //pluginDescriptor.Installed = true;
            //restart application
            _webHelper.RestartAppDomain();


            // return RedirectToAction("List");
            return SuccessNotification("Installed");
        }
        public ActionResult Uninstall(string systemName)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
            //    return AccessDeniedView();


            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                //No plugin found with the specified id
                return ErrorNotification("plugin not found!");

            //check whether plugin is installed
            if (!pluginDescriptor.IsEnabled)
                return ErrorNotification("plugin is not Installed!");

            //uninstall plugin
            pluginDescriptor.Instance().Uninstall();

            //  SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.Uninstalled"));

            //restart application
            _webHelper.RestartAppDomain();


            return SuccessNotification("Installed");
        }

        public ActionResult ReloadList()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
            //    return AccessDeniedView();

            //restart application
            _webHelper.RestartAppDomain();
            return SuccessNotification("ReloadList!"); ;
        }

        public ActionResult EditPopup(string btnId, string formId, PluginModel model)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
            //    return AccessDeniedView();

            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(model.SystemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                //No plugin found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //we allow editing of 'friendly name', 'display order', store mappings
                pluginDescriptor.FriendlyName = model.FriendlyName;
                pluginDescriptor.DisplayOrder = model.DisplayOrder;
                pluginDescriptor.LimitedToStores.Clear();

                PluginFileParser.SavePluginDescriptionFile(pluginDescriptor);
                //reset plugin cache
                _pluginFinder.ReloadPlugins();
            }

            return View();
        }

        //public ActionResult ConfigureMiscPlugin(string systemName)
        //{
        //    //if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
        //    //    return AccessDeniedView();


        //    var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IMiscPlugin>(systemName);
        //    if (descriptor == null || !descriptor.Installed)
        //        return Redirect("List");

        //    var plugin = descriptor.Instance<IMiscPlugin>();

        //    string actionName, controllerName;
        //    RouteValueDictionary routeValues;
        //    plugin.GetConfigurationRoute(out actionName, out controllerName, out routeValues);
        //    var model = new MiscPluginModel();
        //    model.FriendlyName = descriptor.FriendlyName;
        //    model.ConfigurationActionName = actionName;
        //    model.ConfigurationControllerName = controllerName;
        //    model.ConfigurationRouteValues = routeValues;
        //    return View(model);
        //}


        [NonAction]
        protected virtual PluginModel PreparePluginModel(PluginDescriptor pluginDescriptor,
            bool prepareLocales = true)
        {
            var pluginModel = pluginDescriptor.ToModel();
            //logo
            pluginModel.LogoUrl = pluginDescriptor.GetLogoUrl(_webHelper);

            //if (prepareLocales)
            //{
            //    //locales
            //    AddLocales(_languageService, pluginModel.Locales, (locale, languageId) =>
            //    {
            //        locale.FriendlyName = pluginDescriptor.Instance().GetLocalizedFriendlyName(_localizationService, languageId, false);
            //    });
            //}


            //if (prepareStores)
            //{
            //    //stores
            //    pluginModel.AvailableStores = _storeService
            //        .GetAllStores()
            //        .Select(s => s.ToModel())
            //        .ToList();
            //    pluginModel.SelectedStoreIds = pluginDescriptor.LimitedToStores.ToArray();
            //    pluginModel.LimitedToStores = pluginDescriptor.LimitedToStores.Count > 0;
            //}


            //configuration URLs

            if (pluginDescriptor.IsEnabled)
            {
                //specify configuration URL only when a plugin is already installed

                //plugins do not provide a general URL for configuration
                //because some of them have some custom URLs for configuration
                //for example, discount requirement plugins require additional parameters and attached to a certain discount
                var pluginInstance = pluginDescriptor.Instance();
                string configurationUrl = null;
                //if (pluginInstance is IPaymentMethod)
                //{
                //    //payment plugin
                //    configurationUrl = Url.Action("ConfigureMethod", "Payment", new { systemName = pluginDescriptor.SystemName });
                //}
                //else if (pluginInstance is IShippingRateComputationMethod)
                //{
                //    //shipping rate computation method
                //    configurationUrl = Url.Action("ConfigureProvider", "Shipping", new { systemName = pluginDescriptor.SystemName });
                //}
                //else if (pluginInstance is ITaxProvider)
                //{
                //    //tax provider
                //    configurationUrl = Url.Action("ConfigureProvider", "Tax", new { systemName = pluginDescriptor.SystemName });
                //}
                //else if (pluginInstance is IExternalAuthenticationMethod)
                //{
                //    //external auth method
                //    configurationUrl = Url.Action("ConfigureMethod", "ExternalAuthentication", new { systemName = pluginDescriptor.SystemName });
                //}
                //else if (pluginInstance is IWidgetPlugin)
                //{
                //    //Misc plugins
                //    configurationUrl = Url.Action("ConfigureWidget", "Widget", new { systemName = pluginDescriptor.SystemName });
                //}
                //else if (pluginInstance is IMiscPlugin)
                //{
                //    //Misc plugins
                //    configurationUrl = Url.Action("ConfigureMiscPlugin", "Plugin", new { systemName = pluginDescriptor.SystemName });
                //}
                pluginModel.ConfigurationUrl = configurationUrl;




                ////enabled/disabled (only for some plugin types)
                //if (pluginInstance is IPaymentMethod)
                //{
                //    //payment plugin
                //    pluginModel.CanChangeEnabled = true;
                //    pluginModel.IsEnabled = ((IPaymentMethod)pluginInstance).IsPaymentMethodActive(_paymentSettings);
                //}
                //else if (pluginInstance is IShippingRateComputationMethod)
                //{
                //    //shipping rate computation method
                //    pluginModel.CanChangeEnabled = true;
                //    pluginModel.IsEnabled = ((IShippingRateComputationMethod)pluginInstance).IsShippingRateComputationMethodActive(_shippingSettings);
                //}
                //else if (pluginInstance is ITaxProvider)
                //{
                //    //tax provider
                //    pluginModel.CanChangeEnabled = true;
                //    pluginModel.IsEnabled = pluginDescriptor.SystemName.Equals(_taxSettings.ActiveTaxProviderSystemName, StringComparison.InvariantCultureIgnoreCase);
                //}
                //else if (pluginInstance is IExternalAuthenticationMethod)
                //{
                //    //external auth method
                //    pluginModel.CanChangeEnabled = true;
                //    pluginModel.IsEnabled = ((IExternalAuthenticationMethod)pluginInstance).IsMethodActive(_externalAuthenticationSettings);
                //}
                //else if (pluginInstance is IWidgetPlugin)
                //{
                //    //Misc plugins
                //    pluginModel.CanChangeEnabled = true;
                //    pluginModel.IsEnabled = ((IWidgetPlugin)pluginInstance).IsWidgetActive(_widgetSettings);
                //}

            }
            return pluginModel;
        }


    }
}
