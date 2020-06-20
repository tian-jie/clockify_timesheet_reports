using Infrastructure.Core;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class WechatMessageLogView : IViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public List<NewsInfoView> Content { get; set; }
        public DateTime PublishDate { get; set; }
        public string Type { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (WechatMessageLog)model;
            var result = new WechatMessageLogView()
            {
                PublishDate = entity.CreatedTime,
                Id = entity.Id,
                Type = ((WechatMessageLogType)entity.ContentType).ToString(),
                UserName =  entity.UserName
            };
		    result.Content = JsonConvert.DeserializeObject<List<NewsInfoView>>(entity.Content);
            return result;
        }
    }
}
