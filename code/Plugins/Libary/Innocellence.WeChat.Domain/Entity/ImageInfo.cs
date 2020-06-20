using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class ImageInfo : EntityBase<int>
    {
        //[Id("Id",IsDbGenerated=true)]
        public override Int32 Id { get; set; }

        public int OwnerId { get; set; }
        /// <summary>
        /// 图片的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 图片的URL地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 图片内容
        /// </summary>
        public byte[] Content { get; set; }


    }
}
