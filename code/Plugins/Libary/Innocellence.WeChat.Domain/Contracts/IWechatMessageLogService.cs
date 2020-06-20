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
    public interface IWechatMessageLogService : IDependency, IBaseService<WechatMessageLog>
    {
        List<WechatMessageLogView> GetList(Expression<Func<WechatMessageLog, bool>> predicate);

        int Insert(WechatMessageLog entity);
    }
}
