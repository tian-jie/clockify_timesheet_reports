using Innocellence.CA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.CA.Service.Common
{

    public class ConstData
    {
        public const string  LAN_EN ="EN";
        public const string  LAN_CN ="ZH";
        public const string  STATUS_NEW = "Saved";
        public const string STATUS_PUBLISH = "Published";
    }

    public   class SysCommon
    {

        public static SysWechatConfig objCurWeChatConfig = null;


        //public static readonly string Token = CommonService.GetSysConfig(SysConfigCode.WeixinToken,"");// WebConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        //public static readonly string EncodingAESKey = CommonService.GetSysConfig(SysConfigCode.WeixinEncodingAESKey, "");//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        //public static readonly string AppId = CommonService.GetSysConfig(SysConfigCode.WeixinAppId, "");//与微信公众账号后台的AppId设置保持一致，区分大小写。
        //public static readonly string CorpId = CommonService.GetSysConfig(SysConfigCode.WeixinCorpId, "");//与微信企业账号后台的CorpId设置保持一致，区分大小写。
        //public static readonly string CorpSecret = CommonService.GetSysConfig(SysConfigCode.WeixinCorpSecret, "");

        public static string GetUserName(IIdentity identity)
        {
            string identityName = identity == null ? null : identity.Name;
            if (string.IsNullOrEmpty(identityName))
                return identityName;
            int index = identityName.IndexOf('\\');
            if (index > 0 && index < (identityName.Length - 1))
            {
                identityName = identityName.Substring(index + 1);
            }
            index = identityName.IndexOf('@');
            if (index > 0 && index < (identityName.Length - 1))
            {
                identityName = identityName.Remove(index);
            }

            return identityName;
        }
    }
}
