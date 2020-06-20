using Infrastructure.Core.Logging;
using Infrastructure.Utility;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChatTalk.Domain.Entity;
using Innocellence.WeChatTalk.Domain.Service;
using Innocellence.WeChatTalk.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Innocellence.WeChatTalk.Controllers
{
    public class MultiTalkController : WeChatBaseController<MultiTalk, MultiTalkView>
    {
        private readonly IMultiTalkService _multiTalkService;
        private readonly IWechatMPUserService _wechatMPUserService;
        private readonly IAddressBookService _wechatUserService;

        ILogger _log = LogManager.GetLogger("MultiTalkController");

        public MultiTalkController(
            IMultiTalkService objService,
            IWechatMPUserService wechatMPUserService,
            IAddressBookService wechatUserService
            )
            : base(objService)
        {
            _multiTalkService = objService;
            _wechatMPUserService = wechatMPUserService;
            _wechatUserService = wechatUserService;
            //ViewBag.AppId = AppId;
        }
        // GET: MultiTalk
        public ActionResult talk(int wechatid) 
        {

            WeChatInfo wechatInfo = new WeChatInfo();
            wechatInfo.EnterRoomDate = DateTime.Now;
            wechatInfo.WechatUserId = ViewBag.WeChatUserID;
            var talkWhiteCompany = CommonService.GetSysConfig("MultiTalkWhiteCompany", "").Split(',');
            _log.Debug("WechatUserId:{0} wechatid:{1} whiteCompany{2}", wechatInfo.WechatUserId, wechatid, talkWhiteCompany);
            //var currentUser = _wechatMPUserService.GetUserByOpenId("ox1b8t4fmbhwV4kRqu_lGEBjBCdw");
            if (!string.IsNullOrEmpty(wechatInfo.WechatUserId))
            {

                var weChatConfig = WeChatCommonService.GetWeChatConfigByID(wechatid);
                //服务号
                if (weChatConfig.IsCorp.HasValue && !weChatConfig.IsCorp.Value)
                {
                    var currentMPUser = _wechatMPUserService.GetUserByOpenId(wechatInfo.WechatUserId);
                    wechatInfo.ImgHeadUrl = currentMPUser.HeadImgUrl;
                    wechatInfo.AppId = wechatid;
                    wechatInfo.ClientName = currentMPUser.NickName;
                }
                else //企业号
                {
                    var currentUser = _wechatUserService.GetMemberByUserId(wechatInfo.WechatUserId);

                    if (string.IsNullOrEmpty(currentUser.CompanyID) || !talkWhiteCompany.Contains(currentUser.CompanyID))
                    {
                        _log.Debug("No Auth happens!!");
                        return Redirect(string.Format("/wechattalk/multitalk/noauth?wechatid={0}", wechatid));
                    }
                    else {
                        wechatInfo.AppId = wechatid;
                        wechatInfo.ClientName = currentUser.UserName;
                        if (string.IsNullOrEmpty(currentUser.Avatar))
                            wechatInfo.ImgHeadUrl = "/Plugins/Innocellence.WechatTalk/Content/img/no_image.jpg";
                        else
                            wechatInfo.ImgHeadUrl = currentUser.Avatar;
                    }

                                                     
                }
            }

            _log.Debug("GetJSSDKConfig:{0}", wechatid);

            var wxConfig = WeChatCommonService.GetJSSDKConfig(wechatid, Request.Url.AbsoluteUri.ToString().Replace(":5001", ""));

            _log.Debug("GetJSSDKConfig Signature:{0} URL:{1}", wxConfig.Signature, Request.Url.AbsoluteUri.ToString().Replace(":5001", ""));
            wechatInfo.SdkAppId = wxConfig.AppId;
            wechatInfo.Timestamp = wxConfig.Timestamp;
            wechatInfo.NonceStr = wxConfig.NonceStr;
            wechatInfo.Signature = wxConfig.Signature;

           
            return View(wechatInfo);
        }

        public JsonResult GetHistoryTalk(int wechatid, int pageindex, int pagesize,string enterdate)
        {
            var enterRoomDate = System.Convert.ToDateTime(enterdate);
            int total = _multiTalkService.Repository.Entities.Where(l => l.AppId == wechatid && enterRoomDate > l.CreatedDate).Count();
            var talkList = _multiTalkService.GetList<MultiTalkView>
                           (l => l.AppId == wechatid && enterRoomDate > l.CreatedDate, pageindex, pagesize,ref total, null)
                           .OrderBy(x => x.CreatedDate).ToList();

            var weChatConfig = WeChatCommonService.GetWeChatConfigByID(wechatid);
            //服务号
            if (weChatConfig.IsCorp.HasValue && !weChatConfig.IsCorp.Value)
            {
                foreach (var msg in talkList)
                {
                    var newMsg = _wechatMPUserService.Repository.Entities.Where(x => x.OpenId == msg.OpenId);
                    msg.Name = newMsg.FirstOrDefault().NickName;
                    //msg.Name = "sss";
                    if (newMsg.Any()&& !string.IsNullOrEmpty(newMsg.FirstOrDefault().HeadImgUrl))
                        msg.ImgHeadUrl = newMsg.FirstOrDefault().HeadImgUrl;
                        
                    else
                        msg.ImgHeadUrl = "/Plugins/Innocellence.WechatTalk/Content/img/no_image.jpg";
                }
            }
            else //企业号
            {
                foreach (var msg in talkList)
                {
                    var newMsg = _wechatUserService.Repository.Entities.Where(x => x.UserId == msg.OpenId);
                    msg.Name = newMsg.FirstOrDefault().UserName;
                    //msg.Name = "sss";

                    if (newMsg.Any()&& !string.IsNullOrEmpty(newMsg.FirstOrDefault().Avatar))
                        msg.ImgHeadUrl = newMsg.FirstOrDefault().Avatar;
                    else
                        msg.ImgHeadUrl = "/Plugins/Innocellence.WechatTalk/Content/img/no_image.jpg";
                }
            }


            return Json(new {
                 data = talkList,
                 page = pageindex               
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult noAuth() {
            return View();
        }

    }
}