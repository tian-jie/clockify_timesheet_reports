using System;
using System.Net;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Service;
using Infrastructure.Utility.Data;
using Infrastructure.Core.Data;
using Infrastructure.Web.UI;
using System.IO;

namespace Innocellence.WeChatMeeting.Controllers
{
    public class LocalLogsController : Controller
    {
        public LocalLogsController()
        {

        }

        [AllowAnonymous]
        public ActionResult GetLocalLogs(string logfilename)
        {
            string logName = Server.MapPath("/Logs/" + logfilename);
            var sr = new StreamReader(logName);
            string log = sr.ReadToEnd().Replace("\r\n", "<br/>");
            ViewBag.Log = log;
            return View();
        }
    }
}
