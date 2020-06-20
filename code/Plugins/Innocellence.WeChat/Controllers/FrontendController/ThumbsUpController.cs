using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Infrastructure.Utility.IO;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Common;
using NetUtilityLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChat.Controllers
{
    public partial class ThumbsUpController : WeChatBaseController<ThumbsUp, ThumbsUpCountView>
    {
        private string[] imgPath = new string[] { "Media", "ThumbsUp" };

        public ThumbsUpController(IThumbsUpService objService)
            : base(objService)
        {
            // _newsService = newsService;
        }

        [HttpGet]
        public ActionResult List(string module, int code)
        {
            // TODO: WeChatUserID怎么获取
            string WeChatUserID = "C217355";
            var tuv = new ThumbsUpCountView()
            {
                // 性能方面，看能不能给ThumbsUpCount的Count去掉。
                ThumbsUpCount = _newsService.Repository.Entities.Where(a => a.TableName == module && a.RecordID == code).Count(),
                AmIThumbsUp = (_newsService.Repository.Entities.Where(a => a.TableName == module && a.RecordID == code && a.WeChatUserID == WeChatUserID).FirstOrDefault() == null)
            };

            return Json(tuv);
        }


    }
}
