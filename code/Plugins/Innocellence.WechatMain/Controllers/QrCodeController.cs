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
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Authentication.Authentication;
using Innocellence.WeChat.Domain.Common;
using System.Net;
using System.IO;
using Innocellence.WeChat.Domain;
using System.Data.Common;
using System.Data.SqlClient;
using Infrastructure.Core.Infrastructure;

namespace Innocellence.WeChatMain.Controllers
{
    public class QrCodeController : BaseController<QrCodeMPItem, QrCodeView>
    {
        private IFocusHistoryService _historyService;
        public QrCodeController(IQrCodeService objectService, IFocusHistoryService historyService)
            : base(objectService)
        {
            _historyService = historyService;
        }

        public override ActionResult Index()
        {
            ViewBag.AppId = int.Parse(Request["appId"]);
            return View();
        }

        [HttpPost]
        public ActionResult EditDescription(QrCodeView model)
        {
            ((IQrCodeService)_BaseService).EditDescription(model, User.Identity.Name);
            return Json(new { result = 200 });
        }

        [HttpPost]
        public ActionResult AddQrCodeAll(int AppID)
        {
            var listSrc = _BaseService.Repository.SqlQuery("select null Deleted,null UpdatedDate,null UpdatedUserID, null CreatedUserID, id,0 AppId,0 SceneId,'' Url, name Description,null CreatedDate from QrCodeFrom", false).ToList();
            var list = listSrc.Select(a => (QrCodeView)new QrCodeView().ConvertAPIModel(a)).ToList();

            var autoService = EngineContext.Current.Resolve<IAutoReplyService>();
            var tagService = EngineContext.Current.Resolve<ISystemUserTagService>();

            //Expression<Func<AutoReply, bool>> predicate = d => true;
            var listTag = tagService.GetList<SystemUserTagView>(1000, d => true);

            foreach (var a in list)
            {
                a.AppId = AppId;
                AddQrCode(a);
                var tag = listTag.Find(b => b.Name == a.Description);

                var obj = new AutoReplyView()
                {
                    AppId = AppID,
                    Description = "",
                    Name = a.Description + "[扫码关注]",
                    KeywordType = 362,
                    IsDeleted = false,
                    Keywords = new List<AutoReplyKeywordView>() { new AutoReplyKeywordView() { Keyword = a.SceneId.ToString(), SecondaryType = 0 } },
                    Contents = new List<AutoReplyContentView>() { new AutoReplyContentView() {   PrimaryType=2, SecondaryType=0,   UserTags=new List<int>() { tag.Id }, IsEncrypt=false,IsNewContent=false,
                     Content="您好，终于等到你啦~"
                    } }
                };
                autoService.Add(obj);

                var obj1 = new AutoReplyView()
                {
                    AppId = AppID,
                    Description = "",
                    Name = a.Description + "[扫码]",
                    KeywordType = 50,
                    IsDeleted = false,
                    Keywords = new List<AutoReplyKeywordView>() { new AutoReplyKeywordView() { Keyword = a.SceneId.ToString(), SecondaryType = 0 } },
                    Contents = new List<AutoReplyContentView>() { new AutoReplyContentView() {   PrimaryType=2, SecondaryType=0, UserTags=new List<int>() { tag.Id },IsEncrypt=false,IsNewContent=false,
                     Content="您好，终于等到你啦~"
                    } }
                };
                autoService.Add(obj1);

                _BaseService.Repository.SqlExcute("delete QrCodeFrom where id=" + a.Id);
            }

            return Json(new { result = 200 });
        }

        [HttpPost]
        public ActionResult AddQrCode(QrCodeView model)
        {
            var config = WeChatCommonService.GetWeChatConfigByID(model.AppId);
            var sceneId = ((IQrCodeService)_BaseService).GenerateSceneId();
            var result = QrCodeApi.Create(config.WeixinAppId, config.WeixinCorpSecret, 0, sceneId);
            if (result.errcode == Weixin.ReturnCode.请求成功)
            {
                var url = QrCodeApi.GetShowQrCodeUrl(result.ticket);
                string ourPath = "/content/OrCodeMP" + result.url.Substring(result.url.IndexOf("/q/")) + ".jpg";
                string savePath = Server.MapPath(ourPath);
                if (!System.IO.File.Exists(savePath))//判断文件是否存在 
                {
                    string saveFolderPath = savePath.Replace(savePath.Split('\\').Last(), "");

                    if (!Directory.Exists(saveFolderPath))//判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(saveFolderPath);//不存在则创建文件夹 
                    }
                    var mClient = new WebClient();
                    mClient.DownloadFile(url, savePath);
                }
                model.Url = ourPath;
                model.SceneId = sceneId;
                ((IQrCodeService)_BaseService).AddOrCode(model, User.Identity.Name);
            }

            return Json(new { result = 200 });
        }


        public override List<QrCodeView> GetListEx(Expression<Func<QrCodeMPItem, bool>> predicate, PageCondition ConPage)
        {
            string strDescription = Request["txtDescription"];

            var q = GetListPrivate(ref predicate, ConPage, strDescription);

            return q;
        }

        private List<QrCodeView> GetListPrivate(ref Expression<Func<QrCodeMPItem, bool>> predicate, PageCondition ConPage, string strDescription)
        {
            predicate = predicate.AndAlso(a => a.Deleted == false);
            if (!string.IsNullOrEmpty(strDescription))
            {
                predicate = predicate.AndAlso(a => a.Description.Contains(strDescription));
            }
            ////var q = _objService.Repository.Entities.Where(predicate).ToList().Select(a => (QrCodeView)(new QrCodeView()).ConvertAPIModel(a)).ToList();
            //var q = _BaseService.GetList<QrCodeView>(predicate, ConPage);
            //q.ForEach(a =>
            //{
            //    var historyList = _historyService.Repository.Entities.Where(h => h.QrCodeSceneId == a.Id).ToList();
            //    a.PeopleCount = historyList.Where(t => t.Status == 1).Sum(b => b.PeopleCount.Value);
            //    a.PurePeopleCount = historyList.Where(t => t.Status == 1).Sum(b => b.PurePeopleCount.Value) - historyList.Where(t => t.Status == 4).Sum(b => b.PurePeopleCount.Value);
            //});

            int total = ConPage.RowCount;
            int appId = -1;
            if (int.TryParse(Request["AppId"], out appId))
            {
                var q = (_BaseService as IQrCodeService).GetListAll(ConPage.PageIndex, ConPage.PageSize, ref total, strDescription, appId);
                ConPage.RowCount = total;
                return q;
            }
            return null;
        }
    }
}