using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System.Collections.Generic;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IArticleImagesService : IDependency, IBaseService<ArticleImages>
    {
        List<T> GetListByAppID<T>(int appId) where T : IViewModel, new();
    }
}
