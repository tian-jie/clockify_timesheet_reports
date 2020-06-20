using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Data;
using Infrastructure.Core.Infrastructure;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Service
{
    public class DictionaryService : BaseService<DictionaryEntity>, IDictionaryService
    {
        private static readonly ICacheManager _cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(DictionaryService)));

        public IList<DictionaryEntity> QueryList(Func<DictionaryEntity, bool> query)
        {
            var list = getList();
            return list.Where(query).ToList();
        }

        private IEnumerable<DictionaryEntity> getList()
        {
            return _cacheManager.Get("GlobleDictionary", () => Repository.Entities.ToList());
        }
    }
}
