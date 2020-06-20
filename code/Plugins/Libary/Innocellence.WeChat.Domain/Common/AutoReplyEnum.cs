using System.ComponentModel;
namespace Innocellence.WeChat.Domain.Common
{
    /// <summary>
    /// 口令类型
    /// </summary>
    public enum AutoReplyKeywordEnum
    {
        [Description("文本")]
        TEXT = 1,
        [Description("菜单")]
        MENU = 2,
        [Description("图片")]
        IMAGE = 3,
        [Description("语音")]
        AUDIO = 4,
        [Description("视频")]
        VIDEO = 5,
        [Description("地理位置")]
        LOCATION = 6,
        [Description("进入应用")]
        EnterEvent = 311,
        [Description("首次进入")]
        FirstEnterEvent = 312,
        //[Description("关注账号")]
        //SubscribeEvent = 313,
        //[Description("地理位置")]
        //LOCATION = 6,
        //[Description("链接")]
        //LINK = 7,
        [Description("所有")]
        ALL = 100   // 如果口令类型设置为“所有”，那么不管用户发送任何类型口令，在没有与之匹配的口令时, 都会做同样的回应
    }

    /// <summary>
    /// 口令类型
    /// </summary>
    public enum AutoReplyMPKeywordEnum
    {
        [Description("文本")]
        TEXT = 1,
        [Description("菜单")]
        MENU = 2,
        [Description("图片")]
        IMAGE = 3,
        [Description("语音")]
        AUDIO = 4,
        [Description("视频")]
        VIDEO = 5,
        [Description("地理位置")]
        LOCATION = 6,
        [Description("链接")]
        LINK = 7,
        [Description("扫码")]
        SCAN = 50,
        //[Description("关注")]
        //FOCUS = 51,
        //[Description("进入应用")]
        //EnterEvent = 311,

        [Description("关注账号")]
        SubscribeEvent = 312,
        [Description("扫码关注")]
        SubscribeWithScan = 362,
        [Description("所有")]
        ALL = 100   // 如果口令类型设置为“所有”，那么不管用户发送任何类型口令，在没有与之匹配的口令时, 都会做同样的回应
    }

    /// <summary>
    /// 文本匹配
    /// </summary>
    public enum AutoReplyTextMatchEnum
    {
        [Description("完全")]
        EQUAL = 1,
        [Description("前缀")]
        START_WITH = 2,
        [Description("后缀")]
        END_WITH = 3,
        [Description("包含")]
        CONTAIN = 4
    }

    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum AutoReplyMenuEnum
    {
        [Description("普通")]
        NORMAL = 1,
        [Description("扫码带提示")]
        SCAN_WITH_PROMPT = 2,
        [Description("扫码推事件")]
        SCAN_PUSH_EVENT = 3
    }

    /// <summary>
    /// 如果回复类型为图文消息，可以自动回复最新的前几条，或者手动选择
    /// </summary>
    public enum AutoReplyNewsEnum
    {
        [Description("回复最新")]
        LATEST = 1,
        [Description("手动选择")]
        MANUAL = 2
    }

    /// <summary>
    /// 回复类型
    /// </summary>
    public enum AutoReplyContentEnum
    {
        [Description("链接")]
        LINK = 1,
        [Description("文本")]
        TEXT = 2,
        [Description("图片")]
        IMAGE = 3,
        [Description("语音")]
        VOICE = 4,
        [Description("视频")]
        VIDEO = 5,
        [Description("图文")]
        NEWS = 6,
        [Description("文件")]
        FILE = 7
    }
}
