using Infrastructure.Core;
using System;

namespace Innocellence.WeChat.Domain.Entity
{
    public class SysAttachmentsItem : EntityBase<int>
    {
        public override Int32 Id { get; set; }
        public Int32? AppId { get; set; }
        public DateTime? MediaExpireTime { get; set; } //应该是计算文件过期时间,是否保留?能否更新?Schedule更新? 暂留
        public String Extension { get; set; }
        public String AttachmentUrl { get; set; } //全路径, 不带host, content/xxx, 需要压缩, 最大支持1024*768
        public Int64? FileSize { get; set; }
        public String Description { get; set; }
        public String MediaId { get; set; }
        public Int32? Type { get; set; }
        public String Duration { get; set; } //语音/视频文件的时长
        public DateTime? CreateTime { get; set; }
        public String AttachmentTitle { get; set; } //用于显示文件Title
        public Int32? Width { get; set; }
        public String ThumbUrl { get; set; } //压缩之后的路径, 用于预览, 按照宽200进行等比压缩
        public Int32? Height { get; set; }
        public String UserName { get; set; }
        public String UserId { get; set; }
        public Boolean? IsDeleted { get; set; }
        public Int32? DownloadCounts { get; set; } //For File
        public Boolean? Opend { get; set; } //for file
    }
}
