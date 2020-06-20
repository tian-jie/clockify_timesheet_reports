using Infrastructure.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.WeChat.Domain.Contracts;
using Infrastructure.Core;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Service
{
    public partial class AccountManageService : BaseService<AccountManage>, IAccountManageService
    {
       
        public List<T> GetList<T>(Expression<Func<AccountManage, bool>> predicate) where T : IViewModel, new()
        {
            
            var list= 
                Repository.Entities.Where(predicate).OrderByDescending(m => m.CreatedDate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            
            return list;
        }



        public AccountManage Add(AccountManageView view)
        {
            var entity = view.ConvertToEntity();
            Repository.Insert(entity);
            return entity;
        }

        public int Update(AccountManage model)
        {
            return Repository.Update(model);
        }

        public List<AccountManage> GetAllAccountManage()
        {
            return this.Repository.Entities.ToList();
        }

        public AccountManage GetById(int Id)
        {
            return this.Repository.GetByKey(Id);
        }
        
        public void DelleteAccountManage(AccountManageView views)
        {
            var list=this.Repository.Entities.Where(a => a.Id==views.Id).FirstOrDefault();
            if (list!=null)
            {
                list.Id = views.Id;
                list.IsDeleted = views.IsDeleted;
                list.AccountName = views.AccountName;
                list.AccountDescription = views.AccountDescription;
                list.AccountLogo = views.AccountLogoPath;
                list.QrCode = views.QrCodePath;
                this.Repository.Update(list);
            }
        }
    }
}
