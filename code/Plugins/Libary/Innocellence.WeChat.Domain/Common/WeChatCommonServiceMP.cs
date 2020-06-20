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
using Innocellence.Weixin.MP.Helpers;
using System.Collections.Specialized;
using Innocellence.Weixin;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.Weixin.Helpers;
using Innocellence.Weixin.MP.AdvancedAPIs.UserTag;

namespace Innocellence.WeChat.Domain.Common
{
    public partial class WeChatCommonService : Innocellence.WeChat.Domain.Contracts.ICommonService
    {
        // public ILogger Logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static string GetSignature(string param, HttpRequestBase Request)
        {
            //获取所有的参数
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var _params = param.Split(',');

            foreach (var a in _params)
            {
                if (a == "url")
                {
                    dict.Add(a, Request[a].Split(',')[0]);
                }
                else
                {
                    dict.Add(a, Request[a]);
                }
            }

            var config = GetWeChatConfigByID(int.Parse(Request["AppID"]));
            //Todo
            // dict.Add("appSignKey", "iMih0xabKQdw8CBbkTM5Ley84WhN4oL6u5lbDui6G9tUlQo7fJE1CcktZ2UiETnU1FZ0R3ZvzYLKOzmaziyms5QuMia8czkEwFv2TQUg4G45Ha0aHPEHXnhjVqUPnKPJ");

            dict.Add("appSignKey", config.AppSignKey);

            StringBuilder sb = new StringBuilder();

            OrdinalComparer comp = new OrdinalComparer();

            //参数排序
            var keys = dict.Keys.OrderBy(a => a, comp);

            foreach (var a in keys)
            {
                sb.AppendFormat("&{0}={1}", a, dict[a]);
            }

            Logger.Debug("GetSha1 Key:" + sb.ToString());

            //加密
            return EncryptHelper.GetSha1(sb.ToString().Substring(1)).ToLower();
        }

        /// <summary>
        /// 同步微信用户
        /// </summary>
        /// <param name="WeixinOpenId"></param>
        /// <param name="userInfo"></param>
        /// <param name="config"></param>
        public static void SycUserFromWeixinMP(string WeixinOpenId, ref WechatMPUser userInfo, SysWechatConfig config)
        {
            Logger.Debug("SycUserFromWeixin");
            Innocellence.Weixin.MP.AdvancedAPIs.User.BatchGetUserInfoData user = new Innocellence.Weixin.MP.AdvancedAPIs.User.BatchGetUserInfoData
            {
                openid = WeixinOpenId,
                lang = Language.zh_CN.ToString()
            };
            var result = Innocellence.Weixin.MP.AdvancedAPIs.UserApi.BatchGetUserInfo(config.WeixinAppId,
                config.WeixinCorpSecret,
                new List<Innocellence.Weixin.MP.AdvancedAPIs.User.BatchGetUserInfoData> {
                        user
                });
            Logger.Debug("SycUserFromWeixin result");
            if (result != null && result.user_info_list != null)
            {
                Logger.Debug("result count :{0}", result.user_info_list.Count);
                var objuserInfo = result.user_info_list.Select(a => WechatMPUserView.ConvertWeChatUserToMpUser(a, config.AccountManageId.Value, config.Id)).ToList().FirstOrDefault();
                if (userInfo == null)
                {
                    userInfo = objuserInfo;
                }
                if (objuserInfo != null)
                {
                    Logger.Debug("SycUserFromWeixin userInfo: " + userInfo.OpenId + userInfo.NickName);
                    objuserInfo.AccountManageId = config.AccountManageId;
                    if (objuserInfo.SubScribe == 0) //未关注
                    {
                        objuserInfo.Id = userInfo.Id;
                        objuserInfo.IsCanceled = true;
                        userInfo = objuserInfo;
                    }
                    else
                    {
                        objuserInfo.Id = userInfo.Id;
                    }
                    Logger.Debug("SycUserFromWeixin userInfo id: {0}, SubScribe: {1}", objuserInfo.Id, objuserInfo.SubScribe);
                    IWechatMPUserService wechatMPUserService = EngineContext.Current.Resolve<IWechatMPUserService>();
                    wechatMPUserService.RegistToWeiXin(objuserInfo);
                    Logger.Debug("SycUserFromWeixin userInfo updated.");
                }
                else
                {
                    Logger.Debug("SycUserFromWeixin Error objuserInfo=null.{0}", WeixinOpenId);
                }
            }
        }

        public static List<TagJson_Tag> GetAllGroupFromServer(int accountManageId)
        {
            var lst = cacheManager.Get<List<TagJson_Tag>>("TagJson_Tag_" + accountManageId, () =>
             {
                 var config = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(p => p.AccountManageId == accountManageId && !p.IsCorp.Value);
                 if (null != config)
                 {
                     var returnValue = Innocellence.Weixin.MP.AdvancedAPIs.UserTagApi.Get(config.WeixinAppId, config.WeixinCorpSecret);
                     if (returnValue != null && returnValue.tags != null && returnValue.tags.Count > 0)
                     {
                         return returnValue.tags;
                     }
                 }
                 return new List<TagJson_Tag> { };
             });
            return lst;
        }

        public static void CleanGroupCache(int accountManageId)
        {
            cacheManager.Remove("TagJson_Tag_" + accountManageId);
        }
    }

    public class OrdinalComparer : System.Collections.Generic.IComparer<String>
    {
        public int Compare(String x, String y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}
