using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Infrastructure.Core;
using System.ComponentModel.DataAnnotations;

namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("Category")]
    /// <summary>
    /// 示例：
    ///     1. 新闻APP看新闻：(http://chinaca-zgkx.XXX.com:25001/News/ArticleInfo/wxdetail/15)
    ///         UserID  : WeChatUserID
    ///         AppID   : 6
    ///         FunctionID: News/ArticleInfo/wxdetail
    ///         Content : 15
    ///         
    ///     2. 点菜单：(开网页View，交给网页处理，同1)
    ///     
    ///     3. 点菜单：(click event，点了CORP_NEWS菜单)
    ///         UserID  : WeChatUserID
    ///         AppID   : 6
    ///         FunctionID: CORP_NEWS
    ///         Content : 空
    ///     
    ///     4. 发消息：(click event，文字消息，内容Hello)
    ///         UserID  : WeChatUserID
    ///         AppID   : 6
    ///         FunctionID: TEXT_EVENT
    ///         Content : Hello
    ///     
    ///     5. 其他网页：(类似1，https://www.chinacmp.XXX.com:25001/News/ArticleInfo/Edit/15)
    ///         UserID  : WeChatUserID
    ///         AppID   : 6
    ///         FunctionID: News/ArticleInfo/Edit
    ///         Content : 15
    /// </summary>
    public partial class UserBehavior : EntityBase<int>
	{
        public override Int32 Id { get; set; }
        /// <summary>
        /// WeChatUserID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 微信企业号的APPID（如新闻是6）
        /// </summary>
        public int AppId { get; set; }
        /// <summary>
        /// 功能
        ///     看新闻：就用url的一部分News/ArticleInfo/wxdetail
        ///     点菜单：就用菜单的ID（CORP_NEWS）
        ///     发消息：分文字消息（TEXT）、图片消息（NEWS）、语音消息（Voice），每种消息类型给一个固定ID
        ///     其他网页：用网页中比较有代表性的字符串
        /// </summary>
        public string FunctionId { get; set; }
        /// <summary>
        /// 在上面那个功能中，具体的信息
        ///     如看新闻：用url中News/ArticleInfo/wxdetail/15，就用15
        ///     点菜单：文字消息，这里存文字消息的内容
        /// </summary>
        public string Content { get; set; }
        public string Url { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Device { get; set; }
        public string ClientIp { get; set; }
        //内容类型
        ///     1. URL中带News 到ArticleInfo表中根据id反查Article信息

        ///     2. URL中带Message 到Message表中根据id反查Message信息
        /// 
        ///     3. URL中带 QuestionDetail  到QuestionManage表中根据id反查question信息
        ///     
        ///     4. 点菜单：(click event，点了CORP_NEWS菜单)

        ///     5. URL中带wxlink
        /// 
        ///     6. faq （index）
        /// 
        ///     7. 在线服务 （raisequestion）/feedback（用户反馈）
        /// 
        ///     8. 发消息：(click event，文字消息，内容Hello)

        ///     9. URL中带caadmin
        
        ///     10. 进入app
        
        ///     11.URL中带 FaqInfo 到FaqInfo表中根据id反查Faq信息
       
        ///     12.URL中带 EventManage   到Event表中根据id反查Event信息（注册与签到）
        public int? ContentType { get; set; }
	}
}
