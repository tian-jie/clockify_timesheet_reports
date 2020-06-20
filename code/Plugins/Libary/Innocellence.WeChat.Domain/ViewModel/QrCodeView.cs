using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ModelsView
{
    public class QrCodeView : IViewModel
    {
        public int Id { get; set; }
        public int AppId { get; set; }
        public int? SceneId { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int PurePeopleCount { get; set; }
        public int PeopleCount { get; set; }
        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (QrCodeMPItem)model;
            QrCodeView result = new QrCodeView()
            {
                Id = entity.Id,
                SceneId = entity.SceneId,
                Url = entity.Url,
                Description = entity.Description,
            };
            return result;
        }
    }
}
