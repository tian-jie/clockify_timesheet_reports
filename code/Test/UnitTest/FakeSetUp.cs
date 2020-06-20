using System;
using System.Collections.Generic;
using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web.Domain.Entity;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
   public class FakeSetUp
    {
        protected ContainerBuilder builder;
        protected ICacheManager cache;
        protected IDisposable Disposable;

        [TestInitialize]
        public virtual void GlobalSetup()
        {
            builder = new ContainerBuilder();
            builder.RegisterModule(new CacheModule());
            builder.RegisterType<DefaultCacheHolder>().As<ICacheHolder>().SingleInstance();
            cache = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(SetUp)));

            //手动添加缓存
            cache.Get("Category", () => new List<Category>{new Category
            {
                CategoryCode = "601",
                CategoryName = "公司新闻",
                //CategoryTypeCode = 6,
            }});

            Disposable = ShimsContext.Create();
        }

        [TestCleanup]
        public virtual void ClearUp()
        {
            Disposable.Dispose();
        }
    }
}
