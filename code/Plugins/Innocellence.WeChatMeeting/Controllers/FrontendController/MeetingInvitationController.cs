using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Innocellence.WeChat.Domain;
using Innocellence.WeChatMeeting.Domain.Entity;
using Innocellence.WeChatMeeting.Domain.ViewModel;
using Innocellence.WeChatMeeting.Domain.Service;
using System.Web.Mvc;
using System.IO;
using Infrastructure.Web.Domain.Service;
using Newtonsoft.Json;

namespace Innocellence.WeChatMeeting.Controllers
{
    public class MeetingInvitationController : WeChatBaseController<MeetingInvitation, MeetingInvitationView>
    {
        private IMeetingInvitationService _meetingInvitationService;
        private IMeetingService _meetingService;
        private IMeetingFileService _meetingFileService;
        private IMeetingTravelInfoService _meetingTravelInfoService;
        private IMeetingPerInfoService _meetingPerInfoService;

        public MeetingInvitationController(IMeetingInvitationService objService)
            : base(objService)
        {
            _meetingInvitationService = objService;
        }

        public MeetingInvitationController(IMeetingInvitationService objService, IMeetingService meetingService, IMeetingFileService meetingFileService, IMeetingTravelInfoService meetingTravelInfoService, IMeetingPerInfoService meetingPerInfoService)
            : base(objService)
        {
            _meetingInvitationService = objService;
            _meetingService = meetingService;
            _meetingFileService = meetingFileService;
            _meetingTravelInfoService = meetingTravelInfoService;
            _meetingPerInfoService = meetingPerInfoService;
        }



        #region 签到成功页面
        public ActionResult ScanResult(int? appId, string MeetingId)
        {
            int meetingId = int.Parse(MeetingId);

            MeetingInvitationView miv = new MeetingInvitationView();
            string userid = ViewBag.WeChatUserID;


            if (!string.IsNullOrEmpty(userid))
            {
                var meet = _meetingService.Repository.Entities.Where(m => m.Id == meetingId && m.IsDeleted == false).FirstOrDefault();
                if (meet != null)//判断会议是否存在
                {


                    //根据会议id，用户id 获取数据
                    var mivInfo = _meetingInvitationService.Repository.Entities.Where(m => m.MeetingId == meetingId && m.UserId == userid && m.IsDeleted == false).FirstOrDefault();
                    if (mivInfo != null)//判断当前用户是否接收到会议邀请
                    {
                        //if (endtime > nowtime)//判断会议是否结束
                        // {

                        if (mivInfo.State == 0) { ViewBag.Res = "对不起，请在会议邀请页面确认报名才能签到！"; }//0 接收到会议邀请但没有确认报名，1 确认报名成功
                        else
                        {
                            //var endtime = mivInfo.EndDateTime; //会议结束时间
                            var signStartTime = mivInfo.SignStartTime;//签到开始时间
                            var signEndTime = mivInfo.SignEndTime;//签到结束时间
                            var nowtime = DateTime.Now;

                            if (!string.IsNullOrEmpty(signStartTime.ToString()) && signEndTime.ToString() == "")//只有签到开始时间
                            {

                                if (nowtime >= signStartTime)
                                {
                                    GetSignInfo(miv, mivInfo, meet);
                                }
                                else
                                {
                                    SetCommonInfo(mivInfo, meet);
                                    ViewBag.Res = "签到未开始！";
                                    
                                }
                            }

                            if (!string.IsNullOrEmpty(signEndTime.ToString()) && signStartTime.ToString() == "")//只有签到结束时间
                            {
                                if (nowtime <= signEndTime)
                                {
                                    GetSignInfo(miv, mivInfo, meet);
                                }
                                else
                                {
                                    SetCommonInfo(mivInfo, meet);
                                    ViewBag.Res = "签到已结束！";

                                }
                            }

                            if (!string.IsNullOrEmpty(signStartTime.ToString()) && !string.IsNullOrEmpty(signEndTime.ToString()))//签到开始结束时间都存在
                            {
                                if (nowtime < signStartTime && mivInfo.CheckInDt == null && mivInfo.CheckIn == false)
                                {
                                    SetCommonInfo(mivInfo, meet);
                                    ViewBag.Res = "签到未开始！";
                                }
                                else if (nowtime > signEndTime && mivInfo.CheckInDt == null && mivInfo.CheckIn == false)
                                {
                                    SetCommonInfo(mivInfo, meet);
                                    ViewBag.Res = "签到已结束！";

                                }
                                else
                                {
                                    GetSignInfo(miv, mivInfo, meet);
                                }
                            }

                            if (string.IsNullOrEmpty(signStartTime.ToString()) && string.IsNullOrEmpty(signEndTime.ToString()))
                            {
                                GetSignInfo(miv, mivInfo, meet);

                            }


                            // }
                            //else
                            //{
                            //    ViewBag.Res = "会议已结束！";
                            //}
                        }
                    }
                    else
                    {
                        ViewBag.Res = "对不起,您可能没有接收到会议邀请！";
                    }
                }
                else
                {
                    ViewBag.Res = "对不起,此会议已删除或不存在！";
                }

            }
            else
            {
                return Redirect("/noCropPermission.html");
            }

            return View("../MeetingInvitation/ScanResult");
        }

        private void SetCommonInfo(MeetingInvitation mivInfo, Meeting meet)
        {
            ViewBag.Title = meet.Title;
            ViewBag.StartDateTime = mivInfo.StartDateTime;
            ViewBag.EndDateTime = mivInfo.EndDateTime;
            ViewBag.Location = meet.Location;
            ViewBag.Owner = meet.Owner;
        }
        private void GetSignInfo(MeetingInvitationView miv, MeetingInvitation mivInfo, Meeting meet)
        {
            #region
            if (mivInfo.CheckInDt == null && mivInfo.CheckIn == false)//判断是否签到过
            {

                miv.Id = mivInfo.Id;
                miv.CheckIn = true;
                miv.CheckInDt = DateTime.Now;

                _meetingInvitationService.UpdateView(miv);

                ViewBag.Res = "签到成功！";
                ViewBag.Title = meet.Title;
                ViewBag.StartDateTime = mivInfo.StartDateTime;
                ViewBag.EndDateTime = mivInfo.EndDateTime;
                ViewBag.Location = meet.Location;
                ViewBag.Owner = meet.Owner;

            }
            else
            {
                ViewBag.Res = "您已签到过！";
                ViewBag.Title = meet.Title;
                ViewBag.StartDateTime = mivInfo.StartDateTime;
                ViewBag.EndDateTime = mivInfo.EndDateTime;
                ViewBag.Location = meet.Location;
                ViewBag.Owner = meet.Owner;
            }
            #endregion
        }



        #endregion

        #region 会议邀请详情页（不用）
        //public ActionResult InvitationDetail()
        //{
        //    string appId = Request["appid"];
        //    ViewBag.AppID = appId;

        //    int meetingId = Convert.ToInt32(Request["MeetingId"]);

        //    var meetInfo = _meetingService.Repository.Entities.Where(m => m.IsDeleted == false && m.Id == meetingId).FirstOrDefault();
        //    if (meetInfo != null)
        //    {
        //        ViewBag.Title = meetInfo.Title;
        //        ViewBag.Location = meetInfo.Location;
        //        ViewBag.Owner = meetInfo.Owner;
        //        ViewBag.StartDateTime = meetInfo.StartDateTime;
        //        ViewBag.EndDateTime = meetInfo.EndDateTime;

        //        if (!string.IsNullOrEmpty(meetInfo.SignStartTime.ToString()))
        //        {
        //            ViewBag.SignStartTime = meetInfo.SignStartTime;
        //        }
        //        else { ViewBag.SignStartTime = ""; }
        //        if (!string.IsNullOrEmpty(meetInfo.SignEndTime.ToString()))
        //        {
        //            ViewBag.SignEndTime = meetInfo.SignEndTime;
        //        }
        //        else { ViewBag.SignEndTime = ""; }

        //        //会议资料
        //        var meetFile = _meetingFileService.Repository.Entities.Where(f => f.MeetingId == meetingId && f.IsDeleted == false).ToList();
        //        if (meetFile.Count > 0)
        //        {
        //            var wechatBaseUrl = CommonService.GetSysConfig("WeChatUrl", "");
        //            ViewBag.MeetFile = meetFile;
        //            ViewBag.FileUrl = wechatBaseUrl;
        //        }
        //        else
        //        {
        //            ViewBag.MeetFile = "";
        //        }


        //    }

        //    return View("../MeetingInvitation/InvitationDetail");
        //}
        #endregion


        #region 确认报名（会议邀请load）
        public ActionResult MeetingInvitation()
        {
            int meetingId = Convert.ToInt32(Request["MeetingId"]);
            string TimeClass = Request["TimeClass"];
            string appid = Request["AppID"];
            string userid = ViewBag.WeChatUserID;
            ViewBag.AppId = appid;
            var checkhasRecord = _meetingInvitationService.Repository.Entities.Where(x => x.MeetingId == meetingId && x.IsDeleted == false && x.UserId == userid && x.State == 1 &&!string.IsNullOrEmpty(x.TimeClass)).Any();

            if (checkhasRecord && string.IsNullOrEmpty(TimeClass))
            {
                //var detailUrl = string.Format("http://{3}/WeChatMeeting/MeetingInvitation/InvitationDetail?MeetId={0}&AppID={1}&wechatid={2}", meetingId, appid, appid, Request.Url.Host);
                //_Logger.Debug(detailUrl);
                //Redirect(detailUrl);
                return RedirectToAction("InvitationDetail", "MeetingInvitation", new { MeetId = meetingId, AppID = appid, wechatid = appid });
            }
            else
            {
                if (!string.IsNullOrEmpty(TimeClass))
                {
                    ViewBag.TimeClass = TimeClass;
                }
                else
                {
                    ViewBag.TimeClass = "";
                }

                //int meetingId = 71;
                ViewBag.MeetId = meetingId;

                var meetInfo = _meetingService.Repository.Entities.Where(m => m.IsDeleted == false && m.Id == meetingId).FirstOrDefault();
                if (meetInfo != null)
                {
                    ViewBag.Title = meetInfo.Title;
                    ViewBag.Location = meetInfo.Location;
                    ViewBag.Owner = meetInfo.Owner;
                    ViewBag.TimeSlot = JsonConvert.DeserializeObject(meetInfo.TimeSlot);


                    //会议资料
                    var meetFile = _meetingFileService.Repository.Entities.Where(f => f.MeetingId == meetingId && f.IsDeleted == false).ToList();
                    if (meetFile.Count > 0)
                    {
                        var wechatBaseUrl = CommonService.GetSysConfig("WeChatUrl", "");
                        ViewBag.MeetFile = meetFile;
                        ViewBag.FileUrl = wechatBaseUrl;
                    }
                    else
                    {
                        ViewBag.MeetFile = "";
                    }
                }

                return View();
            }
            

        }
        #endregion

        #region 确认报名（update MeetingInvitation data）
        public ActionResult ConfirmSignUp(string data, int meetid)
        {
            // string userid = "13555989031"; //ViewBag.WeChatUserID;
            string userid = ViewBag.WeChatUserID;
            try
            {
                var objModal = _meetingInvitationService.Repository.Entities.Where(m => m.UserId == userid && m.MeetingId == meetid && m.IsDeleted == false).FirstOrDefault();
                if (objModal != null)
                {
                    string[] array = data.Split(',');

                    objModal.StartDateTime = Convert.ToDateTime(array[0]);
                    objModal.EndDateTime = Convert.ToDateTime(array[1]);
                    objModal.SignStartTime = Convert.ToDateTime(array[2]);
                    objModal.SignEndTime = Convert.ToDateTime(array[3]);
                    objModal.TimeClass = array[4];
                    objModal.State = 1;

                    _meetingInvitationService.Repository.Update(objModal);
                }
            }
            catch (Exception ex)
            {
                return Json(new { results = new { Data = 500 } });
            }



            return Json(new { results = new { Data = 200 } });
        }
        #endregion

        #region 会议邀请详情页
        public ActionResult InvitationDetail(int MeetId,string AppID)
        {
            // string userid = "13555989031"; //ViewBag.WeChatUserID;
            string userid = ViewBag.WeChatUserID;

            int meetingId = MeetId;
            ViewBag.MeetId = meetingId;

            string appid = AppID;
            ViewBag.AppId = appid;

            _Logger.Debug("has in Invitation Detail");
            var meetInfo = _meetingService.Repository.Entities.Where(m => m.IsDeleted == false && m.Id == meetingId).FirstOrDefault();
            if (meetInfo != null)
            {
                ViewBag.Title = meetInfo.Title;
                ViewBag.Location = meetInfo.Location;
                ViewBag.Owner = meetInfo.Owner;

                //会议资料
                var meetFile = _meetingFileService.Repository.Entities.Where(f => f.MeetingId == meetingId && f.IsDeleted == false).ToList();
                if (meetFile.Count > 0)
                {
                    var wechatBaseUrl = CommonService.GetSysConfig("WeChatUrl", "");
                    ViewBag.MeetFile = meetFile;
                    ViewBag.FileUrl = wechatBaseUrl;
                }
                else
                {
                    ViewBag.MeetFile = "";
                }

                //会议邀请人员信息
                var meetInvitation = _meetingInvitationService.Repository.Entities.Where(m => m.IsDeleted == false && m.MeetingId == meetingId && m.UserId == userid && m.State == 1).FirstOrDefault();
                if (meetInvitation != null)
                {
                    ViewBag.StartDateTime = meetInvitation.StartDateTime;
                    ViewBag.EndDateTime = meetInvitation.EndDateTime;
                    ViewBag.SignStartTime = meetInvitation.SignStartTime;
                    ViewBag.SignEndTime = meetInvitation.SignEndTime;
                    ViewBag.TimeClass = meetInvitation.TimeClass;

                }
            }

            return View();
        }
        #endregion

        #region 行程信息加载
        public ActionResult Travel()
        {
            int meetingid = Convert.ToInt32(Request["meetingid"]);
            //string userid = "13555989031"; //ViewBag.WeChatUserID;
            string userid = ViewBag.WeChatUserID;

            ViewBag.MeetingId = meetingid;
            string appid = Request["AppID"];
            ViewBag.AppId = appid;

            //每个人每个会议最多可填8条行程信息
            var travelInfo = _meetingTravelInfoService.Repository.Entities.Where(m => m.MeetingId == meetingid
                && m.UserId == userid && m.IsDeleted == false).ToList();

            if (travelInfo.Count > 0)
            {

                ViewBag.TravelList = JsonConvert.SerializeObject(travelInfo);
            }
            else
            {
                ViewBag.TravelList = "";
            }

            return View();
        }
        #endregion

        #region 添加行程信息
        public ActionResult AddTravelInfo(List<MeetingTravelInfoView> list)
        {
            MeetingTravelInfoView objModel = new MeetingTravelInfoView();
            //string userid = "13555989031"; //ViewBag.WeChatUserID;
            //string username = "tx";//ViewBag.WeChatUserName; 

            string userid = ViewBag.WeChatUserID;
            string username = ViewBag.WeChatUserName;


            try
            {
                int meetingId = (int)list[0].MeetingId;
                //每个人每个会议最多可填8条行程信息
                var travelList = _meetingTravelInfoService.Repository.Entities.Where(m => m.UserId == userid && m.IsDeleted == false && m.MeetingId== meetingId);
                if (travelList.Any())
                {
                    if (travelList.ToList().Count() > 8)
                    {
                        return Json(new { results = new { Data = 500, Message = "您最多可以添加8条行程信息" } });
                    }
                    
                }

                foreach (var item in list[0].MeetingTravelInfoViews)
                {
                    objModel.MeetingId = list[0].MeetingId;
                    objModel.UserId = userid;
                    objModel.UserName = username;
                    objModel.Mode = item.Mode.Trim();
                    objModel.Flight_Train = item.Flight_Train;
                    objModel.DepartureTime = item.DepartureTime;
                    objModel.ArrivalTime = item.ArrivalTime;
                    objModel.ExpectArrivalTime = item.ExpectArrivalTime;
                    objModel.CreatedDate = DateTime.Now;
                    objModel.IsDeleted = false;
                    objModel.Type = item.Type;

                    _meetingTravelInfoService.InsertView(objModel);
                }



            }
            catch (Exception ex)
            {
                _Logger.Debug(ex);
                return Json(new { results = new { Data = 500, Message = "系统错误，请联系管理员" } });
            }

            return Json(new { results = new { Data = 200 } });

        }
        #endregion

        #region 报名信息加载
        public ActionResult SignUp()
        {
            // string userid = "13555989031"; //ViewBag.WeChatUserID;
            string userid = ViewBag.WeChatUserID;

            string appid = Request["AppID"];
            ViewBag.AppId = appid;

            var perInfo = _meetingPerInfoService.Repository.Entities.Where(m => m.UserId == userid && m.IsDeleted == false).FirstOrDefault() == null
                ? new MeetingPerInfo() : _meetingPerInfoService.Repository.Entities.Where(m => m.UserId == userid && m.IsDeleted == false).FirstOrDefault();


            return View(perInfo);
        }
        #endregion

        #region 添加报名信息
        public ActionResult AddSignUpInfo(string per)
        {
            MeetingPerInfoView perInfo = new MeetingPerInfoView();
            try
            {
                _Logger.Debug("per :{0}", per);
                string userid = ViewBag.WeChatUserID;
                _Logger.Debug("userid:{0}", userid);
                var objModel = _meetingPerInfoService.Repository.Entities.Where(m => m.IsDeleted == false && m.UserId == userid).FirstOrDefault();
                string[] arry = per.Split(',');

                if (objModel == null)//add
                {
                    perInfo.UserId = userid;
                    perInfo.Name = arry[0];
                    perInfo.ContactPhone = arry[1];
                    perInfo.Mailbox = arry[2];
                    perInfo.AssistantName = arry[3];
                    perInfo.AssistantPhone = arry[4];
                    perInfo.EmergencyContact = arry[5];
                    perInfo.EmergencyConPhone = arry[6];
                    perInfo.CreatedDate = DateTime.Now;
                    perInfo.IsDeleted = false;
                    _meetingPerInfoService.InsertView(perInfo);
                }
                else
                { //edit
                    objModel.Name = arry[0];
                    objModel.ContactPhone = arry[1];
                    objModel.Mailbox = arry[2];
                    objModel.AssistantName = arry[3];
                    objModel.AssistantPhone = arry[4];
                    objModel.EmergencyContact = arry[5];
                    objModel.EmergencyConPhone = arry[6];
                    _meetingPerInfoService.Repository.Update(objModel);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                return Json(new { results = new { Data = 500 } });
            }

            return Json(new { results = new { Data = 200 } });
        }
        #endregion

        #region 我的会议
        public ActionResult MyMeeting(string strWhere,int? wechatid)
        {

            string str = "";

            try
            {
                string userid = ViewBag.WeChatUserID;
                var InvitationInfoAll = _meetingInvitationService.Repository.Entities.Where(m => m.IsDeleted == false && m.State == 1 && m.UserId == userid);
                var allMeetingId = InvitationInfoAll.Select(allId => allId.MeetingId).ToList();
                var meetList = _meetingService.Repository.Entities.Where(ml => ml.IsDeleted == false && allMeetingId.Contains(ml.Id)).ToList().OrderByDescending(m => m.StartDateTime).ToList();
                List<Meeting> newMeetList = new List<Meeting>();

                if (!string.IsNullOrEmpty(strWhere))//0全部  1最近三个月  2最近半年  3最近一年 (默认1)
                {
                    if (strWhere == "0")
                    {
                        newMeetList = meetList;
                    }
                    else if (strWhere == "1")
                    {
                        newMeetList = meetList.Where(nm => nm.StartDateTime <= DateTime.Now && nm.StartDateTime >= DateTime.Now.AddMonths(-3)).ToList();
                    }
                    else if (strWhere == "2")
                    {
                        newMeetList = meetList.Where(nm => nm.StartDateTime <= DateTime.Now && nm.StartDateTime >= DateTime.Now.AddMonths(-6)).ToList();
                    }
                    else if (strWhere == "3")
                    {
                        newMeetList = meetList.Where(nm => nm.StartDateTime <= DateTime.Now && nm.StartDateTime >= DateTime.Now.AddYears(-1)).ToList();
                    }
                }
                else
                {
                    newMeetList = meetList.Where(nm => nm.StartDateTime <= DateTime.Now && nm.StartDateTime >= DateTime.Now.AddMonths(-3)).ToList();
                }


                //string userid = "13555989031"; //ViewBag.WeChatUserID;

                _Logger.Debug("userId:{0}  |||||||  All meeting Id {1}", userid, allMeetingId);

                //var meetList = _BaseService.Repository.
                //    SqlQuery(@"select *  from [dbo].[Meeting]
                //               where IsDeleted='false' and Id in (select MeetingId from [dbo].[MeetingInvitation] 
                //               where IsDeleted='false' and UserId='" + userid + "' and State=1) " + str + " order by CreatedDate desc").ToList();

                var mlist = newMeetList.Select(a => (MeetingView)new MeetingView().ConvertAPIModel(a)).ToList();

                List<MeetingView> list = new List<MeetingView>();

                foreach (var item in mlist)
                {
                    var InvitationInfo = InvitationInfoAll.Where(m => m.MeetingId == item.Id).FirstOrDefault();
                    if (InvitationInfo != null)
                    {
                        var obj = new MeetingView()
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Location = item.Location,
                            STime = InvitationInfo.StartDateTime == null ? null : InvitationInfo.StartDateTime.Value.ToString("yyyy/MM/dd HH:mm"),
                            ETime = InvitationInfo.EndDateTime == null ? null : InvitationInfo.EndDateTime.Value.ToString("yyyy/MM/dd HH:mm"),
                            SsTime = InvitationInfo.SignStartTime == null ? null : InvitationInfo.SignStartTime.Value.ToString("yyyy/MM/dd HH:mm"),
                            SeTime = InvitationInfo.SignEndTime == null ? null : InvitationInfo.SignEndTime.Value.ToString("yyyy/MM/dd HH:mm"),
                            IsPerInfo = _meetingPerInfoService.Repository.Entities.Where(m => m.IsDeleted == false && m.UserId == userid).FirstOrDefault() == null ? "未填" : "已填",
                            IsTravel = _meetingTravelInfoService.Repository.Entities.Where(m => m.IsDeleted == false && m.MeetingId == item.Id && m.UserId == userid).FirstOrDefault() == null ? "未填" : "已填",
                            TimeClass = InvitationInfo.TimeClass == null ? null : InvitationInfo.TimeClass,
                        };

                        list.Add(obj);
                    }

                }

                ViewBag.WechatId = wechatid;

                if (list.Count() > 0)
                {
                    ViewBag.MeetList = list;
                }
                else
                {
                    ViewBag.MeetList = "";
                }

            }
            catch (Exception e)
            {
                _Logger.Debug(e);
            }          
            return View();
        }
        #endregion

        #region 加载更多
        public ActionResult GetMoreData()
        {
            //string userid = "13555989031"; //ViewBag.WeChatUserID;
            string userid = ViewBag.WeChatUserID;


            var meetList = _BaseService.Repository.
                SqlQuery(@"select *  from [dbo].[Meeting]
                           where IsDeleted='false' and Id in (select MeetingId from [dbo].[MeetingInvitation] 
                           where IsDeleted='false' and UserId='" + userid + "' and State=1) order by CreatedDate desc").Skip(20).ToList();

            var mlist = meetList.Select(a => (MeetingView)new MeetingView().ConvertAPIModel(a)).ToList();

            List<MeetingView> list = new List<MeetingView>();

            foreach (var item in mlist)
            {
                var InvitationInfo = _meetingInvitationService.Repository.Entities.Where(m => m.UserId == userid && m.MeetingId == item.Id
                    && m.IsDeleted == false && m.State == 1).FirstOrDefault();
                if (InvitationInfo != null)
                {
                    var obj = new MeetingView()
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Location = item.Location,
                        STime = InvitationInfo.StartDateTime == null ? null : InvitationInfo.StartDateTime.Value.ToString("yyyy/MM/dd HH:mm"),
                        ETime = InvitationInfo.EndDateTime == null ? null : InvitationInfo.EndDateTime.Value.ToString("yyyy/MM/dd HH:mm"),
                        SsTime = InvitationInfo.SignStartTime == null ? null : InvitationInfo.SignStartTime.Value.ToString("yyyy/MM/dd HH:mm"),
                        SeTime = InvitationInfo.SignEndTime == null ? null : InvitationInfo.SignEndTime.Value.ToString("yyyy/MM/dd HH:mm"),
                        IsPerInfo = _meetingPerInfoService.Repository.Entities.Where(m => m.IsDeleted == false && m.UserId == userid).FirstOrDefault() == null ? "未填" : "已填",
                        IsTravel = _meetingTravelInfoService.Repository.Entities.Where(m => m.IsDeleted == false && m.MeetingId == item.Id && m.UserId == userid).FirstOrDefault() == null ? "未填" : "已填",
                        TimeClass = InvitationInfo.TimeClass == null ? null : InvitationInfo.TimeClass

                    };

                    list.Add(obj);
                }

            }



            if (list.Count() > 0)
            {
                ViewBag.MeetList = list;
            }
            else
            {
                ViewBag.MeetList = "";
            }

            return Json(list);
        }
        #endregion




    }
}