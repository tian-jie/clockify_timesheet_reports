using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChatMeeting.Domain.Entity;
using Innocellence.WeChatMeeting.Domain.Service;
using Innocellence.WeChatMeeting.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Common;
using System.Text;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.Weixin.MP.AdvancedAPIs.GroupMessage;
using Innocellence.WeChat.Domain.ViewModelFront;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.WeChat.Domain.ViewModel;
using System.IO;
using System.Web.Configuration;
using System.Drawing.Imaging;
using Innocellence.Weixin.MP.AdvancedAPIs;
using System.Net;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing;
using Innocellence.WeChat.Domain;
using Newtonsoft.Json;
using Innocellence.Weixin.Entities;

namespace Innocellence.WeChatMeeting.Controllers
{
    public class MeetingController : BaseController<Meeting, MeetingView>
    {
        private readonly IMeetingService _meetingService;
        private IMeetingInvitationService _meetingInvitationService;
        private IMeetingFileService _meetingFileService;
        private IMeetingTravelInfoService _meetingTravelInfoService;
        private IMeetingPerInfoService _meetingPerInfoService;
        private IWechatMPUserService _WechatMPUserService;
        private ICommonService _commonService;
        const string StrJsonTypeTextHtml = "text/html";
        private string UploadFilePath = "Content/UploadFiles/Meeting/";



        public MeetingController(IMeetingService objService)
            : base(objService)
        {
            _meetingService = objService;
            ViewBag.AppId = AppId;
        }

        public MeetingController(IMeetingService objmeetingService, ICommonService commonService, IMeetingInvitationService meetingInvitationService, IMeetingFileService meetingFileService, IMeetingTravelInfoService meetingTravelInfoService, IMeetingPerInfoService meetingPerInfoService, IWechatMPUserService WechatMPUserService)
            : base(objmeetingService)
        {

            _meetingService = objmeetingService;
            _commonService = commonService;
            _meetingInvitationService = meetingInvitationService;
            _meetingFileService = meetingFileService;
            _meetingTravelInfoService = meetingTravelInfoService;
            _meetingPerInfoService = meetingPerInfoService;
            _WechatMPUserService = WechatMPUserService;

        }



        #region 加载
        [HttpGet]
        public virtual ActionResult Index(int? appId)
        {
            AppId = appId.GetValueOrDefault();
            ViewBag.AppId = AppId;

            return View("../Meeting/Index");
        }
        #endregion

        #region 编辑(会议资料)
        [HttpGet]
        public virtual ActionResult Edit(int? appId, string id)
        {
            AppId = appId.GetValueOrDefault();

            ViewBag.AppId = AppId;
            var appInfo = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.Id == AppId);
            if (appInfo != null)
            {
                ViewBag.IsCorp = (appInfo.IsCorp == null || appInfo.IsCorp == false) ? 0 : 1;
            }

            var obj = GetObject(id);

            //根据会议id查询会议资料
            int mid = Convert.ToInt32(Request["id"]);

            ViewBag.MeetingFileList = _meetingFileService.Repository.Entities.Where(m => m.MeetingId == mid && m.IsDeleted == false).ToList();

            return View("../Meeting/Edit", obj);
        }
        #endregion

        #region 会议列表

        public override List<MeetingView> GetListEx(Expression<Func<Meeting, bool>> predicate, PageCondition ConPage)
        {
            string strTitle = Request["txtTitle"];
            //string txtStartDateTime = Request["txtStartDateTime"];
            //string txtEndDateTime = Request["txtEndDateTime"];
            string select_time = Request["select_time"];

            var q = GetListPrivate(ref predicate, ConPage, strTitle, string.Empty, select_time);

            return q.ToList();
        }

        protected List<MeetingView> GetListPrivate(ref Expression<Func<Meeting, bool>> predicate, PageCondition ConPage, string strTitle = "", string strNewsCate = "", string select_time = "", bool isAdmin = false)
        {
            predicate = predicate.AndAlso(a => a.IsDeleted == false);

            if (!string.IsNullOrEmpty(strTitle))
            {
                predicate = predicate.AndAlso(a => a.Title.Contains(strTitle));
            }

            if (!string.IsNullOrEmpty(select_time) && select_time == "tmonth")//最近三个月
            {
                DateTime dt = DateTime.Now;
                DateTime dateAdd = dt.AddMonths(-3);
                predicate = predicate.AndAlso(a => a.CreatedDate <= dt && a.CreatedDate >= dateAdd);
            }
            if (!string.IsNullOrEmpty(select_time) && select_time == "smonth")//最近半年
            {
                DateTime dt = DateTime.Now;
                DateTime dateAdd = dt.AddMonths(-6);
                predicate = predicate.AndAlso(a => a.CreatedDate <= dt && a.CreatedDate >= dateAdd);
            }
            if (!string.IsNullOrEmpty(select_time) && select_time == "year")//最近一年
            {
                DateTime dt = DateTime.Now;
                DateTime dateAdd = dt.AddYears(-1);
                predicate = predicate.AndAlso(a => a.CreatedDate <= dt && a.CreatedDate >= dateAdd);
            }

            //if (!string.IsNullOrEmpty(txtStartDateTime) && string.IsNullOrEmpty(txtEndDateTime))
            //{
            //    DateTime sTime = Convert.ToDateTime(txtStartDateTime);
            //    DateTime dateAdd = sTime.AddDays(1);
            //    predicate = predicate.AndAlso(a => a.StartDateTime >= sTime && a.StartDateTime <= dateAdd);
            //}

            //if (!string.IsNullOrEmpty(txtEndDateTime) && string.IsNullOrEmpty(txtStartDateTime))
            //{
            //    DateTime eTime = Convert.ToDateTime(txtEndDateTime);
            //    DateTime dateAdd = eTime.AddDays(1);
            //    predicate = predicate.AndAlso(a => a.EndDateTime >= eTime && a.EndDateTime <= dateAdd);
            //}


            //if (!string.IsNullOrEmpty(txtStartDateTime) && !string.IsNullOrEmpty(txtEndDateTime))
            //{
            //    DateTime startTime = Convert.ToDateTime(txtStartDateTime);
            //    DateTime endTime = Convert.ToDateTime(txtEndDateTime);

            //    predicate = predicate.AndAlso(a => a.StartDateTime >= startTime && a.EndDateTime <= endTime);

            //}


            ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

            var q = _meetingService.GetList<MeetingView>(predicate, ConPage);



            return q;
        }

        #endregion


        #region add or edit
        [HttpPost]
        public ActionResult AddMeeting(List<MeetingView> list)
        {
            MeetingInvitationView miv = new MeetingInvitationView();
            MeetingView objModal = new MeetingView();
            MeetingFileView mfv = new MeetingFileView();
            int meetId = 0;



            if (list[0].Id == 0)
            {

                objModal.EnterpriseAppId = list[0].EnterpriseAppId;
                objModal.EnterpriseCorpId = list[0].EnterpriseCorpId;
                objModal.AccountType = 1;//企业号
                objModal.IsLimitPerNumber = false;//是否有人数限制
                objModal.CreatedUser = User.Identity.Name;
                objModal.Title = list[0].Title;
                objModal.Location = list[0].Location;
                objModal.Content = list[0].Content;
                objModal.Owner = list[0].Owner;
                objModal.Request = getPer(list);
                objModal.PersonArray = list[0].PersonArray;
                objModal.TimeSlot = list[0].TimeSlot;

                GetTime(list, objModal, "add");


                _meetingService.InsertView(objModal);//add

                int mid = objModal.Id;//会议id
                meetId = mid;

                #region 向会议邀请人员表中插入数据


                miv.MeetingId = mid;
                miv.Type = "1";//NonEssential（非必须0） Must（必须1）
                miv.State = 0;//0邀请 1接受 2拒绝
                miv.CheckIn = false;//是否签到(默认0)

                #region person
                if (list[0].SendToPerson != null)
                {
                    var perIds = list[0].SendToPerson;
                    var perNames = list[0].SendToPersonName;

                    for (int i = 0; i < perIds.Count; i++)
                    {

                        miv.UserId = perIds[i];
                        miv.UserName = perNames[i];

                        _meetingInvitationService.InsertView(miv);//add 邀请人员
                    }
                }
                #endregion

                #region group
                if (list[0].SendToGroup != null)//group
                {
                    for (int i = 0; i < list[0].SendToGroup.Count; i++)
                    {
                        var groupMembers = GetAllVisiblePersonGroup(list[0].EnterpriseAppId, list[0].SendToGroup[i], int.Parse(list[0].EnterpriseCorpId));
                        if (groupMembers != null)
                        {
                            foreach (var member in groupMembers)
                            {
                                miv.UserId = member.WeixinId;
                                miv.UserName = member.WeixinName;

                                _meetingInvitationService.InsertView(miv);//add 邀请人员
                            }
                        }
                    }
                }
                #endregion

                #region tag
                if (list[0].SendToTag != null)//tag
                {
                    for (int i = 0; i < list[0].SendToTag.Count; i++)
                    {

                        var tagMembers = GetTagMembers(int.Parse(list[0].SendToTag[i]), int.Parse(list[0].EnterpriseAppId));
                        if (tagMembers != null)
                        {
                            foreach (var member in tagMembers.userlist)
                            {
                                miv.UserId = member.userid;
                                miv.UserName = member.name;

                                _meetingInvitationService.InsertView(miv);//add 邀请人员
                            }
                        }

                    }

                }
                #endregion

                #endregion

                #region 向会议资料表中插入数据
                if (list[0].FileName != null && list[0].TargetFilePath != null && list[0].SaveFullName != null)
                {
                    string[] filename = list[0].FileName.Split(',');
                    string[] targetfilepath = list[0].TargetFilePath.Split(',');
                    string[] fullname = list[0].SaveFullName.Split(',');


                    mfv.MeetingId = mid;
                    mfv.CreatedDate = DateTime.Now;

                    for (int i = 0; i < filename.Length; i++)
                    {
                        mfv.FileName = filename[i];
                        mfv.FilePath = Path.Combine(targetfilepath[i], fullname[i]).Replace("\\", "/");

                        _meetingFileService.InsertView(mfv);//add
                    }
                }


                #endregion

            }
            else
            {
                meetId = list[0].Id;
                objModal.Id = list[0].Id;
                objModal.Title = list[0].Title;
                objModal.Location = list[0].Location;
                objModal.Content = list[0].Content;
                objModal.Owner = list[0].Owner;
                objModal.Request = getPer(list);
                objModal.PersonArray = list[0].PersonArray;
                objModal.TimeSlot = list[0].TimeSlot;

                GetTime(list, objModal, "edit");


                _BaseService.UpdateView(objModal);//edit

                #region 修改会议邀请人员表中数据
                int meetingId = list[0].Id;

                //先删除该会议下的所有人员
                //var m_list = _meetingInvitationService.Repository.Entities.Where(m => m.IsDeleted == false && m.MeetingId == meetingId).ToList();
                //foreach (var item in m_list)
                //{

                //    var del_res = _meetingInvitationService.Repository.Delete(item.Id);

                //}




                miv.MeetingId = meetingId;
                miv.Type = "1";//NonEssential（非必须0） Must（必须1）
                miv.State = 0;//0邀请 1接受 2拒绝
                miv.CheckIn = false;//是否签到(默认0)

                #region person
                if (list[0].SendToPerson != null)
                {
                    var perIds = list[0].SendToPerson;
                    var perNames = list[0].SendToPersonName;

                    for (int i = 0; i < perIds.Count; i++)
                    {
                        string userid = perIds[i];
                        var mi = _meetingInvitationService.Repository.Entities.Where(m => m.MeetingId == meetingId && m.UserId == userid && m.IsDeleted == false).FirstOrDefault();
                        if (mi == null)
                        {
                            miv.UserId = perIds[i];
                            miv.UserName = perNames[i];
                            _meetingInvitationService.InsertView(miv);//add 邀请人员
                        }
                        else {
                            mi.State = 0;
                            _meetingInvitationService.Repository.Update(mi);
                        }


                    }
                }
                #endregion

                #region group
                if (list[0].SendToGroup != null)//group
                {
                    for (int i = 0; i < list[0].SendToGroup.Count; i++)
                    {
                        var groupMembers = GetAllVisiblePersonGroup(list[0].EnterpriseAppId, list[0].SendToGroup[i], int.Parse(list[0].EnterpriseCorpId));
                        if (groupMembers != null)
                        {
                            foreach (var member in groupMembers)
                            {
                                string userid = member.WeixinId;
                                var mi = _meetingInvitationService.Repository.Entities.Where(m => m.MeetingId == meetingId && m.UserId == userid && m.IsDeleted == false).FirstOrDefault();
                                if (mi == null)
                                {
                                    miv.UserId = member.WeixinId;
                                    miv.UserName = member.WeixinName;

                                    _meetingInvitationService.InsertView(miv);//add 邀请人员
                                }

                            }
                        }
                    }
                }
                #endregion

                #region tag
                if (list[0].SendToTag != null)//tag
                {
                    for (int i = 0; i < list[0].SendToTag.Count; i++)
                    {

                        var tagMembers = GetTagMembers(int.Parse(list[0].SendToTag[i]), int.Parse(list[0].EnterpriseAppId));
                        if (tagMembers != null)
                        {
                            foreach (var member in tagMembers.userlist)
                            {
                                string userid = member.userid;
                                var mi = _meetingInvitationService.Repository.Entities.Where(m => m.MeetingId == meetingId && m.UserId == userid && m.IsDeleted == false).FirstOrDefault();

                                if (mi == null)
                                {
                                    miv.UserId = member.userid;
                                    miv.UserName = member.name;

                                    _meetingInvitationService.InsertView(miv);//add 邀请人员
                                }

                            }
                        }

                    }

                }
                #endregion

                #endregion

                #region 修改会议资料表中数据
                if (list[0].FileName != null && list[0].TargetFilePath != null && list[0].SaveFullName != null)
                {
                    string[] filename = list[0].FileName.Split(',');
                    string[] targetfilepath = list[0].TargetFilePath.Split(',');
                    string[] fullname = list[0].SaveFullName.Split(',');

                    mfv.MeetingId = meetingId;
                    mfv.CreatedDate = DateTime.Now;

                    for (int i = 0; i < filename.Length; i++)
                    {
                        mfv.FileName = filename[i];
                        mfv.FilePath = Path.Combine(targetfilepath[i], fullname[i]).Replace("\\", "/");

                        _meetingFileService.InsertView(mfv);//add
                    }
                }
                #endregion

            }

            //发送会议邀请
            try
            {
                var wechatBaseUrl = CommonService.GetSysConfig("WeChatUrl", "");
                 //var wechatBaseUrl = "http://localhost:8082/";

                var lstArticle = new List<Article>();
                string appid = list[0].EnterpriseAppId.ToString();

                var meetUrl = string.Format("{0}/WeChatMeeting/MeetingInvitation/MeetingInvitation?AppID={1}&MeetingId={2}&wechatid={3}", wechatBaseUrl, appid, meetId, appid);

                var meetContent = @"<a href='" + meetUrl + "'>" + list[0].Title + "</a>";


                var objConfig = WeChatCommonService.GetWeChatConfigByID(int.Parse(list[0].EnterpriseAppId));
                string strToken = (objConfig.IsCorp != null && !objConfig.IsCorp.Value) ? Innocellence.Weixin.MP.CommonAPIs.AccessTokenContainer.GetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret) : Innocellence.Weixin.QY.CommonAPIs.AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);

                lstArticle.Add(new Article()
                {
                    Title = string.Format("{0} --- 会议邀请", list[0].Title),
                    Description = "",
                    // PicUrl = aiv.ThumbImageId == null ? wechatBaseUrl+"/Content/img/LogoRed.png" : string.Format("{0}/Common/PushFile?id={1}&FileName={2}", wechatBaseUrl, aiv.ThumbImageId, aiv.ThumbImageUrl),
                    //PicUrl = objModel.ImageCoverUrl == null ? wechatBaseUrl + "Content/img/LogoRed.png" : string.Format("{0}{1}", wechatBaseUrl, objModel.ImageCoverUrl),

                    PicUrl = string.Format("{0}/Content/img/meeting.jpg", wechatBaseUrl),
                    Url = meetUrl
                });

                _Logger.Debug("{0} meetUrl：{1}", list[0].Title, meetUrl);
                MassApi.SendNews(strToken, getPer(list), "", "", objConfig.WeixinAppId.ToString(), lstArticle, 0);

            }
            catch (Exception ex)
            {
                _Logger.Debug(ex);
                return Json(new { results = new { Data = 500 } });

            }

            return Json(new { results = new { Data = 200 } });
        }


        #endregion

        #region 获取人员信息
        private string getPer(List<MeetingView> list)
        {
            var userid = string.Empty;

            #region person
            if (list[0].SendToPerson != null)//person
            {
                var perIds = list[0].SendToPerson;
                var perNames = list[0].SendToPersonName;

                for (int i = 0; i < perIds.Count; i++)
                {
                    if (string.IsNullOrEmpty(userid))
                    {
                        userid = perIds[i];
                    }
                    else
                    {
                        userid = userid + '|' + perIds[i];
                    }

                }

            }
            #endregion

            #region group
            if (list[0].SendToGroup != null)//group
            {
                for (int i = 0; i < list[0].SendToGroup.Count; i++)
                {
                    var groupMembers = GetAllVisiblePersonGroup(list[0].EnterpriseAppId, list[0].SendToGroup[i], int.Parse(list[0].EnterpriseCorpId));
                    if (groupMembers != null)
                    {
                        foreach (var member in groupMembers)
                        {
                            if (string.IsNullOrEmpty(userid))
                            {
                                userid = member.WeixinId;
                            }
                            else
                            {
                                userid = userid + '|' + member.WeixinId;
                            }
                        }
                    }
                }
            }
            #endregion

            #region tag
            if (list[0].SendToTag != null)//tag
            {
                for (int i = 0; i < list[0].SendToTag.Count; i++)
                {

                    var tagMembers = GetTagMembers(int.Parse(list[0].SendToTag[i]), int.Parse(list[0].EnterpriseAppId));
                    if (tagMembers != null)
                    {
                        foreach (var member in tagMembers.userlist)
                        {
                            if (string.IsNullOrEmpty(userid))
                            {
                                userid = member.userid;
                            }
                            else
                            {
                                userid = userid + '|' + member.userid;
                            }
                        }
                    }

                }

            }
            #endregion

            return userid;
        }
        #endregion

        #region 获取标签下人员
        public static GetTagMemberResult GetTagMembers(int tagId, int appId)
        {
            return MailListApi.GetTagMember(WeChatCommonService.GetWeiXinToken(appId), tagId);
        }
        #endregion

        #region 获取组下人员
        public List<PersonGroup> GetAllVisiblePersonGroup(string appId, string groupId, int accountManageId)
        {
            List<PersonGroup> list = new List<PersonGroup>();

            var id = int.Parse(appId);
            SysWechatConfig appInfo = null;
            if (id == 0)
            {
                appInfo = WeChatCommonService.lstSysWeChatConfig.SingleOrDefault(a => a.WeixinAppId == "0" && a.AccountManageId == accountManageId);
            }
            else
            {
                appInfo = WeChatCommonService.lstSysWeChatConfig.SingleOrDefault(a => a.Id == id && a.AccountManageId == accountManageId);
            }

            string token = (appInfo.IsCorp != null && !appInfo.IsCorp.Value) ? Innocellence.Weixin.MP.CommonAPIs.AccessTokenContainer.GetToken(appInfo.WeixinCorpId, appInfo.WeixinCorpSecret) : Innocellence.Weixin.QY.CommonAPIs.AccessTokenContainer.TryGetToken(appInfo.WeixinCorpId, appInfo.WeixinCorpSecret);

            var members = MailListApi.GetDepartmentMemberInfo(token, Int32.Parse(groupId), 0, 1);

            foreach (var member in members.userlist)
            {
                PersonGroup person = new PersonGroup();
                person.WeixinId = member.userid;
                person.WeixinName = member.name;
                list.Add(person);
            }

            return list;

        }
        #endregion

        #region 上下文对象
        public class PerGroup
        {
            public virtual int Id { get; set; }
            public virtual List<String> SendToGroup { get; set; }
            public virtual List<String> SendToPerson { get; set; }
            public virtual List<String> SendToPersonName { get; set; }

            public virtual List<String> SendToGroupName { get; set; }
            public virtual List<String> SendToTag { get; set; }
            public virtual List<String> SendToTagName { get; set; }

            public virtual String PersonArray { get; set; }
            public virtual String Title { get; set; }
            public virtual String Location { get; set; }
            //public virtual DateTime? StartDateTime { get; set; }
            //public virtual DateTime? EndDateTime { get; set; }
            public virtual DateTime? SignUpStartTime { get; set; }
            public virtual DateTime? SignUpEndTime { get; set; }
            public virtual String Owner { get; set; }

            public virtual Boolean? IsSignUp { get; set; }
            public virtual String Content { get; set; }
            public virtual String EnterpriseAppId { get; set; }
            public virtual String EnterpriseCorpId { get; set; }
            public virtual String AccountType { get; set; }
            public virtual String CreatedUser { get; set; }

        }

        public class timeContext
        {
            public virtual DateTime? StartDateTime { get; set; }
            public virtual DateTime? EndDateTime { get; set; }
            public virtual DateTime? SignStartTime { get; set; }
            public virtual DateTime? SignEndTime { get; set; }
            public virtual string Type { get; set; }
        }


        #endregion

        #region 上传文件
        public ActionResult Uploadfile()
        {
            try
            {
                string thumbNailPath = string.Empty;
                string allFilesName = string.Empty;
                string targetFilePath = string.Empty;
                string serverFileName = string.Empty;
                string duration = string.Empty;
                string realFileName = string.Empty;

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] != null)
                    {
                        FileInfo fi = new FileInfo(Request.Files[i].FileName);
                        realFileName = Request.Files[i].FileName;
                        serverFileName = Guid.NewGuid().ToString().Replace("-", "") + fi.Extension;
                        string uploadFileType = Request.QueryString["type"].ToLower();
                        targetFilePath = InitTargetFilePathDir(uploadFileType);
                        //上传原文件
                        ProcessPostedFile(serverFileName, Request.Files[i], targetFilePath.Trim('/'), uploadFileType);

                        if (string.IsNullOrEmpty(allFilesName))
                        {
                            allFilesName = serverFileName;
                        }
                        else
                        {
                            allFilesName = allFilesName + "," + serverFileName;
                        }
                    }
                    else
                    {
                        return Json(new { success = false, timeout = false },
                            StrJsonTypeTextHtml);
                    }
                }
                return Json(new { success = true, timeout = false, serverFileName = serverFileName, targetFilePath = targetFilePath, duration = duration, realFileName = realFileName },
                    StrJsonTypeTextHtml);
            }
            catch (Exception ex)
            {
            }
            return Json(new { success = false, timeout = false },
                StrJsonTypeTextHtml);
        }


        private string InitTargetFilePathDir(string uploadFileType)
        {
            CheckUploadFolderPath();
            string targetFilePath = string.Empty;

            targetFilePath = UploadFilePath;

            string saveDir = Server.MapPath("~/");
            DateTime now = DateTime.Now;
            List<string> fix = new List<string>()
            {
                targetFilePath,
                now.Year.ToString(),
                now.Month.ToString(),
                now.Day.ToString()
            };
            for (int i = 0; i < fix.Count; i++)
            {
                if (i != 0)
                {
                    targetFilePath = Path.Combine(targetFilePath, fix[i]);
                }
                string path = Path.Combine(saveDir, targetFilePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            targetFilePath = targetFilePath.Replace("\\", "/");
            return targetFilePath + "/";
        }


        private void CheckUploadFolderPath()
        {
            try
            {
                //string uploadFolderName = WebConfigurationManager.AppSettings["UploadFolderName"];
                string uploadFolderName = CommonService.GetSysConfig("UploadFolderName", "");

                List<string> fix = new List<string>()
                {
                    "/Content",
                    uploadFolderName,
                    "Meeting",
                    
                };
                string path = Server.MapPath("~/");
                try
                {
                    fix.ForEach(f =>
                    {
                        path = Path.Combine(path, f);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    });
                }
                catch (Exception)
                {

                }
                UploadFilePath = Path.Combine(fix[0], fix[1], fix[2]);
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }
        }


        private void ProcessPostedFile(string saveFullName, HttpPostedFileBase postedFile, string targetFilePath, string uploadFileType)
        {
            try
            {
                string saveDir = Path.Combine(Server.MapPath("~/"), targetFilePath);
                if (Directory.Exists(saveDir) == false)//如果不存在就创建file文件夹 
                {
                    Directory.CreateDirectory(saveDir);
                }
                if (postedFile != null)
                {
                    postedFile.SaveAs(Path.Combine(saveDir, saveFullName));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 删除会议资料
        public ActionResult DelFile(string id)
        {
            _meetingFileService.Repository.Delete(int.Parse(id));

            return Json("successful");
        }
        #endregion

        #region 生成二维码
        //public ActionResult ShowQRCode(string meetingId, string appid)
        //{

        //    #region
        //    //QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
        //    //QrCode qrCode = qrEncoder.Encode(mUrl);
        //    ////保存成png文件
        //    //string filename = @"D:\封面图\url.png";
        //    //GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
        //    //using (FileStream stream = new FileStream(filename, FileMode.Create))
        //    //{
        //    //    render.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
        //    //}
        //    #endregion

        //    return Json("successful");

        //}


        public ActionResult ShowQRCodeImg(string meetingId, string appid)
        {
            var wechatBaseUrl = CommonService.GetSysConfig("WeChatUrl", "");
            //WebConfigurationManager.AppSettings["WebUrl"]

            string mUrl = string.Format("{0}/WeChatMeeting/MeetingInvitation/ScanResult?AppID={1}&MeetingId={2}&wechatid={3}"
               , wechatBaseUrl, appid, meetingId, appid);

            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(mUrl, out qrCode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(10, QuietZoneModules.Four), System.Drawing.Brushes.Black, System.Drawing.Brushes.White);



            string filename = HttpUtility.UrlEncode(meetingId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png", Encoding.GetEncoding("UTF-8"));

            string saveDir = Path.Combine(Server.MapPath("~/"), "Content/QRcode/");
            if (Directory.Exists(saveDir) == false)//如果不存在就创建file文件夹 
            {
                Directory.CreateDirectory(saveDir);
            }



            using (FileStream stream = new FileStream(saveDir + filename, FileMode.Create))
            {
                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);

                //return File(stream, "image/jpeg", filename);

            }

            ViewBag.QRCodeImg = "/Content/QRcode/" + filename;
            // return Json("/Content/QRcode/" + filename);

            return View();
        }


        #endregion


        #region 会议邀请人员加载
        public ActionResult Detail()
        {
            int meetingId = Convert.ToInt32(Request["meetingId"]);
            int appId = Convert.ToInt32(Request["appid"]);
            ViewBag.meetingId = meetingId;
            ViewBag.AppId = appId;

            return View();
        }
        #endregion

        #region 会议邀请人员列表
        public ActionResult GetInvitationList()
        {

            int meetId = Convert.ToInt32(Request["meetingId"]);
            string userName = Request["txtUserName"];
            string checkInDt = Request["txtCheckInDt"];
            string flag = Request["IscheckIn"];

            Expression<Func<MeetingInvitation, bool>> predicate;
            predicate = m => m.MeetingId == meetId && m.IsDeleted == false;

            if (!string.IsNullOrEmpty(flag))
            {
                if (flag == "1")
                {
                    predicate = predicate.AndAlso(m => m.CheckIn == false);
                }
                else if (flag == "2")
                {
                    predicate = predicate.AndAlso(m => m.CheckIn == true);
                }

            }

            if (!string.IsNullOrEmpty(userName))
            {
                predicate = predicate.AndAlso(m => m.UserName.Contains(userName));
            }

            if (!string.IsNullOrEmpty(checkInDt))
            {
                DateTime dt = Convert.ToDateTime(checkInDt);
                DateTime dateAdd = dt.AddDays(1);
                predicate = predicate.AndAlso(m => m.CheckInDt >= dt && m.CheckInDt <= dateAdd);

            }


            var param = new PageParameter();
            TryUpdateModel(param);
            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            param.length = param.length == 0 ? 10 : param.length;
            var iTotal = param.iRecordsTotal;


            var invitationList = _meetingInvitationService.Repository.Entities.Where(predicate).ToList();



            // 取当前页
            var list = new List<MeetingInvitation>();
            var page = (int)Math.Floor(param.start / param.length * 1.0d) + 1;
            if (invitationList.Count != 0)
            {
                iTotal = invitationList.Count();

                list = invitationList.OrderBy(a => a.CheckInDt).Skip((page - 1) * param.length).Take(param.length).ToList();
            }


            var t = from m in list
                    select new
                    {
                        Id = m.Id,
                        MeetingId = m.MeetingId,
                        Type = m.Type,
                        UserId = m.UserId,
                        UserName = m.UserName,
                        State = m.State,
                        CheckIn = m.CheckIn,
                        CheckInDt = m.CheckInDt,
                        StartDateTime = m.StartDateTime,
                        EndDateTime = m.EndDateTime,
                        SignStartTime = m.SignStartTime,
                        SignEndTime = m.SignEndTime,
                        TimeClass = m.TimeClass,
                        Operation = ""

                    };

            return Json(new
            {
                sEcho = param.draw,
                iTotalRecords = iTotal,
                iTotalDisplayRecords = iTotal,
                aaData = t
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region 获取会议开始时间最早的一组
        private void GetTime(List<MeetingView> list, MeetingView objModal, string flag)
        {
            var timelist = JsonConvert.DeserializeObject<List<timeContext>>(list[0].TimeSlot);
            DateTime t = DateTime.Now;

            if (timelist.Count == 1)
            {
                objModal.StartDateTime = timelist[0].StartDateTime;
                objModal.EndDateTime = timelist[0].EndDateTime;
            }
            else
            {
                for (int i = 0; i < timelist.Count - 1; i++)
                {

                    for (int j = i + 1; j < timelist.Count; j++)
                    {
                        if (timelist[i].StartDateTime < timelist[j].StartDateTime)
                        {
                            t = Convert.ToDateTime(timelist[i].StartDateTime);
                            timelist[i].StartDateTime = timelist[j].StartDateTime;
                            timelist[j].StartDateTime = t;

                        }
                        else
                        {
                            t = Convert.ToDateTime(timelist[j].StartDateTime);
                            timelist[j].StartDateTime = timelist[i].StartDateTime;
                            timelist[i].StartDateTime = t;
                        }
                    }
                }


                var timelist1 = JsonConvert.DeserializeObject<List<timeContext>>(list[0].TimeSlot);

                for (int k = 0; k < timelist1.Count; k++)
                {
                    if (flag == "add")
                    {
                        if (timelist1[k].StartDateTime.ToString().Substring(0, 14) == t.ToString().Substring(0, 14))
                        {
                            objModal.StartDateTime = t;
                            objModal.EndDateTime = timelist1[k].EndDateTime;
                        }
                    }
                    else
                    {
                        if (timelist1[k].StartDateTime == t)
                        {
                            objModal.StartDateTime = timelist1[k].StartDateTime;
                            objModal.EndDateTime = timelist1[k].EndDateTime;
                        }
                    }

                }


            }
        }
        #endregion

        #region 出行信息加载
        public ActionResult TravelInfo()
        {
            int meetingId = Convert.ToInt32(Request["meetingId"]);
            int appId = Convert.ToInt32(Request["appid"]);
            ViewBag.meetingId = meetingId;
            ViewBag.AppId = appId;

            return View();
        }
        #endregion

        #region 出行信息列表
        public ActionResult GetTravelList()
        {
            int meetId = Convert.ToInt32(Request["meetingId"]);
            string userName = Request["txtUserName"];
            string departureTime = Request["txtDepartureTime"];
            string arrivalTime = Request["txtArrivalTime"];
            string mode = Request["txtMode"];

            Expression<Func<MeetingTravelInfo, bool>> predicate;
            predicate = m => m.MeetingId == meetId && m.IsDeleted == false;

            if (!string.IsNullOrEmpty(mode) && mode != "0")
            {
                predicate = predicate.AndAlso(m => m.Mode == mode);
            }

            if (!string.IsNullOrEmpty(userName))
            {
                predicate = predicate.AndAlso(m => m.UserName == userName);
            }

            if (!string.IsNullOrEmpty(departureTime) && string.IsNullOrEmpty(arrivalTime))
            {
                DateTime sTime = Convert.ToDateTime(departureTime);
                DateTime dateAdd = sTime.AddDays(1);
                predicate = predicate.AndAlso(a => a.DepartureTime >= sTime && a.DepartureTime <= dateAdd);
            }

            if (!string.IsNullOrEmpty(arrivalTime) && string.IsNullOrEmpty(departureTime))
            {
                DateTime eTime = Convert.ToDateTime(arrivalTime);
                DateTime dateAdd = eTime.AddDays(1);
                predicate = predicate.AndAlso(a => a.ArrivalTime >= eTime && a.ArrivalTime <= dateAdd);
            }


            if (!string.IsNullOrEmpty(departureTime) && !string.IsNullOrEmpty(arrivalTime))
            {
                DateTime startTime = Convert.ToDateTime(departureTime);
                DateTime endTime = Convert.ToDateTime(arrivalTime);

                predicate = predicate.AndAlso(a => a.DepartureTime >= startTime && a.ArrivalTime <= endTime);

            }


            var param = new PageParameter();
            TryUpdateModel(param);
            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            param.length = param.length == 0 ? 10 : param.length;
            var iTotal = param.iRecordsTotal;


            var travelList = _meetingTravelInfoService.Repository.Entities.Where(predicate).ToList();



            // 取当前页
            var list = new List<MeetingTravelInfo>();
            var page = (int)Math.Floor(param.start / param.length * 1.0d) + 1;
            if (travelList.Count != 0)
            {
                iTotal = travelList.Count();

                list = travelList.OrderBy(a => a.CreatedDate).Skip((page - 1) * param.length).Take(param.length).ToList();
            }


            var t = from m in list
                    select new
                    {
                        Id = m.Id,
                        MeetingId = m.MeetingId,
                        UserId = m.UserId,
                        UserName = m.UserName,
                        Mode = m.Mode,
                        Flight_Train = m.Flight_Train,
                        DepartureTime = m.DepartureTime,
                        ArrivalTime = m.ArrivalTime,
                        ExpectArrivalTime = m.ExpectArrivalTime,
                        CreatedDate = m.CreatedDate,
                        Remarks = m.Remarks,
                        IsDeleted = m.IsDeleted

                    };

            return Json(new
            {
                sEcho = param.draw,
                iTotalRecords = iTotal,
                iTotalDisplayRecords = iTotal,
                aaData = t
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取报名人员详情
        public ActionResult GetSignUpInfo(string userid)
        {
            var perInfo = _meetingPerInfoService.Repository.Entities.Where(m => m.UserId == userid && m.IsDeleted == false).FirstOrDefault();
            if (perInfo != null)
            {
                object json = new
                {
                    Name = perInfo.Name,
                    Mailbox = perInfo.Mailbox,
                    ContactPhone = perInfo.ContactPhone,
                    AssistantName = perInfo.AssistantName,
                    AssistantPhone = perInfo.AssistantPhone,
                    EmergencyContact = perInfo.EmergencyContact,
                    EmergencyConPhone = perInfo.EmergencyConPhone,
                    CreatedDate = perInfo.CreatedDate
                };

                return Json(json);
            }
            return null;
        }
        #endregion



       




    }
}