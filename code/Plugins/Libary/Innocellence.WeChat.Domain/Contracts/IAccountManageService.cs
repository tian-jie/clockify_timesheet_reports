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
    public interface IAccountManageService : IDependency, IBaseService<AccountManage>
    {
        List<T> GetList<T>(Expression<Func<AccountManage, bool>> predicate) where T : IViewModel, new();
        AccountManage Add(AccountManageView view);
        int Update(AccountManage view);
        AccountManage GetById(int Id);
        List<AccountManage> GetAllAccountManage();
        void DelleteAccountManage(AccountManageView view);
    }
}
