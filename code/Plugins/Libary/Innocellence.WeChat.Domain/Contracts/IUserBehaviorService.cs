using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Contracts.ViewModel;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IUserBehaviorService : IDependency, IBaseService<UserBehavior>
    {
        List<UserBehaviorView> GetByList(DateTime begindate, DateTime endTime);
        List<int> GetAgentList(DateTime begindate, DateTime endTime);
    }
}
