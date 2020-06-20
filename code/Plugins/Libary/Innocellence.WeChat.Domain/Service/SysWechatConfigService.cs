// -----------------------------------------------------------------------
//  <copyright file="AutoReplyService.cs" company="Innocellence">
//      Copyright (c) 2014-2015 Innocellence. All rights reserved.
//  </copyright>
//  <last-editor>@Innocellence</last-editor>
//  <last-date>2016-07-13 17:21</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.Weixin.QY.AdvancedAPIs.App;
using Innocellence.Weixin.QY.CommonAPIs;
using Innocellence.Weixin.Entities.Menu;
using Infrastructure.Core.Logging;

namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 微信菜单管理
    /// </summary>
    public partial class SysWechatConfigService : BaseService<SysWechatConfig>, ISysWechatConfigService
    {
        private readonly ISysMenuServiceEx _sysMenuServiceEx;
        private readonly ICategoryService _categoryService;


        private static ILogger log = LogManager.GetLogger("SysWechatConfigService");

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sysMenuServiceEx"></param>
        /// <param name="categoryService"></param>
        public SysWechatConfigService(ISysMenuServiceEx sysMenuServiceEx, ICategoryService categoryService)
            : base("DefaultConnection")
        {
            _sysMenuServiceEx = sysMenuServiceEx;
            _categoryService = categoryService;
        }

        /// <summary>
        /// 
        /// </summary>
        public SysWechatConfigService()
            : base("DefaultConnection")
        {
        }

        /// <summary>
        /// 同步一个企业号下面所有的Aapp信息到平台中
        /// </summary>
        /// <param name="accountManageId"></param>
        /// <param name="accessToken"></param>
        /// <param name="wxAppList"></param>
        /// <param name="appList"></param>
        /// <param name="isSyncMenu"></param> 
        /// <returns></returns>
        public void SyncWechatApps(int accountManageId, string accessToken, List<GetAppList_AppInfo> wxAppList, List<SysWechatConfig> appList,bool isSyncMenu)
        {
            int rootMenuParentId = 0;

            SysMenu model = _sysMenuServiceEx.Repository.Entities.Where(p => p.ParentID == 0 && p.AccountManageId == accountManageId && p.MenuName != "System Admin").FirstOrDefault();

            if (model != null)
            {
                rootMenuParentId = model.Id;
            }


            // 遍历平台已有的App
            foreach (var sysWechatConfig in appList)
            {
                var wxAppId = sysWechatConfig.WeixinAppId;
                var wxApp = wxAppList.FirstOrDefault(x => x.agentid == wxAppId);

                // 删除
                if (wxApp == null)
                {
                    sysWechatConfig.IsDeleted = true;
                    Repository.Update(sysWechatConfig, new List<string> { "IsDeleted" });

                    _sysMenuServiceEx.Repository.SqlExcute("update sysmenu set isdeleted=1 where appid="+ sysWechatConfig.Id.ToString()); //删除导航，正常应该还要删除权限映射
                }
                // 更新
                else
                {
                    // 目前需要更新的只有App名
                    sysWechatConfig.AppName = wxApp.name;
                    Repository.Update(sysWechatConfig, new List<string> { "AppName" });
                }

                //// 初始化时使用
                //if (sysWechatConfig.WeixinAppId != "0")
                //{
                //    _sysMenuServiceEx.InitAppSysMenu(rootMenuParentId, sysWechatConfig.Id, sysWechatConfig.AppName, accountManageId);
                //}
            }


            var configSrc = WeChatCommonService.lstSysWeChatConfig.Find(a=>a.AccountManageId==accountManageId&& a.WeixinCorpId!=null);

            log.Debug("开始同步企业号应用：{0}", accountManageId);

            // 在平台中不存在的情况
            foreach (var wxApp in wxAppList)
            {
                var obj = appList.Find(x => x.WeixinAppId == wxApp.agentid);

                // 追加
                if (obj==null|| obj.Id==0)
                {


                    var config = new SysWechatConfig
                    {
                        // 这两项需要在在编辑页面填写（从微信服务器后台拷贝而来）
                        WeixinToken = "",
                        WeixinEncodingAESKey = "",
                        AccountManageId = accountManageId,
                        // CorpId应该存于主表AccountManage中，所以此处不赋值
                        WeixinCorpId = configSrc.WeixinCorpId,
                        WeixinCorpSecret=configSrc.WeixinCorpSecret,
                        IsCorp = true,
                        AppName = wxApp.name,
                        WeixinAppId = wxApp.agentid,
                    };

                    log.Debug("开始同步新应用：{0} {1}", config.AppName,config.WeixinAppId);

                    Repository.Insert(config);

                    // 添加默认的后台管理菜单SysMenu
                    _sysMenuServiceEx.InitAppSysMenu(rootMenuParentId,config.Id, config.AppName, accountManageId);

                    // 从App服务器同步微信端菜单
                    SyncWeixinMenu(accessToken, int.Parse(config.WeixinAppId), config.Id);

                    log.Debug("同步新应用结束：{0} {1}", config.AppName, config.WeixinAppId);
                }
                else
                {
                    obj.WeixinCorpId = configSrc.WeixinCorpId;
                    obj.WeixinCorpSecret = configSrc.WeixinCorpSecret;
                    if (obj.IsDeleted==true) // 以前删除过的，恢复
                    {

                        _sysMenuServiceEx.Repository.SqlExcute("update sysmenu set isdeleted=0 where appid=" + obj.Id.ToString()); //恢复导航

                        if (isSyncMenu) //同步菜单
                        {
                            _categoryService.Repository.SqlExcute("update category set isdeleted=1 where appid=" + obj.Id.ToString()); //恢复导航
                                                                                                                                       // 从App服务器同步微信端菜单
                            SyncWeixinMenu(accessToken, int.Parse(obj.WeixinAppId), obj.Id);
                        }
                        else
                        {
                            _categoryService.Repository.SqlExcute("update category set isdeleted=0 where appid=" + obj.Id.ToString()); //恢复导航
                        }
                        


                    }
                    obj.IsDeleted = false;
                    Repository.Update(configSrc);
                }         
            }

            // 其他相关处理
            // 刷新cache数据AppList
            WeChatCommonService.ClearCache(3);
            // 刷新cache数据Category：微信菜单
            CommonService.RemoveCategoryFromCache();

            // TODO：目前同步微信端菜单仅仅是对新加的App。对于管理平台已存在的App，暂时不去同步，否则页面效率会更加低下。理论上来说，菜单的添加都应该在管理平台来做，而不是微信后台

        }

        /// <summary>
        /// 同步微信菜单到平台DB
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="agentId"></param>
        /// <param name="appId"></param>
        private void SyncWeixinMenu(string accessToken, int agentId, int appId)
        {
            // 从App服务器同步微信端菜单
            var menus = CommonApi.GetMenu(accessToken, agentId);

            if (menus != null && menus.menu != null)
            {
                // 这个index用于主菜单的排序
                var iIndex = 0;
                foreach (var menu in menus.menu.button)
                {
                    iIndex++;

                    // 如果只有主菜单
                    if (menu is SingleButton)
                    {
                        var mainMenu = menu as SingleButton;
                        SaveMenu(mainMenu.name, 0, "", iIndex, mainMenu, "", appId);
                    } 
                    // 如果包含子菜单
                    else if (menu is SubButton)
                    {
                         var mainMenu = menu as SubButton;

                        int iRet = SaveMenu(mainMenu.name, 0, "", iIndex, mainMenu, "", appId);

                        // 这个index用于子菜单的排序
                        int iIndex1 = 0;

                        foreach (var subMenu in mainMenu.sub_button)
                        {
                            iIndex1++;
                            SaveMenu(subMenu.name, iRet, "", iIndex1, subMenu, "", appId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 保存微信菜单信息到DB
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="parentCode"></param>
        /// <param name="strCode"></param>
        /// <param name="iOrder"></param>
        /// <param name="btn"></param>
        /// <param name="strUrl"></param>
        /// <param name="iAppId"></param>
        /// <returns></returns>
        private int SaveMenu(string strName, int parentCode, string strCode, int iOrder, BaseButton btn, string strUrl, int iAppId)
        {
            var strType = "";

            if (btn is SingleButton)
            {
                var button = btn as SingleButton;
                strType = button.type;

                switch (button.type)
                {
                    case "view":
                        strUrl = (btn as SingleViewButton).url;
                        break;

                    case "pic_weixin":
                    default:
                        dynamic dd = button;
                        strCode = dd.key;
                        break;

                }
            }

            var bt = new ButtonReturnType()
            {
                ResponseMsgType = "News",
                Content = "",
                Button = new MenuButton()
                {
                    key = strCode,
                    name = strName,
                    type = strType,
                    url = strUrl
                }
            };

            var entity = new Category()
            {
                AppId = iAppId,
                CategoryCode = strCode,
                CategoryName = strName,
                CategoryOrder = iOrder,
                ParentCode = parentCode,
                Function = Newtonsoft.Json.JsonConvert.SerializeObject(bt),
                CategoryDesc = strUrl,
                IsAdmin = false,
                IsDeleted = false
            };

            _categoryService.Repository.Insert(entity);

            return entity.Id;
        }
      
    }
}