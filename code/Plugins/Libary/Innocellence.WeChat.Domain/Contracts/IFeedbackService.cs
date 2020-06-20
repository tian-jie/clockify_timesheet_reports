using System.Collections.Generic;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IFeedbackService : IDependency, IBaseService<FeedBackEntity>
    {
        IList<FeedBackView> QueryList(int appId, string menuCode = null);
    }
}
