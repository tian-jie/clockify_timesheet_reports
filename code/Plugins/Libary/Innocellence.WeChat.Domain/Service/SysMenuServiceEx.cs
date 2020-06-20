// -----------------------------------------------------------------------
//  <copyright file="AutoReplyService.cs" company="Innocellence">
//      Copyright (c) 2014-2015 Innocellence. All rights reserved.
//  </copyright>
//  <last-editor>@Innocellence</last-editor>
//  <last-date>2016-07-13 17:21</last-date>
// -----------------------------------------------------------------------

using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 业务实现——后台管理菜单管理模块
    /// </summary>
    public partial class SysMenuServiceEx : BaseService<SysMenu>, ISysMenuServiceEx
    {

        public SysMenuServiceEx()
            : base("DefaultConnection")
        {
        }

        /// <summary>
        /// 此处最好改成读配置文件。
        /// </summary>
        /// <param name="rootMenuParentId"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="accountManageId"></param>
        public void InitAppSysMenu(int rootMenuParentId,int appId, string appName, int accountManageId)
        {


            var d=  CommonService.GetSysConfig("APPMenus", "");

            var str = d.Replace("{0}", accountManageId.ToString()).Replace("{1}", rootMenuParentId.ToString())
                .Replace("{2}", System.DateTime.Now.ToString()).Replace("{3}", Repository.LoginUserName)
                .Replace("{4}", appId.ToString()).Replace("{5}", appName);
           // var str=  string.Format(d, accountManageId, rootMenuParentId,System.DateTime.Now, Repository.LoginUserName,  appId);
            var list = JsonConvert.DeserializeObject<List<SysMenu>>(str);
            var parent = list.Find(a => a.ParentID == rootMenuParentId);
            Repository.Insert(parent);
            list.Remove(parent);
            foreach(var a in list)
            {
                a.ParentID = parent.Id;
                a.Id = 0;
            }

            Repository.Insert(list);

            //// 根Menu
            //var rootMenu = new SysMenu
            //{
            //    ParentID = rootMenuParentId,
            //    AppId = appId,
            //    MenuName = appName,
            //    MenuTitle = appName,
            //    MenuImg = "fa fa-newspaper-o",
            //    MenuType = 1,
            //    MenuGroup = "CA",
            //    NavigateUrl = "/",
            //    SortCode = 1,
            //    IsDisplay = true,
            //    IsDeleted = false,
            //    AccountManageId = accountManageId
            //};

            //Repository.Insert(rootMenu);

            //var rootMenuId = rootMenu.Id;

            //// 子Menu - 发消息
            //var subMenu1 = new SysMenu
            //{
            //    ParentID = rootMenuId,
            //    AppId = appId,
            //    MenuName = "发消息",
            //    MenuTitle = "发消息",
            //    MenuImg = "glyphicon glyphicon-comment",
            //    MenuType = 1,
            //    MenuGroup = "CA",
            //    NavigateUrl = string.Format("/WeChatMain/Message/EditNews?appid={0},/WeChatMain/Message/*,/Tagmanage/*", appId),
            //    SortCode = 1,
            //    IsDisplay = true,
            //    IsDeleted = false,
            //    AccountManageId = accountManageId
            //};
            //Repository.Insert(subMenu1);

            //// 子Menu - 消息列表
            //var subMenu2 = new SysMenu
            //{
            //    ParentID = rootMenuId,
            //    AppId = appId,
            //    MenuName = "消息列表",
            //    MenuTitle = "消息列表",
            //    MenuImg = "glyphicon glyphicon-book",
            //    MenuType = 1,
            //    MenuGroup = "CA",
            //    NavigateUrl = "/WeChatMain/Message/Index?wechatid=" + appId + "&appid=" + appId + ",/WeChatMain/Message/*,/WeChatMain/messagelog/*",
            //    SortCode = 2,
            //    IsDisplay = true,
            //    IsDeleted = false,
            //    AccountManageId = accountManageId
            //};
            //Repository.Insert(subMenu2);



            //// 子Menu - 图文编辑
            //var subMenu3 = new SysMenu
            //{
            //    ParentID = rootMenuId,
            //    AppId = appId,
            //    MenuName = "图文编辑",
            //    MenuTitle = "图文编辑",
            //    MenuImg = "glyphicon glyphicon-edit",
            //    MenuType = 1,
            //    MenuGroup = "CA",
            //    NavigateUrl = "/WeChatMain/ArticleInfo/index?wechatid=" + appId + "&appid=" + appId + ",/WeChatMain/ArticleInfo/*",
            //    SortCode = 3,
            //    IsDisplay = true,
            //    IsDeleted = false,
            //    AccountManageId = accountManageId
            //};
            //Repository.Insert(subMenu3);

            //// 子Menu - 素材管理
            //var subMenu4 = new SysMenu
            //{
            //    ParentID = rootMenuId,
            //    AppId = appId,
            //    MenuName = "素材管理",
            //    MenuTitle = "素材管理",
            //    MenuImg = "glyphicon glyphicon-file",
            //    MenuType = 1,
            //    MenuGroup = "CA",
            //    NavigateUrl = "/WechatMain/FileManage/index?appid=" + appId + ",/WeChatMain/FileManage/*",
            //    SortCode = 4,
            //    IsDisplay = true,
            //    IsDeleted = false,
            //    AccountManageId = accountManageId
            //};
            //Repository.Insert(subMenu4);

            //// 子Menu - 口令管理
            //var subMenu5 = new SysMenu
            //{
            //    ParentID = rootMenuId,
            //    AppId = appId,
            //    MenuName = "口令管理",
            //    MenuTitle = "口令管理",
            //    MenuImg = "glyphicon glyphicon-volume-up",
            //    MenuType = 1,
            //    MenuGroup = "CA",
            //    NavigateUrl = "/WeChatMain/AutoReply/Index?appid=" + appId + ",/WeChatMain/AutoReply/*",
            //    SortCode = 5,
            //    IsDisplay = true,
            //    IsDeleted = false,
            //    AccountManageId = accountManageId
            //};
            //Repository.Insert(subMenu5);

            //// 子Menu - 消息历史
            //var subMenu6 = new SysMenu
            //{
            //    ParentID = rootMenuId,
            //    AppId = appId,
            //    MenuName = "消息历史",
            //    MenuTitle = "消息历史",
            //    MenuImg = "glyphicon glyphicon-envelope",
            //    MenuType = 1,
            //    MenuGroup = "CA",
            //    NavigateUrl = "/WeChatMain/WechatUserRequestMessageLog/Index?appid=" + appId + ",/WeChatMain/WechatUserRequestMessageLog/*",
            //    SortCode = 6,
            //    IsDisplay = true,
            //    IsDeleted = false,
            //    AccountManageId = accountManageId
            //};
            //Repository.Insert(subMenu6);

            //// 子Menu - 菜单管理
            //var subMenu7 = new SysMenu
            //{
            //    ParentID = rootMenuId,
            //    AppId = appId,
            //    MenuName = "菜单管理",
            //    MenuTitle = "菜单管理",
            //    MenuImg = "glyphicon glyphicon-th",
            //    MenuType = 1,
            //    MenuGroup = "CA",
            //    NavigateUrl = "/wechatmain/appmanage/CategoryManageList/" + appId + ",/wechatmain/appmanage/*,/wechatmain/appmenu/*,/appmanage/*",
            //    SortCode = 7,
            //    IsDisplay = true,
            //    IsDeleted = false,
            //    AccountManageId = accountManageId
            //};
            //Repository.Insert(subMenu7);

            //// 子Menu - 报表
            //var subMenu8 = new SysMenu
            //{
            //    ParentID = rootMenuId,
            //    AppId = appId,
            //    MenuName = "报表",
            //    MenuTitle = "报表",
            //    MenuImg = "glyphicon fa fa-file-excel-o",
            //    MenuType = 1,
            //    MenuGroup = "CA",
            //    NavigateUrl = "/WeChatMain/AppReport/Report/" + appId + ",/WeChatMain/AppReport/*",
            //    SortCode = 8,
            //    IsDisplay = true,
            //    IsDeleted = false,
            //    AccountManageId = accountManageId
            //};
            //Repository.Insert(subMenu8);

            //// 子Menu - 设置
            //var subMenu9 = new SysMenu
            //{
            //    ParentID = rootMenuId,
            //    AppId = appId,
            //    MenuName = "设置",
            //    MenuTitle = "设置",
            //    MenuImg = "glyphicon glyphicon-cog",
            //    MenuType = 1,
            //    MenuGroup = "CA",
            //    NavigateUrl = "/WeChatMain/AppManage/Edit/" + appId + ",/WeChatMain/AppManage/*",
            //    SortCode = 9,
            //    IsDisplay = true,
            //    IsDeleted = false,
            //    AccountManageId = accountManageId
            //};
            //Repository.Insert(subMenu9);

        }
      
    }
}