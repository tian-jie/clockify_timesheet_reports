using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.IO;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.Domain.Service.Common;
using Infrastructure.Web.UI;
using Innocellence.Authentication.Authentication;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChatMain.Services;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.AdvancedAPIs.App;
using Innocellence.Weixin.QY.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChatMain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.Service;
using Newtonsoft.Json;
using Innocellence.WeChat.Domain.ViewModel;
using System.IO;
using System.Transactions;
using Innocellence.Weixin.QY.AdvancedAPIs.Mass;
using Innocellence.Weixin.QY;
using Innocellence.WeChat.Domain.ViewModelFront;
using Innocellence.Weixin.MP.AdvancedAPIs.GroupMessage;
using Innocellence.WeChat.Domain;
using Infrastructure.Core.Infrastructure;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.Entities;
using MP = Innocellence.Weixin.MP;
using System.Drawing;

namespace Innocellence.WeChatMain.Controllers
{
    public partial class MessageController : BaseController<ArticleInfo, ArticleInfoView>
    {
        //  protected BaseService<ArticleThumbsUp> _articleThumbsUpService = new BaseService<ArticleThumbsUp>("CAAdmin");
        //  protected BaseService<ArticleImages> _articelImageService = new BaseService<ArticleImages>("CAAdmin");
        private static readonly IDataPermissionCheck permission = new MessageDataPermissionService();
        private static string wechatBaseUrl = CommonService.GetSysConfig("WeChatUrl", "");
        //    private readonly IMessageService _messageService;
        //   private readonly IWechatMessageLogService _messageLogService;
        //   private readonly IWechatPreviewMessageLogService _previewMessageLogService;
        private readonly IAttachmentsItemService _attachmentsItemService;
        private readonly BaseService<AccountManage> _accountManageService = new BaseService<AccountManage>();
        private ISysWechatConfigService _SysWechatConfigService;
        private IWechatMPUserService _WechatMPUserService;
        private IWeChatUserRequestMessageLogHandler _WeChatUserRequestMessageLogHandler;
        private IMessageLogService _MessageLogService;
        private IAddressBookService _addressBookService;
        IArticleInfoService _objServiceArticle;
        public MessageController(IArticleInfoService objService, int appId)
            : base(objService)
        {
            AppId = appId;
            ViewBag.AppId = AppId;
        }

        public MessageController(
            IArticleInfoService objServiceArticle,
            IAttachmentsItemService attachmentsItemService,
            ISysWechatConfigService SysWechatConfigService,
                  IWechatMPUserService WechatMPUserService,
                  IMessageLogService messageLogService,
                  IAddressBookService addressBookService,
            IWeChatUserRequestMessageLogHandler WeChatUserRequestMessageLogHandler)
            : base(objServiceArticle)
        {
            // _objService = objService;
            //_messageService = objService;
            _objServiceArticle = objServiceArticle;
            _attachmentsItemService = attachmentsItemService;
            _WechatMPUserService = WechatMPUserService;
            _SysWechatConfigService = SysWechatConfigService;
            _WeChatUserRequestMessageLogHandler = WeChatUserRequestMessageLogHandler;
            _MessageLogService = messageLogService;
            _addressBookService = addressBookService;
            //   _previewMessageLogService = previewMessageLogService;
            //  AppId = (int)CategoryType.Undefined;
            ViewBag.AppId = AppId;
            ViewBag.KeywordTypes = new Dictionary<string, string>() {
                {WechatMessageLogType.file.ToString(), "文件"},
                {WechatMessageLogType.image.ToString(), "图片"},
                {WechatMessageLogType.news.ToString(), "图文"},
                {WechatMessageLogType.text.ToString(), "文本"},
                {WechatMessageLogType.video.ToString(), "视频"},
                {WechatMessageLogType.voice.ToString(), "语音"} };
        }

        [HttpGet, DataSecurityFilter]
        public ActionResult Edit(string id, int appId)
        {
            ViewBag.AppId = appId;
            AppId = appId;

            PrepareEditData();
            var obj = GetObject(id);

            if (!string.IsNullOrEmpty(id))
            {
                base.AppDataPermissionCheck(permission, appId, obj.AppId.GetValueOrDefault());
            }

            return View("../Message/Edit", obj);
        }

        [HttpGet]
        public ActionResult EditNews(string id, int appId)
        {
            var config = WeChatCommonService.GetWeChatConfigByID(appId);
            if (null != config && !string.IsNullOrEmpty(config.CoverUrl))
            {
                ViewBag.AppImg = config.CoverUrl;
            }
            else
            {
                ViewBag.AppImg = "/Content/picture/AccountLogo0.jpg";
                int AccountManageId = 0;
                if (int.TryParse(Request.Cookies["AccountManageId"].Value, out AccountManageId))
                {
                    ViewBag.AppImg = _accountManageService.Repository.GetByKey(AccountManageId).AccountLogo;
                }
                ViewBag.AppImg = _accountManageService.Repository.GetByKey(AccountManageId).AccountLogo;
            }
            return View("../Message/EditNews");
        }

        [HttpPost]
        public ActionResult SendNews(List<NewsInfoView> newsList, bool isPreview = false)
        {
            int IsSec = 0;
            try
            {
                var news = newsList[0];
                if ("video".Equals(news.NewsCate))
                {
                    news.ImageContent = SaveBase64ImageToServer(news.ImageContent);
                }
                IsSec = news.isSecurityPost.HasValue && news.isSecurityPost.Value ? 1 : 0;
                var objConfig = WeChatCommonService.GetWeChatConfigByID(news.AppId);
                string strToken = (objConfig.IsCorp != null && !objConfig.IsCorp.Value) ? Innocellence.Weixin.MP.CommonAPIs.AccessTokenContainer.GetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret) : Innocellence.Weixin.QY.CommonAPIs.AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);
                string strUser = news.SendToPerson == null ? null : string.Join("|", news.SendToPerson);
                string strDept = news.SendToGroup == null ? null : string.Join("|", news.SendToGroup);
                string strTags = news.SendToTag == null ? null : string.Join("|", news.SendToTag);

                // var configs = Infrastructure.Web.Domain.Service.CommonService.lstSysConfig;
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Suppress,
                    new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    MassResult result = null;
                    var lstArticles = GetArticleList(newsList, isPreview);
                    if (!isPreview)
                    {
                        InsertMsgLog(string.Join(",", lstArticles.Select(a => a.Id)), newsList[0]);
                    }
                    if (news.PostType == (int)MessagePostTypeEnum.定时推送)
                    {
                        transactionScope.Complete();
                    }
                    else
                    {
                        result = WechatCommon.SendMsgQY(news.AppId, news.NewsCate == "news" && IsSec == 1 ? "mpnews" : news.NewsCate, strUser, strDept, strTags, "", lstArticles, IsSec, isPreview);
                        if (result.errcode == Weixin.ReturnCode_QY.请求成功)
                        {
                            transactionScope.Complete();
                        }
                    }
                }
                //if (pp != null)
                //{
                //    _attachmentsItemService.ThumbImageAndInsertIntoDB(pp);
                //}

            }
            catch (Exception e)
            {
                _Logger.Error(e, "An error occurred while sending news.");
                return Json(new { results = new { Data = 500 } });
            }
            return Json(new { results = new { Data = 200 } });
        }

        private void InsertMsgLog(string newsIdList, NewsInfoView firstNews)
        {
            try
            {
                if (!string.IsNullOrEmpty(newsIdList) && firstNews != null)
                {
                    var msgLogView = new MessageLogView()
                    {
                        NewsIdList = newsIdList,
                        AppId = firstNews.AppId,
                        MsgContentType = (int)Enum.Parse(typeof(WechatMessageLogType), firstNews.NewsCate),
                    };
                    this._MessageLogService.InsertView(msgLogView);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("an error occured when add send msg log :{0}", ex);
            }
        }

        private string SaveBase64ImageToServer(string imageContent)
        {
            string filePath = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(imageContent))
                {
                    string startStr = "data:image/png;base64,";
                    if (imageContent.StartsWith(startStr))
                    {
                        string temp;
                        filePath = WeChatUserRequestMessageLogHandler.GetFilePath(".png", out temp);
                        byte[] arr = Convert.FromBase64String(imageContent.Replace(startStr, ""));
                        MemoryStream ms = new MemoryStream(arr);
                        Bitmap bmp = new Bitmap(ms);
                        bmp.Save(filePath);
                        filePath = temp.Trim('/');
                    }
                    else
                    {
                        filePath = imageContent;
                    }
                }
                _Logger.Debug("base64 image file path :{0}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _Logger.Error("Base64StringToImage 转换失败\nException：" + ex.Message);
            }
            return filePath;
        }

        private List<ArticleInfoView> GetArticleList(List<NewsInfoView> newsList, bool isPreview, string strSendType = null, SearchUserMPView searchCondition = null, bool toDB = true)
        {
            List<ArticleInfoView> lstArticles = new List<ArticleInfoView>();

            int i = 0;

            var codeArt = Guid.NewGuid();

            newsList.ForEach(a => { a.Id = i++; a.NewsCode = codeArt.ToString(); });
            var content = JsonConvert.SerializeObject(newsList);


            foreach (var a in newsList)
            {
                var entity = a.ConvertToEntityArticle();
                entity.NewsInfo = a;
                // entity.UserName = User.Identity.Name;
                if (a.NewsCate.Equals("news", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (a.materialId.HasValue && a.materialId.Value > 0)
                    {
                        entity.Id = a.materialId.Value;
                    }
                    entity.SecurityLevel = a.SecurityLevel;
                    if (!string.IsNullOrEmpty(strSendType))
                    {
                        switch (strSendType)
                        {
                            case "ToAll":
                            case "ByTag":
                                entity.ToTag = entity.Group.Value.ToString();
                                break;
                            case "ByOpenId":
                                string[] userOpenIds = _WechatMPUserService.GetUserBySearchCondition(searchCondition, AccountManageID).Select(u => u.OpenId).ToArray();
                                entity.ToUser = string.Join(",", userOpenIds);
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (a.PostType == (int)MessagePostTypeEnum.定时推送)
                {
                    entity.ScheduleSendTime = DateTime.Parse(a.ScheduleSendTime.Date)
                        .AddHours(a.ScheduleSendTime.Hour)
                        .AddMinutes(a.ScheduleSendTime.Minute);
                    var canPass = false;
                    if (entity.ScheduleSendTime.HasValue)
                    {
                        var now = DateTime.Now;
                        TimeSpan ts = entity.ScheduleSendTime.Value - now;
                        canPass = ts.Minutes >= 30;
                    }
                    if (!canPass)
                    {
                        throw new Exception("定时推送时间必须比当前时间延后30分钟!");
                    }
                }
                entity.ArticleURL = a.ArticleURL;
                entity.ArticleContentEdit = content;
                entity.ArticleType = 1;  //标记是消息
                entity.IsLike = a.IsLike;
                entity.ShowComment = a.ShowComment;
                entity.ShowReadCount = a.ShowReadCount;
                entity.IsWatermark = a.IsWatermark;
                entity.NoShare = a.NoShare;
                entity.OrderID = a.Id;
                entity.ArticleCode = codeArt;

                if (a.PostType != (int)MessagePostTypeEnum.定时推送 && !isPreview)
                {
                    entity.ArticleStatus = "Published";
                    entity.PublishDate = DateTime.Now;
                }
                else
                {
                    if (isPreview)
                    {
                        entity.PreviewStartDate = DateTime.Now;
                    }
                    entity.ArticleStatus = "Saved";
                }

                if (toDB)
                {
                    if (entity.Id == 0)
                    {
                        _objServiceArticle.InsertView(entity);
                    }
                    else
                    {
                        //通过选择素材进行的图文类型消息均更新到图文列表中，而不需要出现在消息列表中
                        if (a.NewsCate.Equals("news", StringComparison.CurrentCultureIgnoreCase))
                        {
                            entity.ArticleType = 0;
                        }
                        _objServiceArticle.UpdateView(entity);
                    }
                }
                lstArticles.Add(entity);
            }

            return lstArticles;
        }

        [HttpGet]
        public ActionResult GetNews(int id, int subId, string code)
        {
            if (string.IsNullOrEmpty(code) || code == "null" || code == "0")
            {
                //var model = _objServiceArticle.Repository.Entities.Where(a => a.Id == id && a.ContentType == (int)WechatMessageLogType.news).FirstOrDefault();
                var model = _objServiceArticle.Repository.Entities.Where(a => a.Id == id).FirstOrDefault();
                if (model != null)
                {
                    //var list = JsonConvert.DeserializeObject<List<NewsInfoView>>(model.ArticleContentEdit);
                    //ViewBag.Content = list.Where(a => a.Id == subId).First();
                    if (!string.IsNullOrEmpty(model.ArticleURL))
                    {
                        return Redirect(model.ArticleURL);
                    }
                    else
                    {
                        return View(model);
                    }
                }
            }
            else
            {
                Guid ACode = Guid.Parse(code);
                var model = _objServiceArticle.Repository.Entities.Where(a => a.ArticleCode == ACode && a.OrderID == subId && a.ContentType == (int)WechatMessageLogType.news).FirstOrDefault();
                if (model != null)
                {
                    //var list = JsonConvert.DeserializeObject<List<NewsInfoView>>(model.ArticleContentEdit);
                    //ViewBag.Content = list.Where(a => a.Id == subId).First();
                    if (!string.IsNullOrEmpty(model.ArticleURL))
                    {
                        return Redirect(model.ArticleURL);
                    }
                    else
                    {
                        return View(model);
                    }
                }
            }

            return null;
        }

        public virtual void PrepareEditData()
        {
            string accessToken = WeChatCommonService.GetWeiXinToken(AppId);
            //修改tag数据源 先根据AppId获取应用信息 allowTag Pending
            var app = AppApi.GetAppInfo(accessToken, AppId);
            GetAppInfo_AllowTags allowTags = app.allow_tags;

            var tagList = MailListApi.GetTagList(accessToken).taglist;//.Where(x=> allowTags.tagid.Contains(int.Parse(x.tagid))).ToList()

            ViewBag.taglist = tagList;
        }

        public JsonResult GetSubDepartment(string id, string departlist)
        {
            string accessToken = WeChatCommonService.GetWeiXinToken(AppId);
            //修改Department数据源 先根据AppId获取应用信息 allowPartys  Pending

            var config = WeChatCommonService.GetWeChatConfigByID(AppId);

            var app = AppApi.GetAppInfo(accessToken, int.Parse(config.WeixinAppId));
            GetAppInfo_AllowPartys allowPartys = app.allow_partys;

            // TODO: async/await执行较长等待的task
            var subdepartList = MailListApi.GetDepartmentList(accessToken, Int32.Parse(id)).department;//.Where(x => allowPartys.partyid.Contains(x.id)).ToList()

            var listReturn = EasyUITreeData.GetTreeData(subdepartList, "id", "name", "parentid");

            listReturn.ForEach(a =>
            {
                a.state = "closed";
            });

            if (!string.IsNullOrEmpty(departlist))
            {
                var departids = departlist.Split('|');
                EasyUITreeData.SetChecked<string>(departids.ToList(), listReturn);
            }

            return Json(listReturn, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckSendObjects(int appId, string partyids, string tagids, string userids)
        {
            CheckResult checkResult;
            var selectedDepartmentIds = string.IsNullOrEmpty(partyids) ? new List<int>() : partyids.Trim().Split('|').Select(x => int.Parse(x)).ToList();
            var selectedTagIds = string.IsNullOrEmpty(tagids) ? new List<int>() : tagids.Trim().Split('|').Select(x => int.Parse(x)).ToList();
            var selectedWeChatUserIDs = string.IsNullOrEmpty(userids) ? new List<string>() : userids.Trim().Split('|').ToList();

            var isPass = _objServiceArticle.CheckMessagePushRule(appId, AccountManageID, selectedDepartmentIds, selectedTagIds, selectedWeChatUserIDs, out checkResult);
            return Json(new { isPass = isPass, errorinfo = checkResult }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, DataSecurityFilter]
        public virtual ActionResult Index(int appId)
        {
            var appInfo = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.Id == appId);
            if (!appInfo.IsCorp.Value)
            {
                ViewBag.KeywordTypes.Remove(WechatMessageLogType.file.ToString());
            }
            ViewBag.AppId = appId;
            return View("../Message/Index");
        }

        public override List<ArticleInfoView> GetListEx(Expression<Func<ArticleInfo, bool>> predicate, PageCondition ConPage)
        {
            string strTitle = Request["txtArticleTitle"];
            string txtDate = Request["txtDate"];
            int appId = int.Parse(Request["APPID"]);
            var q = GetListPrivate(ref predicate, ConPage, strTitle, txtDate, appId);

            return q.ToList();
        }

        //BackEnd校验
        public override bool BeforeAddOrUpdate(ArticleInfoView objModal, string Id)
        {
            bool validate = true;
            StringBuilder errMsg = new StringBuilder();
            if (objModal.ThumbImageId == null)
            {
                validate = false;
                errMsg.Append(T("Please upload image for push in wechat.<br/>"));
            }

            if (string.IsNullOrWhiteSpace(objModal.ArticleURL))
            {
                if (string.IsNullOrWhiteSpace(objModal.ArticleContent))
                {
                    validate = false;
                    errMsg.Append(T("Please input message content or url.<br/>"));
                }
            }

            if (string.IsNullOrWhiteSpace(objModal.ArticleTitle))
            {
                validate = false;
                errMsg.Append(T("Please input message title.<br/>"));
            }

            if (!validate)
            {
                ModelState.AddModelError("Invalid Input", errMsg.ToString());
            }

            return validate;
        }

        //Post方法
        [HttpPost]
        [ValidateInput(false)]
        public override JsonResult Post(ArticleInfoView objModal, string Id)
        {
            AppId = objModal.AppId.GetValueOrDefault();

            //trim.防止用户出现操作失误
            if (objModal.Previewers != null)
            {
                objModal.Previewers = objModal.Previewers.Trim();
            }

            if (string.IsNullOrEmpty(Id) && objModal.Id != 0)
            {
                Id = objModal.Id.ToString();
            }

            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            InsertOrUpdate(objModal, Id);

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public virtual void InsertOrUpdate(ArticleInfoView objModal, string Id)
        {
            //objModal.AppId = AppId;
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                _BaseService.InsertView(objModal);
            }
            else
            {
                var lst = new List<string>(){
                  "Title","Content","Comment","URL",
                  "Previewers","toDepartment","toTag","toUser","IsLike"};

                _BaseService.UpdateView(objModal, lst);
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="Id">消息ID</param>
        /// <param name="ispush">是否推送到前台</param>
        /// <returns></returns>
        public virtual JsonResult ChangeStatus(string Id, int appid, bool ispush)
        {
            int iRet = 0;

            var objModel = _BaseService.Repository.GetByKey(int.Parse(Id));

            if (objModel.ArticleStatus == null || objModel.ArticleStatus == ConstData.STATUS_NEW)
            {
                objModel.ArticleStatus = ConstData.STATUS_PUBLISH;
                objModel.PublishDate = DateTime.Now;

                //向前台推送提醒信息
                ArticleInfoView aiv = (ArticleInfoView)new ArticleInfoView().ConvertAPIModel(objModel);

                if (ispush)
                {
                    //var article = new ArticleInfoView()
                    //{

                    //    ArticleTitle = objModel.ArticleTitle,
                    //    ImageCoverUrl = aiv.ThumbImageUrl,

                    //    //Description = objModel.Comment,
                    //    //Url = string.Format("{0}/Message/WxDetailh5/{1}", wechatBaseUrl, objModel.Id)
                    //};

                    WechatCommon.PublishMessage(objModel.AppId.GetValueOrDefault(), new List<ArticleInfoView>() { aiv }, aiv.ToUser, aiv.ToDepartment, aiv.ToTag);
                }
            }
            else
            {
                objModel.ArticleStatus = ConstData.STATUS_NEW;
                objModel.PublishDate = null;
            }

            iRet = _BaseService.Repository.Update(objModel, new List<string>() { "Status", "PublishDate" });

            if (iRet > 0)
            {
                return SuccessNotification("Success");
            }
            else
            {
                return ErrorNotification("Change Status Falid");
            }

        }

        public JsonResult GetPublishHistory()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual JsonResult WxPreview(ArticleInfoView objModal)
        {
            //从前台form里拿数据
            string Id = objModal.Id.ToString();

            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            InsertOrUpdate(objModal, Id);

            //向前台推送提醒信息
            var article = new ArticleInfoView()
            {
                //Title = "[预览]" + objModal.Title,
                //PicUrl = string.Format("{0}/Common/PushFile?id={1}&FileName={2}", wechatBaseUrl, objModal.ThumbImageId, objModal.ThumbImageUrl),
                //Description = objModal.Comment,
                //Url = string.Format("{0}/Message/WxPreviewh5/{1}", wechatBaseUrl, objModal.Id)
            };


            //为了fix掉新建文章预览 微信接口报错后 再save 出现2条数据
            try
            {

                WechatCommon.PublishMessage(objModal.AppId.GetValueOrDefault(), new List<ArticleInfoView>() { article }, objModal.Previewers, "", "");
            }

            catch (Exception ex)
            {
                return SuccessNotification(string.Format("{0};{1}", objModal.Id, ex.Message));
            }

            return SuccessNotification(string.Format("{0}", objModal.Id));
        }

        protected List<ArticleInfoView> GetListPrivate(ref Expression<Func<ArticleInfo, bool>> predicate, PageCondition ConPage, string strTitle, string txtDate, int appId)
        {
            predicate = predicate.AndAlso(a => a.IsDeleted == false && a.ArticleType == 1 && a.AppId == appId);

            if (!string.IsNullOrEmpty(strTitle))
            {
                predicate = predicate.AndAlso(a => a.ArticleTitle.Contains(strTitle));
            }

            if (!string.IsNullOrEmpty(txtDate))
            {
                DateTime dateTime = Convert.ToDateTime(txtDate);
                DateTime dateAdd = dateTime.AddDays(1);
                predicate = predicate.AndAlso(a => a.PublishDate >= dateTime && a.PublishDate <= dateAdd);
            }


            string TXTTYPE = Request["TXTTYPE"];


            if (!string.IsNullOrEmpty(TXTTYPE))
            {
                var contentType = (int)Enum.Parse(typeof(WechatMessageLogType), TXTTYPE, true);
                predicate = predicate.AndAlso(a => a.ContentType == contentType);
            }



            //TODO:
            //predicate = predicate.AndAlso(a => a.AppId == AppId);

            var q = _BaseService.GetList<ArticleInfoView>(predicate, ConPage);
            return q;
        }

        public JsonResult GetItem(int id)
        {
            var item = _objServiceArticle.GetById<ArticleInfoView>(id);

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public override ActionResult Export()
        {
            string strType = Request["txtType"];
            string txtDate = Request["txtDate"];

            int appid;
            if (int.TryParse(Request["appId"], out appid))
            {
                AppId = appid;
            }

            return ExportCSV(strType, txtDate);
        }

        /// <summary>
        /// CSV文件出力
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportCSV(string type, string pulishDate)
        {

            Expression<Func<ArticleInfo, bool>> predicate = x => x.AppId == AppId && x.ArticleType == 1 && x.IsDeleted == false;

            if (!string.IsNullOrEmpty(type))
            {
                var contentType = (int)Enum.Parse(typeof(WechatMessageLogType), type, true);
                predicate = predicate.AndAlso(a => a.ContentType == contentType);
            }

            if (!string.IsNullOrEmpty(pulishDate))
            {
                DateTime condDate = Convert.ToDateTime(pulishDate);
                DateTime condDateEnd = condDate.AddDays(1);
                predicate = predicate.AndAlso(x => x.CreatedDate >= condDate && x.CreatedDate <= condDateEnd);
            }

            // APP列表
            //var appInfo = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.WeixinAppId == AppId.ToString());

            List<ArticleInfoView> reportList = this._objServiceArticle.GetList<ArticleInfoView>(predicate).OrderByDescending(x => x.PublishDate).ThenByDescending(x => x.Id).ToList();
            reportList.ForEach(a =>
            {
                switch (a.ContentType)
                {
                    case 1: //'text':
                        a.ContentTypeName = "文本";
                        break;
                    case 2://'news'
                        a.ContentTypeName = "图文";
                        break;
                    case 4:// 'file'
                        a.ContentTypeName = "文件";
                        break;
                    case 3://'image':
                        a.ContentTypeName = "图片";
                        break;
                    case 5://'video':
                        a.ContentTypeName = "视频";
                        break;
                    case 6://'voice':
                        a.ContentTypeName = "语音";
                        break;
                }
            });

            return exportToCSV(reportList);
        }

        private ActionResult exportToCSV(List<ArticleInfoView> wechatFollowReportList)
        {
            string[] headLine = { "CreatedUserID", "ContentTypeName", "PublishDate" };
            // wechatFollowReportList.ForEach(a => a.Type = ViewBag.KeywordTypes[a.Type]);
            CsvSerializer<ArticleInfoView> csv = new CsvSerializer<ArticleInfoView>();
            csv.UseLineNumbers = false;
            var sRet = csv.SerializeStream(wechatFollowReportList, headLine);

            string fileHeadName = ((CategoryType)AppId).ToString();

            string fileName = fileHeadName + "_Message_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            return File(sRet, "text/comma-separated-values", fileName);
        }

        public ActionResult WechatServiceMessage()
        {
            var config = WeChatCommonService.GetWeChatConfigByID(AppId);
            if (null != config && !string.IsNullOrEmpty(config.CoverUrl))
            {
                ViewBag.AppImg = config.CoverUrl;
            }
            return View();
        }

        [HttpPost]
        public ActionResult WechatServiceSendMessage(List<NewsInfoView> newsList, bool isPreview = false)
        {
            SysWechatConfig wechat = WeChatCommonService.lstSysWeChatConfig.Find(p => p.AccountManageId == AccountManageID && !p.IsCorp.Value);
            if (wechat == null)
            {
                return Json(new { results = new { Data = 500 } });
            }
            try
            {
                NewsInfoView news = newsList[0];
                if ("video".Equals(news.NewsCate))
                {
                    news.ImageContent = SaveBase64ImageToServer(news.ImageContent);
                }
                string strSendType = string.Empty;
                SearchUserMPView searchCondition = new SearchUserMPView
                {
                    Group = news.Group,
                    Province = news.Province,
                    City = news.City,
                    Sex = news.Sex
                };
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    SendResult returnResult = null;
                    List<NewsModel> articles = new List<NewsModel>();
                    var lstArticles = GetArticleList(newsList, isPreview, strSendType, searchCondition);
                    if (!isPreview && news.PostType == (int)MessagePostTypeEnum.定时推送)
                    {
                        transactionScope.Complete();
                    }
                    else
                    {
                        string[] openids = null;

                        if (isPreview)
                        {
                            openids = news.SendToPerson;
                        }
                        //发送消息
                        returnResult = WechatCommonMP.SendMsgMP(lstArticles, searchCondition, openids);
                        if (returnResult.errcode == Weixin.ReturnCode.请求成功)
                        {
                            transactionScope.Complete();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.Error(e, "An error occurred while sending mp news.");
                // _Logger.Error(e.ToString());
                return Json(new { results = new { Data = 500 } });
            }
            return Json(new { results = new { Data = 200 } });
        }

        //private SendResult MPSendMessage(string SendType, SysWechatConfig wechat, string groupId, string value, SearchUserMPView searchCondition, Weixin.MP.GroupMessageType type, string[] openids)
        //{
        //    SendResult returnResult = null;

        //    if (openids != null)
        //    {
        //        foreach (var openid in openids)
        //        {
        //            returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessagePreview(wechat.WeixinAppId, wechat.WeixinCorpSecret, type, value, openid);
        //        }
        //        return returnResult;
        //    }

        //    switch (SendType)
        //    {
        //        case "ToAll":
        //            returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessageByGroupId(wechat.WeixinAppId, wechat.WeixinCorpSecret, groupId, value, type, true, 1000);
        //            break;
        //        case "ByTag":
        //            returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessageByGroupId(wechat.WeixinAppId, wechat.WeixinCorpSecret, groupId, value, type, false, 1000);
        //            break;
        //        case "ByOpenId":
        //            string[] userOpenIds = _WechatMPUserService.GetUserBySearchCondition(searchCondition, AccountManageID).Select(u => u.OpenId).ToArray();
        //            returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessageByOpenId(wechat.WeixinAppId, wechat.WeixinCorpSecret, type, value, 1000, userOpenIds);
        //            break;
        //    }
        //    return returnResult;

        //}

        [HttpPost]
        public ActionResult PostQuickResponse(string openId, List<NewsInfoView> newsList)
        {
            try
            {
                var news = newsList[0];
                if ("video".Equals(news.NewsCate))
                {
                    news.ImageContent = SaveBase64ImageToServer(news.ImageContent);
                }
                SysWechatConfig wechat = WeChatCommonService.lstSysWeChatConfig.Find(p => p.Id == news.AppId && !p.IsCorp.Value);
                if (wechat == null)
                {
                    return Json(new { results = new { Data = 500 } });
                }
              ResponseMessageBase response = null;

                var lstArticles = GetArticleList(newsList, false, news.NewsCate, null, false);

                //发送消息
                WechatCommonMP.SendMsg(news.NewsCate, "", lstArticles, "", new string[] { openId }, null, true);

                switch (news.NewsCate)
                {
                    case "text":
                        response = new ResponseMessageText() { Content = news.NewsContent };
                        //returnResult = CustomApi.SendText(wechat.WeixinAppId, wechat.WeixinCorpSecret, openId, news.NewsContent);
                        break;
                    case "news":
                        //var lstArticles = GetArticleList(newsList, false);
                        //List<NewsModel> articles = new List<NewsModel>();
                        //string host = CommonService.GetSysConfig("Content Server", "");
                        //if (host.EndsWith("/"))
                        //{
                        //    host = host.Substring(0, host.Length - 1);
                        //}
                        //for (int i = 0; i < lstArticles.Count; i++)
                        //{
                        //    var item = lstArticles[i];
                        //    var filePath = Server.MapPath("~/") + item.ImageCoverUrl.Insert(item.ImageCoverUrl.LastIndexOf('.'), "_T");
                        //    if (0 == i)
                        //    {
                        //        filePath = filePath.Replace("_T", "_B");
                        //    }
                        //    var ret0 = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(wechat.WeixinAppId, wechat.WeixinCorpSecret, Weixin.MP.UploadMediaFileType.thumb, filePath);
                        //    var picUrl = (host + item.ImageCoverUrl).Replace("\\", "/");
                        //    var url = string.Format("{0}/MPNews/ArticleInfo/WxDetail/{1}?wechatid={2}&isPreview={3}", host, item.Id, item.AppId, false);

                        //    articles.Add(new NewsModel() { title = item.ArticleTitle, content = WechatCommonMP.ImageConvert(item.ArticleContent, wechat.Id), thumb_url = picUrl, thumb_media_id = ret0.thumb_media_id, content_source_url = url });
                        //}
                        //var ret = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryNews(wechat.WeixinAppId, wechat.WeixinCorpSecret, 10000, articles.ToArray());
                        //returnResult = CustomApi.SendMpNews(wechat.WeixinAppId, wechat.WeixinCorpSecret, openId, ret.media_id);
                        string _newsHost = Infrastructure.Web.Domain.Service.CommonService.GetSysConfig("WeChatUrl", "");
                        response = new ResponseMessageNews()
                        {
                            Articles = lstArticles.Select(a => new Article()
                            {
                                Title = a.ArticleTitle,
                                Url = string.Format("{0}/{3}/ArticleInfo/wxdetail/{1}?wechatid={2}&isAutoReply={4}", _newsHost, a.Id, news.AppId, "mpnews", 0),// _newsHost + "/News/ArticleInfo/wxdetail/" + a.Id + "?wechatid=" + appId,
                                PicUrl = _newsHost + a.ImageCoverUrl,
                                Description = a.ArticleComment
                            }).ToList()
                        };
                        break;
                    case "image":
                        //WechatCommon.GetMediaInfo(AutoReplyContentEnum.IMAGE, news, news.AppId);
                        //returnResult = CustomApi.SendImage(wechat.WeixinAppId, wechat.WeixinCorpSecret, openId, news.MediaId);
                        response = new ResponseMessageImage() { Image = new Weixin.Entities.Image() { MediaId = news.MediaId } };
                        break;
                    case "video":
                        //WechatCommon.GetMediaInfo(AutoReplyContentEnum.VIDEO, news, news.AppId);
                        ////var ret1 = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.GetOpenIdVideoMediaIdResult(wechat.WeixinAppId, wechat.WeixinCorpSecret, news.MediaId, news.NewsTitle, news.NewsComment);
                        ////news.MediaId = ret1.media_id;
                        ////news.MediaCreateTime = ret1.created_at;
                        //returnResult = CustomApi.SendVideo(wechat.WeixinAppId, wechat.WeixinCorpSecret, openId, news.MediaId, news.NewsTitle, news.NewsComment);
                        response = new ResponseMessageVideo() { Video = new Video() { MediaId = news.MediaId } };
                        break;
                    case "voice":
                        //WechatCommon.GetMediaInfo(AutoReplyContentEnum.VOICE, news, news.AppId);
                        //returnResult = CustomApi.SendVoice(wechat.WeixinAppId, wechat.WeixinCorpSecret, openId, news.MediaId);
                        response = new ResponseMessageVoice() { Voice = new Voice() { MediaId = news.MediaId } };
                        break;
                    default:
                        throw new Exception("Invalid media type.");
                }
                //var user = _WechatMPUserService.GetUserByOpenId(openId);
                response.ToUserName = openId;
                var task = _WeChatUserRequestMessageLogHandler.WriteResponseLogMP(new List<Innocellence.Weixin.Entities.IResponseMessageBase> { response }, news.AppId.ToString(), false);
                return Json(new { results = new { Data = 200, Object = task.Result } });
            }
            catch (Exception e)
            {
                _Logger.Error(e, "An error occurred while PostQuickResponse.");
                return Json(new { results = new { Data = 500, Message = e.Message } });
            }


        }
    }
}
