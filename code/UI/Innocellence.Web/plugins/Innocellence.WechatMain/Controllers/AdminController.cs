using Antlr.Runtime.Misc;
using Infrastructure.Utility.Data;
using Infrastructure.Web.MVC.Attribute;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Innocellence.Authentication.Attribute;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.Domain.Model;
using Innocellence.WeChat.Domain.ModelsView;
using Infrastructure.Core.Data;
using System.Text;
using Innocellence.WeChat.Domain.Common;
using System.ComponentModel.DataAnnotations;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChatMain.Controllers
{

    public class AdminController : BaseController<AccountManage, AccountManageView>
    {
        private readonly IAccountManageService _accountManageService;
        private readonly ISysMenuService _menuService;
        private readonly ISysWechatConfigService _wechatConfigService;
        protected BaseService<ArticleImages> _articelImageService = new BaseService<ArticleImages>("CAAdmin");



        public AdminController(IAccountManageService objService, ISysMenuService menuService, ISysWechatConfigService wechatConfigService)
            : base(objService)
        {
            _accountManageService = objService;
            _menuService = menuService;
            _wechatConfigService = wechatConfigService;
        }

        /// <summary>
        /// 删除功能
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteAccountManage()
        {

            var accountManageView = new AccountManageView();
            accountManageView.Id = Convert.ToInt32(Request["ids"]);
            accountManageView.IsDeleted = true;
            _accountManageService.DelleteAccountManage(accountManageView);
            //   return View("AccountManage");
            return RedirectToAction("AccountManage");
        }


        public JsonResult UpdateAccountManage()
        {
            var accountManageView = new AccountManageView();
            if (Request["ids"].Length > 0)
            {
                var list = _accountManageService.GetAllAccountManage();
                var listAll = list.Where(a => a.Id == Convert.ToInt32(Request["ids"]) && a.IsDeleted == false).ToList();

                foreach (var item in listAll)
                {
                    accountManageView.AccountName = item.AccountName;
                    accountManageView.AccountDescription = item.AccountDescription;
                }
                ViewBag.listAll = accountManageView;

            }
            var res = new JsonResult();

            var person = new { _accountName = accountManageView.AccountName, _accountDescription = accountManageView.AccountDescription };
            res.Data = person;
            //return Json(accountManageView.AccountName, accountManageView.AccountDescription, JsonRequestBehavior.AllowGet);
            return Json(new { _accountName = accountManageView.AccountName, _accountDescription = accountManageView.AccountDescription }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 添加公众号和服务号更新公众号
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAccountManage(string AccountName, string AccountDescription, HttpPostedFileBase AccountLogo, HttpPostedFileBase QrCode, string cbox)
        {

            return View("AccountManage");
        }

        /// <summary>
        /// 目前暂时不支持手动添加企业号与服务号
        /// 该方法不完整暂时弃用
        /// </summary>
        /// <param name="accountManageView"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAccountManages(MpAppAddView addMpAppView)
        {
            if (!ModelState.IsValid)
            {
                StringBuilder builder = new StringBuilder();
                var errorList = ModelState.Values.Where(a => a.Errors.Count > 0).ToList();
                foreach (var error in errorList)
                {
                    foreach (var item in error.Errors)
                    {
                        builder.Append(item.ErrorMessage).Append("</br>");
                    }
                }
                throw new Exception(builder.ToString());
            }

            try
            {
                AccountManageView accountManageView = new AccountManageView()
                {
                    CorpId = addMpAppView.WeixinAppId,
                    AccountName = addMpAppView.AccountName,
                    AccountDescription = addMpAppView.AccountDescription,
                    AccountLogo = addMpAppView.AccountLogo,
                    QrCode = addMpAppView.QrCode,
                    AccountType = addMpAppView.AccountType,
                };
                AccountManage model = _accountManageService.Add(accountManageView);
                if (accountManageView.AccountLogo.ContentLength > 0)
                {
                    //获得保存路径
                    string newName = "AccountLogo" + model.Id.ToString() + accountManageView.AccountLogo.FileName.ToString().Substring(accountManageView.AccountLogo.FileName.ToString().IndexOf("."));
                    string filePath = Path.Combine(HttpContext.Server.MapPath("~/"), "Content/picture",
                                    Path.GetFileName(newName));
                    accountManageView.AccountLogo.SaveAs(filePath);
                    string resoucePath = Path.Combine(("/Content/picture"), Path.GetFileName(newName));
                    model.AccountLogo = resoucePath.Replace("\\", "/");
                }

                if (accountManageView.QrCode.ContentLength > 0)
                {
                    //获得保存路径
                    string newName = "AccountQrCode" + model.Id.ToString() + accountManageView.QrCode.FileName.ToString().Substring(accountManageView.QrCode.FileName.ToString().IndexOf("."));
                    string filePath = Path.Combine(HttpContext.Server.MapPath("~/"), "Content/picture",
                                    Path.GetFileName(newName));
                    accountManageView.QrCode.SaveAs(filePath);
                    string resoucePath = Path.Combine(("/Content/picture"), Path.GetFileName(newName));
                    model.QrCode = resoucePath.Replace("\\", "/");
                }
                _accountManageService.Update(model);
                //在SysWeCahtConfig 表中添加企业小助手(企业号) 或 服务号名称(服务号)的APP,WeixinAppid = 0, 为了让企业号可以load 出来
                int wechatid = AddSysWeChatConfigNode(model, addMpAppView);
                //在Menu表中添加AccountManageID 列, 用来标记, Infrastructure.Web.Domain.dll
                //查找AccountManage在Menu 中的节点的条件为: ParentId=0 && AccountManageID = AccountManageID
                //添加Menu节点信息, 并生成Default Menu
                AddMenuNodeForAccountManagement(model, wechatid);

                var list = FilterAccountManageByUser();
                list = list.Where(a => a.AccountType == accountManageView.AccountType).ToList();

                return Json(new { result = 200});
            }
            catch (Exception e)
            {
                _Logger.Debug("Add New MP APP Error:", e);
                return Json(new { result = 400, message = e });
                //throw e;
            }

            
        }

        private int AddSysWeChatConfigNode(AccountManage model, MpAppAddView addMpAppView)
        {
            SysWechatConfig sysView = new SysWechatConfig()
            {
                AccountManageId = model.Id,
                AppName = model.AccountName,
                CreatedDate = DateTime.Now,
                CreatedUserID = User.Identity.Name,
                WeixinToken = addMpAppView.WeixinToken,
                WeixinEncodingAESKey = addMpAppView.WeixinEncodingAESKey,
                WeixinAppId = addMpAppView.WeixinAppId,
                WeixinCorpId = addMpAppView.WeixinAppId,
                WeixinCorpSecret = addMpAppView.WeixinCorpSecret,
                IsCorp = model.AccountType == 0,
            };
            _Logger.Debug("Create SysConfigView:  {0}", sysView);
            _wechatConfigService.Repository.Insert(sysView);
            int mainMenuId = sysView.Id;
            return mainMenuId;

        }

        /// <summary>
        /// 为AccountManage 添加Menu 节点
        /// 除此之外还需要添加默认Menu, 目前还未完成.
        /// [SysWechatConfig]表也需要更新, 什么时候更新?
        /// </summary>
        /// <param name="model"></param>
        private void AddMenuNodeForAccountManagement(AccountManage model,int wechatid)
        {
            //SysRole 的SortCode 默认最大, 目前默认值为999, 需要在服务器上改成int.MaxValue
            //在计算SortCode 时, 需要将其过滤
            //Expression<System.Func<SysMenu, bool>> p = m => (int)m.ParentID == 1 && (bool)m.IsDeleted == false && (int)m.SortCode < int.MaxValue;
            //var list = _menuService.GetList<SysMenuView>(p, new PageCondition());

            ///先添加主要服务号菜单
            SysMenu menuView = new SysMenu()
            {
                MenuName = model.AccountName,
                MenuTitle = model.AccountName,
                AppId = wechatid,
                ParentID = 0,
                CreatedDate = DateTime.Now,
                CreatedUserID = User.Identity.Name,
                SortCode = 0,
                AccountManageId = model.Id,
                //copy当前website的属性
                MenuImg = "glyphicon glyphicon-time",
                MenuType = 1,
                MenuGroup = "CA",
                NavigateUrl = "/",
                IsDeleted = false,
                IsDisplay = true,
            };
            _menuService.Repository.Insert(menuView);
            int mainMenuId = menuView.Id;
            _Logger.Debug("Create FirstMenuView: {0} -----Id:{1}", menuView, mainMenuId);

            ///添加服务号消息历史菜单 获取消息历史ID
            SysMenu historyMenuView = new SysMenu()
            {
                MenuName = "消息历史",
                MenuTitle = "消息历史",
                AppId = wechatid,
                ParentID = mainMenuId,
                CreatedDate = DateTime.Now,
                CreatedUserID = User.Identity.Name,
                SortCode = 0,
                AccountManageId = model.Id,
                //copy当前website的属性
                MenuImg = "glyphicon glyphicon-envelope",
                MenuType = 1,
                MenuGroup = "CA",
                NavigateUrl = "/",
                IsDeleted = false,
                IsDisplay = true,
            };
            _menuService.Repository.Insert(historyMenuView);
            int historyId = historyMenuView.Id;

            var sql = string.Format("INSERT INTO [SysMenu] ([ParentID], [FormID], [MenuName], [MenuTitle], [MenuImg], [MenuType], [MenuGroup], [NavigateUrl], [SortCode], [IsDisplay], [IsDeleted], [CreatedDate], [CreatedUserID], [AccountManageId], [AppId]) " +
            " VALUES('{0}', NULL, N'图文编辑', N'图文编辑', 'glyphicon glyphicon-th', '1', NULL, '/WeChatMain/ArticleInfo/index?wechatid={2}&appid={2},/WeChatMain/ArticleInfo/*', '6', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')," +
            "('{0}', NULL, N'素材管理', N'素材管理', 'glyphicon glyphicon-th', '1', 'CA', '/WechatMain/FileManage/index?appid={2},/WechatMain/FileManage/*', '7', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')," +
            "('{0}', NULL, N'群发消息列表', N'群发消息列表', 'glyphicon glyphicon-upload', '1', NULL, '/WeChatMain/Message/Index?wechatid={2}&appid={2},/WeChatMain/Message/*,/WeChatMain/messagelog/*,/WechatMain/SendMessageLog/*', '9', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')," +
            "('{0}', NULL, N'消息历史', N'消息历史', 'glyphicon glyphicon-envelope', '1', 'CA', '/', '1', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')," +
            "('{0}', NULL, N'群发消息', N'群发消息', 'glyphicon glyphicon-upload', '1', 'CA', '/WeChatMain/Message/WechatServiceMessage?appid={2},/WeChatMain/Message/*,/UserMp/*', '8', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')," +
            "('{0}', NULL, N'二维码', N'二维码', 'glyphicon glyphicon-qrcode', '1', NULL, '/WechatMain/QrCode/index?appId={2},/WechatMain/QrCode/*', '3', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')," +
            "('{0}', NULL, N'菜单管理', N'菜单管理', 'glyphicon glyphicon-qrcode', '1', NULL, '/WeChatMain/AppManage/MenuManagement?appid={2},/wechatmain/appmanage/*,/wechatmain/appmenu/*,/appmanage/*,/WeChatMain/AutoReply/*', '5', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')," +
            "('{0}', NULL, N'通讯录', N'通讯录', 'glyphicon glyphicon-book', '1', 'System Admin', '/WechatMain/usermp/grouplist,/WechatMain/usermp/*,/WechatMain/Department/*,/WechatMain/WechatUserRequestMessageLog/GetUnreadMsgCount,/WeChatMain/WechatUserRequestMessageLog/*', '2', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')," +
            "('{0}', NULL, N'口令管理', N'口令管理', 'glyphicon glyphicon-volume-up', '1', 'CA', '/WeChatMain/AutoReply/Index?appid={2},/WeChatMain/AutoReply/*', '4', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')," +
            "('{1}', NULL, N'未读', N'未读', 'glyphicon glyphicon-volume-up', '1', 'CA', '/WeChatMain/WechatUserRequestMessageLog/Index?appid={2}&hasReaded=false,/WeChatMain/WechatUserRequestMessageLog/*', '1', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')," +
            "('{1}', NULL, N'所有', N'所有', 'glyphicon glyphicon-volume-up', '1', 'CA', '/WeChatMain/WechatUserRequestMessageLog/Index?appid={2},/WeChatMain/WechatUserRequestMessageLog/*', '2', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')," +
            "('{1}', NULL, N'隐藏自动回复', N'隐藏自动回复', 'glyphicon glyphicon-volume-up', '1', 'CA', '/WeChatMain/WechatUserRequestMessageLog/Index?appid={2}&hiddenAutoReply=true,/WeChatMain/WechatUserRequestMessageLog/*', '3', '1', '0', '{4}', 'cwwhy1', '{3}', '{2}')" , mainMenuId, historyId, wechatid, model.Id,DateTime.Now);
            _Logger.Debug("SQL sectence:", sql);
            _menuService.Repository.SqlExcute(sql);



        }

        [CustomAuthorize]
        public override ActionResult Index()
        {
            var list = FilterAccountManageByUser();
            ViewBag.Company = list.Where(a => a.AccountType == 0).ToList();
            ViewBag.Service = list.Where(a => a.AccountType == 1).ToList();
            return View();
        }

        private List<AccountManage> FilterAccountManageByUser()
        {
            var list = _accountManageService.GetAllAccountManage();
            var user = Session["UserInfo"] as SysUser;
            if (user != null)
            {
                //List<int> appIds = user.Menus.ToList().Where(m => m.AppId != null).Select(m => (int)m.AppId).Distinct().ToList();
                List<int> accountManageIds = WeChatCommonService.lstSysWeChatConfig.Where(a => user.Apps.Contains(a.Id)).Select(a => a.AccountManageId.Value).Distinct().ToList();
                list = list.Where(a => accountManageIds.Contains(a.Id)).ToList();
            }
            return list;
        }

        public ActionResult About()
        {
            var path = Server.MapPath("~/");
            var versionFile = path + @"releasenotes.txt";
            var releaseNotes = "";
            using (var sr = new StreamReader(versionFile))
            {
                releaseNotes = sr.ReadToEnd();
                sr.Close();
            }
            ViewBag.ReleaseNotes = releaseNotes.Replace("\n", "<BR />");
            return View();
        }


        public class MpAppAddView 
        {
            public Int32 Id { get; set; }
            [Required(ErrorMessage = "请上传Qr Code")]
            public HttpPostedFileBase QrCode { get; set; }
            [Required(ErrorMessage = "请上传Account Logo")]
            public HttpPostedFileBase AccountLogo { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "账户名称不能为空")]
            public string AccountName { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "账户描述不能为空")]
            public string AccountDescription { get; set; }
            public string AccountLogoPath { get; set; }
            public string QrCodePath { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "AppID(应用ID)不能为空")]
            public string WeixinAppId { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "AppSecret(应用密钥)不能为空")]
            public string WeixinCorpSecret { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "Token(令牌)不能为空")]
            public string WeixinToken { get; set; }
            public string WeixinEncodingAESKey { get; set; }

            public int AccountType { get; set; }

        }

    }
}