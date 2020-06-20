using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IWechatPreviewMessageLogService : IDependency, IBaseService<WechatPreviewMessageLog>
    {
        List<WechatMessageLogView> GetList(Expression<Func<WechatPreviewMessageLog, bool>> predicate);

        int Insert(WechatPreviewMessageLog entity);

        int Insert(WechatMessageLog entity);
    }
}
