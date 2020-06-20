using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.Authentication.Authentication;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin;
using System.Dynamic;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Newtonsoft.Json;
using Infrastructure.Core.Data;
using System.Text.RegularExpressions;
using Innocellence.WeChat.Domain;
using Infrastructure.Utility.IO;
using Infrastructure.Web.Domain.Service;

namespace Innocellence.WeChatMain.Controllers
{
    public class WechatUserRequestMessageLogController : BaseController<WechatUserRequestMessageLog, WechatUserRequestMessageLogView>
    {
        private readonly IWechatUserRequestMessageLogService _requestMessageLogService;
        private readonly IAddressBookService _addressBookService;
        private readonly IWechatUserRequestMessageTagService _requestMessageTagService;
        private readonly IWechatMPUserService _wechatMPUserService;
        private readonly BaseService<AccountManage> _accountManageService = new BaseService<AccountManage>();

        public WechatUserRequestMessageLogController(IWechatUserRequestMessageLogService objService,
            IAddressBookService addressBookService,
            IWechatUserRequestMessageTagService requestMessageTagService,
            IWechatMPUserService wechatMPUserService)
            : base(objService)
        {
            _requestMessageLogService = objService;
            _addressBookService = addressBookService;
            _requestMessageTagService = requestMessageTagService;
            _wechatMPUserService = wechatMPUserService;
            ViewBag.MessageTypes = GetCurrentSupportMsgTypes();
        }

        private dynamic GetCurrentSupportMsgTypes(bool isHasReaded = false)
        {
            List<dynamic> msgTypes = new List<dynamic>();
            msgTypes = typeof(WechatUserMessageLogContentType).GetAllItems();
            if (isHasReaded)
            {
                List<int> hasReadedDisplayMsgTypes = new List<int>()
                {
                    (int)WechatUserMessageLogContentType.Request_Text,
                    (int)WechatUserMessageLogContentType.Request_Voice,
                    (int)WechatUserMessageLogContentType.Request_Video,
                    (int)WechatUserMessageLogContentType.Request_Image,
                    (int)WechatUserMessageLogContentType.Request_Link,
                    (int)WechatUserMessageLogContentType.Response_Empty
                };
                msgTypes.RemoveAll(obj => !hasReadedDisplayMsgTypes.Contains(obj.Value));
            }
            else
            {
                List<int> notDisplayMsgTypes = new List<int>()
                {
                    (int)WechatUserMessageLogContentType.DEFAULT,
                    (int)WechatUserMessageLogContentType.Request_Event,
                    (int)WechatUserMessageLogContentType.Request_Chat,
                };
                msgTypes.RemoveAll(obj => notDisplayMsgTypes.Contains(obj.Value) || obj.Value > (int)WechatUserMessageLogContentType.Response_Empty);
            }
            msgTypes.RemoveAll(obj => obj.Value == (int)WechatUserMessageLogContentType.Request_ShortVideo);
            if (AppId > 0)
            {
                var accountManage = WeChatCommonService.GetWeChatConfigByID(AppId);
                if (null != accountManage && accountManage.IsCorp.HasValue && accountManage.IsCorp.Value)
                {
                    msgTypes.RemoveAll(obj => obj.Value == (int)WechatUserMessageLogContentType.Request_Link);
                }
            }
            return msgTypes;
        }

        // 初始化list页面
        [HttpGet, DataSecurityFilter]
        public virtual ActionResult Index(int appId)
        {
            ViewBag.AppId = appId;
            ViewBag.MessageTypes = GetCurrentSupportMsgTypes();

            int AccountManageId = 0;
            if (int.TryParse(Request.Cookies["AccountManageId"].Value, out AccountManageId))
            {
                if (_accountManageService.Repository.GetByKey(AccountManageId).AccountType == 1)
                {
                    bool isHasReaded = !string.IsNullOrWhiteSpace(Request["hasReaded"]);
                    if (isHasReaded)
                    {
                        ViewBag.HasReaded = Request["hasReaded"];
                        ViewBag.MessageTypes = GetCurrentSupportMsgTypes(isHasReaded);
                    }
                    if (!string.IsNullOrWhiteSpace(Request["hiddenAutoReply"]))
                    {
                        ViewBag.HiddenAutoReply = Request["hiddenAutoReply"];
                    }
                    return View("MPIndex");
                }
            }
            return base.Index();
        }

        [HttpGet]
        public JsonResult GetTags(int appId)
        {
            var tags = _requestMessageTagService.Repository.Entities.Where(p => p.IsDeleted == false && p.AppID == appId).ToList();
            return Json(tags, JsonRequestBehavior.AllowGet);
        }

        public override List<WechatUserRequestMessageLogView> GetListEx(Expression<Func<WechatUserRequestMessageLog, bool>> predicate, PageCondition ConPage)
        {
            int appId = int.Parse(Request["appId"]);
            string type = Request["type"];
            string queryStr = Request["queryStr"];
            bool hasResponse = bool.Parse(Request["hasResponse"]);
            string tagId = Request["TagFilter"];

            return GetListInner(predicate, ConPage, appId, type, queryStr, hasResponse, tagId);
        }

        private List<WechatUserRequestMessageLogView> GetListInner(Expression<Func<WechatUserRequestMessageLog, bool>> predicate, PageCondition ConPage, int appId, string type, string queryStr, bool hasResponse, string tagId)
        {
            int primaryType = default(int);
            if (int.TryParse(type, out primaryType) && primaryType != (int)WechatUserMessageLogContentType.Response_Empty)
            {
                predicate = predicate.AndAlso(r => r.ContentType == primaryType);
                //primaryType = SeachWithType(primaryType, out predicate);
            }
            else
            {
                predicate.AndAlso(r => r.ContentType < (int)WechatUserMessageLogContentType.Response_Empty);
            }

            predicate = predicate.AndAlso(x => x.AppID == appId);

            if (!string.IsNullOrEmpty(queryStr))
            {
                //不选type, 直接搜索
                if (primaryType == -1)
                {
                    predicate = predicate.AndAlso(x => x.Content.Contains(queryStr) || x.UserName.Contains(queryStr));
                }
                //选择姓名
                else if (primaryType == (int)WechatUserMessageLogContentType.Response_Empty)
                {
                    if (hasResponse)
                    {
                        predicate = predicate.AndAlso(x => x.UserID.Equals(queryStr));
                    }
                    else
                    {
                        predicate = predicate.AndAlso(x => x.UserName.Contains(queryStr));
                    }
                }
                //根据type 进行搜索
                else
                {
                    predicate = predicate.AndAlso(x => x.Content.Contains(queryStr));
                }
            }

            if (hasResponse)
            {
                if (ConPage.SortConditions == null)
                {
                    ConPage.SortConditions = new List<SortCondition>();
                }
                ConPage.SortConditions.Add(new SortCondition("CreatedTime", System.ComponentModel.ListSortDirection.Descending));
                ConPage.PageSize = int.MaxValue;
            }
            else
            {
                predicate = predicate.AndAlso(x => x.ContentType < (int)WechatUserMessageLogContentType.Response_Empty);
            }
            // filter by tags
            if (!string.IsNullOrEmpty(tagId))
            {
                predicate = predicate.AndAlso(x => x.TagId.Equals(tagId));
            }

            //已读/未读
            //如果有匹配的口令且不是All类型, 则标记为已读
            //对于事件类型, click 和view 标记为已读, 其他需要匹配是否有口令关联
            if (!string.IsNullOrEmpty(Request["hiddenHasReaded"]))
            {
                bool hasReaded = bool.Parse(Request["hiddenHasReaded"]);
                if (!hasReaded)
                {

                    List<int> hasReadedDisplayMsgTypes = new List<int>()
                    {
                        (int)WechatUserMessageLogContentType.Request_Text,
                        (int)WechatUserMessageLogContentType.Request_Voice,
                        (int)WechatUserMessageLogContentType.Request_Video,
                        (int)WechatUserMessageLogContentType.Request_Image,
                        (int)WechatUserMessageLogContentType.Request_Link,
                    };
                    predicate = predicate.AndAlso(x => x.HasReaded == false && hasReadedDisplayMsgTypes.Contains(x.ContentType));
                }
            }

            //隐藏自动回复
            //1.如果有匹配的口令且不是All类型, 则需要隐藏
            //2.如果是事件, click 和view 需要隐藏, 其他需要匹配是否有口令关联
            if (!string.IsNullOrWhiteSpace(Request["isHiddenAutoReply"]))
            {
                bool hiddenAutoReply = bool.Parse(Request["isHiddenAutoReply"]);
                if (hiddenAutoReply)
                {
                    List<int> displayContentType = new List<int>() {
                        (int)WechatUserMessageLogContentType.Request_Text,
                        (int)WechatUserMessageLogContentType.Request_Location,
                        (int)WechatUserMessageLogContentType.Request_Image,
                        (int)WechatUserMessageLogContentType.Request_Voice,
                        (int)WechatUserMessageLogContentType.Request_Video,
                    };
                    predicate = predicate.AndAlso(x => x.IsAutoReply == false && displayContentType.Contains(x.ContentType));
                }
            }
            List<WechatUserRequestMessageLogView> list = new List<WechatUserRequestMessageLogView>();
            if (ConPage != null)
            {
                list = _requestMessageLogService.GetList<WechatUserRequestMessageLogView>(predicate, ConPage);
            }
            else
            {
                list = _requestMessageLogService.GetList<WechatUserRequestMessageLogView>(predicate).OrderByDescending(a => a.Id).ToList();
            }


            if (hasResponse)
            {
                list = list.OrderBy(r => r.CreatedTime).ToList();
            }
            else
            {
                list.ForEach(r => r.ContentTypeDisplayStr = ConvertContentTypeToString(r.ContentType));
            }

            //获取头像
            List<WechatUserRequestMessageLogView> result = GetUserAvatar(appId, list);
            //GetUserAvatar(list);
            return result;
        }

        private List<WechatUserRequestMessageLogView> GetUserAvatar(int appId, List<WechatUserRequestMessageLogView> list)
        {
            List<WechatUserRequestMessageLogView> result = new List<WechatUserRequestMessageLogView>();
            if (list != null && list.Count > 0)
            {
                try
                {
                    var accountManage = WeChatCommonService.GetWeChatConfigByID(appId);
                    bool isCrop = accountManage != null && accountManage.IsCorp.HasValue && accountManage.IsCorp.Value;
                    if (isCrop)
                    {
                        result = list.Join(_addressBookService.Repository.Entities,
                                            h => h.UserID,
                                            u => u.UserId,
                                            (h, u) => new WechatUserRequestMessageLogView()
                                            {
                                                Id = h.Id,
                                                AppID = h.AppID,
                                                UserID = h.UserID,
                                                UserName = h.UserName,
                                                Content = h.Content,
                                                ContentType = h.ContentType,
                                                CreatedTime = h.CreatedTime,
                                                TagId = h.TagId,
                                                IsAutoReply = h.IsAutoReply,
                                                HasReaded = h.HasReaded,
                                                Duration = h.Duration,
                                                ContentTypeDisplayStr = h.ContentTypeDisplayStr,
                                                PhotoUrl = u.Avatar,
                                            }
                                            ).ToList();
                    }
                    else
                    {
                        result = list.Join(_wechatMPUserService.Repository.Entities,
                                            h => h.UserID,
                                            u => u.OpenId,
                                            (h, u) => new WechatUserRequestMessageLogView()
                                            {
                                                Id = h.Id,
                                                AppID = h.AppID,
                                                UserID = h.UserID,
                                                UserName = h.UserName,
                                                Content = h.Content,
                                                ContentType = h.ContentType,
                                                CreatedTime = h.CreatedTime,
                                                TagId = h.TagId,
                                                IsAutoReply = h.IsAutoReply,
                                                HasReaded = h.HasReaded,
                                                Duration = h.Duration,
                                                ContentTypeDisplayStr = h.ContentTypeDisplayStr,
                                                PhotoUrl = u.HeadImgUrl,
                                            }
                                            ).ToList();
                    }
                }
                catch (Exception e)
                {
                    result = list;
                    _Logger.Error(e);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据指定id, 返回该id 对应的user的前后各10条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetUserHistory(int id, int appId, int pageSize, int pageNumber, bool hiddenAutoReply)
        {
            try
            {
                _Logger.Debug("get user {0}'s history in {1} between {2} and {3}", id, appId, pageSize, pageNumber);
                var list = _requestMessageLogService.GetRecords<WechatUserRequestMessageLogView>(id, appId, pageSize, pageNumber);
                foreach (var a in list)
                {
                    if (a.ContentType == 106)
                    {
                        //var  a.Content
                        Regex listRegex = new Regex("\"Url\":\"((http[s]?://.*?)/.*?wxdetail/(\\d+).*?)\"", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        //得到匹配的数据集合
                        MatchCollection mc = listRegex.Matches(a.Content);
                        foreach (Match mt in mc)
                        {
                            a.Content = a.Content.Replace(mt.Groups[1].Value, string.Format("{0}/WechatMain/Message/GetNews?id={1}&subid=0&code=0", mt.Groups[2].Value, mt.Groups[3].Value));
                        }

                    }
                }


                var accountManage = WeChatCommonService.GetWeChatConfigByID(appId);
                var config = WeChatCommonService.GetWeChatConfigByID(appId);
                if (accountManage != null)
                {
                    //企业号
                    if (accountManage.IsCorp.HasValue && accountManage.IsCorp.Value)
                    {
                        SysAddressBookMember user = null;
                        list.ForEach(r =>
                        {
                            r.IsCrop = true;
                            if (user == null)
                            {
                                user = _addressBookService.GetMemberByUserId(r.UserID, true);
                            }
                            if (user != null)
                            {
                                //此处需要获取user的其他信息,因此头像直接从user中获取即可,可能会导致两处头像不一致
                                r.UserName = user.UserName;
                                r.PhotoUrl = user.Avatar;
                                r.Mobile = user.Mobile;
                                r.EmployeeNo = string.IsNullOrEmpty(user.EmployeeNo) ? string.Empty : user.EmployeeNo;
                                List<int> departMent = JsonConvert.DeserializeObject<int[]>(user.Department).ToList();
                                string[] departMents = WeChatCommonService.lstDepartment(config.AccountManageId.Value).Where(d => departMent.Contains(d.id)).Select(d => d.name).ToArray();
                                r.Department = string.Join(",", departMents);
                                r.AppLogo = config.CoverUrl;
                            }
                        });
                    }
                    else //服务号
                    {
                        WechatMPUserView user = null;
                        list.ForEach(r =>
                        {
                            r.IsCrop = false;
                            if (user == null)
                            {
                                user = _wechatMPUserService.GetUserByOpenId(r.UserID);
                            }
                            if (user != null)
                            {
                                r.PhotoUrl = user.HeadImgUrl;
                            }
                            r.AppLogo = config.CoverUrl;
                        });
                    }
                }
                //if (hiddenAutoReply)
                //{
                //    list.RemoveAll(h => h.IsAutoReply == true);
                //}
                return Json(
                            new
                            {
                                list = list,
                                isFirst = pageNumber < 0 ? pageSize > list.Count : false,
                                isLast = pageNumber > 0 ? pageSize > list.Count : false,
                            },
                            JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _Logger.Error(e);
                throw;
            }
        }

        public ActionResult GetUnreadMsgCount(int appId)
        {
            List<int> hasReadedDisplayMsgTypes = new List<int>()
            {
                (int)WechatUserMessageLogContentType.Request_Text,
                (int)WechatUserMessageLogContentType.Request_Voice,
                (int)WechatUserMessageLogContentType.Request_Video,
                (int)WechatUserMessageLogContentType.Request_Image,
                (int)WechatUserMessageLogContentType.Request_Link,
            };
            var count = _requestMessageLogService.Repository.Entities.Count(h => h.HasReaded == false && h.AppID == appId && hasReadedDisplayMsgTypes.Contains(h.ContentType));
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetReaded(int appId, string openId, int id)
        {
            try
            {
                string sql = string.Format("update WechatUserRequestMessageLog set HasReaded = 1 where AppID = {0} and UserId like '{1}' and Id = {2}", appId, openId, id);
                _requestMessageLogService.Repository.SqlExcute(sql);
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }
            return null;
        }

        private string ConvertContentTypeToString(int contentType)
        {
            try
            {
                var type = (WechatUserMessageLogContentType)contentType;
                return type.GetDescriptionByName();
            }
            catch (Exception)
            {
                return contentType.ToString();
            }
        }

        private int SeachWithType(int type, out Expression<Func<WechatUserRequestMessageLog, bool>> predicate)
        {
            predicate = m => true;
            switch (type)
            {
                case (int)AutoReplyKeywordEnum.TEXT:
                    predicate = predicate.AndAlso(r => r.ContentType == (int)WechatUserMessageLogContentType.Request_Text);
                    return (int)WechatUserMessageLogContentType.Request_Text;
                case (int)AutoReplyKeywordEnum.MENU:
                    predicate = predicate.AndAlso(r => r.ContentType == (int)WechatUserMessageLogContentType.Request_Event
                                                    || r.ContentType == (int)WechatUserMessageLogContentType.Request_Event_Click
                                                    || r.ContentType == (int)WechatUserMessageLogContentType.Request_Event_View);
                    return (int)WechatUserMessageLogContentType.Request_Event;
                case (int)AutoReplyKeywordEnum.IMAGE:
                    predicate = predicate.AndAlso(r => r.ContentType == (int)WechatUserMessageLogContentType.Request_Image);
                    return (int)WechatUserMessageLogContentType.Request_Image;
                case (int)AutoReplyKeywordEnum.AUDIO:
                    predicate = predicate.AndAlso(r => r.ContentType == (int)WechatUserMessageLogContentType.Request_Voice);
                    return (int)WechatUserMessageLogContentType.Request_Voice;
                case (int)AutoReplyKeywordEnum.VIDEO:
                    predicate = predicate.AndAlso(r => r.ContentType == (int)WechatUserMessageLogContentType.Request_Video
                                                    || r.ContentType == (int)WechatUserMessageLogContentType.Request_ShortVideo);
                    return (int)WechatUserMessageLogContentType.Request_Video;
                case (int)AutoReplyKeywordEnum.LOCATION:
                    predicate = predicate.AndAlso(r => r.ContentType == (int)WechatUserMessageLogContentType.Request_Location);
                    return (int)WechatUserMessageLogContentType.Request_Location;
                //case (int)AutoReplyKeywordEnum.LINK:
                //    predicate = predicate.AndAlso(r => r.ContentType == (int)WechatUserMessageLogContentType.Request_Link);
                //    return (int)WechatUserMessageLogContentType.Request_Link;
                case (int)AutoReplyKeywordEnum.ALL:
                    return (int)AutoReplyKeywordEnum.ALL;
                default:
                    predicate = predicate.AndAlso(r => r.ContentType < (int)WechatUserMessageLogContentType.Response_Empty);
                    return -1;
            }
        }

        public ActionResult AddTag(int appId, string strTagName)
        {
            WechatUserRequestMessageTag model = new WechatUserRequestMessageTag();
            model.AppID = appId;
            model.TagName = strTagName.Trim();
            if (_requestMessageTagService.Repository.Entities.Any(a => a.TagName.Equals(strTagName, StringComparison.CurrentCultureIgnoreCase) && a.AppID == appId && !a.IsDeleted))
            {
                throw new Exception(string.Format("名为\"{0}\"的消息标记已经存在。", model.TagName));
            }
            if (_requestMessageTagService.Repository.Insert(model) > 0)
            {
                var tags = _requestMessageTagService.Repository.Entities.Where(p => p.IsDeleted == false && p.AppID == appId).ToList();
                return Json(new { status = 200, result = tags, id = model.Id.ToString() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content("Error");
            }
        }

        public ActionResult UpdateTag(int Id, string strTagName)
        {
            WechatUserRequestMessageTag model = _requestMessageTagService.Repository.GetByKey(Id);
            model.TagName = strTagName;
            _requestMessageTagService.Repository.Update(model);
            var tags = _requestMessageTagService.Repository.Entities.Where(p => p.IsDeleted == false && p.AppID == model.AppID).ToList();
            return Json(new { status = 200, result = tags }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteTag(int Id, int appId)
        {
            _requestMessageTagService.Repository.Delete(Id);
            var messageList = _requestMessageLogService.Repository.Entities.Where(a => a.TagId.Equals(Id.ToString(), StringComparison.CurrentCultureIgnoreCase)).ToList();
            foreach (var item in messageList)
            {
                item.TagId = null;
                _requestMessageLogService.Repository.Update(item, new List<string>() { "TagId" });
            }
            var tags = _requestMessageTagService.Repository.Entities.Where(p => p.IsDeleted == false && p.AppID == appId).ToList();
            return Json(new { status = 200, result = tags }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// 根据某条历史记录获取全部的标签列表
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //public JsonResult GetSelectedHistoryTagList(int Id)
        //{
        //    var tagList = _requestMessageTagService.Repository.Entities.Where(a => a.IsDeleted == false);
        //    var history = _requestMessageLogService.Repository.Entities.Where(a => a.Id == Id).FirstOrDefault();
        //    if (history == null)
        //    {
        //        return new JsonResult { Data = new { Status = 201, result = "" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //    }
        //    List<string> historyTagList = new List<string> { };
        //    if (!string.IsNullOrWhiteSpace(history.TagId))
        //    {
        //        historyTagList = history.TagId.Split(',').ToList();
        //    }
        //    List<KeyValuePair<WechatUserRequestMessageTag, bool>> result = new List<KeyValuePair<WechatUserRequestMessageTag, bool>> { };
        //    foreach (var tag in tagList)
        //    {
        //        bool selected = false;
        //        if (historyTagList.Contains(tag.Id.ToString()))
        //        {
        //            selected = true;
        //        }
        //        result.Add(new KeyValuePair<WechatUserRequestMessageTag, bool>(tag, selected));
        //    }
        //    return new JsonResult { Data = new { Status = 200, result = result }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        /// <summary>
        /// 给历史记录添加标签
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult AddTagToHistory(int Id, string tagId)
        {
            var history = _requestMessageLogService.Repository.Entities.Where(a => a.Id == Id).FirstOrDefault();
            if (history != null)
            {
                if (!string.IsNullOrEmpty(tagId) && int.Parse(tagId) == -1)
                {
                    history.TagId = null;
                }
                else
                {
                    history.TagId = tagId;
                }
                _requestMessageLogService.Repository.Update(history, new List<string>() { "TagId" });
                return Json(new { status = 200 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = 201 }, JsonRequestBehavior.AllowGet);
        }

        public override ActionResult Export()
        {
            int appId = int.Parse(Request["hiddenAppId"]);
            string type = Request["type"];
            string queryStr = Request["queryStr"];
            bool hasResponse = bool.Parse(Request["hasResponse"]);
            string tagId = Request["TagFilter"];
            Expression<Func<WechatUserRequestMessageLog, bool>> predicate = a => true;
            PageCondition ConPage = null;
            var list = GetListInner(predicate, ConPage, appId, type, queryStr, hasResponse, tagId);
            return ExportToCSV(list, appId);
        }

        private ActionResult ExportToCSV(List<WechatUserRequestMessageLogView> list, int appId)
        {
            List<string> headLine = new List<string> { "Id", "UserName", "Content", "ContentTypeDisplayStr", "CreatedTime" };

            if (_accountManageService.Repository.GetByKey(this.AccountManageID).AccountType == 1)
            {
                headLine.Add("HasReadedString");
            }
            CsvSerializer<WechatUserRequestMessageLogView> csv = new CsvSerializer<WechatUserRequestMessageLogView>();
            csv.UseLineNumbers = false;
            var sRet = csv.SerializeStream(list, headLine.ToArray());

            string fileHeadName = ((CategoryType)appId).ToString();

            string fileName = fileHeadName + "_UserRequestMessage_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            return File(sRet, "text/comma-separated-values", fileName);
        }

    }
}