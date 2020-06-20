using Infrastructure.Utility.Data;

using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChatMain.Controllers;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChatMain.Controllers
{
    public class UserBehaviorController : BaseController<UserBehavior, UserBehaviorView>
    {
        public UserBehaviorController(IUserBehaviorService objService)
            : base(objService)
        {
            _objService = objService;
        }


        //
        // GET: /UserBehavior/
        public ActionResult NewIndex()
        {
            return Content("ok");
        }
       
        //
        // GET: /UserBehavior/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /UserBehavior/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            return Content("ok");
        }

        //
        // POST: /UserBehavior/Create
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
             
                //根据functionId 分析
               
                ulong idInModule = 0;
                // 分析Request，获取IP，设备等信息
                string ip = Request.UserHostAddress;
                string device = Request.UserAgent;
                if (_objService.Repository.Insert(new UserBehavior()
                {
                    UserId = collection["userid"],
                    FunctionId = collection["functionId"],
                    AppId = Convert.ToInt32(collection["Appid"]),
                    Content = collection["Content"],
                    Url = collection["url"],
                    ContentType = int.Parse(collection["contentType"]),
                    Device = device,
                    ClientIp = ip,
                    CreatedTime = DateTime.Now
                }) == 1)
                {
                    return Json(new OperationResult(OperationResultType.Success));
                }
                else
                {
                    throw new Exception("Insert data error.");
                }
           
        }

        //
        // POST: /UserBehavior/b
        [HttpPost]
        public ActionResult b(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                foreach (string key in collection.AllKeys)
                {
                    if ("" == key)
                    {

                    }
                }
                return Json(new OperationResult(OperationResultType.Success));
            }
            catch (Exception ex)
            {
                return Json(new OperationResult(OperationResultType.Error, ex.Message));
            }
        }

        //
        // GET: /UserBehavior/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: /UserBehavior/Edit/5        //

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /UserBehavior/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /UserBehavior/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
