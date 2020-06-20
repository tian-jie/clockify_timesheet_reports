using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class WechatPreviewMessageLog : EntityBase<int>
    {
        public override int Id { get; set; }

        public int AppID { get; set; }

        /// <summary>
        /// request.FromUserName
        /// response.ToUserName
        /// </summary>
        public String UserID { get; set; }

        /// <summary>
        /// 根据UserID 从通讯录中读取
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// Text - 回复文本的内容
        /// News - Article 的ID, 用逗号分隔
        /// 图片,视频,语音,文件等, 存储FileID
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        /// WechatUserMessageLogContentType
        /// </summary>
        public int ContentType { get; set; }

        public DateTime CreatedTime { get; set; }

    }
}
