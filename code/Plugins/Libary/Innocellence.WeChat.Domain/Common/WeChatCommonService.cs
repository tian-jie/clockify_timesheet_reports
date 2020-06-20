using Infrastructure.Core.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using Infrastructure.Core.Logging;
using Infrastructure.Core.Caching;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Core.Infrastructure;
//using Innocellence.WeChatMain.Entity;
using Autofac;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.CommonAPIs;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
//using Innocellence.WeChatMain.Entity;
using Innocellence.Weixin.QY.AdvancedAPIs;
//using Innocellence.WeChatMain.Entity;
using Innocellence.Weixin.QY.AdvancedAPIs.App;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System.Text;
using System.Web;
using Innocellence.Weixin.Helpers;
using Innocellence.Weixin;
using Innocellence.WeChat.Domain.Contracts;
using Newtonsoft.Json;





namespace Innocellence.WeChat.Domain.Common
{
    public partial class WeChatCommonService : Innocellence.WeChat.Domain.Contracts.ICommonService
    {
        // public ILogger Logger { get; set; }

        private static ILogger Logger = LogManager.GetLogger(typeof(WeChatCommonService));


        #region wechatconfig

        /// <summary>
        /// SysWeChatConfig的缓存
        /// </summary>
        public static List<SysWechatConfig> lstSysWeChatConfig
        {
            get
            {
                var lst = cacheManager.Get<List<SysWechatConfig>>("SysWeChatConfig", () =>
                {

                    BaseService<SysWechatConfig> ser = new BaseService<SysWechatConfig>();
                    return ser.Repository.Entities.ToList();
                });

                return lst;
            }
        }

        public static List<AccountManage> lstAccountManage
        {
            get
            {
                var lst = cacheManager.Get<List<AccountManage>>("AccountManage", () =>
                {

                    BaseService<AccountManage> ser = new BaseService<AccountManage>();
                    return ser.Repository.Entities.ToList();
                });

                return lst;
            }
        }

        /// <summary>
        /// 根据WeChatID 获取AccountManage信息
        /// </summary>
        /// <param name="iWeChatID"></param>
        /// <returns></returns>
        public static AccountManage GetAccountManageByWeChatID(int iWeChatID = 0)
        {
            var obj = lstSysWeChatConfig.Find(a => a.Id == iWeChatID);
            if (obj == null)
            {
                throw new Exception("WeChatID Not Found!");
            }

            return lstAccountManage.Find(a => a.Id == obj.AccountManageId);
        }

        ///// <summary>
        ///// 根据appid获取Config信息
        ///// </summary>
        ///// <param name="iAppID"></param>
        ///// <returns></returns>
        //public static SysWechatConfig GetWeChatConfig(int iAppID = 0)
        //{
        //    return GetWeChatConfig(iAppID.ToString());
        //}

        //public static SysWechatConfig GetWeChatConfig(string AppID )
        //{

        //    if (AppID == "0" && lstSysWeChatConfig.Count > 0)
        //    {
        //        return lstSysWeChatConfig.FirstOrDefault();
        //    }

        //    var obj = lstSysWeChatConfig.Find(a => a.WeixinAppId == AppID);

        //    return obj;
        //}

        /// <summary>
        /// 根据系统分配的微信ID获取配置信息
        /// </summary>
        /// <param name="WeChatID"></param>
        /// <returns></returns>
        public static SysWechatConfig GetWeChatConfigByID(int WeChatID)
        {
            var obj = lstSysWeChatConfig.Find(a => a.Id == WeChatID);

            if (obj == null)
            {
                throw new Exception(string.Format("GetWeChatConfigByID： WeChatID {0} 未找到！", WeChatID));
               // return lstSysWeChatConfig.FirstOrDefault();
            }

            return obj;
        }

#endregion


        #region WeChat Token
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewToken"></param>
        /// <returns></returns>
        public static string GetWeiXinToken(int appId, bool getNewToken = false)
        {
            if (appId == 0)
            {
                throw new ArgumentNullException("appId");
            }

            var config = GetWeChatConfigByID(appId);
            if (config.IsCorp.HasValue && !config.IsCorp.Value)
            {
                return Innocellence.Weixin.MP.CommonAPIs.AccessTokenContainer.GetToken(config.WeixinCorpId, config.WeixinCorpSecret, getNewToken);
            }
            else
            {
                return AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret, getNewToken);
            }
           
        }

        /// <summary>
        /// 通过AccountManageId和微信Id来取得token(Add by Liuwei)
        /// </summary>
        /// <param name="accountManageId"></param>
        /// <param name="agentId"></param>
        /// <param name="getNewToken"></param>
        /// <returns></returns>
        public static string GetWeiXinTokenByAgentId(int accountManageId, int agentId = 0, bool getNewToken = false)
        {
            var accessToken = "";
            // 取得微信号ID
           // var account = _accountManageService.Repository.GetByKey(accountManageId);
            // 取得该应用的Secrect
            var config = lstSysWeChatConfig.Find(x => x.AccountManageId == accountManageId && x.WeixinAppId == agentId.ToString());
            if ( config != null)
            {
                accessToken = AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret, getNewToken);
            }

            return accessToken;
        }

        /// <summary>
        /// 通过企业号ID和微信Id来取得token(Add by Liuwei)
        /// </summary>
        /// <param name="weixinCorpId"></param>
        /// <param name="agentId"></param>
        /// <param name="getNewToken"></param>
        /// <returns></returns>
        public static string GetWeiXinTokenByAgentId(string weixinCorpId, int agentId = 0, bool getNewToken = false)
        {
            var accessToken = "";

            // 取得该应用的Secrect
            var config = lstSysWeChatConfig.Find(x => x.WeixinCorpId == weixinCorpId && x.WeixinAppId == agentId.ToString());
            if (config != null)
            {
                accessToken = AccessTokenContainer.TryGetToken(weixinCorpId, config.WeixinCorpSecret, getNewToken);
            }

            return accessToken;
        }

        #endregion



        public static JsSdkUiPackage GetJSSDKConfig(int iAPPID, string strUrl)
        {
          var config=  GetWeChatConfigByID(iAPPID);
            if(config.IsCorp.HasValue && !config.IsCorp.Value){
              return  Innocellence.Weixin.MP.Helpers.JSSDKHelper.GetJsSdkUiPackage(config.WeixinCorpId, config.WeixinCorpSecret, strUrl);
            }else{
                return Innocellence.Weixin.QY.Helpers.JSSDKHelper.GetJsSdkUiPackage(config.WeixinCorpId, config.WeixinCorpSecret, strUrl);
            }
            
        }

        /// <summary>
        /// 同步企业号用户
        /// </summary>
        /// <param name="strUserID"></param>
        /// <param name="user"></param>
        /// <param name="config"></param>
        public static void SyncUserFromWechat(string strUserID,ref SysAddressBookMember user, SysWechatConfig config)
        {

            var token = WeChatCommonService.GetWeiXinToken(config.Id);
            var apiResult = MailListApi.GetMember(token, strUserID);
            IAddressBookService _addressBookServie = EngineContext.Current.Resolve<IAddressBookService>();
            var Nowtime = System.DateTime.Now;
            //没找到用户，直接新建
            if (user == null || user.Id == 0)
            {
                user = new SysAddressBookMember()
                {
                    Avatar = apiResult.avatar,
                    AccountManageId = config.AccountManageId,
                    // CompanyID = apiResult.
                    Gender = apiResult.gender,
                    UserId = apiResult.userid,
                    WeiXinId = apiResult.weixinid,
                    UserName = apiResult.name,
                    Status = 1,
                    Department = JsonConvert.SerializeObject(apiResult.department),
                    EmployeeStatus = Innocellence.WeChat.Domain.ModelsView.AddressBookMemberView.EmployeeStatusEnum.U.ToString(),
                    Mobile = apiResult.mobile,
                    Position = apiResult.position,
                    Email = apiResult.email,
                    CreateTime=DateTime.Now,
                    DeleteFlag=0,
                    SubscribeTime=Nowtime
                };

                _addressBookServie.Repository.Insert(user);
            }
            else
            {
                if (apiResult.errcode == ReturnCode_QY.请求成功)
                {
                    user.Avatar = apiResult.avatar;
                }

                user.Status = 1;
                user.SubscribeTime = Nowtime;
                _addressBookServie.UpdateMember(user);
            }
        }

        //wechat中有类似的方法
        //public static int ConvertDateTimeInt(System.DateTime time)
        //{
        //    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        //    return (int)(time - startTime).TotalSeconds;
        //}


    }



}
