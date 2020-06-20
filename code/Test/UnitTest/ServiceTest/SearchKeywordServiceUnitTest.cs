using Infrastructure.Core.Data;
using Innocellence.CA.Entity;
using Innocellence.CA.ModelsView;
using Innocellence.CA.Service;
using Innocellence.CA.Service.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest.ServiceTest
{
    [TestClass]
    public class SearchKeywordServiceUnitTest : FakeSetUp
    {
        [TestMethod]
        public void GetSearchKeywordsByCategoryTestMethod()
        {
            using (ShimsContext.Create())
            {
                ShimSearchKeywordService.ConstructorIUnitOfWork = (d1, d2) =>
                {
                    d1.Repository = new Repository<SearchKeyword, int>(new MockContent());
                };
                var searchKeywordService = new ShimSearchKeywordService(new SearchKeywordService(new MockContent()));

                SearchKeyword searchKeyword1 = new SearchKeyword();
                searchKeyword1.AppId = 16;
                searchKeyword1.Category = 35;
                searchKeyword1.Keyword = "来一发";
                searchKeyword1.SearchCount = 100;

                searchKeywordService.Instance.Repository.Insert(searchKeyword1);

                SearchKeyword searchKeyword2 = new SearchKeyword();
                searchKeyword2.AppId = 16;
                searchKeyword2.Category = 36;
                searchKeyword2.Keyword = "昆特牌";
                searchKeyword2.SearchCount = 32;

                searchKeywordService.Instance.Repository.Insert(searchKeyword2);

                List<SearchKeywordView> testDataList =
                    searchKeywordService.Instance.GetSearchKeywordsByCategory(16, 2);

                Assert.IsTrue(testDataList.Count == 2);
            }
        }
    }
}
