using System;
using System.IO;
using System.Web.Mvc;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.CommonAPIs;
using Infrastructure.Utility.Secutiry;
using Infrastructure.Web.UI;
using Infrastructure.Web.Domain.Service;
using System.Text;
using System.Linq;
using Innocellence.Weixin.QY.Helpers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Innocellence.Weixin.Helpers;
using Innocellence.WeChat.Domain;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.ViewModel;
using System.Web.Script.Serialization;
using Innocellence.Weixin.Entities;

namespace Innocellence.WeChat.Controllers
{
    public class APIController : APIBaseController<SysAddressBookMember, AddressBookMemberView>
    {


        private const string SESSION_KEY_OAUTH_USERID = "Session_oAuthUserId";

        public APIController(IAddressBookService addressBookServiceService)
            : base(addressBookServiceService)
        {

        }




        /// <summary>
        /// 根据ticket获得员工信息
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public ActionResult GetByTicket(string ticket)
        {
            if (!VerifyParam("ticket"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();
            var t = EncryptionHelper.DecodeFrom64(ticket);
            var key = DesHelper.Decrypt(t, CommonService.GetSysConfig("EncryptKey", ""));

            var openid = key.Split('|')[0];

            var userInfo = ((IAddressBookService)_BaseService).GetMemberByUserId(openid);

            // var userInfo = UserApi.Info(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, openid);


            var userView = GetUserInfo((AddressBookMemberView)new AddressBookMemberView().ConvertAPIModel(userInfo));


            if (userInfo != null)
            {

                var strJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    message = "",
                    success = true,
                    item = userView
                });

                return Content(strJson, "application/json");

                //return Json(new
                //{
                //    message = "",

                //    success = true,
                //    item= userView
                //    //item = new
                //    //{
                //    //    id = userInfo.Id,
                //    //    teamId = userInfo.AccountManageId,
                //    //    userId = userInfo.UserId,
                //    //    name = userInfo.UserName,
                //    //    position = userInfo.Position,
                //    //    mobile = userInfo.Mobile,
                //    //    gender = userInfo.Gender,
                //    //    avatar = userInfo.Avatar,
                //    //    extend1 = "",
                //    //    // telephone = userInfo.te,
                //    //    email = userInfo.Email,
                //    //    wxid = userInfo.WeiXinId,
                //    //    hrcode = userInfo.EmployeeNo,
                //    //    status = userInfo.Status,
                //    //    createTime = userInfo.CreateTime
                //    //},

                //}, JsonRequestBehavior.AllowGet);
            }
            else
            {

                log.Error("用户不存在:ticket:{0} userid:{1}", ticket, openid);

                return Json(new
                {
                    message = "用户不存在！",
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据userId获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetByUserId(string userId)
        {

            if (!VerifyParam("userId"))
            {
                return ErrMsg();
            }

            var result = new Domain.ViewModelFront.AccessJsonView();


            var user = ((IAddressBookService)_BaseService).GetMemberByUserId(userId);
            if (user == null)
            {
                log.Error("用户不存在: userid:{0}", userId);

                result.success = false;
                result.message = "用户不存在！";
            }
            else
            {
                //TODO, DB中现在没有的字段暂时赋为空置
                result.success = true;

                var userView = GetUserInfo((AddressBookMemberView)new AddressBookMemberView().ConvertAPIModel(user));

                //var userView = new AccessUserInfoView();
                //userView.Id = user.Id;
                //userView.position = user.Position;
                //userView.createTime = DateTimeHelper.GetWeixinDateTime(user.CreateTime.Value);
                //userView.birthday = "";
                //userView.contactNumber = user.Mobile; // TODO: 还有个属性Mobile
                //userView.wxid = user.WeiXinId;
                ////userView.departmentList
                //// TODO: 如何将string 类型的user.Department转化为List类型

                //userView.cityName = "";
                //userView.registerAddress = "";
                //userView.teamId = user.AccountManageId;
                //userView.city = "";
                //userView.contactSignTime = "";
                //userView.trainingAgreement = "";
                //userView.name = "";
                //userView.userId = user.UserId;
                //userView.gender = user.Gender.ToString();
                //userView.major = "";
                //userView.university = "";
                //userView.citizenshipNumber = "";
                //userView.status = user.Status;
                //userView.avatar = user.Avatar;
                //userView.englishName = "";
                //userView.confidentialityAgreement = "";
                //userView.degree = "";
                //userView.email = user.Email;
                //userView.address = "";
                //// TODO tag list
                ////userView.tagList = null


                //userView.publicAvatarPath = user.Avatar;
                //userView.hrcode = user.EmployeeNo;
                //userView.mobile = user.Mobile;

                result.item = userView;
            }

            var strJson = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return Content(strJson, "application/json");

            //  return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 根据userId获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult ListByDepartment(int id, string keyword, int status, int page, int size)
        {

            if (!VerifyParam("id,keyword,status,page,size"))
            {
                return ErrMsg();
            }


            Expression<Func<SysAddressBookMember, bool>> predicate = (a) => true;
            PageCondition ConPage = new PageCondition() { PageIndex = page, PageSize = size };

            if (id > 0)
            {
                predicate = predicate.AndAlso(a => a.Department.StartsWith("[" + id.ToString() + ",")
                || a.Department.StartsWith("," + id.ToString() + "]")
                || a.Department.StartsWith("[" + id.ToString() + "]")
                || a.Department.StartsWith("," + id.ToString() + ","));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                predicate = predicate.AndAlso(a => a.UserName.Contains(keyword));
            }

            if (status > -1)
            {
                if (status == 2)
                {
                    predicate = predicate.AndAlso(a => a.Status == 1);
                }
                else if (status == 0)
                {
                    predicate = predicate.AndAlso(a => a.DeleteFlag == 1);
                }
                else if (status == 1)
                {
                    predicate = predicate.AndAlso(a => a.DeleteFlag == 0);
                }
            }

            var lst = ((IAddressBookService)_BaseService).GetList<AddressBookMemberView>(predicate, ConPage);

            List<AccessUserInfoView> lstRet = new List<AccessUserInfoView>();


            foreach (var user in lst)
            {

                //TODO, DB中现在没有的字段暂时赋为空置

                var userView = GetUserInfo(user);
                lstRet.Add(userView);


            }

            var strJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, items = lstRet, RowCount = ConPage.RowCount });

            return Content(strJson, "application/json");
        }

        private AccessUserInfoView GetUserInfo(AddressBookMemberView user)
        {
            var userView = new AccessUserInfoView();
            //  userView.Id = user.Id;
            userView.id = user.Id;
            userView.position = user.Position;
            userView.createTime = DateTimeHelper.GetWeixinDateTime(user.CreateTime.Value);
            userView.birthday = "";
            userView.contactNumber = user.Mobile; // TODO: 还有个属性Mobile
            userView.wxid = user.WeiXinId;
            if (user.DepartmentIds != null)
            {
                userView.departmentList = user.DepartmentIds.ToList().Select(a => new Domain.ViewModelFront.AccessDepartmentView() { id = a }).ToList(); ;
            }

            // TODO: 如何将string 类型的user.Department转化为List类型

            userView.cityName = "";
            userView.registerAddress = "";
            userView.teamId = user.AccountManageId;
            userView.city = "";
            userView.contactSignTime = "";
            userView.trainingAgreement = "";
            userView.name = user.UserName;
            userView.userId = user.UserId;
            userView.gender = user.Gender.ToString();
            userView.major = "";
            userView.university = "";
            userView.citizenshipNumber = "";
            userView.status = user.Status;
            userView.avatar = user.Avatar;
            userView.englishName = "";
            userView.confidentialityAgreement = "";
            userView.degree = "";
            userView.email = user.Email;
            userView.address = "";
            // TODO tag list
            if (user.TagList != null)
            {
                userView.tagList = user.TagList.ToList().Select(a => new TagEntity() { TagId = a }).ToList();
            }


            userView.publicAvatarPath = user.Avatar;
            userView.hrcode = user.EmployeeNo;
            userView.hrcodeNew = user.EmployeeNo;
            userView.mobile = user.Mobile;

            return userView;
        }


        /// <summary>
        /// 获取jssdk参数
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult sign(int appId, string url)
        {

            if (!VerifyParam("url"))
            {
                return ErrMsg();
            }

            log.Debug("API sign Start  appId:{0} Uri:{1} Request URL:{2}", appId, url, Request.Url);

            var config = WeChatCommonService.GetWeChatConfigByID(appId);

            var ret = Weixin.QY.Helpers.JSSDKHelper.GetJsSdkUiPackage(config.WeixinCorpId, config.WeixinCorpSecret, url);


            log.Debug("API sign End  UserID:{0} noncestr:{1} Signature:{2} Timestamp:{3}", config.WeixinAppId, ret.NonceStr, ret.Signature, ret.Timestamp);

            return Json(new
            {
                appid = ret.AppId,
                noncestr = ret.NonceStr,
                sign = ret.Signature,
                timestamp = ret.Timestamp
            }, JsonRequestBehavior.AllowGet);



        }


        /// <summary>
        /// 多媒体文件下载接口
        /// 接口调用频率限制默认10000次/day
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public ActionResult DownloadMedia(string mediaId)
        {
            // var result = new AccessJsonView();
            // TODO:从微信服务器获取文件

            var weChatConfig = GetWechatConfig();

            MemoryStream ms = new MemoryStream();

            var token = Innocellence.Weixin.QY.CommonAPIs.AccessTokenContainer.GetToken(weChatConfig.WeixinCorpId, weChatConfig.WeixinCorpSecret);

            var ret = Innocellence.Weixin.QY.AdvancedAPIs.MediaApi.Get(token, mediaId, ms);
            ms.Position = 0;

            var strName = ret["Content-disposition"];
            if (strName == null)
            {
                strName = "NoName";

            }
            else if (strName.Length > 30)
            {
                strName = strName.Substring(30).Replace("\"", "");
            }

            return File(ms, ret["Content-Type"], strName);

            // return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
        }



        [ValidateInput(false)]
        public ActionResult SendTextMsg(string UserID, string MsgContent)
        {
            if (!VerifyParam("UserID,MsgContent"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();

            var token = Innocellence.Weixin.QY.CommonAPIs.AccessTokenContainer.GetToken(weChatConfig.WeixinCorpId, weChatConfig.WeixinCorpSecret);

            var objResult = MassApi.SendText(token, UserID, "", "", weChatConfig.WeixinAppId.ToString(), MsgContent, 0);

            if (objResult.errcode == Weixin.ReturnCode_QY.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return ErrMsg(objResult.errmsg);
            }


            // return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="by">类型</param>
        /// <param name="value">用户,by是userid，支持10000每次，其他支持1000每次</param>
        /// <param name="applicationId"></param>
        /// <param name="data">内容</param>
        /// <param name="type">类型text和news，默认text</param>
        /// <param name="safe">true 保密 false 不保密</param>
        /// <returns></returns>
        public ActionResult SendMsg(string by, string value, string applicationId, string data, string type, string safe)
        {
            if (!VerifyParam("by,value,applicationId,data,type,safe"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();

            var toUsrids = value;

            var token = Innocellence.Weixin.QY.CommonAPIs.AccessTokenContainer.GetToken(weChatConfig.WeixinCorpId, weChatConfig.WeixinCorpSecret);


            Expression<Func<SysAddressBookMember, bool>> predicate = (a) => a.DeleteFlag == 0 && a.Status == 1;
            PageCondition ConPage = new PageCondition() { PageIndex = 1, PageSize = 1000 };

            if (value.IndexOf("|") >= 0)
            {
                var toUsers = value.Split('|');
                switch (by)
                {
                    case "id":
                        var toInt = toUsers.Select(a => int.Parse(a)).ToArray();

                        predicate = predicate.AndAlso(a => toInt.Contains(a.Id));
                        break;
                    case "hrcode":
                        predicate = predicate.AndAlso(a => toUsers.Contains(a.EmployeeNo));
                        break;
                    case "userid":
                        predicate = null;
                        break;
                    case "wxid":
                        predicate = predicate.AndAlso(a => toUsers.Contains(a.WeiXinId));
                        break;
                    case "email":
                        predicate = predicate.AndAlso(a => toUsers.Contains(a.Email));
                        break;
                    default:
                        predicate = null;
                        break;

                }
            }
            else
            {
                switch (by)
                {
                    case "id":
                        predicate = predicate.AndAlso(a => a.Id == int.Parse(value));
                        break;
                    case "hrcode":
                        predicate = predicate.AndAlso(a => a.EmployeeNo == value);
                        break;
                    case "userid":
                        predicate = null;
                        break;
                    case "wxid":
                        predicate = predicate.AndAlso(a => a.WeiXinId == value);
                        break;
                    case "email":
                        predicate = predicate.AndAlso(a => a.Email == value);
                        break;
                    default:
                        predicate = null;
                        break;

                }
            }




            if (predicate != null)
            {
                var lst = ((IAddressBookService)_BaseService).GetList<AddressBookMemberView>(predicate, ConPage).Select(a => a.UserId).ToArray();

                if (lst.Length > 0)
                {
                    toUsrids = string.Join("|", lst);
                }
                else
                {
                    return ErrMsg("用户未找到！" + value);
                }

            }


            Weixin.QY.AdvancedAPIs.Mass.MassResult objResult;

            if (type == "news")
            {
                var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Article>>(data);

                objResult = MassApi.SendNews(token, toUsrids, "", "", weChatConfig.WeixinAppId.ToString(), lst, safe == "true" ? 1 : 0);
            }
            else
            {
                objResult = MassApi.SendText(token, toUsrids, "", "", weChatConfig.WeixinAppId.ToString(), data, safe == "true" ? 1 : 0);
            }



            if (objResult.errcode == Weixin.ReturnCode_QY.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return ErrMsg(objResult.errmsg);
            }


            // return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// {"appid":"wx41a2bf0afed3b33d","noncestr":"USUtYzXJw4wELBKX","sign":"ed18cbd26d2f82b44dd2fb70743b484e68e68d3c","timestamp":"1438746212"}
        /// </summary>
        /// <returns></returns>
        public ActionResult GetJsSDK(int appId, string url)
        {

            //if (!VerifyParam("url"))
            //{
            //    return ErrMsg();
            //}

            log.Debug("web API sign Start  appId:{0} Uri:{1} Request URL:{2}", appId, url, Request.Url);

            var config = WeChatCommonService.GetWeChatConfigByID(appId);

            var ret = Weixin.QY.Helpers.JSSDKHelper.GetJsSdkUiPackage(config.WeixinCorpId, config.WeixinCorpSecret, url);


            log.Debug("web API sign End  UserID:{0} noncestr:{1} Signature:{2} Timestamp:{3}", config.WeixinAppId, ret.NonceStr, ret.Signature, ret.Timestamp);

            return Json(new
            {
                appid = ret.AppId,
                noncestr = ret.NonceStr,
                sign = ret.Signature,
                timestamp = ret.Timestamp
            }, JsonRequestBehavior.AllowGet);

        }


    }
}