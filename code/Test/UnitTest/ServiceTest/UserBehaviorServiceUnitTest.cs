using Infrastructure.Core.Data;
using Innocellence.CA.Entity;
using Innocellence.CA.Service;
using Innocellence.CA.Service.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTest.ServiceTest
{
    [TestClass]
    public class UserBehaviorServiceUnitTest : FakeSetUp
    {
        [TestMethod]
        public void GetByListTestMethod()
        {
            using (ShimsContext.Create())
            {
                ShimUserBehaviorService.ConstructorIUnitOfWork = (d1, d2) =>
                {
                    d1.Repository = new Repository<UserBehavior, int>(new MockContent());
                };

                var userBehaviorService = new ShimUserBehaviorService(new UserBehaviorService(new MockContent()));
                DateTime dtTestDate = DateTime.Now;
                UserBehavior userBehavior1 = new UserBehavior();
                userBehavior1.AppId = 16;
                userBehavior1.ClientIp = "121.199.31.226";
                userBehavior1.Content = "46";
                userBehavior1.CreatedTime = dtTestDate;
                userBehavior1.FunctionId = "/News/ArticleInfo/Index";
                userBehavior1.UserId = "cwwhy1";

                userBehaviorService.Instance.Repository.Insert(userBehavior1);

                UserBehavior userBehavior2 = new UserBehavior();
                userBehavior2.AppId = 15;
                userBehavior2.ClientIp = "121.199.31.226";
                userBehavior2.Content = "33";
                userBehavior2.CreatedTime = dtTestDate.AddSeconds(5);
                userBehavior2.FunctionId = "/News/ArticleInfo/Index";
                userBehavior2.UserId = "cwwhy1";

                userBehaviorService.Instance.Repository.Insert(userBehavior2);

                UserBehavior userBehavior3 = new UserBehavior();
                userBehavior3.AppId = 17;
                userBehavior3.ClientIp = "121.199.31.226";
                userBehavior3.Content = "46";
                userBehavior3.CreatedTime = dtTestDate.AddSeconds(10);
                userBehavior3.FunctionId = "/News/ArticleInfo/Index";
                userBehavior3.UserId = "cwwhy1";

                userBehaviorService.Instance.Repository.Insert(userBehavior3);

                Dictionary<int, int> testDataList1 =
                    userBehaviorService.Instance.GetByList(dtTestDate.AddSeconds(-1), dtTestDate.AddSeconds(15));

                Assert.IsTrue(testDataList1.Count == 3);

                Dictionary<int, int> testDataList2 =
                    userBehaviorService.Instance.GetByList(dtTestDate.AddSeconds(5), dtTestDate.AddSeconds(15));

                Assert.IsTrue(testDataList2.Count == 2);

                Dictionary<int, int> testDataList3 =
                    userBehaviorService.Instance.GetByList(dtTestDate.AddSeconds(-5), dtTestDate.AddSeconds(3));

                Assert.IsTrue(testDataList3.Count == 1);

                Dictionary<int, int> testDataList4 =
                    userBehaviorService.Instance.GetByList(dtTestDate.AddSeconds(-5), dtTestDate.AddSeconds(-1));

                Assert.IsTrue(testDataList4.Count == 0);
            }

        }

        [TestMethod]
        public void GetAgentListTestMethod()
        {
            using (ShimsContext.Create())
            {
                ShimUserBehaviorService.ConstructorIUnitOfWork = (d1, d2) =>
                {
                    d1.Repository = new Repository<UserBehavior, int>(new MockContent());
                };

                var userBehaviorService = new ShimUserBehaviorService(new UserBehaviorService(new MockContent()));
 
                DateTime dtTestDate = DateTime.Now;
                UserBehavior userBehavior1 = new UserBehavior();
                userBehavior1.AppId = 16;
                userBehavior1.ClientIp = "121.199.31.226";
                userBehavior1.Content = "46";
                userBehavior1.CreatedTime = dtTestDate;
                userBehavior1.FunctionId = "/News/ArticleInfo/Index";
                userBehavior1.UserId = "cwwhy1";

                userBehaviorService.Instance.Repository.Insert(userBehavior1);

                UserBehavior userBehavior2 = new UserBehavior();
                userBehavior2.AppId = 16;
                userBehavior2.ClientIp = "121.199.31.226";
                userBehavior2.Content = "43";
                userBehavior2.CreatedTime = dtTestDate.AddSeconds(5);
                userBehavior2.FunctionId = "/News/ArticleInfo/Index";
                userBehavior2.UserId = "cwwhy1";

                userBehaviorService.Instance.Repository.Insert(userBehavior2);

                List<int> testDataList =
                    userBehaviorService.Instance.GetAgentList(dtTestDate.AddSeconds(-5), dtTestDate.AddSeconds(10));

                Assert.IsTrue(testDataList.Count == 1);
            }

        }
    }

}
