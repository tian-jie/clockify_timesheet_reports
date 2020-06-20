using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
    //[Table("ArticleInfoView")]

    public partial class ImageInfoView : IViewModel
    {

        public Int32 Id { get; set; }

        public Int32 OwnerId { get; set; }
        public String Name { get; set; }
        public String Url { get; set; }
        public byte[] Content { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ImageInfo)obj;
            Id = entity.Id;
            Name = entity.Name;
            Url = entity.Url;
            Content = entity.Content;
            OwnerId = entity.OwnerId;

            return this;
        }

    }
}
