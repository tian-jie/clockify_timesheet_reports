using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Data;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web.Domain.Entity;
using Innocellence.WeChat.Domain.Entity;
using NUnit.Framework;

namespace UnitTest
{
    [TestFixture]
    public class SetUp
    {
        protected ContainerBuilder builder;
        protected ICacheManager cache;

        [SetUp]
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
            },new Category()
            {
                CategoryCode ="HRES_NEWS",
                CategoryName="每周要闻"
            }
            });
        }

        [TearDown]
        public virtual void End()
        {

        }
    }

    [TestFixture]
    public class DbSetUp : SetUp
    {
        [SetUp]
        public void DBConfig()
        {
            var Asss = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("Innocellence") || a.FullName.Contains("Infrastructure"));
            foreach (var ass in Asss)
            {
                DatabaseInitializer.AddMapperAssembly(ass);
            }

            ModelMappers.MapperRegister();
        }
    }

}


