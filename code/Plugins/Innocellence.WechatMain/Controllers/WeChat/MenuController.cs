/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：MenuController.cs
    文件功能描述：自定义菜单设置工具Controller
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Innocellence.Weixin.QY;
using Innocellence.Weixin.QY.CommonAPIs;
using Innocellence.Weixin.QY.Entities;
using Innocellence.Weixin.Entities.Menu;
using System.Web.Configuration;
using Innocellence.WeChat.Domain.Contracts;
using Infrastructure.Web.Domain.Contracts;

namespace Innocellence.WeChatMain.Controllers
{
    public class MenuController : WinxinBaseController
    {

        public MenuController(ICategoryService newsService)
            : base(newsService)
        {
           // _newsService = newsService;
        }

        //
        // GET: /Menu/

        public override ActionResult Index()
        {
            GetMenuResult result = new GetMenuResult();

            //初始化
            for (int i = 0; i < 3; i++)
            {
                var subButton = new SubButton();
                for (int j = 0; j < 5; j++)
                {
                    var singleButton = new SingleClickButton();
                    subButton.sub_button.Add(singleButton);
                }
            }



            //CommonAPIs.CommonApi.GetToken(CorpId, EncodingAESKey);

            ViewBag.Token = GetToken();

           // return RedirectToAction("index", "home");

            return View(result);
        }

        //public ActionResult GetToken()
        //{
        //    try
        //    {
        //        if (!AccessTokenContainer.CheckRegistered(CorpId))
        //        {
        //            AccessTokenContainer.Register(CorpId, CorpSecret);
        //        }
        //        var result = AccessTokenContainer.GetTokenResult(CorpId); //CommonAPIs.CommonApi.GetToken(CorpId, EncodingAESKey);

        //        //也可以直接一步到位：
        //        //var result = AccessTokenContainer.TryGetToken(CorpId, EncodingAESKey);
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        //TODO:为简化代码，这里不处理异常（如Token过期）
        //        return Json(new { error = "执行过程发生错误！" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public ActionResult CreateMenu(string token, GetMenuResultFull resultFull)
        {
            try
            {
                
                //重新整理按钮信息
                var bg =CommonApi.GetMenuFromJsonResult(resultFull).menu;
                var result = CommonApi.CreateMenu(token, AppId, bg);
                var json = new
                {
                    Success = result.errmsg == "ok",
                    Message = result.errmsg
                };
                return Json(json);
            }
            catch (Exception ex)
            {
                var json = new { Success = false, Message = ex.Message };
                return Json(json);
            }
        }

        public ActionResult GetMenu(string token)
        {
            var result = CommonApi.GetMenu(token, AppId);
            if (result == null)
            {
                return Json(new { error = "菜单不存在或验证失败！" }, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMenu(string token)
        {
            try
            {
                var result = CommonApi.DeleteMenu(token, AppId);
                var json = new
                               {
                                   Success = result.errmsg == "ok",
                                   Message = result.errmsg
                               };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new { Success = false, Message = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
