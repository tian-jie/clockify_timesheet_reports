using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Contracts.ViewModel
{
    public class UserBehaviorView : IViewModel
    {
        public Int32 Id { get; set; }
        //public string OId { get; set; }
        //public string ActionId { get; set; }
        //public DateTime Time { get; set; }

        public string UserId { get; set; }
        public int AppId { get; set; }
        public string FunctionId { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Device { get; set; }
        public string ClientIp { get; set; }
        public int Count { get; set; }
        public int ContentType { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (UserBehavior)obj;
            Id = entity.Id;
            UserId = entity.UserId;
            AppId = entity.AppId;
            FunctionId = entity.FunctionId;
            Content = entity.Content;
            Url = entity.Url;
            CreatedTime = entity.CreatedTime;
            Device = entity.Device;
            ClientIp = entity.ClientIp;
            ContentType = entity.ContentType.GetValueOrDefault();
            return this;
        }
    }
}
