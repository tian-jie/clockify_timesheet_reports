using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class AttachmentsItemView : IViewModel
    {
        public Int32 Id { get; set; }
        public Int32? AppId { get; set; }
        public DateTime? MediaExpireTime { get; set; } //应该是计算文件过期时间,是否保留?能否更新?Schedule更新? 暂留
        public String Extension { get; set; }
        public String AttachmentUrl { get; set; } //全路径, 不带host, content/xxx, 需要压缩, 最大支持1024*768
        public Int64? FileSize { get; set; }
        public String Description { get; set; }
        public String MediaId { get; set; }
        public Int32? Type { get; set; }
        public String Duration { get; set; } //暂留, 作用不清楚
        public DateTime? CreateTime { get; set; }
        public String AttachmentTitle { get; set; } //用于显示文件Title
        public Int32? Width { get; set; } //压缩之后的?
        public String ThumbUrl { get; set; } //压缩之后的路径, 用于预览, //w,h 最大100, 可以适当修改.
        public Int32? Height { get; set; } //压缩之后的?
        public String UserName { get; set; }
        public String UserId { get; set; }
        public Boolean? IsDeleted { get; set; }
        public Int32? DownloadCounts { get; set; } //For File
        public Boolean? Opend { get; set; } //for file

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (SysAttachmentsItem)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            MediaExpireTime = entity.MediaExpireTime;
            Extension = entity.Extension;
            AttachmentUrl = entity.AttachmentUrl;
            FileSize = entity.FileSize;
            Description = entity.Description;
            MediaId = entity.MediaId;
            Type = entity.Type;
            Duration = entity.Duration;
            CreateTime = entity.CreateTime;
            AttachmentTitle = entity.AttachmentTitle;
            Width = entity.Width;
            ThumbUrl = entity.ThumbUrl;
            Height = entity.Height;
            UserName = entity.UserName;
            UserId = entity.UserId;
            IsDeleted = entity.IsDeleted;
            DownloadCounts = entity.DownloadCounts;
            Opend = entity.Opend;

            return this;
        }

        public SysAttachmentsItem ConvertToEntity()
        {
            SysAttachmentsItem entity = new SysAttachmentsItem();
            entity.Id = Id;
            entity.AppId = AppId;
            entity.MediaExpireTime = MediaExpireTime;
            entity.Extension = Extension;
            entity.AttachmentUrl = AttachmentUrl;
            entity.FileSize = FileSize;
            entity.Description = Description;
            entity.MediaId = MediaId;
            entity.Type = Type;
            entity.Duration = Duration;
            entity.CreateTime = CreateTime;
            entity.AttachmentTitle = AttachmentTitle;
            entity.Width = Width;
            entity.ThumbUrl = ThumbUrl;
            entity.Height = Height;
            entity.UserName = UserName;
            entity.UserId = UserId;
            entity.IsDeleted = IsDeleted;
            entity.DownloadCounts = DownloadCounts;
            entity.Opend = Opend;
            return entity;
        }
    }

    public class AttachmentsItemPostProperty
    {
        public string ServerPath { get; set; }
        public string SaveFullName { get; set; }
        public string FileName { get; set; }
        public string TargetFilePath { get; set; }
        public string UploadFileType { get; set; }
        public int AppId { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string VideoCoverSrc { get; set; }
        public string ViewId { get; set; }
        public String MediaId { get; set; }
        public DateTime? MediaExpireTime { get; set; }
        /// <summary>
        /// 新闻封面不需要存入素材DB,
        /// 新闻封面需要生成固定宽为200,500的两张缩略图
        /// </summary>
        public bool IsNewsCover { get; set; }
    }
}
