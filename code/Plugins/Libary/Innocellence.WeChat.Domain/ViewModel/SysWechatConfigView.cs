using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class SysWechatConfigView : IViewModel
    {
        public Int32 Id { get; set; }

        public String WeixinToken { get; set; }
        public String WeixinEncodingAESKey { get; set; }

        public String WeixinCorpId { get; set; }
        public String WeixinCorpSecret { get; set; }

        public Int32? AccountManageId { get; set; }

        //是否企业号
        public Boolean? IsCorp { get; set; }

        //应用名称
        public string AppName { get; set; }
        public String WeixinAppId { get; set; }

        public String AccessToken { get; set; }
        public DateTime? AccessTokenExpireTime { get; set; }

        public String WelcomeMessage { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public String UpdatedUserID { get; set; }

        public Boolean? IsDeleted { get; set; }

        public String CoverUrl { get; set; }


        public IViewModel ConvertAPIModel(object entity)
        {
            var obj = (SysWechatConfig)entity;

            Id = obj.Id;
            WeixinToken = obj.WeixinToken;
            WeixinEncodingAESKey = obj.WeixinEncodingAESKey;
            WeixinCorpId = obj.WeixinCorpId;
            WeixinCorpSecret = obj.WeixinCorpSecret;
            IsCorp = obj.IsCorp;
            AccountManageId = obj.AccountManageId;
            AppName = obj.AppName;
            WeixinAppId = obj.WeixinAppId;
            AccessToken = obj.AccessToken;
            AccessTokenExpireTime = obj.AccessTokenExpireTime;
            WelcomeMessage = obj.WelcomeMessage;
            CreatedDate = obj.CreatedDate;
            CreatedUserID = obj.CreatedUserID;
            UpdatedDate = obj.UpdatedDate;
            UpdatedUserID = obj.UpdatedUserID;
            IsDeleted = obj.IsDeleted;

            CoverUrl = obj.CoverUrl;

            return this;
        }
    }
}
