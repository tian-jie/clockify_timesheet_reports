using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Core.Logging;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.ModelsView;
using Innocellence.Activity.Services;
using Innocellence.WeChat.Domain.Contracts.CommonEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Service
{
    public class EventProfileService : BaseService<EventProfileEntity>, IEventProfileService
    {
        private static readonly object _lock = new object();
        private static readonly object _checkinLock = new object();
        private static readonly object _checkinAnnualLock = new object();
        private readonly IEventService _eventService;
        private readonly IPollingResultService _pollingResultService;
        private readonly IPollingAnswerService _pollingAnswerService;
        private static readonly ILogger Logger = LogManager.GetLogger("wechat");

        public EventProfileService(IEventService eventService, IPollingResultService pollingResultService,
            IPollingAnswerService pollingAnswerService)
            : base("CAAdmin")
        {
            _eventService = eventService;
            _pollingResultService = pollingResultService;
            _pollingAnswerService = pollingAnswerService;
        }

        public ResultResponse<object, EventStatus> RegisteredEvent(int eventId, string userId)
        {
            var eventEntity = _eventService.Repository.GetByKey(eventId);
            var dateTimeNow = DateTime.Now;
            var registers =
                Repository.Entities.Where(x => x.EventId == eventId && x.TypeCode == "Registered" && x.IsDeleted != true)
                    .ToList();

            ResultResponse<object, EventStatus> checkResult = CheckinRegister(eventEntity, registers, userId,
                dateTimeNow);

            if (checkResult.Status != EventStatus.Continue)
            {
                return checkResult;
            }

            //TODO 可以注册
            lock (_lock)
            {
                if (eventEntity.MaxUser > 0 && registers.Count() + 1 > eventEntity.MaxUser)
                {
                    return new ResultResponse<object, EventStatus>
                    {
                        Entity = eventEntity,
                        Status = EventStatus.OverMaxUser
                    };
                }

                Repository.Insert(new EventProfileEntity
                {
                    EventId = eventId,
                    OperatedDateTime = dateTimeNow,
                    TypeCode = "Registered",
                    UserId = userId
                });

                //TODO:极端情况下可能还会有问题，但概率很低
                var eventEntityNew = _eventService.Repository.GetByKey(eventId);
                if (eventEntityNew.IsDeleted.GetValueOrDefault())
                {
                    return new ResultResponse<object, EventStatus>
                    {
                        Entity = eventEntity,
                        Status = EventStatus.CanceledEvent
                    };
                }

                return new ResultResponse<object, EventStatus>
                {
                    Status = EventStatus.Success,
                    Entity = eventEntity
                };
            }
        }

        /// <summary>
        /// 独立check逻辑
        /// </summary>
        /// <param name="eventEntity"></param>
        /// <param name="registers"></param>
        /// <param name="userId"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        public ResultResponse<object, EventStatus> CheckinRegister(EventEntity eventEntity,
            List<EventProfileEntity> registers, string userId, DateTime now)
        {
            if (eventEntity.IsDeleted.GetValueOrDefault())
            {
                return new ResultResponse<object, EventStatus>
                {
                    Entity = eventEntity,
                    Status = EventStatus.CanceledEvent
                };
            }

            var eventProfile = registers.Find(x => x.UserId == userId && x.IsDeleted == false);
            if (eventProfile != null)
            {
                //注册过了
                return new ResultResponse<object, EventStatus>
                {
                    Entity = new { Event = eventEntity, Profile = eventProfile },
                    Status = EventStatus.RepeatRegistered
                };
            }

            if (eventEntity.RegisteredStartedDateTime > now)
                return new ResultResponse<object, EventStatus> { Entity = eventEntity, Status = EventStatus.NotStarted };

            if (eventEntity.RegisteredEndedDateTime < now)
                return new ResultResponse<object, EventStatus> { Entity = eventEntity, Status = EventStatus.Finished };

            if (eventEntity.RegisteredStartedDateTime > now || eventEntity.RegisteredEndedDateTime < now)
                return new ResultResponse<object, EventStatus>
                {
                    Entity = eventEntity,
                    Status = EventStatus.OutOfRegDateRange
                };

            if (eventEntity.MaxUser > 0 && registers.Count() + 1 > eventEntity.MaxUser)
            {
                return new ResultResponse<object, EventStatus>
                {
                    Entity = eventEntity,
                    Status = EventStatus.OverMaxUser
                };
            }

            return new ResultResponse<object, EventStatus>
            {
                Status = EventStatus.Continue,
                Entity = eventEntity
            };
        }

        public ResultResponse<object, EventStatus> CheckinEvent(int eventId, string userId)
        {
            var eventEntity = _eventService.Repository.GetByKey(eventId);
            //var userInfo =
            //    WeChatCommonService.lstUserWithDeptTag.Find(
            //        x => x.userid.Equals(userId, StringComparison.OrdinalIgnoreCase));
            var dateNow = DateTime.Now;

            if (eventEntity.CheckinStartedDateTime != null)
            {
                if (eventEntity.CheckinStartedDateTime > dateNow || dateNow > eventEntity.CheckinEndedDateTime)
                    return new ResultResponse<object, EventStatus>
                    {
                        Entity = eventEntity,
                        Status = EventStatus.OutOfCheckinDateRange
                    };
            }

            var profiles =
                Repository.Entities.Where(x => x.EventId == eventId && x.UserId == userId && x.IsDeleted == false)
                    .Select(x => new { x.Id, x.TypeCode, x.UserId })
                    .ToList();

            var checkInProfile = profiles.FirstOrDefault(x => x.TypeCode == EventTypeCode.Checkin.ToString());
            if (checkInProfile != null)
            {
                //return new ResultResponse<object, EventStatus>
                //{
                //    Status = EventStatus.RepeatCheckin,
                //    Entity = checkInProfile
                //};
            }
            else
            {
                if (eventEntity.CheckinStartedDateTime != null && eventEntity.RegisteredStartedDateTime != null && eventEntity.IsNeedRegisterBeforeCheckin)
                {
                }
                else
                {
                    if (eventEntity.RegisteredStartedDateTime != null)
                    {
                        var registerProfile = profiles.FirstOrDefault(x => x.TypeCode == EventTypeCode.Registered.ToString());
                        if (registerProfile == null)
                        {
                            return new ResultResponse<object, EventStatus>
                            {
                                Status = EventStatus.UnRegistered,
                            };
                        }
                    }
                }
            }

            var insertProfile = new EventProfileEntity
            {
                EventId = eventId,
                UserId = userId,
                //UserName = userInfo != null ? userInfo.name : "Lilly",
                //ImgUrl = userInfo != null ? userInfo.avatar : "/Content/img/icon_avatar_default.png",
                OperatedDateTime = dateNow,
                TypeCode = "Checkin",
                IsDisplay1 = false,
                IsDisplay2 = false,
                IsDisplay3 = false,
                IsDeleted = false
            };

            if (checkInProfile == null)
            {
                Repository.Insert(insertProfile);
            }

            return new ResultResponse<object, EventStatus>
            {
                Status = EventStatus.Success,
                Entity = insertProfile
            };


        }

        public ResultResponse<object, EventStatus> CheckinAnnualEvent(int eventId, string userId)
        {
            var eventEntity = _eventService.Repository.GetByKey(eventId);
            var dateNow = DateTime.Now;
            //var userInfo =
            //    WeChatCommonService.lstUserWithDeptTag.Find(
            //        x => x.userid.Equals(userId, StringComparison.CurrentCultureIgnoreCase));

            if (eventEntity.CheckinStartedDateTime > dateNow || dateNow > eventEntity.CheckinEndedDateTime)
                return new ResultResponse<object, EventStatus>
                {
                    Entity = eventEntity,
                    Status = EventStatus.OutOfCheckinDateRange
                };

            lock (_checkinAnnualLock)
            {
                //重复签到
                var profile =
                    Repository.Entities.FirstOrDefault(
                        x =>
                            x.EventId == eventId && x.UserId == userId && x.TypeCode == "Checkin" &&
                            x.IsDeleted == false);
                if (profile != null)
                {
                    return new ResultResponse<object, EventStatus>
                    {
                        Status = EventStatus.Success,
                        Entity = profile
                    };
                }
                //如果成功签到，则插入用户记录
                var insertProfile = new EventProfileEntity
                {
                    EventId = eventId,
                    UserId = userId,
                    //UserName = userInfo != null ? userInfo.name : "Lilly",
                    //ImgUrl = userInfo != null ? userInfo.avatar : "/Content/img/icon_avatar_default.png",
                    OperatedDateTime = dateNow,
                    TypeCode = "Checkin",
                    IsDisplay1 = false,
                    IsDisplay2 = false,
                    IsDisplay3 = false,
                    IsDeleted = false
                };

                Repository.Insert(insertProfile);

                return new ResultResponse<object, EventStatus>
                {
                    Status = EventStatus.Success,
                    Entity = insertProfile
                };
            }

        }

        public override List<T> GetList<T>(Expression<Func<EventProfileEntity, bool>> predicate, PageCondition con)
        {
            int total = con.RowCount;
            int pageIndex = con.PageIndex;
            int pageSize = con.PageSize;
            List<SortCondition> sortConditions = con.SortConditions;

            //if (total <= 0)
            //{
            //    total = Repository.Entities.Count(predicate);
            //}
            var source = Repository.Entities.Distinct().Where(predicate);

            if (sortConditions == null || sortConditions.Count == 0)
            {
                source = source.OrderByDescending(m => m.Id);
            }
            else
            {
                int count = 0;
                IOrderedQueryable<EventProfileEntity> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? CollectionPropertySorter<EventProfileEntity>.OrderBy(source, sortCondition.SortField,
                            sortCondition.ListSortDirection)
                        : CollectionPropertySorter<EventProfileEntity>.ThenBy(orderSource, sortCondition.SortField,
                            sortCondition.ListSortDirection);
                    count++;
                }
                source = orderSource;
            }

            //var lst = source != null
            //    ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            //    : Enumerable.Empty<EventProfileEntity>();

            IEnumerable<EventProfileEntity> resultList = null;

            if (source != null)
            {
                resultList = source.ToList().Distinct(new EventProfileComparer());
                total = resultList.ToList().Count;
                resultList = resultList.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                resultList = Enumerable.Empty<EventProfileEntity>();
                total = 0;
            }

            con.RowCount = total;

            return resultList.ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
        }

        public List<T> GetList<T>(Expression<Func<EventProfileEntity, bool>> predicate) where T : IViewModel, new()
        {

            var lst =
                Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }

        public void GetUserList(List<EventProfileEntityView> lists)
        {
            // 获取部门列表
            //List<EmployeeInfoWithDept> empDetails = WeChatCommonService.lstUserWithDeptTag;
            //lists.ForEach(item =>
            //{
            //    var emp = empDetails.SingleOrDefault(a => a.userid.ToUpper().Equals(item.UserId.ToUpper()));
            //    if (emp != null)
            //    {
            //        item.deptLvs = emp.deptLvs;
            //        item.UserName = emp.name;
            //        item.Email = emp.email;
            //    }
            //});
        }

        public IQueryable<EventProfileEntity> GetUserUnderEvent(int eventId, IList<string> userTypes)
        {
            if (eventId == 0)
            {
                throw new ArgumentException(@"eventId 不能为0.", "eventId");
            }

            if (userTypes.Any())
            {
                return
                    Repository.Entities.Where(x => x.EventId == eventId && userTypes.Contains(x.TypeCode))
                        .GroupBy(x => x.UserId)
                        .Select(x => x.FirstOrDefault());
            }
            return
                Repository.Entities.Where(x => x.EventId == eventId)
                    .GroupBy(x => x.UserId)
                    .Select(x => x.FirstOrDefault());
        }

        public bool CancelEvent(int eventId, string lillyId, int? pollingId)
        {
            //var eventInfo =
            //    _eventService.Repository.Entities.Where(x => x.Id == eventId && x.IsDeleted != true)
            //        .Select(x => new { x.Id, x.EndedDateTime })
            //        .FirstOrDefault();
            //if (eventInfo == null)
            //{
            //    return false;
            //}

            //if (eventInfo.EndedDateTime < DateTime.Now)
            //{
            //    return false;
            //}

            //try
            //{
            //    using (var scope = new TransactionScope())
            //    {
            //        Repository.Entities.Where(x => x.EventId == eventId && x.UserId == lillyId && x.IsDeleted != true && x.TypeCode == "Registered")
            //            .Update(x => new EventProfileEntity { IsDeleted = true });
            //        if (pollingId.GetValueOrDefault() != 0)
            //        {
            //            _pollingResultService.Repository.Entities.Where(
            //                x => x.PollingId == pollingId && x.UserId == lillyId && x.IsDeleted != true)
            //                .Update(x => new PollingResultEntity { IsDeleted = true });
            //            _pollingAnswerService.Repository.Entities.Where(
            //                x => x.LillyId == lillyId && x.PollingId == pollingId && x.IsDeleted != true)
            //                .Update(x => new PollingAnswerEntity { IsDeleted = true });
            //        }
            //        scope.Complete();
            //    }
            //}
            //catch (Exception e)
            //{
            //    Logger.Error(e, "cancel event exception");
            //    return false;
            //}
            //return true;

            throw new NotImplementedException();
        }
    }

    public class EventProfileComparer : IEqualityComparer<EventProfileEntity>
    {
        public bool Equals(EventProfileEntity x, EventProfileEntity y)
        {

            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.UserId == y.UserId;
        }

        public int GetHashCode(EventProfileEntity eventProfile)
        {
            if (Object.ReferenceEquals(eventProfile, null)) return 0;

            return eventProfile.UserId.GetHashCode();
        }
    }

    public enum EventTypeCode
    {
        Checkin,
        Registered
    }

}
