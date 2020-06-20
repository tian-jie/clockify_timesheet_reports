using System.Collections.Generic;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Contracts.ViewModel;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IAppUserService : IDependency
    {
        IList<AppUserView> QueryUser(string searchKey,int AccountMangageID, PageCondition pageCondition);
    }
}
