using System;
using System.IO;
using System.Web.Mvc;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.WeChat.Domain.ViewModelFront;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.CommonAPIs;
using Infrastructure.Utility.Secutiry;
using Infrastructure.Web.UI;
using Infrastructure.Web.Domain.Service;
using System.Text;
using System.Linq;
using Innocellence.Weixin.MP.Helpers;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Infrastructure.Core.Logging;
using Innocellence.Weixin.MP;


using System.Threading.Tasks;
using System.Web.Configuration;
using System.Net.Http;
using Innocellence.Weixin.MP.AdvancedAPIs.Card;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.MP.Entities;
using Innocellence.WeChat.Domain;
using System.Net;
using Innocellence.Weixin.Helpers;
using Innocellence.Weixin.HttpUtility;

namespace Innocellence.WeChatMP.Controllers
{
    public class APIController : APIBaseController<WechatMPUser, WechatMPUserView>
    {


        private const string SESSION_KEY_OAUTH_USERID = "Session_oAuthUserId";
        private ILogger log { get { return LogManager.GetLogger("WeChat"); } }

        public APIController(IWechatMPUserService addressBookServiceService)
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

            var userInfo = UserApi.Info(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, openid);

            if (userInfo.errcode == Weixin.ReturnCode.请求成功)
            {
                var UserView = _BaseService.GetList<WechatMPUserView>(0, a => a.OpenId == userInfo.openid, null).FirstOrDefault();
                if (UserView == null)
                {
                    return ErrMsg("simuid 没有找到!");
                }

                return Json(new
                {
                    message = "",
                    nickname = userInfo.nickname,
                    realName = "",
                    simuid = UserView.Id,
                    avatar = userInfo.headimgurl,
                    success = true,
                    hrcode = ""
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = userInfo.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 发送模板消息
        /// 
        /// </summary>
        /// <param name="qrticket"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SendTemplateMessage(string openids, string templateid, string url, string data)
        {

            // TODO:向微信服务器发送二维码验证请求

            if (!VerifyParam("openids,templateid,url,data"))
            {
                return ErrMsg();
            }

            //查询用户
            //var userid = int.Parse(uid);

            // var user = _BaseService.Repository.Entities.FirstOrDefault(a => a.Id == userid);
            if (string.IsNullOrEmpty(openids))
            {
                return ErrMsg("openids 不能为空！");
            }

            if (string.IsNullOrEmpty(templateid))
            {
                return ErrMsg("templateid 不能为空！");
            }

            if (string.IsNullOrEmpty(data))
            {
                return ErrMsg("data 不能为空！");
            }

            //查询设备列表
            var weChatConfig = GetWechatConfig();

            // object ob = Newtonsoft.Json.JsonConvert.DeserializeObject(data);

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            object ob = jsSerializer.DeserializeObject(data);

            var strOpenIds = openids.Split(';');

            StringBuilder sb = new StringBuilder();

            foreach (var strOpenid in strOpenIds)
            {

                var openid = strOpenid;

                try
                {
                    if (openid.Length < 25) //simuid和openid区分
                    {
                        var user = _BaseService.Repository.Entities.FirstOrDefault(a => a.SimUID == openid);
                        if (user != null)
                        {
                            openid = user.OpenId;
                        }else
                        {
                            sb.AppendFormat("OpenId or simuid 未找到:{0}。\r\n", openid);
                        }

                    }

                    var ret = TemplateApi.SendTemplateMessage(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, openid, templateid, url, ob);
                    if (ret.errcode != Weixin.ReturnCode.请求成功)
                    {
                        sb.AppendFormat("errcode:{0} ErrMsg:{1}。\r\n", ret.errcode, ret.errmsg);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, strOpenid);
                    sb.AppendFormat("openid:{0} err:{1}。\r\n", openid, ex.Message);
                }


            }


            // var ret = DeviceApi.DeviceList(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, user.OpenId);
            if (sb.Length == 0)
            {
                log.Debug("SendTemplateMessage 请求成功");
                return Json(new
                {
                    message = "",
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return ErrMsg(sb.ToString());
            }

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

            if (!VerifyParam("mediaId"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();

            MemoryStream ms = new MemoryStream();
            var ret = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.Get(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, mediaId, ms);
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

            //

            //return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 绑定设备，解绑设备接口
        /// </summary>
        /// <returns></returns>
        public ActionResult BindDevice()
        {
            // var result = new AccessJsonView();

            if (!VerifyParam("ticket,device_id,simuid,action"))
            {
                return ErrMsg();
            }

            var ticket = Request["ticket"];
            var deviceId = Request["device_id"];
            var simuid = int.Parse(Request["simuid"]);
            var action = Request["action"];

            var weChatConfig = GetWechatConfig();

            var list = _BaseService.GetList<WechatMPUserView>(0, a => a.Id == simuid, null);
            if (list == null || list.Count == 0)
            {
                return ErrMsg("simuid 没有找到!");
            }

            // TODO:绑定设备
            if (action.Equals("bind"))
            {
                var ret = DeviceApi.DeviceBindUnbind(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, true, ticket, deviceId, list[0].OpenId);
                if (ret.errcode != Weixin.ReturnCode.请求成功)
                {
                    return Json(new
                    {
                        message = ret.errmsg,
                        success = false
                    }, JsonRequestBehavior.AllowGet);
                }
                // 解绑设备
            }
            else if (action.Equals("unbind"))
            {
                var ret = DeviceApi.DeviceBindUnbind(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, true, ticket, deviceId, list[0].OpenId);
                if (ret.errcode != Weixin.ReturnCode.请求成功)
                {
                    return Json(new
                    {
                        message = ret.errmsg,
                        success = false
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 利用deviceid更新设备属性 (包括获取deviceid和二维码)
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterDevice(string action, string data)
        {
            //  var result = new AccessJsonView();



            if (!VerifyParam("action,data"))
            {
                return ErrMsg();
            }

            // var action = Request["action"];
            // var data = Request["data"];

            var weChatConfig = GetWechatConfig();
            // TODO:注册设备
            if (action.Equals("register"))
            {
                var ret = DeviceApi.DeviceRegister(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, data);

                if (ret.errcode == Weixin.ReturnCode.请求成功)
                {
                    return Json(new
                    {
                        message = "",
                        success = true,
                        resp = ret.resp
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        message = ret.errmsg,
                        success = false
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            // 设备授权验证
            else if (action.Equals("auth"))
            {
                //product_id 是绑定到账号的

                var ret = DeviceApi.DeviceAuthority(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, weChatConfig.ProductID);

                if (ret.errcode == Weixin.ReturnCode.请求成功)
                {
                    return Json(new
                    {
                        message = "",
                        deviceid = ret.deviceid,
                        qrticket = ret.qrticket,
                        success = true
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        message = ret.errmsg,
                        success = false
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new
            {
                message = "action 参数错误！",
                success = false
            }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 验证二维码
        /// 微信硬件接口返回的deviceType 字段目前为服务号原始ID，SIM系统目前忽略了这个字段
        /// </summary>
        /// <param name="qrticket"></param>
        /// <returns></returns>
        public ActionResult Verify(string qrticket)
        {

            // TODO:向微信服务器发送二维码验证请求

            if (!VerifyParam("qrticket"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();
            var ret = DeviceApi.VerifyQRCode(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, qrticket);
            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    device_type = ret.device_type,
                    mac = ret.mac,
                    device_id = ret.device_id,
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 查询设备状态
        /// 设备状态，目前取值如下： 
        /// 0：未授权 1：已经授权（尚未被用户绑定） 2：已经被用户绑定 3：属性未设置
        /// </summary>
        /// <param name="device_id"></param>
        /// <returns></returns>
        public ActionResult CheckDeviceStatus(string device_id)
        {

            if (!VerifyParam("device_id"))
            {
                return ErrMsg();
            }
            // var result = new AccessJsonView();
            // TODO:向微信服务器发送查询设备状态请求

            var weChatConfig = GetWechatConfig();
            var ret = DeviceApi.DeviceStatus(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, device_id);
            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {

                return Json(new { message = "", status = ret.status, success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 获取用户绑定的所有设备
        /// 
        /// </summary>
        /// <param name="qrticket"></param>
        /// <returns></returns>
        public ActionResult GetDeviceList(string simuid)
        {

            // TODO:向微信服务器发送二维码验证请求

            if (!VerifyParam("simuid"))
            {
                return ErrMsg();
            }

            //查询用户
            var userid = int.Parse(simuid);

            var user = _BaseService.Repository.Entities.FirstOrDefault(a => a.Id == userid);
            if (user == null)
            {
                return ErrMsg("simuid 不存在！");
            }

            //查询设备列表
            var weChatConfig = GetWechatConfig();
            var ret = DeviceApi.DeviceList(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, user.OpenId);
            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true,
                    device_list = ret.device_list,
                    simuid = simuid
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return ErrMsg(ret.errmsg);

            }

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
            var config = WeChatCommonService.GetWeChatConfigByID(appId);
            var ret = Weixin.MP.Helpers.JSSDKHelper.GetJsSdkUiPackage(config.WeixinCorpId, config.WeixinCorpSecret, url);

            return Json(new
            {
                appid = ret.AppId,
                noncestr = ret.NonceStr,
                sign = ret.Signature,
                timestamp = ret.Timestamp
            }, JsonRequestBehavior.AllowGet);
        }



        //// 取得token
        //private string getWxToken(int agentid = 0)
        //{
        //    var config = WeChatCommonService.GetWeChatConfigByID(agentid);
        //    var strToken = AccessTokenContainer.GetToken(config.WeixinCorpId, config.WeixinCorpSecret);
        //    if (!string.IsNullOrEmpty(strToken))
        //    {
        //        return strToken;
        //    }

        //    return null;
        //}



        #region 查看卡券详情接口
        /// <summary>
        /// 查看卡券详情接口
        /// </summary>
        /// <param name="cardId">卡券ID</param>
        /// <returns></returns>
        public ActionResult GetCardDetail(string cardId)
        {
            if (!VerifyParam("cardId"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();

            var ret = CardApi.CardDetailGet(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, cardId);

            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true,
                    card = ret.card


                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion

        #region 批量查询卡券列表
        /// <summary>
        /// 批量查询卡列表
        /// </summary>
        /// <param name="offset">查询卡列表的起始偏移量，从0 开始，即offset: 5 是指从从列表里的第六个开始读取。</param>
        /// <param name="count">需要查询的卡片的数量（数量最大50）</param>
        /// <returns></returns>
        public ActionResult GetCardList(int offset, int count)
        {
            string os = offset.ToString();
            string cou = count.ToString();

            if (!VerifyParam("os,cou"))
            {
                return ErrMsg();
            }


            var weChatConfig = GetWechatConfig();

            var ret = CardApi.CardBatchGet(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, offset, count);
            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true,
                    card_id_list = ret.card_id_list,
                    total_num = ret.total_num

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion

        #region 获取用户已领取卡券
        /// <summary>
        /// 获取用户已领取卡券
        /// </summary>
        /// <param name="openId">需要查询的用户openid</param>
        /// <param name="cardId">卡券ID。不填写时默认查询当前appid下的卡券。</param>
        /// <returns></returns>
        public ActionResult GetCardListByOpenId(string openId, string cardId)
        {
            if (!VerifyParam("openId"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();

            var ret = CardApi.GetCardList(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, openId, cardId);

            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true,
                    card_list = ret.card_list

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region 拉取卡券概况数据接口
        /// <summary>
        /// 拉取卡券概况数据接口
        /// </summary>
        /// <param name="beginDate">查询数据的起始时间。</param>
        /// <param name="endDate">查询数据的截至时间。</param>
        /// <param name="condSource">卡券来源，0为公众平台创建的卡券数据、1是API创建的卡券数据 </param>
        /// <returns></returns>
        public ActionResult GetCardBizuinInfo(string beginDate, string endDate, int condSource)
        {
            string cSource = condSource.ToString();

            if (!VerifyParam("beginDate,endDate,cSource"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();

            var ret = CardApi.GetCardBizuinInfo(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, beginDate, endDate, condSource);
            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true,
                    list = ret.list


                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region 创建二维码接口(投放二维码)
        /// <summary>
        /// 创建二维码接口(投放二维码)
        /// </summary>
        /// <param name="cardId">卡券ID</param>
        /// <param name="code">卡券Code码,use_custom_code字段为true的卡券必须填写，非自定义code不必填写。</param>
        /// <param name="openId">指定领取者的openid，只有该用户能领取。bind_openid字段为true的卡券必须填写，非指定openid不必填写。 </param>
        /// <param name="expireSeconds">指定二维码的有效时间，范围是60 ~ 1800秒。不填默认为永久有效。</param>
        /// <param name="isUniqueCode">指定下发二维码，生成的二维码随机分配一个code，领取后不可再次扫描。填写true或false。默认false，注意填写该字段时，卡券须通过审核且库存不为0。 </param>
        /// <param name="outer_id">领取场景值，用于领取渠道的数据统计，默认值为0，字段类型为整型，长度限制为60位数字。用户领取卡券后触发的事件推送中会带上此自定义场景值。 </param>
        /// <returns></returns>
        public ActionResult CreateQRCode(string cardId, string code, string openId, string expireSeconds, bool isUniqueCode, string outer_id)
        {
            string qrcode_url = string.Empty;
            if (!VerifyParam("cardId,code,openId,outer_id"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();

            var ret = CardApi.CreateQR(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, cardId, code, openId, null, false, outer_id);


            if (ret.ticket.Contains("="))
            {
                qrcode_url = ret.ticket.Replace("=", "%3D").Replace("=", "%3D");
            }
            else
            {
                qrcode_url = ret.ticket;
            }

            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true,
                    ticket = ret.ticket,
                    show_qrcode_url = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + qrcode_url


                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion


        #region 暂时不用

        #region 图文消息群发卡券
        /// <summary>
        /// 图文消息群发卡券(目前该接口仅支持填入非自定义code的卡券,自定义code的卡券需先进行code导入后调用。 )
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public ActionResult GetHtml(string cardId)
        {
            if (!VerifyParam("cardId"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();

            var ret = CardApi.GetHtml(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, cardId);


            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true,
                    content = ret.content
                });
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region 根据分组群发卡券消息
        /// <summary>
        /// 根据分组群发卡券消息(目前该接口仅支持填入非自定义code的卡券,自定义code的卡券需先进行code导入后调用。 )
        /// </summary>
        /// <param name="groupId">群发到的分组的group_id，参加用户管理中用户分组接口，若is_to_all值为true，可不填写group_id</param>
        /// <param name="value">群发卡券时传入cardId</param>
        /// <param name="isToAll">用于设定是否向全部用户发送，值为true或false，选择true该消息群发给所有用户，选择false可根据group_id发送给指定群组的用户</param>
        /// <returns></returns>
        public ActionResult SendGroupMessageByGroupId(string groupId, string value, bool isToAll)
        {
            string is_toall = isToAll.ToString();

            if (isToAll == true)
            {
                if (!VerifyParam("value,is_toall"))
                {
                    return ErrMsg();
                }
            }
            else
            {
                if (!VerifyParam("groupId,value,is_toall"))
                {
                    return ErrMsg();
                }

                if (string.IsNullOrEmpty(groupId))
                {
                    return ErrMsg("groupId不能为空！");
                }

            }


            if (string.IsNullOrEmpty(value))
            {
                return ErrMsg("cardId不能为空！");
            }


            var weChatConfig = GetWechatConfig();

            var ret = GroupMessageApi.SendGroupMessageByGroupId(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, groupId, value, GroupMessageType.wxcard, isToAll);

            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true,
                    msg_id = ret.msg_id
                });
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region 根据OpenID列表群发卡券消息
        /// <summary>
        /// 根据OpenID列表群发卡券消息(目前该接口仅支持填入非自定义code的卡券,自定义code的卡券需先进行code导入后调用。 )
        /// </summary>
        /// <param name="value">群发卡券时传入cardId</param>
        /// <param name="openIds">openId字符串数组(OpenID最少2个，最多10000个 )</param>
        /// <returns></returns>
        public ActionResult SendGroupMessageByOpenId(string value, string openIds)
        {
            if (!VerifyParam("value,openIds"))
            {
                return ErrMsg();
            }

            string[] openIdArr = openIds.Split(',');

            var weChatConfig = GetWechatConfig();

            var ret = GroupMessageApi.SendGroupMessageByOpenId(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, GroupMessageType.wxcard, value, 1000, openIdArr);

            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true,
                    msg_id = ret.msg_id
                });
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion

        #region 客服消息下发卡券(查看card_ext字段详情及签名规则，特别注意客服消息接口投放卡券仅支持非自定义Code码的卡券)
        /// <summary>
        /// 客服消息下发卡券
        /// </summary>
        /// <param name="openId">普通用户openid</param>
        /// <param name="cardId"></param>
        /// <param name="cardExt"></param>
        /// <returns></returns>
        public ActionResult SendCard(string openId, string cardId, CardExt cardExt)
        {
            if (!VerifyParam("openId,cardId,cardExt"))
            {
                return ErrMsg();
            }

            if (string.IsNullOrEmpty(openId))
            {
                return ErrMsg("openId不能为空！");
            }

            if (string.IsNullOrEmpty(cardId))
            {
                return ErrMsg("cardId不能为空！");
            }

            var weChatConfig = GetWechatConfig();

            var ret = CustomApi.SendCard(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, openId, cardId, cardExt);

            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true


                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }




        }
        #endregion




        #region 设置自助核销接口
        /// <summary>
        /// 设置自助核销接口
        /// 注意：设置自助核销的card_id必须已经配置了门店，否则会报错。
        /// 错误码，0为正常；43008为商户没有开通微信支付权限
        /// </summary>
        /// <param name="cardId">卡券ID</param>
        /// <param name="isOpen">是否开启自助核销功能，填true/false</param>
        /// <returns></returns>
        public ActionResult SelfConsumecellSet(string cardId, bool isOpen)
        {
            string is_open = isOpen.ToString();

            if (!VerifyParam("cardId,is_open"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();

            var ret = CardApi.SelfConsumecellSet(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, cardId, isOpen);
            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true


                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }



        }
        #endregion

        #region 设置卡券失效接口
        /// <summary>
        /// 设置卡券失效接口
        /// </summary>
        /// <param name="code">需要设置为失效的code</param>
        /// <param name="cardId">自定义code 的卡券必填。非自定义code 的卡券不填。</param>
        /// <returns></returns>
        public ActionResult CardUnavailable(string code, string cardId)
        {
            if (!VerifyParam("code"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();

            var ret = CardApi.CardUnavailable(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, code, cardId);


            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true


                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region 卡券消耗code(消耗code接口是核销卡券的唯一接口，仅支持核销有效期内的卡券，否则会返回错误码invalid time)
        /// <summary>
        /// 卡券消耗code
        /// </summary>
        /// <param name="code">需核销的Code码。 </param>
        /// <param name="cardId">卡券ID。创建卡券时use_custom_code填写true时必填。非自定义Code不必填写。</param>
        /// <returns></returns>
        public ActionResult CardConsume(string code, string cardId)
        {
            if (!VerifyParam("code"))
            {
                return ErrMsg();
            }

            var weChatConfig = GetWechatConfig();

            var ret = CardApi.CardConsume(weChatConfig.WeixinAppId, weChatConfig.WeixinCorpSecret, code, cardId);
            if (ret.errcode == Weixin.ReturnCode.请求成功)
            {
                return Json(new
                {
                    message = "",
                    success = true,
                    card = ret.card,
                    openid = ret.openid

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    message = ret.errmsg,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion


        #endregion




        // RequestMessageEvent_User_Get_Card requestMessage


        //RequestMessageEvent_User_Consume_Card requestMessage


        // RequestMessageEvent_User_Consume_Card requestMessage = new RequestMessageEvent_User_Consume_Card();


       // RequestMessageEvent_Card_Pass_Check requestMessage = new RequestMessageEvent_Card_Pass_Check();


       



        //        string test = "{
        //    "card": {
        //        "card_type": "GENERAL_COUPON", 
        //        "general_coupon": {
        //            "base_info": {
        //                "logo_url": "http://mmbiz.qpic.cn/mmbiz/lprtsiabGHGz8DfXbIXKhqBuLcuv2SvkguEBBXVwQCI3khyv3IicCxyo0a843ryXwjxicmiaTpGdzyVBUznPzvxKZQ/0", 
        //                "brand_name": "健康中国行", 
        //                "code_type": "CODE_TYPE_TEXT", 
        //                "title": "山核桃优惠券", 
        //                "sub_title": "购买只需0.1元", 
        //                "color": "Color010", 
        //                "notice": "购买时请录入优惠码", 
        //                "description": "需要自负邮费，消费金额到达79元可免邮费", 
        //                "date_info": {
        //                    "type": "DATE_TYPE_FIX_TIME_RANGE", 
        //                    "begin_timestamp": 1480316662, 
        //                    "end_timestamp": 1482908662
        //                }, 
        //                "sku": {
        //                    "quantity": 0
        //                }, 
        //                "get_limit": 1, 
        //                "use_custom_code": true, 
        //                "get_custom_code_mode": "GET_CUSTOM_CODE_MODE_DEPOSIT", 
        //                "bind_openid": false, 
        //                "can_share": true, 
        //                "can_give_friend": true, 
        //                "center_title": "快速购买", 
        //                "center_sub_title": "立刻把山核桃带回家", 
        //                "center_url": "www.j1.com", 
        //                "custom_url_name": "立即使用", 
        //                "custom_url": "http://www.j1.com", 
        //                "custom_url_sub_title": "去键一网购买", 
        //                "promotion_url_name": "更多活动", 
        //                "promotion_url": "http://www.ijkang.com.", 
        //                "source": "健康中国行"
        //            }, 
        //            "default_detail": "0元购物"
        //        }
        //    }
        //}";


    }
}