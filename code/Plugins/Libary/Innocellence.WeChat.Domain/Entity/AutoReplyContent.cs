using Infrastructure.Core;
using System;

namespace Innocellence.WeChat.Domain.Entity
{
    public class AutoReplyContent : EntityBase<int>
    {
        public override Int32 Id { get; set; }

        public Int32? AutoReplyId { get; set; }

        public Int32? PrimaryType { get; set; }

        public Int32? SecondaryType { get; set; }

        public Boolean? IsEncrypt { get; set; }

        public String Content { get; set; }

        public Int32? FileID { get; set; }

        public string MediaId { get; set; }

        public String NewsID { get; set; }

        public bool? IsNewContent { get; set; }

        public string UserTags { get; set; }

        public string MessageTags { get; set; }

        public string UserGroups { get; set; }

        public string InterfaceLink { get; set; }

    }
}