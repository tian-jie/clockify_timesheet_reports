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
    public class WechatPreviewMessageLogService : BaseService<WechatPreviewMessageLog>, IWechatPreviewMessageLogService
    {
        public WechatPreviewMessageLogService()
            : base("CAADMIN")
        {

        }
        public int Insert(WechatPreviewMessageLog entity)
        {
            return this.Repository.Insert(entity);
        }

        public int Insert(WechatMessageLog entity)
        {
            var transEntity = new WechatPreviewMessageLog();
            typeof(WechatMessageLog).GetProperties(System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.GetProperty |
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly).ToList().ForEach(a =>
            {
                transEntity.GetType().GetProperty(a.Name).SetValue(transEntity, a.GetValue(entity));
            });

            return this.Repository.Insert(transEntity);
        }

        public List<WechatMessageLogView> GetList(Expression<Func<WechatPreviewMessageLog, bool>> predicate)
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
