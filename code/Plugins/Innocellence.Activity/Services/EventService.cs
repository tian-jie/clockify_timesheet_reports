using Infrastructure.Core.Data;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.ModelsView;
using Innocellence.Activity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Service
{
    public class EventService : BaseService<EventEntity>, IEventService
    {
        public EventService()
            : base("CAAdmin")
        {

        }

        public List<EventEntityView> QueryList(Expression<Func<EventEntity, bool>> predicate)
        {
            var lst = Repository.Entities.Where(predicate).Where(a => a.IsDeleted != true).OrderByDescending(x => x.Id).ToList()
                .Select(n => (EventEntityView)(new EventEntityView().ConvertAPIModel(n)))
                .ToList();
            return lst;
        }

        public IQueryable<EventEntity> GetActivityEvents(int appId)
        {
            if (appId == 0)
            {
                throw new ArgumentException(@"appId 不能为0.", "appId");
            }
            return Repository.Entities.Where(x => appId == x.AppId && x.IsDeleted == false).OrderByDescending(x => x.Id);
        }
    }
}
