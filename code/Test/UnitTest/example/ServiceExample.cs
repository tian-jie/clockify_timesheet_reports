using Infrastructure.Core.Data;
using Innocellence.CA.Entity;
using Innocellence.CA.ModelsView;
using Innocellence.CA.Services;
using Innocellence.CA.Services.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.example
{
    [TestClass]
    public class ServiceExample : FakeSetUp
    {
        [TestMethod]
        public void DicService()
        {
            ShimArticleInfoService.Constructor = d =>
            {
                d.Repository = new Repository<ArticleInfo, int>(new MockContent());
            };

            var dicService = new ShimArticleInfoService(new ArticleInfoService());

            var instance = dicService.Instance;

            instance.Repository.Insert(new ArticleInfo { Id = 1, AppId = 10, ArticleTitle = "test" });

            var view = instance.GetById<ArticleInfoView>(1);

            Assert.IsTrue(view.AppId == 10);
        }
    }
}
