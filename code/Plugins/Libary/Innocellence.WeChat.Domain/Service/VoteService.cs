using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Service
{
    public class VoteService : BaseService<VoteEntity>, IVoteService
    {
        public VoteService()
            : base("CAAdmin")
        {
          
        }


        public IList<VoteEntity> QueryList(Expression<Func<VoteEntity, bool>> query)
        {
            return Repository.Entities.Where(query).ToList();
        }

        public int InsertView(VoteEntity obj)
        {
            return Repository.Insert(obj);
        }
    }
}
