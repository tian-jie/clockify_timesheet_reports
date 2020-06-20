using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.WeChat.Domain.Common;

namespace UnitTest
{
    [TestClass]
    public class FishFlow
    {
        [TestMethod]
        public void TestMethod1()
        {
            Innocellence.WeChat.WebService.EmployeeInfo s = new Innocellence.WeChat.WebService.EmployeeInfo();
            //s.Url = "";
            s.GCSoapHeaderValue = new Innocellence.WeChat.WebService.GCSoapHeader() { UserName = "@+z2NgyZDvn)wIC.Uj*s", Password = "#%SrbG*a,Vk9^D)4%h2b=@dKrfYXiy" };
            var k= s.GetEmployeeInfo2(533, 10);

            var dd = EncryptionHelper.DecryptDES(k);
        }
    }
}
