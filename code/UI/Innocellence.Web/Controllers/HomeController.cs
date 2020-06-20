using Antlr.Runtime.Misc;
using Infrastructure.Utility.Data;
using Infrastructure.Web.MVC.Attribute;
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
using Infrastructure.Core.Data;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Innocellence.Web.Controllers
{

    public class HomeController : BaseController<SysUser, SysUserView>
    {
       // private readonly IAccountManageService _accountManageService;
        private readonly ISysUserService _menuService;
      //  private readonly ISysWechatConfigService _wechatConfigService;
       // protected BaseService<ArticleImages> _articelImageService = new BaseService<ArticleImages>("CAAdmin");



        public HomeController(ISysUserService menuService)
            : base(menuService)
        {
           // _accountManageService = objService;
            _menuService = menuService;
           // _wechatConfigService = wechatConfigService;
        }

        public override ActionResult Index()
        {
            return Redirect("/WeChatMain/admin/index");
        }



        /// <summary>
        /// 添加公众号和服务号更新公众号
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAccountManage(string AccountName, string AccountDescription, HttpPostedFileBase AccountLogo, HttpPostedFileBase QrCode, string cbox)
        {

            return View("AccountManage");
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



    }
}