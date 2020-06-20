using Infrastructure.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Innocellence.WeChatMain.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult HttpError()
        {
            return View("Error");
        }
        public ActionResult NotFound()
        {
            return View("Error");
        }
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}