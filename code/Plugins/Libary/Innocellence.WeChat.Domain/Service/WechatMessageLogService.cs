using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Service
{
    public class WechatMessageLogService : BaseService<WechatMessageLog>, IWechatMessageLogService
    {
        public WechatMessageLogService()
            : base("CAADMIN")
        {

        }
        public int Insert(WechatMessageLog entity)
        {
            return this.Repository.Insert(entity);
        }

        public List<WechatMessageLogView> GetList(Expression<Func<WechatMessageLog, bool>> predicate)
        {
            var lst = Repository.Entities.Where(predicate).ToList().Select(a => new WechatMessageLogView
            {
                Id = a.Id,
                Content = JsonConvert.DeserializeObject<List<NewsInfoView>>(a.Content),
                PublishDate = a.CreatedTime,
                Type = ((WechatMessageLogType)a.ContentType).ToString(),
                UserName = a.UserName,
            }).ToList();

            return lst;
        }
    }
}
