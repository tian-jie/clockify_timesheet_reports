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
using Newtonsoft.Json;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Security.Cryptography;

namespace Innocellence.WeChatMP.Controllers
{
    public class WentangAPIController : APIBaseController<WechatMPUser, WechatMPUserView>
    {
        private WebServiceAgent _Agent = new WebServiceAgent(CommonService.GetSysConfig("WentangshequService", "http://120.92.191.39:9012/Service1.asmx"));
        private const string SESSION_KEY_OAUTH_USERID = "Session_oAuthUserId";
        private ILogger log { get { return LogManager.GetLogger("WeChat"); } }
        private IWechatMPUserService _WechatMPUserService;
        private IQrCodeService _QrCodeService;
        private IFocusHistoryService _FocusHistoryService;
        public WentangAPIController(IWechatMPUserService addressBookServiceService,
                  IWechatMPUserService WechatMPUserService,
                  IFocusHistoryService FocusHistoryService,
                  IQrCodeService QrCodeService)
            : base(addressBookServiceService)
        {
            _WechatMPUserService = WechatMPUserService;
            _QrCodeService = QrCodeService;
            _FocusHistoryService = FocusHistoryService;
        }

        /// <summary>
        /// {"appid":"wx41a2bf0afed3b33d","noncestr":"USUtYzXJw4wELBKX","sign":"ed18cbd26d2f82b44dd2fb70743b484e68e68d3c","timestamp":"1438746212"}
        /// </summary>
        /// <returns></returns>
        public ActionResult GetJsSDK(int appId, string url)
        {
            if (!VerifyParam("url"))
            {
                return ErrMsg();
            }
            var config = WeChatCommonService.GetWeChatConfigByID(appId);
            var ret = JSSDKHelper.GetJsSdkUiPackage(config.WeixinCorpId, config.WeixinCorpSecret, url);

            return Json(new
            {
                appid = ret.AppId,
                noncestr = ret.NonceStr,
                sign = ret.Signature,
                timestamp = ret.Timestamp
            }, JsonRequestBehavior.AllowGet);
        }

        #region QRCode Activity


        /// <summary>
        /// 根据用户openId 创建或者更新新的二维码
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="appid"></param>
        /// <param name="activityName"></param>
        /// <param name="activityid"></param>
        /// <returns></returns>
        public ActionResult CreateOrUpdateQrCodeByOpenId(string openId, string wechatappid, string activityName, string activityid, string isUpdate)
        {
            if (!VerifyParam("openId,wechatappid,activityName,activityid,isUpdate"))
            {
                return ErrMsg();
            }
            int newAppid = System.Convert.ToInt32(wechatappid);
            int newactivityId = System.Convert.ToInt32(activityid);

            try
            {
                var config = WeChatCommonService.GetWeChatConfigByID(newAppid);
                var user = _WechatMPUserService.Repository.Entities.Where(u => u.OpenId.Equals(openId, StringComparison.CurrentCultureIgnoreCase) && u.AccountManageId == config.AccountManageId && u.IsCanceled == false).FirstOrDefault();
                var qrcode = _QrCodeService.Repository.Entities.Where(q => q.RelatedUserId == user.Id);
                QrCodeMPItem newqr = new QrCodeMPItem();
                if (qrcode.Any())
                {
                    newqr = qrcode.FirstOrDefault();
                    UpdateQRCode(newqr, newAppid, isUpdate);
                }
                else
                {

                    newqr.AppId = newAppid;
                    newqr.Description = activityName;
                    newqr.CreatedDate = DateTime.Now;
                    newqr.Deleted = false;
                    newqr.CreatedUserID = User.Identity.Name;
                    newqr.UpdatedDate = DateTime.Now;
                    newqr.UpdatedUserID = User.Identity.Name;
                    newqr.RelatedUserId = user.Id;
                    _QrCodeService.Repository.Insert(newqr);
                    ///添加临时二维码时 先插入数据获取当前记录id
                    ///目前商定 所有活动用二维码 由10位数字组成 前三位代表活动id，后7位代表活动用二维码id。      
                    ///
                    newqr.SceneId = newactivityId * 10000000 + newqr.Id;
                    //var config = WeChatCommonService.GetWeChatConfigByID(newAppid);
                    ///临时用二维码 过期时间为2592000秒 30天

                    ///x向腾讯申请二维码 并获取二维码url 
                    var result = QrCodeApi.Create(config.WeixinAppId, config.WeixinCorpSecret, 2592000, (int)newqr.SceneId);
                    if (result.errcode == Weixin.ReturnCode.请求成功)
                    {
                        var qrurl = QrCodeApi.GetShowQrCodeUrl(result.ticket);
                        var userinfo = Innocellence.Weixin.MP.AdvancedAPIs.UserApi.Info(config.WeixinCorpId, config.WeixinCorpSecret, openId);
                        string userHeadUrl = userinfo.headimgurl;
                        newqr.Url = CombineQrCodeAndHeadImg(userHeadUrl, qrurl);
                    }
                    ///重新更新当前model
                    _QrCodeService.Repository.Update(newqr);

                }
                return Json(new { QrcodeUrl = newqr.Url, SceneId = newqr.SceneId, Status = 200, UpdatedDate = newqr.UpdatedDate }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }
            return Json(new { Status = 400 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过sceneId 获取二维码链接
        /// </summary>
        /// <param name="sceneId"></param>
        /// <returns></returns>
        /// 

        public ActionResult GetQrUrlBySceneId(string wechatappid, string sceneId)
        {

            if (!VerifyParam("wechatappid,sceneId"))
            {
                return ErrMsg();
            }
            int newappid = System.Convert.ToInt32(wechatappid);
            int newsceneId = System.Convert.ToInt32(sceneId);
            try
            {
                var qrCode = _QrCodeService.Repository.Entities.Where(x => x.SceneId == newsceneId);


                if (qrCode.Any())
                {
                    var userOpenId = "";
                    var thisqr = qrCode.FirstOrDefault();

                    UpdateQRCode(thisqr, newappid, string.Empty);
                    return Json(new { Status = 200, QRurl = thisqr.Url, UpdatedDate = thisqr.UpdatedDate }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }
            return Json(new { Status = 400 }, JsonRequestBehavior.AllowGet);
        }


        public void UpdateQRCode(QrCodeMPItem thisqr, int newappid, string isUpdate)
        {

            if (thisqr.UpdatedDate.Value.AddMonths(1) < DateTime.Now || isUpdate.Equals("true"))
            {
                var config = WeChatCommonService.GetWeChatConfigByID(newappid);
                var result = QrCodeApi.Create(config.WeixinAppId, config.WeixinCorpSecret, 2592000, (int)thisqr.SceneId);
                if (result.errcode == Weixin.ReturnCode.请求成功)
                {
                    var user = _WechatMPUserService.Repository.Entities.Where(u => u.Id == thisqr.RelatedUserId);
                    if (user.Any())
                    {
                        var userinfo = Innocellence.Weixin.MP.AdvancedAPIs.UserApi.Info(config.WeixinCorpId, config.WeixinCorpSecret, user.FirstOrDefault().OpenId);
                        string userHeadUrl = userinfo.headimgurl;
                        var qrurl = QrCodeApi.GetShowQrCodeUrl(result.ticket);
                        thisqr.Url = CombineQrCodeAndHeadImg(userHeadUrl, qrurl);
                        thisqr.UpdatedDate = DateTime.Now;
                    }
                }
                ///重新更新当前model
                _QrCodeService.Repository.Update(thisqr);
            }
        }


        /// <summary>
        /// 稳糖注册登录时通过用户openid获取 推荐人的customerId
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        /// 
        public ActionResult GetCustomerNoFromInvitied(string openId)
        {
            if (!VerifyParam("openId"))
            {
                return ErrMsg();
            }

            try
            {
                var findHistory = _FocusHistoryService.Repository.Entities
                                    .Where(focus => focus.UserId == openId && focus.Status == 1 && focus.QrCodeSceneId > 10000000)
                                    .OrderByDescending(x => x.CreatedTime);
                if (findHistory.Any())
                {
                    var qrSceneId = findHistory.FirstOrDefault().QrCodeSceneId;
                    _Logger.Debug("qrSceneId :{0}", qrSceneId);
                    var qrcode = _QrCodeService.Repository.Entities.Where(q => q.SceneId == qrSceneId);

                    if (qrcode.Any())
                    {
                        var relateUserId = qrcode.FirstOrDefault().RelatedUserId;
                        _Logger.Debug("relateUserId :{0}", relateUserId);

                        if (relateUserId != null && relateUserId != 0)
                        {
                            var relateuser = _WechatMPUserService.Repository.Entities.Where(user => user.Id == relateUserId);
                            if (relateuser.Any())
                            {
                                var inviteCustomerNo = relateuser.FirstOrDefault().CustomerNO;
                                _Logger.Debug("inviteCustomerNo :{0}", inviteCustomerNo);

                                return Json(new { Status = 200, Custormer = inviteCustomerNo }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.Debug(e);
                Json(new { Status = 400 }, JsonRequestBehavior.AllowGet);
            }
            ///未找到邀请人信息
            return Json(new { Status = 500 }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetHeadImgUrls(string openIds)
        {
            try
            {
                List<string> allOpenIds = JsonConvert.DeserializeObject<List<String>>(openIds);

                var relateuser = _WechatMPUserService.Repository.Entities.Where(user => allOpenIds.Contains(user.OpenId));

                List<OpenIdAndImgUrls> openList = new List<OpenIdAndImgUrls>();
                foreach (var user in relateuser)
                {
                    OpenIdAndImgUrls open = new OpenIdAndImgUrls();
                    open.ImgUrl = user.HeadImgUrl;
                    open.OpenId = user.OpenId;
                    open.NickName = user.NickName;
                    openList.Add(open);
                }

                string stringOpenList = JsonConvert.SerializeObject(openList);
                return Json(new { Status = 200, OpenIdImgs = stringOpenList }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Json(new { Status = 400 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = 500 }, JsonRequestBehavior.AllowGet);

        }

        public class OpenIdAndImgUrls
        {
            public string OpenId { get; set; }
            public string ImgUrl { get; set; }
            public string NickName { get; set; }
        }

        /// <summary>
        /// 下载二维码和头像 并生成新的图片
        /// </summary>
        /// <param name="headUrl"></param>
        /// <param name="qrUrl"></param>
        /// <returns></returns>
        private string CombineQrCodeAndHeadImg(string headUrl, string qrUrl)
        {
            string headfile = DownLoadFiles(headUrl, "Headimg");
            string qrfile = DownLoadFiles(qrUrl, "QRcode");
            return CombinQrWithHeadImg(headfile, qrfile);

        }

        private string CombinQrWithHeadImg(string headUrl, string qrUrl)
        {
            DateTime now = DateTime.Now;
            string ourPath = string.Format("/{0}/{1}/{2}/{3}/{4}.jpg", "content/wentang/finalImg", now.Year.ToString(), now.Month.ToString(), now.Day.ToString(), now.Ticks.ToString());// "/content/wentang/finalImg/" + now.Ticks + ".jpg";
            string savePath = Server.MapPath(ourPath);
            try
            {
                using (System.Drawing.Image qrimg = System.Drawing.Image.FromFile(qrUrl))
                {
                    //如果原图片是索引像素格式之列的，则需要转换
                    Bitmap bmp = new Bitmap(qrimg.Width, qrimg.Height, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(qrimg, 0, 0, qrimg.Width, qrimg.Height);
                        using (System.Drawing.Image headimg = System.Drawing.Image.FromFile(headUrl))
                        {
                            g.DrawImage(headimg, 150, 150, 130, 130);

                            if (!System.IO.File.Exists(savePath))//判断文件是否存在 
                            {
                                string saveFolderPath = savePath.Replace(savePath.Split('\\').Last(), "");

                                if (!Directory.Exists(saveFolderPath))//判断文件夹是否存在 
                                {
                                    Directory.CreateDirectory(saveFolderPath);//不存在则创建文件夹 
                                }
                                bmp.Save(savePath);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }
            finally
            {
                _Logger.Error(qrUrl);
                DeleteTempFile(headUrl, qrUrl);

            }

            return string.Format("http://{0}{1}", Request.Url.Host, ourPath);
        }

        /// <summary>
        /// 下载二维码或者头像
        /// </summary>
        /// <param name="url"></param>
        /// <param name="qrOrHead"></param>
        /// <returns></returns>
        private string DownLoadFiles(string url, string qrOrHead)
        {
            string ourPath = "/content/wentang/" + qrOrHead + "/" + DateTime.Now.Ticks + ".jpg";
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
            return savePath;
        }

        private void DeleteTempFile(params string[] args)
        {
            _Logger.Debug("delete");

            if (args != null && args.Length > 0)
            {
                _Logger.Debug(args);
                foreach (var item in args)
                {
                    try
                    {
                        if (System.IO.File.Exists(item))
                        {
                            System.IO.File.Delete(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
        }

        #endregion

        public ActionResult GetUnionIdByOpenId(string openId)
        {
            if (!VerifyParam("openId") && !string.IsNullOrEmpty(openId))
            {
                return ErrMsg();
            }
            string unionId = string.Empty;
            var user = _WechatMPUserService.Repository.Entities.FirstOrDefault(u => u.IsCanceled == false && u.OpenId.Equals(openId, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                unionId = user.UnionId;
                _Logger.Debug("user:{0}", user.NickName);
            }
            _Logger.Debug("unionId:{0}", unionId);
            return Json(new
            {
                Status = 200,
                UnionId = unionId
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCustomInfoByUnionId(string unionId)
        {
            if (!VerifyParam("unionId"))
            {
                return ErrMsg();
            }
            try
            {
                _Logger.Debug("unionId is :{0}", unionId);
                string customerName = string.Empty;
                string customerMobile = string.Empty;
                string openId = string.Empty;
                if (!string.IsNullOrEmpty(unionId))
                {
                    var user = this._BaseService.Repository.Entities.FirstOrDefault(u => u.IsCanceled == false && u.UnionId.Equals(unionId, StringComparison.OrdinalIgnoreCase));
                    if (user != null)
                    {
                        openId = user.OpenId;
                    }
                }
                _Logger.Debug("openId is :{0}", openId);
                dynamic result = null;
                if (!string.IsNullOrEmpty(openId))
                {
                    result = this._Agent.Invoke("SelectNew", "WechatID", openId);
                }
                else
                {
                    result = this._Agent.Invoke("SelectNew", "UnionId", unionId);
                }
                _Logger.Debug("result is :{0}", result);
                var objRet = JsonConvert.DeserializeObject(result);
                int intStatus = objRet.Status;
                if (intStatus == 1)
                {
                    customerName = objRet.CustomerName;
                    customerMobile = objRet.Mobile;
                }
                return Json(new
                {
                    success = true,
                    message = "",
                    Status = 200,
                    customerName = customerName,
                    customerMobile = customerMobile
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _Logger.Error(e);
                return ErrMsg(e.ToString());
            }
        }


        public ActionResult Test()
        {
            string name = string.Empty;
            var backendbaseUrl = "http://wechat.yey666.com/";
            var apiUrl = string.Format("{0}wentang/api/GetUnionIdByOpenId", backendbaseUrl);

            var lst = new List<KeyValuePair<string, string>>();
            lst.Add(new KeyValuePair<string, string>("openId", "oa3JPwfY2EN74E6URmNMR_woHdiY"));

            var result = CommonWebService.PostInBackend(CommonWebService.GetParas(lst), apiUrl);
            if (result.Status == 200)
            {
                name = result.UnionId.Value;
            }
            return Json(new { name = name }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test1()
        {
            var backendbaseUrl = "http://wechat.yey666.com/";
            var apiUrl = string.Format("{0}wentang/api/GetCustomInfoByUnionId", backendbaseUrl);

            var lst = new List<KeyValuePair<string, string>>();
            lst.Add(new KeyValuePair<string, string>("unionId", "oa3JPwfY2EN74E6URmNMR_woHdiY"));

            var result = CommonWebService.PostInBackend(CommonWebService.GetParas(lst), apiUrl);

            var jsonResult = JsonConvert.DeserializeObject(result);

            return Json(new { test = jsonResult }, JsonRequestBehavior.AllowGet);
        }
    }



    public class CommonWebService
    {

        public static dynamic PostInBackend(string param, string updateUrl)
        {
            dynamic retext;
            byte[] bs = Encoding.ASCII.GetBytes(param);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(updateUrl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理  
                Stream responseStream = wr.GetResponseStream();
                StreamReader readStream = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                retext = JsonConvert.DeserializeObject(readStream.ReadToEnd().ToString());
                readStream.Close();
                return retext;
            }
        }

        public static string GetParas(List<KeyValuePair<string, string>> lstSrc)
        {
            var lst = lstSrc.ToList();
            var appid = "27";
            lst.Add(new KeyValuePair<string, string>("appId", appid));
            lst.Add(new KeyValuePair<string, string>("timestamp", GetTimestamp().ToString()));
            lst.Add(new KeyValuePair<string, string>("nonce", GetNoncestr()));
            lst.Add(new KeyValuePair<string, string>("sign", GetSignature(lst)));
            lst.RemoveAll(a => a.Key == "appSignKey");

            StringBuilder sb = new StringBuilder();
            foreach (var a in lst)
            {
                if (a.Key != "")
                {
                    sb.AppendFormat("&{0}={1}", a.Key, HttpContext.Current.Server.UrlEncode(a.Value));
                }

            }

            return sb.ToString().Substring(1);

        }

        public static string GetSignature(List<KeyValuePair<string, string>> RequestPara)
        {


            RequestPara.Add(new KeyValuePair<string, string>("appSignKey", "R2xBNTNTVmxocTRIbm45V0lKeURZaW93RkpnQTIzUWV3OCtzejllL29ObXdUdVYwSWtXT0dnPT0="));

            StringBuilder sb = new StringBuilder();

            OrdinalComparer comp = new OrdinalComparer();
            //参数排序
            var keys = RequestPara.OrderBy(a => a.Key, comp);

            foreach (var a in keys)
            {
                sb.AppendFormat("&{0}={1}", a.Key, a.Value);
            }

            //加密
            return GetSha1(sb.ToString().Substring(1)).ToLower();
        }
        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetNoncestr()
        {
            Random random = new Random();
            return Weixin.Helpers.EncryptHelper.GetMD5(random.Next(1000).ToString(), "GBK");
        }


        public static string GetSha1(string str)
        {
            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();
            //将mystr转换成byte[] 
            //  ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = Encoding.UTF8.GetBytes(str);
            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");
            return hash;
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }




    }
}