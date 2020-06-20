
using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.WeChatMain.Services;


namespace Innocellence.WeChatMain.Controllers
{
  
    public partial class GlobalMessageController : MessageController {
        public GlobalMessageController(IArticleInfoService objService)
            : base(objService, (int)CategoryType.Undefined)
        {

        }

        protected void GetAppList()
        {
            //将APPList从表里取出 前台页面做过滤用的
            var corpId = CommonService.GetSysConfig("WeixinCorpId", string.Empty);
            var lst = WeChatCommonService.lstSysWeChatConfig.Where(a => a.WeixinCorpId == corpId);
            ViewBag.Apps = lst;
        }

        [ActionName("GlobalMessageIndex")]
        public override ActionResult Index()
        {
            string appid = Request["appId"];

            if (!string.IsNullOrEmpty(appid))
            {
                AppId = int.Parse(appid);
                @ViewBag.AppId = appid;
            }

            GetAppList();
            
            return View("../Message/GlobalIndex");
        }

        [ActionName("GlobalMessageEdit")]
        public override ActionResult Edit(string id)
        {
            GetAppList();
            PrepareEditData();
            var obj = GetObject(id);
            return View("../Message/GlobalEdit", obj);
        }

        public override void PrepareEditData()
        {
            //修改tag数据源 由于是公共的发消息 没选APP之前不知道应用的allow_tags 取全部tag
            string accessToken = WeChatCommonService.GetWeiXinToken(AppId);
            var tagList = MailListApi.GetTagList(accessToken).taglist;
            ViewBag.taglist = tagList;
        }

        //Post方法
        [HttpPost]
        [ValidateInput(false)]
        public override JsonResult Post(ArticleInfoView objModal, string Id)
        {
            if (!objModal.AppId.HasValue)
            {
                ModelState.AddModelError("Empty APP", "Please Select APP.<br/>");
            }
            else
            {
                AppId = objModal.AppId.Value;
            }
            return base.Post(objModal, Id);
        }

        //public override List<MessageView> GetListEx(Expression<Func<Message, bool>> predicate, PageCondition ConPage)
        //{
        //    string appId = Request["appCate"];
        //    if (!string.IsNullOrEmpty(appId))
        //    {
        //        AppId = int.Parse(appId);
        //    }
        //    return base.GetListEx(predicate, ConPage);
        //}

        public override JsonResult ChangeStatus(string Id, int appid, bool ispush)
        {
            AppId = appid;
            return base.ChangeStatus(Id, appid, ispush);
        }

        [HttpPost]
        [ValidateInput(false)]
        public override JsonResult WxPreview(ArticleInfoView objModal)
        {
            if (!objModal.AppId.HasValue)
            {
                ModelState.AddModelError("Empty APP", "Please Select APP.<br/>");
            }
            else
            {
                AppId = objModal.AppId.Value;
            }
            return base.WxPreview(objModal);
        }

        public override void InsertOrUpdate(ArticleInfoView objModal, string Id)
        {
            objModal.AppId = AppId;
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                _BaseService.InsertView(objModal);
            }
            else
            {
                var lst = new List<string>(){
                  "Title","Content","Comment","URL","AppId",
                  "Previewers","toDepartment","toTag","toUser"};

                _BaseService.UpdateView(objModal, lst);
            }
        }

    }


}
