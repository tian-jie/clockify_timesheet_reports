using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Fakes;
using Innocellence.CA.Admin.Controllers;
using Innocellence.CA.Entity;
using Innocellence.CA.ModelsView;
using Innocellence.CA.Services.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.example
{
    [TestClass]
    public class ControllerCase : FakeSetUp
    {
        private ControllerContext controllerContext;
        private readonly IList<ArticleInfo> source = new List<ArticleInfo>{new ArticleInfo
        {
            Id=1,
            ArticleTitle = "abc",
            AppId=1,
            IsDeleted = false
        }};

        [TestInitialize]
        public void InitController()
        {
            controllerContext = new ControllerContext
           {
               HttpContext = FakeHttpContext.Root()
           };

            cache.Get("SysConfig", () => new List<SysConfigModel> {new SysConfigModel
            {
                ConfigName = "config"
            }});
            // controller = new ArticleInfoController(new ArticleInfoService())
            //{
            //    ControllerContext = controllerContext
            //};
        }

        [TestMethod]
        public void TestGetListEx()
        {
            var collection = new NameValueCollection
            {
                { "txtArticleTitle", "abc" },
                { "txtDate", "" } ,
                {"txtSubCate",""}
            };

            var httpContext = controllerContext.HttpContext as FakeHttpContext;
            if (httpContext != null) httpContext.SetRequest(new MockHttpRequest(collection));

            controllerContext.HttpContext = httpContext;

            var artController = new ArticleInfoController(new StubArticleInfoService())
            {
                ControllerContext = controllerContext
            };

            MockData((StubArticleInfoService)artController._BaseService);

            var list = artController.GetListEx(x => x.Id == 1, new PageCondition());

            Assert.IsTrue(1 == list.Count);
        }

        private void MockData(StubArticleInfoService articleInfoService)
        {
            articleInfoService.GetListOf1ExpressionOfFuncOfArticleInfoBooleanPageCondition((e, page) => source.AsQueryable().Where(e).Select(n => (ArticleInfoView)(new ArticleInfoView().ConvertAPIModel(n))).ToList());
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
