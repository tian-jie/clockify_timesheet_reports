using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IVoteService : IDependency, IBaseService<VoteEntity>
    {
        IList<VoteEntity> QueryList(Expression<Func<VoteEntity, bool>> query);
    }

    public interface IDictionaryService : IDependency, IBaseService<DictionaryEntity>
    {
        IList<DictionaryEntity> QueryList(Func<DictionaryEntity, bool> query);
    }
}
