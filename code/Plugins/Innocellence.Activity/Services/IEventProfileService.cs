using Infrastructure.Core;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.ModelsView;
using Innocellence.WeChat.Domain.Contracts.CommonEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Innocellence.Activity.Services
{
    public interface IEventProfileService : IBaseService<EventProfileEntity>, IDependency
    {
        ResultResponse<object, EventStatus> RegisteredEvent(int eventId, string userId);

        ResultResponse<object, EventStatus> CheckinRegister(EventEntity eventEntity, List<EventProfileEntity> registers,
            string userId, DateTime now);

        ResultResponse<object, EventStatus> CheckinEvent(int eventId, string userId);

        ResultResponse<object, EventStatus> CheckinAnnualEvent(int eventId, string userId);

        List<T> GetList<T>(Expression<Func<EventProfileEntity, bool>> predicate) where T : IViewModel, new();
        void GetUserList(List<EventProfileEntityView> lists);

        IQueryable<EventProfileEntity> GetUserUnderEvent(int eventId, IList<string> userTypes);

        bool CancelEvent(int eventId, string lillyId, int? pollingId);
    }

    public enum EventStatus
    {
        OverMaxUser,

        OutOfRegDateRange,

        NotStarted,

        Finished,

        RepeatRegistered,

        Continue,

        Success,

        OutOfCheckinDateRange,

        UnRegistered,

        RepeatCheckin,

        CanceledEvent
    }
}
