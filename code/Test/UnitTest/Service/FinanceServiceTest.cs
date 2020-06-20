using System;
using System.Collections.Generic;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Service;
using NUnit.Framework;

namespace UnitTest.Service
{
    public class FinanceServiceTest : DbSetUp
    {
        [Test]
        public void Insert()
        {
            var service = new FinanceService();

            var rel = service.AddOrUpdate(new List<FinanceQueryEntity>
            {
                new FinanceQueryEntity
                {
                    //XXX = "13654921092",
                    MoneySum =decimal.Round(38218.4398m,4),
                     TEANO="adiwn-x" ,
                     ReceiveDate=DateTime.Now,
                     Status="介绍dfe",
                     CreatedDate = DateTime.Now,
                     UpdateDate=DateTime.Now,
                     CreatedUserID="john",
                     UpdatedUserID="john"
                }
            });

            Assert.IsTrue(rel.InsertCount > 0);
        }

        [Test]
        public void Query()
        {
            var service = new FinanceService();
            int count;
            var list = service.GetFinanceList("C070541", new PageCondition(1, 2), out count);

            Assert.IsTrue(list.Count == 2);
        }
    }
}
