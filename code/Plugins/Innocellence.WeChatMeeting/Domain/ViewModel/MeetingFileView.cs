using Infrastructure.Core;
using Innocellence.WeChatMeeting.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMeeting.Domain.ViewModel
{
    public class MeetingFileView : IViewModel
    {
        public MeetingFileView() { }
        public  Int32 Id { get; set; }
        public Int32? MeetingId { get; set; }
        public String FileName { get; set; }
        public String FilePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (MeetingFile)obj;
            Id = entity.Id;
            MeetingId = entity.MeetingId;
            FileName = entity.FileName;
            FilePath = entity.FilePath;
            CreatedDate = entity.CreatedDate;
            IsDeleted = entity.IsDeleted;

            return this;
        }
    }
}