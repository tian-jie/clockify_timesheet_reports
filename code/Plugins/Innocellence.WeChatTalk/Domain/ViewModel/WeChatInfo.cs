using Infrastructure.Core;
using Innocellence.WeChatTalk.Domain.Entity;
using System;

namespace Innocellence.WeChatTalk.Domain.ViewModel
{
    public class WeChatInfo
    {
        //jssdk config
        public string SdkAppId { get; set; }
        public string Timestamp { get; set; }
        public string NonceStr { get; set; }
        public string Signature { get; set; }

        //login info
        public string WechatUserId { get; set; }
        public string ImgHeadUrl { get; set; }
        public int AppId { get; set; }
        public string ClientName { get; set; }    
        
        public DateTime EnterRoomDate { get; set; }
    }
}
