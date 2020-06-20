using Infrastructure.Core;
using Innocellence.Activity.Contracts.Entity;
using System.Linq;

namespace Innocellence.Activity.Services
{
    public interface IEventService : IDependency, IBaseService<EventEntity>
    {
        IQueryable<EventEntity> GetActivityEvents(int appId);
    }
}
