
using System.ComponentModel;

namespace Innocellence.WeChat.Domain.Common
{
    public enum WechatUserMessageLogContentType
    {
        DEFAULT = 0,

        //RequestMsgType
        [Description("文本")]
        Request_Text = 1,
        [Description("地理位置")]
        Request_Location = 2,
        [Description("图片")]
        Request_Image = 3,
        [Description("语音")]
        Request_Voice = 4,
        [Description("视频")]
        Request_Video = 5,
        [Description("链接")]
        Request_Link = 6,
        [Description("事件")]
        Request_Event = 7,
        [Description("跳转事件")]
        Request_Event_View = 8,
        [Description("点击事件")]
        Request_Event_Click = 9,
        [Description("小视频")]
        Request_ShortVideo = 10,
        [Description("")]
        Request_Chat = 11,
        [Description("扫码事件")]
        Request_Event_Scan = 12,
        [Description("订阅事件")]
        Request_Event_Subscribe = 13,
        [Description("扫码推事件")]
        Request_Event_ScanCodePush = 14,
        [Description("扫码带提示")]
        Request_Event_ScanCodeWait = 15,
        [Description("进入应用")]
        Request_Event_Enter = 16,
        [Description("扫码关注事件")]
        Request_Event_Subscribe_With_Scan = 17,
        [Description("取消关注")]
        Request_Event_UnSubscribe = 18,
        //AutoReplyContentEnum
        [Description("")]
        Response_Empty = 100,
        [Description("")]
        Response_Link = 101,
        [Description("")]
        Response_Text = 102,
        [Description("")]
        Response_Image = 103,
        [Description("")]
        Response_Voice = 104,
        [Description("")]
        Response_Video = 105,
        [Description("")]
        Response_News = 106,
        [Description("")]
        Response_File = 107,
    }
}
