using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using Infrastructure.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChatMain.Controllers
{
    public class MessageLogController : BaseController<WechatMessageLog, WechatMessageLogView>
    {
        public MessageLogController(IWechatMessageLogService objService)
            : base(objService)
        {

        }

        [HttpGet]
        public JsonResult GetItem(int id)
        {
            var item = _BaseService.GetList<WechatMessageLogView>(1, a => a.Id == id).FirstOrDefault();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public override List<WechatMessageLogView> GetListEx(Expression<Func<WechatMessageLog, bool>> predicate, PageCondition ConPage)
        {
            string strType = Request["txtType"];
            string txtDate = Request["txtDate"];

            var q = GetListPrivate(ref predicate, ConPage, strType, txtDate);

            return q.ToList();
        }

        protected List<WechatMessageLogView> GetListPrivate(ref Expression<Func<WechatMessageLog, bool>> predicate, PageCondition ConPage, string strType, string txtDate)
        {
            //predicate = predicate.AndAlso(a => a.IsDeleted == false);

            if (!string.IsNullOrEmpty(strType))
            {
                int contentType = (int)Enum.Parse(typeof(WechatMessageLogType), strType);
                predicate = predicate.AndAlso(a => a.ContentType == contentType);
            }

            if (!string.IsNullOrEmpty(txtDate))
            {
                DateTime dateTime = Convert.ToDateTime(txtDate);
                DateTime dateAdd = dateTime.AddDays(1);
                predicate = predicate.AndAlso(a => a.CreatedTime >= dateTime && a.CreatedTime <= dateAdd);
            }

            var q = _BaseService.GetList<WechatMessageLogView>(predicate, ConPage);
            return q;
        }
    }
}