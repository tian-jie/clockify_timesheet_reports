using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System.Collections.Generic;

namespace Innocellence.WeChat.Domain.Contracts.ViewModel
{
    public class PageBean
    {
        /// <summary>
        /// 
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int current { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int> pages { get; set; }
    }

    public class DXYImagesRootView
    {
        /// <summary>
        /// 
        /// </summary>
        public string thispath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<SysAttachmentsItem> attachments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PageBean pageBean { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }

        //public List<AttachmentsTextItem> items { get; set; }

        //public List<AttachmentsFilesItem> files { get; set; }
    }

    public class DXYQRCodeRootView
    {
        /// <summary>
        /// 
        /// </summary>
        public string thispath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PageBean pageBean { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<QrCodeMPItem> items { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }
    }

   
}
