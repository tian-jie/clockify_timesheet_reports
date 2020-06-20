using System.Collections.Generic;
using System.Linq;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Services;
using NUnit.Framework;

namespace UnitTest.Service
{
    public class DepartmentServiceTest : DbSetUp
    {
        [Test]
        public void GetSubDepartments()
        {
            var appInfo = WeChatCommonService.GetAppInfo(14);

            if (!appInfo.allow_partys.partyid.Any()) return;

            var departments = WeChatCommonService.GetSubDepartments(appInfo.allow_partys.partyid,1);
            Assert.IsTrue(departments.Count() == 5);
        }

        [Test]
        public void CheckMessagePushRuleTest()
        {
            var service = new MessageService();
            CheckResult result;
            //13354289553
            //var isPass = service.CheckMessagePushRule(14, new List<int>(), new List<int>{2}, new List<string>(), out result, false);

            //Assert.IsTrue(isPass);
        }
    }
}
