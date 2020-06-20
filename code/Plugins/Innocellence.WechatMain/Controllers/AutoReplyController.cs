using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Web;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Extensions;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.Domain.Service.Common;
using Infrastructure.Web.UI;
using Innocellence.Authentication.Authentication;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.Weixin.QY;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.CommonAPIs;
using WebGrease.Css.Ast.Selectors;
using Innocellence.WeChat.Domain.ViewModel;
using Newtonsoft.Json;
using Innocellence.Weixin.MP.AdvancedAPIs.UserTag;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Autofac;
using Innocellence.WeChat.Domain.Service;
using Innocellence.Weixin.QY.AdvancedAPIs.Media;
using Innocellence.Weixin.Exceptions;
using Innocellence.WeChatMain.Common;
using Innocellence.Weixin.Helpers;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChatMain.Controllers
{
    public partial class AutoReplyController : BaseController<AutoReply, AutoReplyView>
    {
        private readonly IAutoReplyService _autoReplyService;
        private readonly IArticleInfoService _articleInfoService;
        private readonly IAttachmentsItemService _fileManageService;
        private readonly IAttachmentsItemService _attachmentsItemService;
        private readonly IWechatUserRequestMessageTagService _messageTagService;
        private readonly ISystemUserTagService _systemUserTagService;
        private static ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(SystemUserTagService)));
        public AutoReplyController(IAutoReplyService objService,
            IArticleInfoService articleInfoService,
            IAttachmentsItemService fileManageService,
            IAttachmentsItemService attachmentsItemService,
            IWechatUserRequestMessageTagService messageTagService,
            ISystemUserTagService systemUserTagService)
            : base(objService)
        {
            _autoReplyService = objService;
            _articleInfoService = articleInfoService;
            _fileManageService = fileManageService;
            _attachmentsItemService = attachmentsItemService;
            _messageTagService = messageTagService;
            _systemUserTagService = systemUserTagService;
            AppId = (int)CategoryType.Undefined;
            ViewBag.AppId = AppId;
        }

        public List<SystemUserTag> lstUserTag
        {
            get
            {
                //var lst = cacheManager.Get<List<SystemUserTag>>("UserTagList", () =>
                //{
                //    ISystemUserTagService systemUserTagService = new SystemUserTagService();
                //    var userTagList = systemUserTagService.Repository.Entities.Where(a => a.ParentId != null && a.IsDeleted == false && a.AccountManageId == this.AccountManageID).ToList();
                //    return userTagList;

                //});
                //return lst;
                ISystemUserTagService systemUserTagService = new SystemUserTagService();
                var userTagList = systemUserTagService.Repository.Entities.Where(a => a.ParentId != null && a.IsDeleted == false && a.AccountManageId == this.AccountManageID).ToList();
                var parentList = systemUserTagService.Repository.Entities.Where(a => a.ParentId == null && a.IsDeleted == false && a.AccountManageId == this.AccountManageID).ToList();
                foreach (var userTagItem in userTagList)
                {
                    var dd = parentList.Find(a => a.Id == userTagItem.ParentId);
                    userTagItem.Name = (dd == null ? "" : dd.Name) + " - " + userTagItem.Name;
                }
                return userTagList.OrderBy(a => a.Name).ToList();
            }
        }

        /// <summary>
        /// 初始化list页面
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet, DataSecurityFilter]
        public virtual ActionResult Index(int appId)
        {
            ViewBag.AppId = appId;
            var wechatConfig = WeChatCommonService.GetWeChatConfigByID(appId);
            if (wechatConfig.IsCorp.Value)
            {
                ViewBag.IsCorp = 1;
                ViewBag.KeywordTypes = typeof(AutoReplyKeywordEnum).GetAllItems();
            }
            else
            {
                ViewBag.IsCorp = 0;
                var mpList = typeof(AutoReplyMPKeywordEnum).GetAllItems();
                //mpList.RemoveAll(e => e.Value == (int)AutoReplyMPKeywordEnum.EnterEvent);
                ViewBag.KeywordTypes = mpList;
            }
            return base.Index();
        }

        /// <summary>
        /// 取得List数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="ConPage"></param>
        /// <returns></returns>
        public override List<AutoReplyView> GetListEx(Expression<Func<AutoReply, bool>> predicate, PageCondition ConPage)
        {
            // TODO: NEED CHECK APP ID
            int appId = int.Parse(Request["appid"]);
            string type = Request["type"];
            string name = Request["name"];
            bool queryAll = false;
            if (bool.TryParse(Request["queryAll"], out queryAll))
            {
                if (queryAll)
                {
                    ConPage.PageSize = int.MaxValue;
                }
            }

            if (!string.IsNullOrEmpty(type))
            {
                int primaryType = int.Parse(type);
                predicate = predicate.AndAlso(x => x.PrimaryType == primaryType);
            }

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim().ToLower();
                predicate = predicate.AndAlso(x => x.Name.ToLower().Contains(name));
            }

            predicate = predicate.AndAlso(x => x.AppId == appId && x.IsDeleted == false);

            var list = _objService.GetList<AutoReplyView>(predicate, ConPage);

            // get type name;
            var wechatConfig = WeChatCommonService.GetWeChatConfigByID(appId);
            if (wechatConfig.IsCorp.Value)
            {
                ViewBag.IsCorp = 1;
                list.ForEach(x => x.KeywordTypeName = ((AutoReplyKeywordEnum)x.KeywordType).GetDescriptionByName());
            }
            else
            {
                list.ForEach(x => x.KeywordTypeName = ((AutoReplyMPKeywordEnum)x.KeywordType).GetDescriptionByName());
            }

            return list;
        }

        /// <summary>
        /// 初始化编辑页面
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet, DataSecurityFilter]
        public virtual ActionResult Edit(int appId)
        {
            var autoReplyId = Request["id"];

            var view = new AutoReplyView();

            // edit
            if (!string.IsNullOrEmpty(autoReplyId))
            {
                var id = int.Parse(autoReplyId);
                view = _autoReplyService.GetDetail(id);
            }

            var wechatConfig = WeChatCommonService.GetWeChatConfigByID(appId);
            if (wechatConfig.IsCorp.Value)
            {
                // 企业号口令类型
                ViewBag.KeywordTypes = typeof(AutoReplyKeywordEnum).GetAllItems().OrderBy(a => a.Value);
                ViewBag.IsCorp = 1;
                var tagList = WeChatCommonService.GetTagListByAccountManageID(this.AccountManageID);
                Dictionary<int, string> UserTagMap = new Dictionary<int, string>();
                tagList.ForEach(a => UserTagMap.Add(int.Parse(a.tagid), a.tagname));
                ViewBag.UserTagMap = UserTagMap;
            }
            else
            {
                // 服务号口令类型
                ViewBag.KeywordTypes = typeof(AutoReplyMPKeywordEnum).GetAllItems().OrderBy(a => a.Value);
                ViewBag.IsCorp = 0;
                var tagList = _messageTagService.Repository.Entities.Where(a => a.IsDeleted == false && a.AppID == appId).ToList();
                Dictionary<int, string> messageTagMap = new Dictionary<int, string>();
                tagList.ForEach(a => messageTagMap.Add(a.Id, a.TagName));
                ViewBag.MessageTagMap = messageTagMap;
                Dictionary<int, string> groupMap = new Dictionary<int, string>();
#if (DEBUG)
                {
                    groupMap.Add(1, "groupName1");
                    groupMap.Add(2, "groupName2");
                }
#else
                {
                    var groupResult = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.Get(wechatConfig.WeixinAppId, wechatConfig.WeixinCorpSecret);
                    groupResult.tags.ForEach(g => groupMap.Add(g.id, g.name));
                }
#endif
                ViewBag.GroupMap = groupMap;
                Dictionary<int, string> userTagMap = new Dictionary<int, string>();
                lstUserTag.ForEach(t => userTagMap.Add(t.Id, t.Name));
                ViewBag.UserTagMap = userTagMap;
            }
            // 文本匹配类型
            ViewBag.TextMatchTypes = typeof(AutoReplyTextMatchEnum).GetAllItems();
            // 菜单类型
            ViewBag.MenuTypes = typeof(AutoReplyMenuEnum).GetAllItems();
            // 回复类型
            ViewBag.ReplyTypes = typeof(AutoReplyContentEnum).GetAllItems();
            // 回复类型
            ViewBag.ReplyNewsTypes = typeof(AutoReplyNewsEnum).GetAllItems();
            ViewBag.AppId = appId;

            return View("../AutoReply/Edit", view);
        }

        [HttpGet]
        public JsonResult GetContent(int id)
        {
            var view = new AutoReplyNewView();
            view.Main = _autoReplyService.GetDetail(id);
            if (view.Main.Contents[0].IsNewContent.Value)
            {
                view.Send = JsonConvert.DeserializeObject<List<NewsInfoView>>(view.Main.Contents[0].Content);
            }
            else
            {
                if (view.Main.Contents[0].PrimaryType == (int)AutoReplyContentEnum.TEXT)
                {
                    view.Send = new List<NewsInfoView>() { new NewsInfoView() { NewsCate = "text", NewsContent = view.Main.Contents[0].Content, isSecurityPost = view.Main.Contents[0].IsEncrypt } };
                }
                else if ((view.Main.Contents[0].PrimaryType == (int)AutoReplyContentEnum.VIDEO ||
                    view.Main.Contents[0].PrimaryType == (int)AutoReplyContentEnum.VOICE ||
                    view.Main.Contents[0].PrimaryType == (int)AutoReplyContentEnum.IMAGE ||
                    view.Main.Contents[0].PrimaryType == (int)AutoReplyContentEnum.FILE) && view.Main.Contents[0].FileID.HasValue)
                {


                    var fileInfo = _fileManageService.GetById<AttachmentsItemView>(view.Main.Contents[0].FileID.Value);

                    view.Send = new List<NewsInfoView>() { new NewsInfoView() {
                        NewsCate =Enum.Parse(typeof(AutoReplyContentEnum), view.Main.Contents[0].PrimaryType.ToString()).ToString().ToLower(), NewsContent = "",
                         AppId=view.Main.AppId,  NewsTitle=fileInfo.AttachmentTitle, ImageContent="/"+fileInfo.ThumbUrl, VideoContent="/"+fileInfo.AttachmentUrl, FileSrc="/"+fileInfo.AttachmentUrl,
                          SoundSrc="/"+fileInfo.AttachmentUrl, MediaId=fileInfo.MediaId,  NewsComment=fileInfo.Description,isSecurityPost=view.Main.Contents[0].IsEncrypt,
                          RealFileName=fileInfo.AttachmentTitle


                    } };
                }
                else if (view.Main.Contents[0].PrimaryType == (int)AutoReplyContentEnum.NEWS && !string.IsNullOrEmpty(view.Main.Contents[0].NewsID))
                {
                    var ListArticle = _articleInfoService.GetListByNewsIDs(view.Main.Contents[0].NewsID);
                    string strCate = Enum.Parse(typeof(AutoReplyContentEnum), view.Main.Contents[0].PrimaryType.ToString()).ToString().ToLower();
                    ListArticle.ForEach(a => { a.NewsCate = strCate; a.isSecurityPost = view.Main.Contents[0].IsEncrypt; a.NewsID = view.Main.Contents[0].NewsID; });
                    view.Send = ListArticle;

                }
            }
            view.GetExtraContent(view.Main.Contents);
            return Json(view, JsonRequestBehavior.AllowGet);
        }

        #region old post logic
        /// <summary>
        /// 添加，编辑操作
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        //[HttpPost, DataSecurityFilter]
        //public virtual JsonResult EditPost(int appId)
        //{
        //    var autoReplyId = int.Parse(Request.Form.Get("id"));
        //    var wechatConfig = WeChatCommonService.GetWeChatConfigByID(appId);
        //    var autoReplyView = new AutoReplyView();
        //    autoReplyView.Id = autoReplyId;
        //    autoReplyView.Name = Request.Form.Get("name");
        //    autoReplyView.Description = Request.Form.Get("decription");
        //    autoReplyView.PrimaryType = int.Parse(Request.Form.Get("keywordType"));
        //    autoReplyView.AppId = int.Parse(Request.Form.Get("AppId"));
        //    if (!wechatConfig.IsCorp.Value)
        //    {
        //        List<int> checkedGroupIds = new List<int>();
        //        List<int> checkedUserTagIds = new List<int>();
        //        List<int> checkedMessageTagIds = new List<int>();
        //        string groupStr = Request.Form.Get("UserGroups");
        //        if (!string.IsNullOrEmpty(groupStr))
        //        {
        //            checkedGroupIds = groupStr.Split(",".ToCharArray()).Select(a => int.Parse(a)).ToList();
        //        }
        //        string userTagStr = Request.Form.Get("UserTags");
        //        if (!string.IsNullOrEmpty(userTagStr))
        //        {
        //            checkedUserTagIds = userTagStr.Split(",".ToCharArray()).Select(a => int.Parse(a)).ToList();
        //        }
        //        string messageTagStr = Request.Form.Get("MessageTags");
        //        if (!string.IsNullOrEmpty(messageTagStr))
        //        {
        //            checkedMessageTagIds = messageTagStr.Split(",".ToCharArray()).Select(a => int.Parse(a)).ToList();
        //        }
        //        autoReplyView.UserTags = checkedUserTagIds;
        //        autoReplyView.UserGroups = checkedGroupIds;
        //        autoReplyView.MessageTags = checkedMessageTagIds;
        //    }

        //    // 口令类型
        //    autoReplyView.Keywords = GetAutoReplySubTypes(autoReplyView);

        //    // 口令回复
        //    autoReplyView.Contents = GetAutoReplyList(autoReplyView);

        //    // 输入校验
        //    if (!BeforeAddOrUpdate(autoReplyView) || !ModelState.IsValid)
        //    {
        //        return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
        //    }

        //    // add
        //    if (autoReplyId == 0)
        //    {
        //        _autoReplyService.Add(autoReplyView);
        //    }
        //    // Edit
        //    else
        //    {
        //        _autoReplyService.Update(autoReplyView);
        //    }

        //    //return RedirectToAction("Index", new RouteValueDictionary { {"appId", appId} });
        //    return Json(doJson(null), JsonRequestBehavior.AllowGet);
        //}
        #endregion

        [HttpPost]
        public JsonResult EditPost(AutoReplyNewView model)
        {
            var wechatConfig = WeChatCommonService.GetWeChatConfigByID(model.Main.AppId);

            if (string.IsNullOrWhiteSpace(model.Main.Description))
            {
                model.Main.Description = string.Empty;
            }

            // 口令类型
            model.Main.Keywords = GetAutoReplySubTypes(model);

            // 口令回复
            model.Main.Contents = GetAutoReplyList(model);

            //if (!wechatConfig.IsCorp.Value)
            //{
            model.SetExtraContent(model.Main.Contents);
            //}

            // 输入校验
            //if (!BeforeAddOrUpdate(model) || !ModelState.IsValid)
            //{
            //    return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            //}

            // add
            if (model.Main.Id == 0)
            {
                _autoReplyService.Add(model.Main);
            }
            // Edit
            else
            {
                _autoReplyService.Update(model.Main);
            }

            //return RedirectToAction("Index", new RouteValueDictionary { {"appId", appId} });
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得图文消息一览
        /// </summary>
        /// <returns></returns>
        public ActionResult GetArticleList()
        {
            var gridRequest = new GridRequest(Request);
            var expression = FilterHelper.GetExpression<ArticleInfo>(gridRequest.FilterGroup);

            // TODO: how to add App ID to expression
            var appId = int.Parse(Request["AppId"]);
            expression = expression.AndAlso(x => x.AppId == appId);
            var rowCount = gridRequest.PageCondition.RowCount;
            var listEx = this.GetArticleListPrivate(expression, gridRequest.PageCondition);

            return base.GetPageResult(listEx, gridRequest);
        }

        /// <summary>
        /// 取得文件一览
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFileList()
        {
            var gridRequest = new GridRequest(Request);
            var predicate = FilterHelper.GetExpression<SysAttachmentsItem>(gridRequest.FilterGroup);
            predicate = predicate.AndAlso(x => x.IsDeleted == false);
            var fileType = int.Parse(Request["ReplyType"]);
            // 根据回复类型来按文件类型来检索
            // TODO: DB中保存的文件类型需要跟上传时的一致，目前来说，迁移数据是后缀名，但上传source中是文件的type
            switch (fileType)
            {
                case (int)AutoReplyContentEnum.IMAGE:
                    {
                        predicate = predicate.AndAlso(x => x.Type == (int)AttachmentsType.IMAGE);
                        break;
                    }
                case (int)AutoReplyContentEnum.VOICE:
                    {
                        predicate = predicate.AndAlso(x => x.Type == (int)AttachmentsType.AUDIO);
                        break;
                    }
                case (int)AutoReplyContentEnum.VIDEO:
                    {
                        predicate = predicate.AndAlso(x => x.Type == (int)AttachmentsType.VIDEO);
                        break;
                    }
                case (int)AutoReplyContentEnum.FILE:
                    {
                        predicate = predicate.AndAlso(x => x.Type == (int)AttachmentsType.FILE);
                        break;
                    }
            }

            gridRequest.PageCondition.SortConditions.Add(new SortCondition("CreateTime", System.ComponentModel.ListSortDirection.Descending));
            var list = _attachmentsItemService.GetList<AttachmentsItemView>(predicate, gridRequest.PageCondition);

            return base.GetPageResult(list, gridRequest);
        }

        /// <summary>
        /// 上传文件到微信服务器
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="fileId"></param>
        /// <param name="replyType"></param>
        /// <returns></returns>
        public ActionResult UploadFileToWx(int appId, int fileId, int replyType)
        {
            var fileEntity = _attachmentsItemService.Repository.Entities.FirstOrDefault(x => x.Id == fileId);
            if (fileEntity == null || !System.IO.File.Exists(Path.Combine(Server.MapPath("~/"), fileEntity.AttachmentUrl)))
            {
                ModelState.AddModelError("Invalid Input", "此文件不存在，请确保已经上传到服务器");
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            // 取得微信ID，非DB中用的AppID
            var config = WeChatCommonService.GetWeChatConfigByID(appId);
            var dic = new Dictionary<string, Stream>();
            var fullPath = Path.Combine(Server.MapPath("~/"), fileEntity.AttachmentUrl);
            var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            string fileName = fileEntity.AttachmentTitle;
            if (!fileName.EndsWith(fileEntity.Extension))
            {
                fileName = fileName + fileEntity.Extension;
            }
            dic.Add(fileName, fileStream);
            // 取得微信Token
            var wxToken = getWxToken(appId);

            var uploadFileType = UploadMediaFileType.file;
            switch (replyType)
            {
                case (int)AutoReplyContentEnum.IMAGE:
                    {
                        uploadFileType = UploadMediaFileType.image;
                        break;
                    }
                case (int)AutoReplyContentEnum.VOICE:
                    {
                        uploadFileType = UploadMediaFileType.voice;
                        break;
                    }
                case (int)AutoReplyContentEnum.VIDEO:
                    {
                        uploadFileType = UploadMediaFileType.video;
                        break;
                    }
                default:
                    {
                        uploadFileType = UploadMediaFileType.file;
                        break;
                    }
            }

            var ret = MediaApi.Upload(wxToken, uploadFileType, dic, "", 10000 * 50);

            // 更新FileManage表的MediaID
            _attachmentsItemService.UpdateMediaId(fileEntity.Id, ret.media_id, DateTimeHelper.GetDateTimeFromXml(ret.created_at));

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        private List<ArticleInfoView> GetArticleListPrivate(Expression<Func<ArticleInfo, bool>> predicate, PageCondition ConPage)
        {
            // 当前app下的图文: 此条件已经在前台JS中添加都predicate里面了
            //predicate = predicate.AndAlso(x => x.AppId == appId);
            // 只取得发布后的新闻
            predicate = predicate.And(x => x.ArticleStatus == ConstData.STATUS_PUBLISH);
            // 排序
            ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

            // 取得list
            var list = _articleInfoService.GetList<ArticleInfoView>(predicate, ConPage);

            // 分类一览
            var categoryList = CommonService.GetCategory((AppId));

            // 取得分类名
            list = ChangeCategoryCodeToCategoryName(list, categoryList);

            return list;
        }

        //public List<FileManageView> GetFileListPrivate(Expression<Func<FileManage, bool>> predicate, PageCondition ConPage)
        //{
        //    //predicate = predicate.AndAlso(x => x.CreatedUserID.Equals(User.Identity.Name));

        //    ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

        //    var q = _fileManageService.GetList<FileManageView>(predicate, ConPage);

        //    return q.ToList();
        //}

        //BackEnd校验
        private bool BeforeAddOrUpdate(AutoReplyNewView modal)
        {
            var validate = true;
            var errMsg = new StringBuilder();
            var objModal = modal.Main;
            if (string.IsNullOrEmpty(objModal.Name) || objModal.Name.Length > 20)
            {
                validate = false;
                errMsg.Append(T("请输入20字符以内的口令名称.<br/>"));
            }

            if (objModal.KeywordType == 0)
            {
                validate = false;
                errMsg.Append(T("请输入口令类型.<br/>"));
            }

            // 口令校验
            if (objModal.KeywordType == (int)AutoReplyKeywordEnum.TEXT ||
                objModal.KeywordType == (int)AutoReplyKeywordEnum.MENU)
            {
                if (objModal.Keywords == null || objModal.Keywords.Count == 0)
                {
                    validate = false;
                    errMsg.Append(T("请输入口令内容.<br/>"));
                }
                else
                {
                    if (objModal.KeywordType == (int)AutoReplyKeywordEnum.TEXT)
                    {
                        foreach (var keyword in objModal.Keywords)
                        {
                            if (String.IsNullOrEmpty(keyword.Keyword) || keyword.Keyword.Length > 20)
                            {
                                validate = false;
                                errMsg.Append(T("请输入20字符以内的口令.<br/>"));
                            }
                        }
                    }
                    else if (objModal.KeywordType == (int)AutoReplyKeywordEnum.MENU &&
                             objModal.Keywords[0].SecondaryType == (int)AutoReplyMenuEnum.NORMAL)
                    {
                        validate = false;
                        errMsg.Append(T("请输入20字符以内的菜单Key.<br/>"));
                    }
                }
            }

            // 口令回复内容校验
            if (objModal.Contents == null || objModal.Contents.Count == 0)
            {
                validate = false;
                errMsg.Append(T("请输入口令回复内容.<br/>"));
            }
            else
            {
                foreach (var content in objModal.Contents)
                {
                    switch (content.PrimaryType)
                    {
                        case (int)AutoReplyContentEnum.TEXT:
                        case (int)AutoReplyContentEnum.LINK:
                            {
                                if (String.IsNullOrEmpty(content.Content))
                                {
                                    validate = false;
                                    errMsg.Append(T("请输入口令回复内容.<br/>"));
                                }
                                break;
                            }
                        case (int)AutoReplyContentEnum.NEWS:
                            {
                                // 如果是选择最近几条新闻
                                if (content.SecondaryType == (int)AutoReplyNewsEnum.LATEST)
                                {
                                    if (String.IsNullOrEmpty(content.Content))
                                    {
                                        validate = false;
                                        errMsg.Append(T("请输入最新图文消息条数.<br/>"));
                                    }
                                }
                                else if (content.SecondaryType == (int)AutoReplyNewsEnum.MANUAL)
                                {
                                    if (String.IsNullOrEmpty(content.NewsID))
                                    {
                                        validate = false;
                                        errMsg.Append(T("请选择图文消息"));
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                if (content.FileID == null || content.FileID == 0)
                                {
                                    validate = false;
                                    errMsg.Append(T("请选择文件"));
                                }
                                break;
                            }
                    }
                }
            }

            if (!validate)
            {
                ModelState.AddModelError("Invalid Input", errMsg.ToString());
            }

            return validate;
        }

        /// <summary>
        /// 取得口令子类型数据
        /// </summary>
        /// <param name="autoReplyView"></param>
        /// <returns></returns>
        private List<AutoReplyKeywordView> GetAutoReplySubTypes(AutoReplyNewView model)
        {
            var list = new List<AutoReplyKeywordView>();
            switch (model.Main.KeywordType)
            {
                // 文本类型
                case (int)AutoReplyKeywordEnum.TEXT:
                    {
                        var matchTypes = model.Main.MatchType;
                        var keywords = model.Main.TextKeyword;
                        if (matchTypes != null && keywords != null)
                        {
                            var keywordCount = keywords.Count;
                            for (int i = 0; i < matchTypes.Count; i++)
                            {
                                if (keywordCount > i)
                                {
                                    var keywordObj = new AutoReplyKeywordView
                                    {
                                        AutoReplyId = model.Main.Id,
                                        SecondaryType = matchTypes[i],
                                        Keyword = keywords[i]
                                    };

                                    list.Add(keywordObj);
                                }
                            }

                        }
                        break;
                    }
                // 菜单类型
                case (int)AutoReplyKeywordEnum.MENU:
                    {
                        var keywordObj = new AutoReplyKeywordView();
                        keywordObj.AutoReplyId = model.Main.Id;
                        keywordObj.SecondaryType = model.Main.MatchType[0];
                        // 普通类型时，有菜单key
                        if (keywordObj.SecondaryType == (int)AutoReplyMenuEnum.NORMAL)
                        {
                            keywordObj.Keyword = model.Main.TextKeyword[0];
                        }
                        list.Add(keywordObj);

                        break;
                    }
                //case (int)AutoReplyKeywordEnum.SubscribeEvent:
                case (int)AutoReplyMPKeywordEnum.SCAN:
                case (int)AutoReplyMPKeywordEnum.SubscribeWithScan:
                    {
                        var keywordObj = new AutoReplyKeywordView();
                        keywordObj.AutoReplyId = model.Main.Id;
                        keywordObj.Keyword = model.Main.TextKeyword[0];
                        list.Add(keywordObj);
                        break;
                    }

            }

            return list;
        }

        #region old content logic
        ///// <summary>
        ///// 取得口令回复一览
        ///// </summary>
        ///// <param name="autoReplyView"></param>
        ///// <returns></returns>
        //private List<AutoReplyContentView> GetAutoReplyList(AutoReplyView autoReplyView)
        //{
        //    // THIS IS NOT A GOOD WAY, SHOULD TRANSFER TO JSON OBJECT IN FRONT END
        //    var list = new List<AutoReplyContentView>();
        //    var replyTypes = Request.Form.GetValues("replyType");
        //    var textReply = Request.Form.Get("saytext");
        //    var newsTitle = Request.Form.Get("msgPictitle");
        //    var newsComment = Request.Form.Get("msgtips");
        //    var newsBody = Request.Form.Get("msgbody");
        //    //var isTextEncrypts = Request.Form.GetValues("isTextEncryptReplace");
        //    var replyNewsTypes = Request.Form.GetValues("replyNewsType");
        //    var replyNewsLatestCounts = Request.Form.GetValues("replyNewsLatestCount");
        //    var replyNewsLists = Request.Form.GetValues("replyNewsList");
        //    var textReplyFiles = Request.Form.GetValues("textReplyFiles");

        //    //if (replyTypes != null)
        //    //{
        //    //    var count = replyTypes.Length;
        //    //    for (int i = 0; i < count; i++)
        //    //    {
        //            var content = new AutoReplyContentView();
        //            content.AutoReplyId = autoReplyView.Id;
        //            content.PrimaryType = 2;//int.Parse(replyTypes[i]);
        //            NewsInfoView view = new NewsInfoView();
        //            switch (content.PrimaryType)
        //            {
        //                #region text
        //                case (int)AutoReplyContentEnum.TEXT:
        //                case (int)AutoReplyContentEnum.LINK:
        //                    {
        //                        content.Content = textReply;
        //                        //if (isTextEncrypts != null)
        //                        //{
        //                        //    content.IsEncrypt = isTextEncrypts[i] == "1";
        //                        //}
        //                        break;
        //                    }
        //                #endregion

        //                #region news
        //                case (int)AutoReplyContentEnum.PHOTO_TEXT:
        //                    {
        //                        view.NewsTitle = newsTitle;
        //                        view.NewsComment = newsComment;
        //                        view.NewsContent = newsBody;
        //                        //    //ImageContent = news
        //                        content.NewsID = Guid.NewGuid().ToString();
        //                        var configs = Infrastructure.Web.Domain.Service.CommonService.lstSysConfig;
        //                        var config = configs.Where(a => a.ConfigName.Equals("Content Server", StringComparison.CurrentCultureIgnoreCase)).First();
        //                        string host = config.ConfigValue;
        //                        if (host.EndsWith("/"))
        //                        {
        //                            host = host.Substring(0, host.Length - 1);
        //                        }
        //                        //var picUrl = host + item.ImageContent;
        //                        var url = host + "WechatMain/AutoReply/GetNews?id=" + content.NewsID;
        //                        content.Content = JsonConvert.SerializeObject(view);
        //                        //if (replyNewsTypes != null)
        //                        //{
        //                        //    content.SecondaryType = int.Parse(replyNewsTypes[i]);
        //                        //    if (content.SecondaryType == (int)AutoReplyNewsEnum.LATEST)
        //                        //    {
        //                        //        if (replyNewsLatestCounts != null) content.Content = replyNewsLatestCounts[i];
        //                        //    }
        //                        //    else if (content.SecondaryType == (int)AutoReplyNewsEnum.MANUAL)
        //                        //    {
        //                        //        if (replyNewsLists != null) content.NewsID = replyNewsLists[i];
        //                        //    }
        //                        //}

        //                        break;
        //                    }
        //                #endregion

        //                case (int)AutoReplyContentEnum.VIDEO:
        //                    view.NewsTitle = newsTitle;
        //                    break;
        //                case (int)AutoReplyContentEnum.FILE:
        //                    view.NewsTitle = newsTitle;
        //                    break;
        //                case (int)AutoReplyContentEnum.IMAGE:
        //                    view.NewsTitle = newsTitle;
        //                    break;
        //                case (int)AutoReplyContentEnum.AUDIO:
        //                    view.NewsTitle = newsTitle;
        //                    break;
        //                default:
        //                    {
        //                        //if (textReplyFiles != null) content.FileID = int.Parse(textReplyFiles[i]);
        //                        break;
        //                    }
        //            }

        //            list.Add(content);
        //    //    }

        //    //}

        //    return list;
        //}
        #endregion

        private List<AutoReplyContentView> GetAutoReplyList(AutoReplyNewView model)
        {
            var list = new List<AutoReplyContentView>();
            var content = new AutoReplyContentView();
            content.AutoReplyId = model.Main.Id;
            var news = model.Send[0]; //保存多条内容。只有新闻存在多条情况
            content.IsEncrypt = news.isSecurityPost;
            var cate = (AutoReplyContentEnum)Enum.Parse(typeof(AutoReplyContentEnum), news.NewsCate, true);
            //选择资源文件
            if (news.materialId.HasValue && news.materialId.Value > 0)
            {
                var config = WeChatCommonService.GetWeChatConfigByID(model.Main.AppId);
                content.FileID = news.materialId;
                content.IsNewContent = false;
                //更新MediaID
                //out 之后news.isSecurityPost 被清空
                Innocellence.WeChatMain.Common.WechatCommon.GetMediaIDByFileID(news.materialId, _attachmentsItemService, config.WeixinCorpId, out news);

                content.Content = JsonConvert.SerializeObject(new List<NewsInfoView> { news });
            }
            else if (cate == AutoReplyContentEnum.NEWS)
            { //图文消息特殊处理
                if (string.IsNullOrEmpty(news.NewsID)) //如果不是选择的素材
                {
                    var listArticle = GetArticleList(model.Send);

                    content.NewsID = string.Join(",", listArticle.Select(a => a.Id).ToArray());
                }
                else
                {
                    content.NewsID = news.NewsID;
                }
                content.IsNewContent = false;
                //content.SecondaryType = model.Main.Contents[0].SecondaryType;
                content.SecondaryType = (int)AutoReplyNewsEnum.MANUAL; //此处缺少功能，临时写死。画面应该增加返回最近的几条新闻
            }
            else
            {
                if (cate != AutoReplyContentEnum.TEXT && cate != AutoReplyContentEnum.LINK) //上传文件处理
                {
                    var mediaId = WechatCommon.GetMediaInfo(cate, news, model.Main.AppId);
                    content.MediaId = mediaId;
                }


                content.IsNewContent = true;
                //content.Content = JsonConvert.SerializeObject(model.Send);  //这个地方不应该存数组，todo
                content.Content = JsonConvert.SerializeObject(new List<NewsInfoView> { news });
            }
            content.PrimaryType = (int)cate;
            list.Add(content);
            return list;
        }

        private List<ArticleInfoView> GetArticleList(List<NewsInfoView> newsList)
        {
            List<ArticleInfoView> lstArticles = new List<ArticleInfoView>();

            int i = 0;
            newsList.ForEach(a => a.Id = i++);
            var content = JsonConvert.SerializeObject(newsList);

            foreach (var a in newsList)
            {
                var entity = a.ConvertToEntityArticle();
                // entity.UserName = User.Identity.Name;

                entity.ArticleContentEdit = content;
                entity.ArticleType = 2;  //标记是口令消息
                entity.IsLike = true;
                entity.ArticleStatus = "Published";
                entity.PublishDate = DateTime.Now;

                entity.ImageCoverUrl = a.ImageSrc; //前端的bug
                entity.OrderID = a.Id;

                _articleInfoService.InsertView(entity);


                lstArticles.Add(entity);
            }

            return lstArticles;

        }
        /// <summary>
        /// CategoryCode转变换CategoryName
        /// 注：从ArticleInfoController中拷贝
        /// </summary>
        /// <param name="articleList">待修正的图文消息列表</param>
        /// <param name="cateList">标签列表</param>
        /// <returns>修正后的图文消息列表</returns>
        private List<ArticleInfoView> ChangeCategoryCodeToCategoryName(List<ArticleInfoView> articleList, List<Category> cateList)
        {
            var list = new List<ArticleInfoView>();

            if (articleList == null || articleList.Count == 0 ||
                cateList == null || cateList.Count == 0)
            {
                return articleList;
            }

            foreach (var article in articleList)
            {
                Category category = cateList.Where(x => article.CategoryId.HasValue &&
                   x.Id == article.CategoryId.Value).Distinct().FirstOrDefault();
                article.CategoryName = category == null ? string.Empty : category.CategoryName;

                list.Add(article);
            }

            return list;
        }

        // THIS METHOD SHOULD BE MOVED TO COMMON SERVICE. ACTUALLY, WinxinBaseController HAS THIS METHOD
        private string getWxToken(int agentid = 0)
        {
            var config = WeChatCommonService.GetWeChatConfigByID(agentid);
            var strToken = AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
            if (!string.IsNullOrEmpty(strToken))
            {
                return strToken;
            }

            return null;
        }

        [HttpGet]
        public ActionResult GetNews(int id, int? subId)
        {
            var view = new AutoReplyNewView();
            view.Main = _autoReplyService.GetDetail(id);
            var news = JsonConvert.DeserializeObject<List<NewsInfoView>>(view.Main.Contents[0].Content)[0];

            return View(news);
        }

        [HttpGet]
        public JsonResult CheckUsed(int appId, int id)
        {
            bool hasUsed = true;
            try
            {
                string startStr = string.Format("{0}:::", id);
                hasUsed = CommonService.lstCategory.Any(c => c.AppId == appId && c.IsDeleted == false && null != c.CategoryCode && c.CategoryCode.StartsWith(startStr));
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }
            return Json(new { hasUsed = hasUsed }, JsonRequestBehavior.AllowGet);
        }
    }
}
