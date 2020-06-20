using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Entity;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Contracts
{    
    public interface IWechatFollowReportService : IDependency, IBaseService<WechatFollowReport>
    {
        List<T> GetListByFromStatisticsDate<T>(Expression<Func<WechatFollowReport, bool>> predicate) where T : IViewModel, new();
    }
}
