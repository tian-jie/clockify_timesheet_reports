using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Service
{
    public class NewsPublishHistoryService : BaseService<NewsPublishHistoryEntity>, INewsPublishHistoryService
    {
    }


    public enum PublishHistoryType
    {
        Article,
        Message
    }

    public enum SendStatus
    {
        Success,
        Failed
    }
}
