using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Innocellence.WeChat.Domain.Contracts.CommonEntity;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Service;
using NUnit.Framework;

namespace UnitTest.Service
{
    public class EventTest : DbSetUp
    {
        [Test]
        public void QueryEvent()
        {
            var service = new EventService();

            var list = service.Repository.Entities.Select(x => new { id = x.Id, name = x.Name }).ToList();

            Assert.IsTrue(list.Any());
        }

        [Test]
        public void Register()
        {
            var resultList = new List<ResultResponse<object, EventStatus>>();
            var service = new EventProfileService(null, new EventService());

            for (int x = 1; x < 20; x++)
            {
                var userId = x > 2 ? "1" : x.ToString(CultureInfo.CurrentCulture);
                resultList.Add(service.RegisteredEvent(1, userId));
            }

            Assert.IsTrue(resultList.Count(x => x.Status == EventStatus.Success) == 2);
        }

        [Test]
        public void Checkin()
        {
            var service = new EventProfileService(null, new EventService());

            var result = service.CheckinEvent(1, "1");

            Assert.IsTrue(result.Status == EventStatus.Success);
        }
    }
}
