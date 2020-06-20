using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Data.Fakes;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Fakes;
using Innocellence.CA.Admin.Controllers;
using Innocellence.CA.Entity;
using Innocellence.CA.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UnitTest.ControllerTest
{
    [TestClass]
    public class ArticleInfoControllerCase
    {
        private ArticleInfoController controller;
        private ControllerContext controllerContext;
        private static StubArticleInfoService stubArticleInfoService;
        private static ShimArticleInfoService shimArticleInfoService;
        private static StubRepository<ArticleInfo, int> repository;
        private ShimRepository<ArticleInfo, int> shimRepository;
        protected ICacheManager cache = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(ArticleInfoControllerCase)));

        [TestInitialize]
        public void InitController()
        {
            controllerContext = new ControllerContext
            {
                HttpContext = FakeHttpContext.Root()
            };

            cache.Get("Category", () => new List<Category>{new Category
            {
                CategoryCode = "601",
                CategoryName = "公司新闻",
                //CategoryTypeCode = 6,
            }});

        }

        [TestMethod]
        public void EditAction()
        {
            using (ShimsContext.Create())
            {
                stubArticleInfoService = new StubArticleInfoService();
                shimArticleInfoService = new ShimArticleInfoService(stubArticleInfoService);
                repository = new StubRepository<ArticleInfo, int>(new MockContent());
                shimRepository = new ShimRepository<ArticleInfo, int>(repository);
                InitService();

                shimArticleInfoService.Bind(repository);

                controller = new ArticleInfoController(shimArticleInfoService.Instance, 6)
                {
                    ControllerContext = controllerContext
                };

                controller.Edit("1");
            }

            //Assert.IsTrue(1 == 1);
        }

        private void InitService()
        {
            shimRepository.GetByKeyT1 = key => new ArticleInfo
            {
                Id = 1,
                ArticleTitle = "test",
                ArticleContent = "content",
                ArticleCateSub = "1",
                ArticleCode = Guid.NewGuid(),
                ArticleComment = "comment",
                //ArticleContentEdit = "edit",
                ArticleStatus = "new",
                ArticleURL = "abc/123.html",
                CreatedDate = DateTime.Now,
                CreatedUserID = "me",
                IsDeleted = false,
                LanguageCode = "zh",
                ReadCount = 0,
                UpdatedDate = DateTime.Now
            };

        }
    }

    public class MockHttpRequest : FakeHttpRequest
    {
        private readonly NameValueCollection _queryStringParams;

        public MockHttpRequest(NameValueCollection queryStringParams)
            : base(null, null, null, queryStringParams, null, null)
        {
            _queryStringParams = queryStringParams;
        }

        public override string this[string key]
        {
            get
            {
                var queryVals = _queryStringParams.GetValues(key);
                if (queryVals != null && queryVals.Length > 0)
                {
                    return queryVals.First();
                }

                throw new ArgumentException(key);
            }
        }
    }
}
